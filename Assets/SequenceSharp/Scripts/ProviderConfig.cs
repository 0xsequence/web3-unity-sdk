

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