#define VUPLEX_OMIT_WEBGL

#if !UNITY_WEBGL || UNITY_EDITOR
#define IS_EDITOR_OR_NOT_WEBGL
#endif
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Events;

#if IS_EDITOR_OR_NOT_WEBGL
using Vuplex.WebView;
#if UNITY_STANDALONE || UNITY_EDITOR
using System.Linq;
using System.Threading;
using System.Net.Sockets;
#endif
#else
using System.Runtime.InteropServices;
using CanvasWebViewPrefab = UnityEngine.GameObject; 
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
        private const int WINDOWS_IPC_PORT = 52836;

        /// <summary>
        /// The URL protocol you've set up for Sequence social login.
        /// </summary>
        public string appProtocol;

        /// <summary>
        /// Called when the Wallet is opened.
        /// You should subscribe to this event and make it visible.
        /// </summary>
        public UnityEvent onWalletOpened;

        /// <summary>
        /// Called when the Wallet is opened.
        /// You should subscribe to this event and make it invisible.
        /// </summary>
        public UnityEvent onWalletClosed;

        /// <summary>
        /// Called when the Wallet is ready to connect.
        /// Do not interact with the Wallet class until it's called, or until readyToConnect is true.
        /// </summary>
        public UnityEvent onReadyToConnect;
        public bool readyToConnect = false;

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

#if IS_EDITOR_OR_NOT_WEBGL
        private CanvasWebViewPrefab _walletWindow;
        private IWebView _internalWebView;
#else
        [DllImport("__Internal")]
        private static extern void Sequence_ExecuteJSInBrowserContext(string js);
#endif

        private bool _walletVisible = true;
        private ulong _callbackIndex;
        private IDictionary<ulong, TaskCompletionSource<string>> _callbackDict = new Dictionary<ulong, TaskCompletionSource<string>>();

        private void Awake()
        {
#if IS_EDITOR_OR_NOT_WEBGL
            if (enableRemoteDebugging)
            {
                Web.EnableRemoteDebugging();
            }
            Web.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 UnitySequence ");
            _walletWindow = CanvasWebViewPrefab.Instantiate();
            _walletWindow.transform.SetParent(this.transform);

            // set Widget to full-size of parent
            var rect = _walletWindow.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localPosition = Vector3.zero;
            rect.localScale = new Vector2(1, 1);

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

            Application.deepLinkActivated += DeepLinkCallback;
            
#if UNITY_STANDALONE_WIN
            // Run a TCP server on Windows standalone to get the auth token from the other instance.
            /**
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****

            Do not add code to this TCP server/client without thinking very carefully; it's easy to get driveby exploited since this is a TCP server any application can talk to.
            Do not increase the attack surface without careful thought.

            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****

            **/
            var syncContext = SynchronizationContext.Current;
            var ipcListener = new Thread(() =>
            {
                var socketConnection = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), WINDOWS_IPC_PORT);
                socketConnection.Start();
                var bytes = new System.Byte[1024];
                var msg = "";
                while (true)
                {
                    using (var connectedTcpClient = socketConnection.AcceptTcpClient())
                    {
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var data = new byte[length];
                                System.Array.Copy(bytes, 0, data, 0, length);
                                // Convert byte array to string message. 							
                                string clientMessage = System.Text.Encoding.ASCII.GetString(data);
                                if (clientMessage.StartsWith("@@@@"))
                                {
                                    msg = clientMessage.Replace("@@@@", "").Replace("$$$$", "");
                                }
                                else
                                {
                                    msg += clientMessage.Replace("$$$$", "");
                                }

                                if (msg.Length > 8192)
                                {
                                    // got some weird garbage, toss it to avoid memory leaks.
                                    msg = "";
                                }

                                if (clientMessage.EndsWith("$$$$"))
                                {
                                    syncContext.Post((data) =>
                                    {
                                        DeepLinkCallback((string)data);

                                    }, msg);
                                }

                            }
                        }
                    }
                }
            });
            ipcListener.IsBackground = true;
            ipcListener.Start();

#endif
#endif
            _HideWallet();
        }
#if IS_EDITOR_OR_NOT_WEBGL
        private void DeepLinkCallback(string link)
        {
            if (!link.Contains("://mobile.skyweaver.net/auth#"))
            {
                _SequenceDebugLogError("Invalid deep link " + link);
                // random deep link, we don't care
                return;
            }
            var authParam = link.Split("://mobile.skyweaver.net/auth#");
            if (authParam.Length != 2)
            {
                _SequenceDebugLogError("Invalid deep link " + link);
                return;
            }
            // use href change so JS can detect it 
            var authUrl = "window.location.href = window.location.protocol +'//' + window.location.hostname + (window.location.port ? (':' + window.location.port) : '') + '/auth#" + authParam[1] + "'";
            _walletWindow.WebView.ExecuteJavaScript(authUrl);
        }
#endif
        public async void Start()
        {

            await Initialize(providerConfig);
        }

        async Task<string> LoadFileFromStreamingAssets(string filename)
        {
            var path = Path.Combine(Application.streamingAssetsPath, filename);
            if (path.Contains("://"))
            {
                var req = UnityWebRequest.Get(path);
                await req.SendWebRequest();
                var text =
                 req.downloadHandler.text;
                req.Dispose();
                return text;
            }
            else
            {
                return await File.ReadAllTextAsync(path);
            }
        }

        public async Task Initialize(ProviderConfig providerConfig)
        {
            _HideWallet();
            //Load Asset Locally Test
            var loadedAsset = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath + "/SequenceAssetBundles/", "sequencebundle"));
            if (loadedAsset == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            Debug.Log("loaded asset" + loadedAsset);

            var assets = loadedAsset.LoadAllAssets();
            

            //End Test
#if IS_EDITOR_OR_NOT_WEBGL
            await _internalWebView.Init(1, 1);

            _internalWebView.SetRenderingEnabled(false);

            _internalWebView.LoadUrl("streaming-assets://sequence/sequence.html");
            await _internalWebView.WaitForNextPageLoadToFinish();
#endif
            var sequenceJS = await LoadFileFromStreamingAssets("sequence/sequence.js");


            var ethersJS = await LoadFileFromStreamingAssets("sequence/ethers.js");
            _InternalRawExecuteJS(sequenceJS + ";" + ethersJS);

#if IS_EDITOR_OR_NOT_WEBGL
            var internalWebViewWithPopups = _internalWebView as IWithPopups;
            if (internalWebViewWithPopups == null)
            {
                throw new IOException("Failed to catch popups from JS controller webview!");
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
#endif

            await ExecuteSequenceJS(@"
                window.seq = window.sequence.sequence;
                window.seq.initWallet(
                    '" + providerConfig.defaultNetworkId + @"',
                    {
                        walletAppURL: '" + providerConfig.walletAppURL + "'," +
#if IS_EDITOR_OR_NOT_WEBGL
                        "transports: { unrealTransport: { enabled: true } } " +
#endif
                    @"}
                );
            ");


#if IS_EDITOR_OR_NOT_WEBGL
            await ExecuteSequenceJS(@"
                window.seq.getWallet().on('close', () => {
                    window.ue.sequencewallettransport.sendmessagetowallet('wallet_closed')
                });
                window.ue.sequencewallettransport.sendmessagetowallet('initialized');
            ");

            await _walletWindow.WaitUntilInitialized();
            _walletWindow.WebView.LoadHtml("<style>*{background:black;}</style>");
            // Doesn't work in mobile webviews :D
#if VUPLEX_STANDALONE && (UNITY_STANDALONE || UNITY_EDITOR)
#nullable enable
            Dictionary<string, HttpBasicAuthCreds>? creds = null;
            try
            {
                var credsText = await LoadFileFromStreamingAssets("sequence/httpBasicAuth.json");
                creds = JsonConvert.DeserializeObject<Dictionary<string, HttpBasicAuthCreds>>(credsText);
            }
            catch
            {
                _SequenceDebugLog("No HTTP Basic Auth credentials provided.");
            }
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
                throw new IOException("Failed to catch popups from JS controller webview!");
            }
            walletWithPopups.SetPopupMode(PopupMode.NotifyWithoutLoading);
            // Open external links in the native browser.
            walletWithPopups.PopupRequested += (_webview, p) =>
            {
                if (!p.Url.StartsWith("http"))
                {
                    _SequenceDebugLog("Sequence prevented opening non-web external link " + p.Url);
                    return;
                }
                Application.OpenURL(p.Url);
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
                if('sequenceStartWalletWebapp' in window) {
                    window.sequenceStartWalletWebapp();
                }
            ");

            _walletWindow.WebView.MessageEmitted += (sender, eventArgs) =>
            {
                _internalWebView.PostMessage(eventArgs.Value);
            };
#else
            await ExecuteSequenceJS(@"
                window.seq.getWallet().on('close', () => {
                console.log('closing wallet');
                    SendMessage('" + this.name + @"', 'SequenceJSWalletClosed');
                });
            ");
#endif

            onReadyToConnect.Invoke();
            readyToConnect = true;
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

#if IS_EDITOR_OR_NOT_WEBGL
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
#else
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
                    SendMessage('" + this.name + @"', 'SequenceJSFunctionReturn', returnString);
                 } catch(err) {
                    const returnString = JSON.stringify({
                        type: 'error',
                        callbackNumber: " + thisCallbackIndex + @",
                        returnValue: JSON.stringify(Object.fromEntries(Object.getOwnPropertyNames(err).map(prop => [JSON.stringify(prop), JSON.stringify(err[prop])])))
                    })
                    SendMessage('" + this.name + @"', 'SequenceJSFunctionError', returnString);
                 }
            })()
            }
        ";
#endif
            _InternalRawExecuteJS(jsToRun);
            return jsPromiseResolved.Task;
        }
        void _InternalRawExecuteJS(string js)
        {
#if IS_EDITOR_OR_NOT_WEBGL
            _internalWebView.ExecuteJavaScript(js);
#else
            Sequence_ExecuteJSInBrowserContext(js);
#endif
        }

#if !IS_EDITOR_OR_NOT_WEBGL
        public void SequenceJSWalletClosed() {
            _HideWallet();
        }
        public void SequenceJSFunctionReturn(string returnVal)
        {
            var promiseReturn = JsonConvert.DeserializeObject<PromiseReturn>(returnVal);

            _callbackDict[promiseReturn.callbackNumber].TrySetResult(promiseReturn.returnValue);
            _callbackDict.Remove(promiseReturn.callbackNumber);
        }
        public void SequenceJSFunctionError(string returnVal)
        {
            var promiseReturn = JsonConvert.DeserializeObject<PromiseReturn>(returnVal);

            _callbackDict[promiseReturn.callbackNumber].TrySetException(new JSExecutionException(promiseReturn.returnValue));
            _callbackDict.Remove(promiseReturn.callbackNumber);
        }
#endif

        /// <summary>
        /// Runs an arbitrary JS string and parses the result into a class.
        /// </summary>
        /// <typeparam name="T">The type to parse into</typeparam>
        /// <param name="js">The JS to execute. Use `return` to return.</param>
        /// <returns>The JSON parsed into `T`, or `null` if parsing fails.</returns>
        public async Task<T> ExecuteSequenceJSAndParseJSON<T>(string js)
        {
            var jsonString = await ExecuteSequenceJS(js);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Connect to the user's wallet.
        /// Will return immediately with ConnectDetails if the user is already connected,
        /// otherwise it will open the wallet with a connection prompt.
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="JSExecutionException">Thrown if the user declines the connection.</exception>
        public Task<ConnectDetails> Connect(ConnectOptions options)
        {
#if !UNITY_WEBGL
            // Don't ever set appProtocol in WebGL.
            if (options.appProtocol == null && appProtocol.Length > 0)
            {
                options.appProtocol = appProtocol;
            }

            // In non-webgl builds, unless you have an app protocol set up, only email signin works.
            if (options.settings.signInOptions == null && options.appProtocol == null)
            {
                _SequenceDebugLogError("Set up an appProtocol in the Wallet prefab to enable social signin. Until you do, only email signin will be supported in non-webGL builds.");
                options.settings.signInOptions = new string[] { "email" };
            }
#endif
            if (options.appProtocol != null)
            {
#if UNITY_EDITOR
                _SequenceDebugLog("You set up an appProtocol, but inside the Unity Editor, only email signin is supported. These signin methods will work in standalone builds.");
                options.settings.signInOptions = new string[] { "email" };
#elif UNITY_STANDALONE_OSX
                // ensure our URL protocol handler is registered - MacOS sometimes doesn't pick it up from Info.plist.
                var appPath = System.IO.Directory.GetParent(Application.dataPath);
                var command = new System.Diagnostics.ProcessStartInfo();
                command.FileName = "/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister";
                command.Arguments = " -R -f " + appPath;
                command.UseShellExecute = false;
                command.CreateNoWindow = true;
                System.Diagnostics.Process.Start(command);
#elif UNITY_STANDALONE_WIN
                // Register a Windows URL protocol handler in the Windows Registry.
                var appPath = Path.GetFullPath(Application.dataPath.Replace("_Data", ".exe"));
                string[] commands = new string[]{
                    $"add HKEY_CURRENT_USER\\Software\\Classes\\{options.appProtocol} /t REG_SZ /d \"URL:Sequence Login for {Application.productName}\" /f",
                    $"add HKEY_CURRENT_USER\\Software\\Classes\\{options.appProtocol} /v \"URL Protocol\" /t REG_SZ /d \"\" /f",
                    $"add HKEY_CURRENT_USER\\Software\\Classes\\{options.appProtocol}\\shell /f",
                    $"add HKEY_CURRENT_USER\\Software\\Classes\\{options.appProtocol}\\shell\\open /f",
                    $"add HKEY_CURRENT_USER\\Software\\Classes\\{options.appProtocol}\\shell\\open\\command /t REG_SZ /d \"\\\"{appPath}\\\" \\\"%1\\\"\" /f",
                };
                foreach(var args in commands) {
                    var command = new System.Diagnostics.ProcessStartInfo();
                    command.FileName = "C:\\Windows\\System32\\reg.exe";
                    command.Arguments = args;
                    command.UseShellExecute = false;
                    command.CreateNoWindow = true;
                    System.Diagnostics.Process.Start(command);
                }
#endif
            }

            return ExecuteSequenceJSAndParseJSON<ConnectDetails>("return seq.getWallet().connect(" + ObjectToJson(options) + ");");
        }

        /// <summary>
        /// Check if the user's wallet is connected.
        /// </summary>
        /// <returns>Whether or not the wallet is connected.</returns>
        public Task<bool> IsConnected()
        {
            return ExecuteSequenceJSAndParseJSON<bool>("return seq.getWallet().isConnected();");
        }

        /// <summary>
        /// Disconnect the user's wallet.
        /// </summary>
        public async Task Disconnect()
        {
            await ExecuteSequenceJS("return seq.getWallet().disconnect();");
        }
        /// <summary>
        /// Get the connected wallet's address.
        /// </summary>
        /// <exception cref="JSExecutionException">Thrown if the wallet isn't connected.</exception>
        public Task<string> GetAddress()
        {
            return ExecuteSequenceJSAndParseJSON<string>("return seq.getWallet().getSigner().getAddress();");
        }

#nullable enable
        /// <summary>
        /// Get the connected wallet's networks.
        /// </summary>
        /// <exception cref="JSExecutionException">Thrown if the wallet isn't connected.</exception>
        public Task<NetworkConfig[]> GetNetworks(string? chainId = null)
        {
            return ExecuteSequenceJSAndParseJSON<NetworkConfig[]>(@"return seq
                .getWallet()
                .getNetworks(" +
                   (chainId == null ? "'" + chainId + "'" : "") +
                ");");
        }
#nullable disable
        /// <summary>
        /// Get the connected wallet's chain ID.
        /// </summary>
        /// <exception cref="JSExecutionException">Thrown if the wallet isn't connected.</exception>
        public Task<ulong> GetChainId()
        {
            return ExecuteSequenceJSAndParseJSON<ulong>("return seq.getWallet().getChainId();");
        }

        /// <returns>The ID of the wallet's Auth Chain.</returns>
        /// <exception cref="JSExecutionException">Thrown if the wallet isn't connected.</exception>
        public Task<ulong> GetAuthChainId()
        {
            return ExecuteSequenceJSAndParseJSON<ulong>("return seq.getWallet().getAuthChainId();");
        }

        /// <returns>Whether or not the wallet is currently open.</returns>
        public Task<bool> IsOpened()
        {
            return ExecuteSequenceJSAndParseJSON<bool>("return seq.getWallet().isOpened();");
        }

#nullable enable
        /// <summary>
        /// Opens the wallet. Configurable with settings &amp; wallet pages.
        /// </summary>
        /// <param name="path">A URL path, e.g. wallet/add-funds</param>
        /// <param name="options">Wallet or Connect settings, for theming etc.</param>
        /// <param name="networkId">The network/chain the wallet will be opened to.</param>
        /// <returns>Whether or not the wallet opened</returns>
        public Task<bool> OpenWallet(string? path, ConnectOptions? options, string? networkId)
        {
            var pathJson = path == null ? "undefined" : "'" + path + "'";
            var optionsJson = options == null ? "undefined" : "{ type: 'openWithOptions', options: " + ObjectToJson(options) + "}";
            var networkIdJson = networkId ?? "undefined";
            return ExecuteSequenceJSAndParseJSON<bool>("return seq.getWallet().openWallet("
                + pathJson + ","
                + optionsJson + ","
                + networkIdJson +
            ");");
        }
#nullable disable

        /// <summary>
        /// Closes the wallet, if it's open.
        /// </summary>
        public async Task CloseWallet()
        {
            await ExecuteSequenceJS("return seq.getWallet().closeWallet();");
            _HideWallet();
        }

#nullable enable
        /// <summary>
        /// Get the connected wallet's session.
        /// </summary>
        /// <exception cref="JSExecutionException">Thrown if the wallet isn't connected.</exception>
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
        private static void _SequenceDebugLog(string message)
        {
            Debug.Log("<color=#a340f5>[Sequence]</color> " + message);
        }
        private static void _SequenceDebugLogError(string message)
        {
            Debug.LogError("<color=#f54073>[Sequence Error]</color> " + message);
        }

        private void _HideWallet()
        {
            if (_walletVisible)
            {
                onWalletClosed.Invoke();
                _walletVisible = false;
#if IS_EDITOR_OR_NOT_WEBGL
                // blank it out!
                if (_walletWindow.WebView != null)
                {
                    _walletWindow.WebView.LoadHtml("<style>*{background:black;}</style>");
                }
#endif
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

        // On Windows standalone, deep link will open a second instance of tghe game.
        // We need to catch this, and send our deep link URL to the already-running instance (through a TCP server)
#if UNITY_STANDALONE_WIN
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            _SequenceDebugLog("Absolute URL: " + Application.absoluteURL);
            var args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1].Contains("://"))
            {
                var socketConnection = new TcpClient("localhost", WINDOWS_IPC_PORT);
                var bytes = System.Text.Encoding.ASCII.GetBytes("@@@@" + args[1] + "$$$$");
                socketConnection.GetStream().Write(bytes, 0, bytes.Length);
                socketConnection.Close();
                Application.Quit();
            }
        }
#endif
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
        public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation webReqOp)
        {
            var tcs = new TaskCompletionSource<object>();
            webReqOp.completed += obj =>
            {
                {
                    if (webReqOp.webRequest.responseCode != 200)
                    {
                        tcs.SetException(new FileLoadException(webReqOp.webRequest.error));
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                }
            };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}