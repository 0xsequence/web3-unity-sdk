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

    private void OnEnable()
    {
        Web.EnableRemoteDebugging();
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
            Debug.Log("main window requested a a popup with url" + eventArgs.Url);
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
            window.vuplex.addEventListener('message', event => window.ue.sequencewallettransport.onmessagefromwallet(event.data));

            window.seq = window.sequence.sequence;
            window.seq.initWallet(
                '" + providerConfig.defaultNetworkId + @"',
                {
                    walletAppURL:'" + providerConfig.walletAppURL + @"',
                    transports: { unrealTransport: { enabled: true } }
                }
            );
        ");

        internalWebView.MessageEmitted += (sender, eventArgs) =>
        {
            Debug.Log("got message from internal: " + eventArgs.Value);
            walletWindow.WebView.PostMessage(eventArgs.Value);
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
            //todo open real browser window at that URL
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
            window.vuplex.addEventListener('message', event => window.ue.sequencewallettransport.onmessagefromsequencejs(event.data));
        ");

        walletWindow.WebView.MessageEmitted += (sender, eventArgs) =>
        {
            Debug.Log("got message from wallet: " + eventArgs.Value);
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