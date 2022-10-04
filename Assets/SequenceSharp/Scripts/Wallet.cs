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

        ExecuteSequenceJS("window.seq = window.sequence.sequence; window.seq.initWallet('" +
                                 providerConfig.defaultNetworkId + "', { walletAppURL:'" +
                                 providerConfig.walletAppURL + "', transports: { unrealTransport: { enabled: true } } });" +
                                 "window.seq.getWallet().on('close', () => window.ue.sequencewallettransport.callbackfromjs(0, 'wallet_closed')); window.ue.sequencewallettransport.callbackfromjs(0, 'initialized');");


        walletWindow.Visible = false;

        var hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
        hardwareKeyboardListener.KeyDownReceived += (sender, eventArgs) =>
        {
            walletWindow.WebView.SendKey(eventArgs.Value);
        };
    }
    public void ExecuteSequenceJS(string js)
    {
        internalWebView.ExecuteJavaScript(js);
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