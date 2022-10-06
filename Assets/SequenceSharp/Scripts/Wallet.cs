#define VUPLEX_OMIT_WEBGL

using UnityEngine;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

#if UNITY_WEBGL
    using System.Runtime.InteropServices;
#else
    using Vuplex.WebView;
    using Vuplex.WebView.Demos;
    using System.IO;
#endif

namespace SequenceSharp
{
    /// <summary>
    /// In builds that aren't WebGL, the GameObject this script is attached to will render the Wallet.
    /// Put this inside a Canvas to position and scale the wallet.
    /// In WebGL builds, the wallet will be a browser popup, and this GameObject will not render anything.
    /// </summary>
    public class Wallet : MonoBehaviour
    {
        [SerializeField] private ProviderConfig providerConfig;

        /// <summary>
        /// Allow debugging the Sequence WebViews through http://localhost:8080
        /// </summary>
        /// <remarks>
        /// This option does nothing in WebGL builds.
        /// </remarks>
        [SerializeField] private bool enableRemoteDebugging;

        /// <summary>
        /// Enables or disables [Native 2D Mode](https://support.vuplex.com/articles/native-2d-mode/),
        /// which makes it so that 3D WebView positions a native 2D webview in front of the Unity game view
        /// instead of displaying web content as a texture in the Unity scene. The default is `false`. If set to `true` and the 3D WebView package
        /// in use doesn't support Native 2D Mode, then the default rendering mode is used instead.
        /// </summary>
        /// <remarks>
        /// Important notes:
        /// <list type="bullet">
        ///   <item>
        ///     Native 2D Mode is only supported for 3D WebView for Android (non-Gecko) and 3D WebView for iOS.
        ///     For other packages, the default render mode is used instead.
        ///   </item>
        ///   <item>Native 2D Mode requires that the canvas's render mode be set to "Screen Space - Overlay".</item>
        /// </list>
        /// </remarks>
        [SerializeField] private bool native2DMode;

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void Sequence_InitJSLibraries();

        [DllImport("__Internal")]
        private static extern void Sequence_ExecuteJSInBrowserContext(string js);
#else
        private CanvasWebViewPrefab walletWindow;
        private IWebView internalWebView;
#endif

        private ulong callbackIndex;
        private IDictionary<ulong, TaskCompletionSource<string>> callbackDict = new Dictionary<ulong, TaskCompletionSource<string>>();

        private void Awake()
        {
#if UNITY_WEBGL

#else
            Web.EnableRemoteDebugging();
            Web.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 UnitySequence ");

            walletWindow = CanvasWebViewPrefab.Instantiate();
            walletWindow.transform.SetParent(this.transform);

            // set Widget to full-size of parent
            var rect = walletWindow.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localPosition = Vector3.zero;

            walletWindow.Native2DModeEnabled = native2DMode;

            internalWebView = Web.CreateWebView();
#endif
        }

        private async void Start()
        {
#if !UNITY_WEBGL
            await internalWebView.Init(1, 1);

            internalWebView.SetRenderingEnabled(false);

            internalWebView.LoadUrl("streaming-assets://sequence/sequence.html");
            await internalWebView.WaitForNextPageLoadToFinish();

            var internalWebViewWithPopups = internalWebView as IWithPopups;
            if (internalWebViewWithPopups == null)
            {
                throw new IOException("Broken!");
            }
            internalWebViewWithPopups.SetPopupMode(PopupMode.NotifyWithoutLoading);
            internalWebViewWithPopups.PopupRequested += (sender, eventArgs) =>
            {
                walletWindow.WebView.LoadUrl(eventArgs.Url);
                // TODO replace this with a WalletOpened callback
                walletWindow.Visible = true;
            };

            internalWebView.MessageEmitted += (sender, eventArgs) =>
            {
                if (eventArgs.Value == "wallet_closed")
                {
                    // TODO replace this with a WalletClosed callback
                    walletWindow.Visible = false;
                }
                else if (eventArgs.Value == "initialized")
                {
                    Debug.Log("Sequence Wallet Initialized!");
                }
                else if (eventArgs.Value.Contains("vuplexFunctionReturn"))
                {
                    var promiseReturn = JsonUtility.FromJson<PromiseReturn>(eventArgs.Value);

                    callbackDict[promiseReturn.callbackNumber].TrySetResult(promiseReturn.returnValue);
                    callbackDict.Remove(promiseReturn.callbackNumber);
                }
                else if (eventArgs.Value.Contains("vuplexFunctionError"))
                {
                    var promiseReturn = JsonUtility.FromJson<PromiseReturn>(eventArgs.Value);

                    callbackDict[promiseReturn.callbackNumber].TrySetException(new JSExecutionException(promiseReturn.returnValue));
                    callbackDict.Remove(promiseReturn.callbackNumber);
                }
                else
                {
                    //Debug.Log("sending message from sequence.js to wallet" + eventArgs.Value);
                    walletWindow.WebView.PostMessage(eventArgs.Value);
                }
            };

            await ExecuteSequenceJS(@"
                window.ue = {
                    sequencewallettransport: {
                        onmessagefromwallet: () => { /* will be overwritten by transport! */ },
                        sendmessagetowallet: (message) => window.vuplex.postMessage(message),
                        logfromjs: console.log,
                        warnfromjs: console.warn,
                        errorfromjs: console.error
                    }
                };
                window.vuplex.addEventListener('message', event => window.ue.sequencewallettransport.onmessagefromwallet(JSON.parse(event.data)));
                window.seq = window.sequence.sequence;
            ");
#else
            // We're in a WebGL build, inject Sequence.js and ethers.js
            await ExecuteSequenceJS(@"
                window.seq = '';
                window.ethers = '';
            ");
#endif


            await ExecuteSequenceJS(@"
                window.seq.initWallet(
                    '" + providerConfig.defaultNetworkId + @"',
                    {
                        walletAppURL: '" + providerConfig.walletAppURL + @"',
                        transports: { unrealTransport: { enabled: true } }
                    }
                );
            ");


#if !UNITY_WEBGL
            await ExecuteSequenceJS(@"
                window.seq.getWallet().on('close', () => {
                    window.ue.sequencewallettransport.sendmessagetowallet('wallet_closed')
                });
                window.ue.sequencewallettransport.sendmessagetowallet('initialized');
            ");

            await walletWindow.WaitUntilInitialized();

            var walletWithPopups = walletWindow.WebView as IWithPopups;
            if (walletWithPopups == null)
            {
                throw new IOException("Broken!");
            }
            walletWithPopups.SetPopupMode(PopupMode.NotifyWithoutLoading);
            walletWithPopups.PopupRequested += (sender, eventArgs) =>
            {
                Application.OpenURL(eventArgs.Url);
            };
            walletWindow.WebView.CloseRequested += (popupWebView, closeEventArgs) =>
            {
                // TODO replace this with a WalletClosed callback
                walletWindow.Visible = false;
            };

            walletWindow.Visible = false;

            walletWindow.WebView.PageLoadScripts.Add(@"
                window.ue = {
                    sequencewallettransport: {
                        onmessagefromsequencejs: () => { /* will be overwritten by transport! */ },
                        sendmessagetosequencejs: (message) => window.vuplex.postMessage(message),
                        logfromjs: console.log,
                        warnfromjs: console.warn,
                        errorfromjs: console.error
                    }
                };
                window.vuplex.addEventListener('message', event => window.ue.sequencewallettransport.onmessagefromsequencejs(JSON.parse(event.data)));
                window.startWalletWebapp();
            ");

            walletWindow.WebView.MessageEmitted += (sender, eventArgs) =>
            {
                //Debug.Log("sending message from wallet to sequence.js" + eventArgs.Value);
                internalWebView.PostMessage(eventArgs.Value);
            };


            var hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
            hardwareKeyboardListener.KeyDownReceived += (sender, eventArgs) =>
            {
                walletWindow.WebView.SendKey(eventArgs.Value);
            };
#endif
        }

        /// <summary>
        /// Execute JS in a context with Sequence.js and Ethers.js
        /// You have a global named `seq`, and a global named `ethers`. To get the wallet, use `seq.getWallet()`.
        /// See https://docs.sequence.xyz for more information
        /// </summary>
        /// <param name="js">The javascript to run. Use `return` to return a value. Returned Promises are automatically awaited.</param>
        /// <returns>A stringified version of your return value.</returns>
        public Task<string> ExecuteSequenceJS(string js)
        {
            var thisCallbackIndex = callbackIndex;
            callbackIndex += 1;

            var jsPromiseResolved = new TaskCompletionSource<string>();

            callbackDict.Add(thisCallbackIndex, jsPromiseResolved);

#if UNITY_WEBGL
            var jsToRun = @"
            const codeToRun = async () => {
                " + js + @"
            };
            (async () => {
                try {
                    const returnValue = await codeToRun();
                    const returnString = JSON.stringify({
                        type: 'return',
                        callbackNumber: " + thisCallbackIndex + @",
                        returnValue: JSON.stringify(returnValue)
                    });
                    SendMessage(" + this.name + @", 'JSFunctionReturn', returnString);
                 } catch(err) {
                    const returnString = JSON.stringify({
                        type: 'error',
                        callbackNumber: " + thisCallbackIndex + @",
                        returnValue: JSON.stringify(err, Object.getOwnPropertyNames(err))
                    })
                    SendMessage(" + this.name + @", 'JSFunctionError', returnString);
                 }
            })()
        ";
            Sequence_ExecuteJSInBrowserContext(jsToRun);
#else
            var jsToRun = @"
            const codeToRun = async () => {
                " + js + @"
            };
            (async () => {
                try {
                    const returnValue = await codeToRun();
                    window.vuplex.postMessage({
                        type: 'vuplexFunctionReturn',
                        callbackNumber: " + thisCallbackIndex + @",
                        returnValue: JSON.stringify(returnValue)
                    });
                 } catch(err) {
                    window.vuplex.postMessage({
                        type: 'vuplexFunctionError',
                        callbackNumber: " + thisCallbackIndex + @",
                        returnValue: JSON.stringify(err, Object.getOwnPropertyNames(err))
                    });
                 }              
            })()
        ";
            internalWebView.ExecuteJavaScript(jsToRun);
#endif
            return jsPromiseResolved.Task;
        }

#if UNITY_WEBGL
        public void JSFunctionReturn(string returnVal)
        {
            {
                var promiseReturn = JsonUtility.FromJson<PromiseReturn>(returnVal);

                callbackDict[promiseReturn.callbackNumber].TrySetResult(promiseReturn.returnValue);
                callbackDict.Remove(promiseReturn.callbackNumber);
            }
        }
        public void JSFunctionError(string returnVal)
        {
                var promiseReturn = JsonUtility.FromJson<PromiseReturn>(returnVal);

                callbackDict[promiseReturn.callbackNumber].TrySetException(new JSExecutionException(promiseReturn.returnValue));
                callbackDict.Remove(promiseReturn.callbackNumber);
            
        }
#endif

        public async Task<T> ExecuteSequenceJSAndParseJSON<T>(string js)
        {
            var jsonString = await ExecuteSequenceJS(js);
            Debug.Log(jsonString);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public Task<ConnectDetails> Connect(ConnectOptions options)
        {
            return ExecuteSequenceJSAndParseJSON<ConnectDetails>("return seq.getWallet().connect(" + ObjectToJson(options) + ");");
        }

        public Task<bool> IsConnected()
        {
            return ExecuteSequenceJSAndParseJSON<bool>("return seq.getWallet().isConnected();");
        }

        public async Task Disconnect()
        {
            await ExecuteSequenceJS("return seq.getWallet().disconnect();");
        }

        public Task<string> GetAddress()
        {
            return ExecuteSequenceJS("return seq.getWallet().getSigner().getAddress();");
        }
        
        public Task<string[]> ContractExample(string contractExampleJsCode)
        {
            return ExecuteSequenceJSAndParseJSON<string[]>(contractExampleJsCode);

        }

#nullable enable
        public Task<NetworkConfig[]> GetNetworks(string? chainId)
        {
            return ExecuteSequenceJSAndParseJSON<NetworkConfig[]>(@"return seq
                .getWallet()
                .getNetworks(" +
                    chainId == null ? "'" + chainId + "'" : "" +
                ");");
        }
#nullable disable

        public Task<ulong> GetChainId()
        {
            return ExecuteSequenceJSAndParseJSON<ulong>("return seq.getWallet().getChainId();");
        }

        public Task<ulong> GetAuthChainId()
        {
            return ExecuteSequenceJSAndParseJSON<ulong>("return seq.getWallet().getAuthChainId();");
        }

        public Task<bool> IsOpened()
        {
            return ExecuteSequenceJSAndParseJSON<bool>("return seq.getWallet().isOpened();");
        }

#nullable enable
        public Task<bool> OpenWallet(string? path, ConnectOptions? options, string? networkId)
        {
            var pathJson = path == null ? "undefined" : "'" + path + "'";
            var optionsJson = options == null ? "undefined" : "{ type: 'openWithOptions', options: " + ObjectToJson(options) + "}";
            var networkIdJson = networkId == null ? "undefined" : networkId;
            return ExecuteSequenceJSAndParseJSON<bool>("return seq.getWallet().openWallet("
                + pathJson + ","
                + optionsJson + ","
                + networkIdJson +
            ");");
        }
#nullable disable

        public async Task CloseWallet() {
            await ExecuteSequenceJS("return seq.getWallet().closeWallet();");
        }

#nullable enable
        public Task<WalletSession?> GetSession()
        {
            return ExecuteSequenceJSAndParseJSON<WalletSession?>("return seq.getWallet().getSession();");
        }

        public string ObjectToJson(object? value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
#nullable disable
    }


    [System.Serializable]
    class PromiseReturn
    {
        public string type;
        public ulong callbackNumber;
        public string returnValue;

        public PromiseReturn(string type, ulong callbackNumber, string returnValue)
        {
            this.type = type;
            this.callbackNumber = callbackNumber;
            this.returnValue = returnValue;
        }
    }

    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}