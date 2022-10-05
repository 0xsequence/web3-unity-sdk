using Vuplex.WebView;
using UnityEngine;
using Vuplex.WebView.Demos;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
namespace SequenceSharp
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] private ProviderConfig providerConfig;
        [SerializeField] private CanvasWebViewPrefab walletWindow;
        private IWebView internalWebView;

        private ulong callbackIndex;
        private IDictionary<ulong, Action<string>> callbackDict = new Dictionary<ulong, Action<string>>();

        private void Awake()
        {
            Web.EnableRemoteDebugging();
            Web.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 UnitySequence ");
        }

        private async void Start()
        {
            internalWebView = Web.CreateWebView();
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
                walletWindow.Visible = true;
            };

            internalWebView.MessageEmitted += (sender, eventArgs) =>
            {
                if (eventArgs.Value == "wallet_closed")
                {
                    walletWindow.Visible = false;
                }
                else if (eventArgs.Value == "initialized")
                {
                    Debug.Log("Sequence Wallet Initialized!");
                }
                else if (eventArgs.Value.Contains("vuplexFunctionReturn"))
                {
                    var promiseReturn = JsonUtility.FromJson<PromiseReturn>(eventArgs.Value);

                    callbackDict[promiseReturn.callbackNumber](promiseReturn.returnValue);
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
            window.seq.initWallet(
                '" + providerConfig.defaultNetworkId + @"',
                {
                    walletAppURL:'" + providerConfig.walletAppURL + @"',
                    transports: { unrealTransport: { enabled: true } }
                }
            );
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
        }

        /// <summary>
        /// Execute JS in a context with Sequence.js and Ethers.js
        /// You have a global named `seq`. To get the wallet, use `seq.getWallet()`.
        /// See https://docs.sequence.xyz for more information
        /// </summary>
        /// <param name="js">The javascript to run. Use `return` to return a value. Returned Promises are automatically awaited.</param>
        /// <returns>A stringified version of your return value.</returns>
        public Task<string> ExecuteSequenceJS(string js)
        {
            var thisCallbackIndex = callbackIndex;
            callbackIndex += 1;

            var jsPromiseResolved = new TaskCompletionSource<string>();
            Action<string> resolvePromiseCallback = (string value) => jsPromiseResolved.TrySetResult(value);

            callbackDict.Add(thisCallbackIndex, resolvePromiseCallback);

            var jsToRun = @"
            const codeToRun = async () => {
                " + js + @"
            };
            (async () => {
                const returnValue = await codeToRun();
                window.vuplex.postMessage({
                    type: 'vuplexFunctionReturn',
                    callbackNumber: " + thisCallbackIndex + @",
                    returnValue: JSON.stringify(returnValue)
                });
            })()
        ";
            internalWebView.ExecuteJavaScript(jsToRun);

            return jsPromiseResolved.Task;
        }

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


    [Serializable]
    class PromiseReturn
    {
        public string type;
        public ulong callbackNumber;
        public string returnValue;
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