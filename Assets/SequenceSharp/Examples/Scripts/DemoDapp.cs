using Newtonsoft.Json;
using SequenceSharp;
using UnityEngine;
using UnityEngine.UI;
public class DemoDapp : MonoBehaviour
{
    [SerializeField] private Wallet wallet;
    [SerializeField] private CanvasGroup walletContainer;

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

    //Exit
    [Header("Exit")]
    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        ShowButtons(false);

        //exit 
        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
            wallet.onWalletOpened.AddListener(() =>
        {
            walletContainer.alpha = 1f;
            walletContainer.interactable = true;
            walletContainer.blocksRaycasts = true;
        });
        wallet.onWalletClosed.AddListener(() =>
        {

            walletContainer.alpha = 0f;
            walletContainer.interactable = false;
            walletContainer.blocksRaycasts = false;
        });
        //wallet ready
        wallet.readyToConnectEvent.AddListener(()=>
        {
            ShowButtons(true);
        });

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
                    bannerUrl = "https://placekitten.com/1200/400",
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

        //simulation
        estimateUnwrapGasBtn.onClick.AddListener(async () =>
        {
            var estimate = await wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet()

                const wmaticContractAddress = '0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270'
                const wmaticInterface = new ethers.utils.Interface(['function withdraw(uint256 amount)'])
                // tx :  sequence.transactions.Transaction 
                const tx= {
                  to: wmaticContractAddress,
                  data: wmaticInterface.encodeFunctionData('withdraw', ['1000000000000000000'])
                }

                const provider = wallet.getProvider()
                if(provider)
                {
                    const estimate = await provider.estimateGas(tx);                   
                    return estimate.toString();
                }
            ");

            Debug.Log("[DemoDapp] estimated gas needed for wmatic withdrawal: " + estimate);
        });
        //transaction
        sendOnDefaultChainBtn.onClick.AddListener(async () =>
        {
            var txnResponse = await wallet.ExecuteSequenceJS(@"


                const signer = seq.getWallet().getSigner();

                const toAddress =ethers.Wallet.createRandom().address;

                    //tx1 : sequence.transactions.Transaction
                const tx1 = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: toAddress,
                    value: ethers.utils.parseEther('1.234'),
                    data: '0x'
                }
                //tx2 : sequence.transactions.Transaction
                const tx2 = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: toAddress,
                    value: ethers.utils.parseEther('0.4242'),
                    data: '0x'
                }
                        
                const provider = signer.provider;
                console.log(`balance of ${toAddress}, before:`, await provider.getBalance(toAddress))
                const txnResponse = await signer.sendTransactionBatch([tx1, tx2])
                console.log(`balance of ${toAddress}, after:`, await provider.getBalance(toAddress))
                return txnResponse;

                    ");
            Debug.Log("[DemoDapp] txnResponse: " + txnResponse);
        });

        sendOnAuthChainBtn.onClick.AddListener(async () =>
        {
            var txnResponse = await wallet.ExecuteSequenceJS(@"
     
                const wallet = seq.getWallet();
                const networks = await wallet.getNetworks()
                const n = networks.find(n => n.isAuthChain)
                const signer = wallet.getSigner(n)

                const toAddress = ethers.Wallet.createRandom().address
                //tx1 : sequence.transactions.Transaction
                const tx1 = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: toAddress,
                    value: ethers.utils.parseEther('1.234'),
                    data: '0x'
                }
                //tx2 : sequence.transactions.Transaction
                const tx2 = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: toAddress,
                    value: ethers.utils.parseEther('0.4242'),
                    data: '0x'
                }
                        
                const provider = signer.provider;
                console.log(`balance of ${toAddress}, before:`, await provider.getBalance(toAddress))
                const txnResponse = await signer.sendTransactionBatch([tx1, tx2])
                console.log(`balance of ${toAddress}, after:`, await provider.getBalance(toAddress))

                return txnResponse;

                    ");
            Debug.Log("[DemoDapp] txnResponse: " + txnResponse);
        });
        sendDAIBtn.onClick.AddListener(async () =>
        {
            var txnResponse = await wallet.ExecuteSequenceJS(@"
                const ERC_20_ABI = [
                {
                    constant: false,
                    inputs: [
                            {
                                internalType: 'address',
                                name: 'recipient',
                                type: 'address'
                            },
                            {
                                internalType: 'uint256',
                                name: 'amount',
                                type: 'uint256'
                            }
                            ],
                    name: 'transfer',
                    outputs: [
                            {
                                internalType: 'bool',
                                name: '',
                                type: 'bool'
                            }
                            ],
                    payable: false,
                    stateMutability: 'nonpayable',
                    type: 'function'
                }
                ]
                const signer = seq.getWallet().getSigner();

                const toAddress = ethers.Wallet.createRandom().address;

                const amount = ethers.utils.parseUnits('5', 18);

                const daiContractAddress = '0x8f3Cf7ad23Cd3CaDbD9735AFf958023239c6A063'; // (DAI address on Polygon)
    
                const tx = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: daiContractAddress,
                    value: 0,
                    data: new ethers.utils.Interface(ERC_20_ABI).encodeFunctionData('transfer', [toAddress, amount.toHexString()])
                }

                const txnResponse = await signer.sendTransactionBatch([tx]);

                return txnResponse;

                ");
            Debug.Log("[DemoDapp] txnResponse: " + txnResponse);
        });

        sendERC1155Btn.onClick.AddListener(() =>
           {
               //todo
               Debug.Log("Todo");
           });
        sendOnRinkebyBtn.onClick.AddListener(async () =>
        {
            await wallet.ExecuteSequenceJS(@"
            const ERC_20_ABI = [
            {
                constant: false,
                inputs: [
                {
                    internalType: 'address',
                    name: 'recipient',
                    type: 'address'
                },
                {
                    internalType: 'uint256',
                    name: 'amount',
                    type: 'uint256'
                }
                ],
                name: 'transfer',
                outputs: [
                {
                    internalType: 'bool',
                    name: '',
                    type: 'bool'
                }
                ],
                payable: false,
                stateMutability: 'nonpayable',
                type: 'function'
            }];
                const signer = seq.getWallet().getSigner();

                const toAddress = ethers.Wallet.createRandom().address;

                const amount = ethers.utils.parseUnits('1', 1);

                const daiContractAddress = '0x4DBCdF9B62e891a7cec5A2568C3F4FAF9E8Abe2b'; // (USDC address on Rinkeby)
                //tx : sequence.transactions.Transaction
                const tx = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: daiContractAddress,
                    value: 0,
                    data: new ethers.utils.Interface(ERC_20_ABI).encodeFunctionData('transfer', [toAddress, amount.toHexString()])
                }

                const txnResp = await signer.sendTransactionBatch([tx], 4);

                return txnResponse;");

        });
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
            Debug.Log("contract example: " + "symbol: " + signer[0] + "balance: " + signer[1]);

        });

        fetchTokenBalanceAndMetadataBtn.onClick.AddListener(async () =>
        {

            string accountAddress = await wallet.GetAddress();
            Debug.Log("[DemoDapp] accountAddress " + accountAddress);

            GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, true);
            BlockChainType blockChainType = BlockChainType.Polygon;

            Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs, (tokenBalances) =>
            {
                Debug.Log(tokenBalances);
            });


        });
    }

    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_WEBGL
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.scaleFactor = 1.0f;
#endif
    }
    void ShowButtons(bool show)
    {
        var buttons = GetComponentsInChildren<Button>();
        foreach(Button btn in buttons)
        {
            btn.enabled = show;
        }
    }


}
