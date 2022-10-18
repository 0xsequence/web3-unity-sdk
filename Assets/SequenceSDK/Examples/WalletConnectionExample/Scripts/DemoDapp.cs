using Newtonsoft.Json;
using SequenceSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    [SerializeField] private Button sendUSDCButton;
    [SerializeField] private Button sendERC1155Btn;
    [SerializeField] private Button sendOnRinkebyBtn;
    //Transactions
    [Header("Transactions")]
    [SerializeField] private Button contractExampleBtn;
    [SerializeField] private Button fetchTokenBalanceAndMetadataBtn;

    //Exit
    [Header("Exit")]
    [SerializeField] private Button exitBtn;

    //Debugging
    [Header("Debug")]
    [SerializeField] private TMP_Text debugWindowText;
    [SerializeField] private ScrollRect debugScrollRect;
    


    private void Awake()
    {
 
        //exit 
        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
            wallet.onWalletOpened.AddListener(() =>
        {
            UnityMainThread.wkr.AddJob(() =>
            {
                walletContainer.alpha = 1f;
                walletContainer.interactable = true;
                walletContainer.blocksRaycasts = true;
            });
        });
        wallet.onWalletClosed.AddListener(() =>
        {
            UnityMainThread.wkr.AddJob(() =>
            {
                walletContainer.alpha = 0f;
                walletContainer.interactable = false;
                walletContainer.blocksRaycasts = false;
            });
        });
        //wallet ready
        wallet.onReadyToConnect.AddListener(()=>
        {
            ShowButtons(true);
        });


        //connection
        connectBtn.onClick.AddListener(async () =>
        {
            try
            {
                var connectDetails = await wallet.Connect(new ConnectOptions
                {
                    app = "Demo Unity Dapp"
                });
                Debug.Log("[DemoDapp] Connect Details:  " + JsonConvert.SerializeObject(connectDetails, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        });

        connectAndAuthBtn.onClick.AddListener(async () =>
        {
            try
            {
                var connectDetails = await wallet.Connect(new ConnectOptions
                {
                    app = "Demo Unity Dapp",
                    authorize = true
                });
                Debug.Log("[DemoDapp] Connect and Auth Details:  " + connectDetails);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });
        connectWithSettingsBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        disconnectBtn.onClick.AddListener(async () =>
        {
            try
            {
                await wallet.Disconnect();
                Debug.Log("[DemoDapp] Disconnected.");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        openWalletBtn.onClick.AddListener(async () =>
        {
            try
            {
                await wallet.OpenWallet(null, null, null);
                Debug.Log("[DemoDapp] Wallet Opened.");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        openWalletWithSettingsBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        });

        closeWalletBtn.onClick.AddListener(async () =>
        {
            try
            {
                await wallet.CloseWallet();
                Debug.Log("[DemoDapp] Wallet Closed!");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        isConnectedBtn.onClick.AddListener(async () =>
        {
            try
            {
                var isConnected = await wallet.IsConnected();
                Debug.Log("[DemoDapp] Is connected? " + isConnected);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });


        isOpenedBtn.onClick.AddListener(async () =>
        {
            try
            {
                var isOpened = await wallet.IsOpened();
                Debug.Log("[DemoDapp] Is opened? " + isOpened);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        defaultChainBtn.onClick.AddListener(() =>
        {
            Debug.Log("TODO");
        });


        authChainBtn.onClick.AddListener(async () =>
        {
            try { 
                var authChainId = await wallet.GetAuthChainId();
                Debug.Log("[DemoDapp] Auth Chain ID:  " + authChainId);
            }catch (Exception e)
            {
                Debug.Log(e);
            }
    });

        chainIDBtn.onClick.AddListener(async () =>
        {
            try
            {
                var chainId = await wallet.GetChainId();
                Debug.Log("[DemoDapp] Chain ID:  " + chainId);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        //signing
        networksBtn.onClick.AddListener(async () =>
        {
            try
            {
                var networks = await wallet.GetNetworks(null);
                Debug.Log("[DemoDapp] Networks :  " + JsonConvert.SerializeObject(networks, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        getAccountsBtn.onClick.AddListener(async () =>
        {
            try
            {
                var accounts = await wallet.ExecuteSequenceJSAndParseJSON<string[]>("return seq.getWallet().getProvider().listAccounts();");
                Debug.Log("[DemoDapp] Accounts :  " + string.Join(", ", accounts));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        getBalanceBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        getWalletStateBtn.onClick.AddListener(async () =>
        {
            try
            {
                var walletState = await wallet.ExecuteSequenceJS("return seq.getWallet().getSigner().getWalletState();");
                Debug.Log("[DemoDapp] Wallet State: " + walletState);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        //simulation
        estimateUnwrapGasBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });
        //transaction
        sendOnDefaultChainBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        sendOnAuthChainBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });
       /* sendUSDCButton.onClick.AddListener(async () =>
        {
            try
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

                const usdcContractAddress = '0x2791bca1f2de4661ed88a30c99a7a9449aa84174'; // (USDC address on Polygon)
    
                const tx = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: usdcContractAddress,
                    value: 0,
                    data: new ethers.utils.Interface(ERC_20_ABI).encodeFunctionData('transfer', [toAddress, amount.toHexString()])
                }

                const txnResponse = await signer.sendTransactionBatch([tx]);

                return txnResponse;

                ");
                Debug.Log("[DemoDapp] txnResponse: " + txnResponse);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });*/

        sendERC1155Btn.onClick.AddListener(() =>
           {
               //todo
               Debug.Log("Todo");
           });
        sendOnRinkebyBtn.onClick.AddListener(async () =>
        {
            try
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        });
        //various
        contractExampleBtn.onClick.AddListener(async () =>
        {
            try
            {
                string[] signer = await wallet.ExecuteSequenceJSAndParseJSON<string[]>(@"
                
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
                contractExample = [symbol, balance.toString()];
                return contractExample;
            ");
                Debug.Log("contract example: " + "symbol: " + signer[0] + "balance: " + signer[1]);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        });

        fetchTokenBalanceAndMetadataBtn.onClick.AddListener(async () =>
        {
            try
            {
                string accountAddress = await wallet.GetAddress();
                Debug.Log("[DemoDapp] accountAddress " + accountAddress);

                GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, true);
                BlockChainType blockChainType = BlockChainType.Polygon;

                var tokenBalances = await Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs);
                Debug.Log(tokenBalances);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        });
    }


    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_WEBGL
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.scaleFactor = 1.0f;
#endif
        ShowButtons(false);
    }

    

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }


    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        debugWindowText.text += logString+ System.Environment.NewLine;

        //debugWindowText.text += System.Environment.NewLine + stackTrace;
        Canvas.ForceUpdateCanvases();
        debugScrollRect.normalizedPosition = new Vector2(0, 0);
    }
    private void ShowButtons(bool show)
    {
        UnityMainThread.wkr.AddJob(() =>
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = show ? 1 : 0;
            canvasGroup.blocksRaycasts = show;
            canvasGroup.interactable = show;
        });
    }
}
