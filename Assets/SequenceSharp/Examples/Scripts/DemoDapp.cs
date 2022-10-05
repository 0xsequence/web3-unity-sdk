using Newtonsoft.Json;
using SequenceSharp;
using UnityEngine;
using UnityEngine.UI;
public class DemoDapp : MonoBehaviour
{
    [SerializeField] private Wallet wallet;

    //Connection
    [Header("Connection")]
    [SerializeField] private Button connectBtn;
    [SerializeField] private Button connectAndAuthBtn;
    [SerializeField] private Button connectWithSettingsBtn;
    [SerializeField] private Button disconnectBtn;
    [SerializeField] private Button openWalletBtn;
    [SerializeField] private Button openWalletWithSettingsBtn;
    [SerializeField] private Button closeWalletBtn;
    [SerializeField] private Button isConnectedBtn;
    [SerializeField] private Button isOpenedBtn;
    [SerializeField] private Button defaultChainBtn;
    [SerializeField] private Button authChainBtn;

    //State
    [Header("State")]
    [SerializeField] private Button chainIDBtn;
    [SerializeField] private Button networksBtn;
    [SerializeField] private Button getAccountsBtn;
    [SerializeField] private Button getBalanceBtn;
    [SerializeField] private Button getWalletStateBtn;

    //Signing
    [Header("Signing")]
    [SerializeField] private Button estimateUnwrapGasBtn;
    //Simulation
    [Header("Simulation")]
    [SerializeField] private Button sendOnDefaultChainBtn;
    [SerializeField] private Button sendOnAuthChainBtn;
    [SerializeField] private Button sendDAIBtn;
    [SerializeField] private Button sendERC1155Btn;
    [SerializeField] private Button sendOnRinkebyBtn;
    //Transactions
    [Header("Transactions")]
    [SerializeField] private Button contractExampleBtn;
    [SerializeField] private Button fetchTokenBalanceAndMetadataBtn;

    private void Start()
    {
        //connection
        connectBtn.onClick.AddListener(async () =>
        {
            var connectDetails = await wallet.Connect(new ConnectOptions
            {
                app = "Demo Unity Dapp"
            });
            Debug.Log("[DemoDapp] Connect Details:  " + JsonConvert.SerializeObject(connectDetails, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        });

        connectAndAuthBtn.onClick.AddListener(async () =>
        {
            var connectDetails = await wallet.Connect(new ConnectOptions
            {
                app = "Demo Unity Dapp",
                authorize = true
            });
            Debug.Log("[DemoDapp] Connect and Auth Details:  " + connectDetails);
        });
        connectWithSettingsBtn.onClick.AddListener(async () =>
        {
            var connectDetails = await wallet.Connect(new ConnectOptions
            {
                app = "Demo Unity Dapp",
                settings = new WalletSettings
                {
                    theme = "indigoDark",
                    bannerURL = "https://placekitten.com/1200/400",
                    includedPaymentProviders = new string[] { PaymentProviderOption.Moonpay },
                    defaultFundingCurrency = CurrencyOption.Matic,
                    defaultPurchaseAmount = 111
                }
            });
            Debug.Log("[DemoDapp] Connect With Settings Details:  " + connectDetails);
        });

        disconnectBtn.onClick.AddListener(async () =>
        {
            await wallet.Disconnect();
            Debug.Log("[DemoDapp] Disconnected.");
        });

        openWalletBtn.onClick.AddListener(async () =>
        {
            await wallet.OpenWallet(null, null, null);
            Debug.Log("[DemoDapp] Wallet Opened.");
        });

        openWalletWithSettingsBtn.onClick.AddListener(async () =>
        {
            await wallet.OpenWallet("wallet/add-funds", new ConnectOptions
            {
                settings = new WalletSettings
                {
                    theme = "goldDark",
                    includedPaymentProviders = new string[] { PaymentProviderOption.Moonpay, PaymentProviderOption.Ramp, PaymentProviderOption.Wyre },
                    defaultFundingCurrency = CurrencyOption.Ether,
                    defaultPurchaseAmount = 400,
                    lockFundingCurrencyToDefault = false

                }
            }, null);
            Debug.Log("[DemoDapp] Wallet Opened with settings.");

        });

        closeWalletBtn.onClick.AddListener(async () =>
        {
            await wallet.CloseWallet();
            Debug.Log("[DemoDapp] Wallet Closed!");

        });

        isConnectedBtn.onClick.AddListener(async () =>
        {
            var isConnected = await wallet.IsConnected();
            Debug.Log("[DemoDapp] Is connected? " + isConnected);
        });


        isOpenedBtn.onClick.AddListener(async () =>
        {
            var isOpened = await wallet.IsOpened();
            Debug.Log("[DemoDapp] Is opened? " + isOpened);
        });

        defaultChainBtn.onClick.AddListener(() =>
        {
            Debug.Log("TODO");
        });


        authChainBtn.onClick.AddListener(async () =>
        {
            var authChainId = await wallet.GetAuthChainId();
            Debug.Log("[DemoDapp] Auth Chain ID:  " + authChainId);
        });

        chainIDBtn.onClick.AddListener(async () =>
        {
            var chainId = await wallet.GetChainId();
            Debug.Log("[DemoDapp] Chain ID:  " + chainId);
        });

        //signing
        networksBtn.onClick.AddListener(async () =>
        {
            var networks = await wallet.GetNetworks(null);
            Debug.Log("[DemoDapp] Networks :  " + JsonConvert.SerializeObject(networks, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        });

        getAccountsBtn.onClick.AddListener(async () =>
        {
            var accounts = await wallet.ExecuteSequenceJSAndParseJSON<string[]>("return seq.getWallet().getProvider().listAccounts();");
            Debug.Log("[DemoDapp] Accounts :  " + string.Join(", ", accounts));
        });

        getBalanceBtn.onClick.AddListener(async () =>
        {
            var balanceChk1 = await wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();
                const provider = wallet.getProvider();
                const account = await wallet.getAddress();
                const bal = await provider.getBalance(account)
                return bal.toString();
            ");
            Debug.Log("[DemoDapp] Balance Check 1 " + balanceChk1);

            var balanceChk2 = await wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();
                const signer = wallet.getSigner();
                const bal = await signer.getBalance().toString();
                return bal.toString();
            ");
            Debug.Log("[DemoDapp] Balance Check 2 " + balanceChk2);
        });

        getWalletStateBtn.onClick.AddListener(async () =>
        {
            var walletState = await wallet.ExecuteSequenceJS("return seq.getWallet().getSigner().getWalletState();");
            Debug.Log("[DemoDapp] Wallet State: " + walletState);
        });
        /*
        //simulation
        /*        estimateUnwrapGasBtn.onClick.AddListener(Sequence.Instance.EstimateUnwrapGas);*/
        //transaction
        /*        sendOnDefaultChainBtn.onClick.AddListener(Sequence.Instance.SendETH);
                sendOnAuthChainBtn.onClick.AddListener(Sequence.Instance.SendETHSidechain);
                sendDAIBtn.onClick.AddListener(Sequence.Instance.SendDAI);
                sendERC1155Btn.onClick.AddListener(Sequence.Instance.Send1155Tokens);
                sendOnRinkebyBtn.onClick.AddListener(Sequence.Instance.SendRinkebyUSDC);*/
        //various
        contractExampleBtn.onClick.AddListener(async () =>
        {
            string[] signer = await wallet.ContractExample(@"
                
                const abi = [
                'function balanceOf(address owner) view returns (uint256)',
                'function decimals() view returns (uint8)',
                'function symbol() view returns (string)',

                'function transfer(address to, uint amount) returns (bool)',

                'event Transfer(address indexed from, address indexed to, uint amount)'
                ]
                var signer = seq.getWallet().getSigner();
                address = '0x2791bca1f2de4661ed88a30c99a7a9449aa84174';
                const usdc = new ethers.Contract(address, abi, signer);

                
                const symbol = await usdc.symbol();
                console.log('Token symbol:',symbol);
                const balance = await usdc.balanceOf(await signer.getAddress());
                console.log('Token Balance', balance.toString());
                contractExample =  [symbol, balance.toString()];
                return contractExample;
            ");
            Debug.Log("contract example: " + "symbol: "+ signer[0]+ "balance: "+signer[1]);

        });

        fetchTokenBalanceAndMetadataBtn.onClick.AddListener(async () =>
        {
            
            string accountAddress = await wallet.GetAddress();
            Debug.Log("[DemoDapp] accountAddress " + accountAddress);

            GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, "", true);
            BlockChainType blockChainType = BlockChainType.Polygon;

            Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs, (tokenBalances) =>
            {
                Debug.Log(tokenBalances);
            });


        });
    }


}
