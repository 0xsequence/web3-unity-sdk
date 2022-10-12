#define VUPLEX_OMIT_WEBGL

using UnityEngine;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Events;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#else
using Vuplex.WebView;

using Vuplex.WebView.Demos;
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

        /// <summary>
        /// Called when the Wallet is opened.
        /// You should subscribe to this event and make it visible.
        /// </summary>
        [SerializeField] public UnityEvent onWalletOpened;

        /// <summary>
        /// Called when the Wallet is opened.
        /// You should subscribe to this event and make it invisible.
        /// </summary>
        [SerializeField] public UnityEvent onWalletClosed;

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
        private static extern void Sequence_ExecuteJSInBrowserContext(string js);
#else
        private CanvasWebViewPrefab _walletWindow;
        private IWebView _internalWebView;
        private CanvasWebViewPrefab _authWindow;

#endif
        private bool _walletVisible = true;

        private ulong _callbackIndex;
        private IDictionary<ulong, TaskCompletionSource<string>> _callbackDict = new Dictionary<ulong, TaskCompletionSource<string>>();

        private void Awake()
        {
#if !UNITY_WEBGL
   
            Debug.Log("[Android Build Debugging] Awake");
            if (enableRemoteDebugging)
            {
                Web.EnableRemoteDebugging();
            }
            Web.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 UnitySequence ");
            Debug.Log("[Android Build Debugging] after Web.SetUserAgent");
            _walletWindow = CanvasWebViewPrefab.Instantiate();
            Debug.Log("[Android Build Debugging] walletWindow" + _walletWindow.ToString());
            _walletWindow.transform.SetParent(this.transform);

            // set Widget to full-size of parent
            var rect = _walletWindow.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localPosition = Vector3.zero;

            _walletWindow.Native2DModeEnabled = native2DMode;

            onWalletOpened.AddListener(() =>
            {
                _walletWindow.Visible = true;
            });
            onWalletClosed.AddListener(() =>
            {
                _walletWindow.Visible = false;
            });

            _internalWebView = Web.CreateWebView();
            Debug.Log("[Android Build Debugging] internalWebView" + _internalWebView.ToString());
#endif
        }

        private async void Start()
        {
            _HideWallet();
#if !UNITY_WEBGL
            await _internalWebView.Init(1, 1);

            _internalWebView.SetRenderingEnabled(false);

            _internalWebView.LoadUrl("streaming-assets://sequence/sequence.html");
            await _internalWebView.WaitForNextPageLoadToFinish();
            Debug.Log("[Android Build Debugging] internalWebviwe Url" + _internalWebView.Url.ToString());

            var internalWebViewWithPopups = _internalWebView as IWithPopups;
            if (internalWebViewWithPopups == null)
            {
                throw new IOException("Broken!");
            }
            internalWebViewWithPopups.SetPopupMode(PopupMode.NotifyWithoutLoading);
            internalWebViewWithPopups.PopupRequested += (sender, eventArgs) =>
            {
                _walletWindow.WebView.LoadUrl(eventArgs.Url);
                _ShowWallet();
            };

            _internalWebView.MessageEmitted += (sender, eventArgs) =>
            {
                if (eventArgs.Value == "wallet_closed")
                {
                    _HideWallet();
                }
                else if (eventArgs.Value == "initialized")
                {
                    _SequenceDebugLog("Wallet Initialized!");
                }
                else if (eventArgs.Value.Contains("vuplexFunctionReturn"))
                {
                    var promiseReturn = JsonConvert.DeserializeObject<PromiseReturn>(eventArgs.Value);

                    _callbackDict[promiseReturn.callbackNumber].TrySetResult(promiseReturn.returnValue);
                    _callbackDict.Remove(promiseReturn.callbackNumber);
                }
                else if (eventArgs.Value.Contains("vuplexFunctionError"))
                {
                    var promiseReturn = JsonConvert.DeserializeObject<PromiseReturn>(eventArgs.Value);

                    _callbackDict[promiseReturn.callbackNumber].TrySetException(new JSExecutionException(promiseReturn.returnValue));
                    _callbackDict.Remove(promiseReturn.callbackNumber);
                }
                else
                {
                    _walletWindow.WebView.PostMessage(eventArgs.Value);
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
            ");
#else
            // We're in a WebGL build, inject Sequence.js and ethers.js
            var sequenceJSRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "sequence/sequence.js"));
            await sequenceJSRequest.SendWebRequest();
            var sequenceJS = sequenceJSRequest.downloadHandler.text;

            var ethersJSRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "sequence/ethers.js"));
            await ethersJSRequest.SendWebRequest();
            var ethersJS = ethersJSRequest.downloadHandler.text;
            await ExecuteSequenceJS(sequenceJS + ";" + ethersJS);
#endif

            await ExecuteSequenceJS(@"
                window.seq = window.sequence.sequence;
                window.seq.initWallet(
                    '" + providerConfig.defaultNetworkId + @"',
                    {
                        walletAppURL: '" + providerConfig.walletAppURL + "'," +
#if !UNITY_WEBGL
                        "transports: { unrealTransport: { enabled: true } } " +
#endif
                    @"}
                );
            ");


#if !UNITY_WEBGL
            await ExecuteSequenceJS(@"
                window.seq.getWallet().on('close', () => {
                    window.ue.sequencewallettransport.sendmessagetowallet('wallet_closed')
                });
                window.ue.sequencewallettransport.sendmessagetowallet('initialized');
            ");

            await _walletWindow.WaitUntilInitialized();


#if UNITY_STANDALONE || UNITY_EDITOR
            var credsRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "sequence/httpBasicAuth.json"));
            await credsRequest.SendWebRequest();
#nullable enable
            Dictionary<string, HttpBasicAuthCreds>? creds = null;
            creds = JsonConvert.DeserializeObject<Dictionary<string, HttpBasicAuthCreds>>(credsRequest.downloadHandler.text);
            if (creds != null)
            {
                _SequenceDebugLog("Loaded HTTP Basic Auth credentials for domains " + string.Join(",", creds.Keys.Select(x => x.ToString())));
            }
            var standaloneWebView = _walletWindow.WebView as StandaloneWebView;
            if (standaloneWebView == null)
            {
                throw new System.Exception("Failed to cast webview to StandaloneWebView");
            }
            standaloneWebView.AuthRequested += (sender, eventArgs) =>
            {
                if (creds == null)
                {
                    _SequenceDebugLogError("[Sequence] HTTP Basic Auth requested by " + eventArgs.Host + " , but no creds file is loaded.");
                    eventArgs.Cancel();
                    return;
                }
                if (!creds.ContainsKey(eventArgs.Host))
                {
                    _SequenceDebugLogError("[Sequence] HTTP Basic Auth requested by " + eventArgs.Host + " , but no creds for that host are in creds file.");
                    eventArgs.Cancel();
                    return;
                }
                _SequenceDebugLog("HTTP Basic Auth executed for" + eventArgs.Host);
                var matchingCreds = creds[eventArgs.Host];
                eventArgs.Continue(matchingCreds.username, matchingCreds.password);
            };

#nullable disable
#endif

            var walletWithPopups = _walletWindow.WebView as IWithPopups;
            if (walletWithPopups == null)
            {
                throw new IOException("Broken!");
            }

            walletWithPopups.SetPopupMode(PopupMode.LoadInNewWebView);
            walletWithPopups.PopupRequested += async (sender, eventArgs) =>
            {

                // TODO signal that we've opened a social login window,
                // so games can make it fullscreen

                //Application.OpenURL(eventArgs.Url);
                Debug.Log("Popup opened with URL: " + eventArgs.Url);
                _authWindow = CanvasWebViewPrefab.Instantiate(eventArgs.WebView);
                _authWindow.transform.SetParent(this.transform);
                var rect = _authWindow.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0, 0);
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.localPosition = Vector3.zero;
                await _authWindow.WaitUntilInitialized();

                _authWindow.WebView.CloseRequested += (popupWebView, closeEventArgs) => {
                    Debug.Log("Closing the popup");
                    _authWindow.Destroy();
                };
                //eventArgs.WebView.LoadUrl(eventArgs.Url);
            };

            _walletWindow.WebView.UrlChanged += (sender, eventArgs) => {
                // Replace myapp:// with your custom scheme
                Debug.Log("url chagned:" + eventArgs.Url);

               // walletWindow.WebView.LoadUrl(eventArgs.Url);

            };
            _walletWindow.WebView.CloseRequested += (popupWebView, closeEventArgs) =>
            {
                _HideWallet();
            };

            _walletWindow.WebView.PageLoadScripts.Add(@"
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
                window.sequenceStartWalletWebapp();
            ");

            _walletWindow.WebView.MessageEmitted += (sender, eventArgs) =>
            {
               
                _internalWebView.PostMessage(eventArgs.Value);
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
            var thisCallbackIndex = _callbackIndex;
            _callbackIndex += 1;

            var jsPromiseResolved = new TaskCompletionSource<string>();

            _callbackDict.Add(thisCallbackIndex, jsPromiseResolved);

#if UNITY_WEBGL
            var jsToRun = @"{
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
                    SendMessage('" + this.name + @"', 'JSFunctionReturn', returnString);
                 } catch(err) {
                    const returnString = JSON.stringify({
                        type: 'error',
                        callbackNumber: " + thisCallbackIndex + @",
                        returnValue: JSON.stringify(Object.fromEntries(Object.getOwnPropertyNames(err).map(prop => [JSON.stringify(prop), JSON.stringify(err[prop])])))
                    })
                    SendMessage('" + this.name + @"', 'JSFunctionError', returnString);
                 }
            })()
            }
        ";
            Sequence_ExecuteJSInBrowserContext(jsToRun);
#else
            var jsToRun = @"{
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
                        returnValue: JSON.stringify(Object.fromEntries(Object.getOwnPropertyNames(err).map(prop => [JSON.stringify(prop), JSON.stringify(err[prop])])))
                    });
                 }              
            })()
            }
        ";
            _internalWebView.ExecuteJavaScript(jsToRun);
#endif
            return jsPromiseResolved.Task;
        }

#if UNITY_WEBGL
        public void JSFunctionReturn(string returnVal)
        {
            {
                var promiseReturn = JsonConvert.DeserializeObject<PromiseReturn>(returnVal);

                callbackDict[promiseReturn.callbackNumber].TrySetResult(promiseReturn.returnValue);
                callbackDict.Remove(promiseReturn.callbackNumber);
            }
        }
        public void JSFunctionError(string returnVal)
        {
                var promiseReturn = JsonConvert.DeserializeObject<PromiseReturn>(returnVal);

                callbackDict[promiseReturn.callbackNumber].TrySetException(new JSExecutionException(promiseReturn.returnValue));
                callbackDict.Remove(promiseReturn.callbackNumber);
            
        }
#endif

        public async Task<T> ExecuteSequenceJSAndParseJSON<T>(string js)
        {
            var jsonString = await ExecuteSequenceJS(js);
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
                   (chainId == null ? "'" + chainId + "'" : "") +
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

        public async Task CloseWallet()
        {
            await ExecuteSequenceJS("return seq.getWallet().closeWallet();");
            if(_walletVisible)
            {
                _HideWallet();
            }
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
        private void _SequenceDebugLog(string message)
        {
            Debug.Log("[Sequence] " + message);
        }
        private void _SequenceDebugLogError(string message)
        {
            Debug.LogError("[Sequence] " + message);
        }

        private void _HideWallet()
        {
            if (_walletVisible)
            {
                onWalletClosed.Invoke();
                _walletVisible = false;
            }
        }

        private void _ShowWallet()
        {
            if (!_walletVisible)
            {
                onWalletOpened.Invoke();
                _walletVisible = true;
            }
        }
    }


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