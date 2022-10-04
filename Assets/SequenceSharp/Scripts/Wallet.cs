using Vuplex.WebView;
using UnityEngine;
using Vuplex.WebView.Demos;
using System.IO;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class Wallet : MonoBehaviour
{
    [SerializeField] private ProviderConfig providerConfig;
    [SerializeField] private CanvasWebViewPrefab walletWindow;
    private IWebView internalWebView;

    private void Awake()
    {
        Web.EnableRemoteDebugging();
        Web.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 UnitySequence");
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

        await ExecuteSequenceJS(@"
            window.ue = {
                sequencewallettransport: {
                    onmessagefromwallet: () => { /* will be overwritten by transport! */ },
                    sendmessagetowallet: (message) => {debugger;window.vuplex.postMessage(message)},
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
                window.ue.sequencewallettransport.callbackfromjs(0, 'wallet_closed')
            });
            window.ue.sequencewallettransport.callbackfromjs(0, 'initialized');
        ");

        internalWebView.MessageEmitted += (sender, eventArgs) =>
        {
            if (eventArgs.Value == "wallet_closed")
            {
                walletWindow.Visible = false;
            }
            else if (eventArgs.Value == "initialized")
            {
                Debug.Log("Sequence wallet initialized!");
            }
            else
            {
                //Debug.Log("sending message from sequence.js to wallet" + eventArgs.Value);
                walletWindow.WebView.PostMessage(eventArgs.Value);
            }
        };


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

    public Task<string> ExecuteSequenceJS(string js)
    {
        return internalWebView.ExecuteJavaScript(js);
    }
}



[System.Serializable]
public class ProviderConfig
{
    /// <summary>Sequence Wallet Webapp URL; default: https://sequence.app/ </summary>
    public string walletAppURL = "https://sequence.app/";

    /**
        <summary>
        A string like "polygon" or "137".
   
        The primary network of a dapp and
        the default network a
        provider will communicate to.
        </summary>
    */
    public string defaultNetworkId = "polygon";

    /* TODO maybe?
     *   // networks is a configuration list of networks used by the wallet. This list
  // is combined with the network list supplied from the wallet upon login,
  // and settings here take precedence such as overriding a relayer setting, or rpcUrl.
  networks?: Partial<NetworkConfig>[]

  // networkRpcUrl will set the provider rpcUrl of the default network
  networkRpcUrl?: string
    */
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