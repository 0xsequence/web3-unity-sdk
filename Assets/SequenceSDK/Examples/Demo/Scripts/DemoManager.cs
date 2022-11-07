using NBitcoin;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using SequenceSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;


public class DemoManager : MonoBehaviour
{
    [SerializeField] private SequenceSharp.Wallet wallet;

    [Header("Canvases")]
    [SerializeField] private GameObject connectCanvas;
    [SerializeField] private GameObject welcomeCanvas;
    [SerializeField] private GameObject addressCanvas;
    [SerializeField] private GameObject collectionCanvas;
    [SerializeField] private DemoUIManager uiManager;


    [Header("Connection")]
    [SerializeField] private Button connectBtn;

    [Header("Welcome Panel")]
    [SerializeField] private Button openWalletBtn;
    [SerializeField] private Button getAddressBtn;
    [SerializeField] private Button viewCollectionBtn;
    [SerializeField] private Button viewHistoryBtn;
    [SerializeField] private Button signMessageBtn;
    [SerializeField] private Button sendUSDCBtn;
    [SerializeField] private Button sendNFTBtn;
    [SerializeField] private Button disconnectBtn;
    [Header("Wallet")]
    [SerializeField] private Button closeWalletBtn;

    [Header("Metamask Test")]
    [SerializeField] private GameObject metamaskPanel;

    [SerializeField] private Button metamaskConnectBtn;
    [SerializeField] private Button metamaskSignMessageBtn;
    [SerializeField] private Button metamaskSendTransactionBtn;
    public Metamask metamask;


    [Header("Test method")]
    [SerializeField] private Button testingBtn;


    [Header("Collection")]
    [SerializeField] private Collection m_collection;
    [Header("AccountAddress")]
    [SerializeField] private AccountAddress m_address;

    private bool m_connected = false; //For UI Only

    private Web3 web3 = new Web3();
    public static DemoManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(wallet);
        
    }
    private void OnEnable()
    {
        //wallet initialize
        if (wallet.readyToConnect)
        {
            StartDemo();
        }
        else
        {
            wallet.onReadyToConnect.AddListener(StartDemo);
        }
        wallet.onWalletOpened.AddListener(DisplayCloseWalletButton);
        wallet.onWalletClosed.AddListener(DisplayWelcomePanel);

        connectBtn.onClick.AddListener(Connect);
        openWalletBtn.onClick.AddListener(OpenWallet);
        getAddressBtn.onClick.AddListener(GetAddress);
        viewCollectionBtn.onClick.AddListener(ViewCollection);
        viewHistoryBtn.onClick.AddListener(ViewHistory);
        signMessageBtn.onClick.AddListener(SignMessage);
        sendUSDCBtn.onClick.AddListener(SendUSDC);
        sendNFTBtn.onClick.AddListener(SendNFT);
        disconnectBtn.onClick.AddListener(Disconnect);

        closeWalletBtn.onClick.AddListener(CloseWallet);

        testingBtn.onClick.AddListener(ABITest);

        //Metamask Test
        metamaskConnectBtn.onClick.AddListener(ConnectMetamask);
        metamaskSignMessageBtn.onClick.AddListener(MetamaskSignMessage);
        metamaskSendTransactionBtn.onClick.AddListener(MetamaskSendTransaction);
        metamask.MetamaskConnectedEvent.AddListener(DisplayMetamaskPanel);
    }



    /// <summary>
    /// Call after wallet is initialized
    /// </summary>
    public async void StartDemo()
    {
        bool isConnected = await wallet.IsConnected();
        m_connected = isConnected;

        bool metamaskInitialized = metamask.IsMetamaskInitialised();
        if (isConnected)
        {
            DisplayWelcomePanel();

        }else if(metamaskInitialized)
        {
            DisplayMetamaskPanel();
        }
        else
        {
            DisplayConnectPanel();
        }
    }
    public void DisplayCloseWalletButton()
    {
        MainThread.wkr.AddJob(() =>
        {
            closeWalletBtn.gameObject.SetActive(true);
        });
    }
    public void HideCloseWalletButton()
    {
        MainThread.wkr.AddJob(() =>
        {
            closeWalletBtn.gameObject.SetActive(false);
        });
    }
    public void DisplayAddressPanel(string accountAddress)
    {
        MainThread.wkr.AddJob(() =>
        {
            addressCanvas.SetActive(true);
            HideWelcomePanel();
            HideConnectPanel();
            HideCollectionPanel();
            m_address.DisplayAccountAddress(accountAddress);
        });
    }
    public void HideAddressPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            addressCanvas.SetActive(false);
        });
    }
    private void DisplayCollectionPanel(TokenBalance[] tokenBalances)
    {
        MainThread.wkr.AddJob(() =>
        {
            collectionCanvas.SetActive(true);
            uiManager.EnableCollectionPanel();

            HideConnectPanel();
            HideWelcomePanel();
            HideAddressPanel();

            m_collection.RetriveContractInfoData(tokenBalances);

        });
    }
    public void HideCollectionPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            collectionCanvas.SetActive(false);
        });
    }
    public void DisplayWelcomePanel()
    {

        MainThread.wkr.AddJob(() =>
        {

            if (m_connected)
            {
                welcomeCanvas.SetActive(true);
                HideConnectPanel();
                HideAddressPanel();
                HideCollectionPanel();

            }
            HideCloseWalletButton();
        });
    }
    public void HideWelcomePanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            welcomeCanvas.SetActive(false);
        });
    }

    public void DisplayConnectPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            connectCanvas.SetActive(true);
            HideWelcomePanel();
            HideAddressPanel();
            HideCollectionPanel();
            HideCloseWalletButton();
        });
    }
    public void HideConnectPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            connectCanvas.SetActive(false);
        });
    }
    public async void Connect()
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

            bool isConnected = await wallet.IsConnected();
            m_connected = isConnected;
            if (isConnected)
            {
                DisplayWelcomePanel();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }



    public async void OpenWallet()
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
    }


    public async void CloseWallet()
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
    }

    public async void GetAddress()
    {

        try
        {
            string accountAddress = await wallet.GetAddress();
            Debug.Log("[DemoDapp] accountAddress " + accountAddress);
            DisplayAddressPanel(accountAddress);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void ViewCollection()
    {
        try
        {
            string accountAddress = await wallet.GetAddress(); //to test"0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            var tokenBalances = await Indexer.FetchMultiplePages(async (pageNumber) =>
            {
                GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, true, new Page
                {
                    page = pageNumber
                });
                BlockChainType blockChainType = BlockChainType.Polygon;
                var balances = await Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs);

                return (balances.page, balances.balances);
            }, 9999);
            List<TokenBalance> tokenBalanceList = new List<TokenBalance>();
            foreach (var tokenBalance in tokenBalances)
            {
                var tokenBalanceWithContract = await Indexer.FetchMultiplePages(async (pageNumber) =>
                {
                    GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, tokenBalance.contractAddress, true, new Page
                    {
                        page = pageNumber
                    });
                    BlockChainType blockChainType = BlockChainType.Polygon;
                    var balances = await Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs);

                    return (balances.page, balances.balances);
                }, 9999);
                tokenBalanceList.AddRange(tokenBalanceWithContract);

            }
            DisplayCollectionPanel(tokenBalanceList.ToArray());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void ViewHistory()
    {
        try
        {
            string accountAddress = await wallet.GetAddress(); //to test"0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            var transactions = await Indexer.FetchMultiplePages(async (pageNumber) =>
            {
                var args = new GetTransactionHistoryArgs(new TransactionHistoryFilter
                {
                    accountAddress = accountAddress
                }, new Page
                {
                    page = pageNumber
                });
                BlockChainType blockChainType = BlockChainType.Polygon;
                var history = await Indexer.GetTransactionHistory(blockChainType, args);
                return (history.page, history.transactions);
            }, 9999);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public async void SignMessage()
    {
        var message = @"1915 Robert Frost
The Road Not Taken

Two roads diverged in a yellow wood,
And sorry I could not travel both
And be one traveler, long I stood
And looked down one as far as I could
To where it bent in the undergrowth

Then took the other, as just as fair,
And having perhaps the better claim,
Because it was grassy and wanted wear
Though as for that the passing there
Had worn them really about the same,

And both that morning equally lay
In leaves no step had trodden black.
Oh, I kept the first for another day!
Yet knowing how way leads on to way,
I doubted if I should ever come back.

I shall be telling this with a sigh
Somewhere ages and ages hence:
Two roads diverged in a wood, and
I took the one less traveled by,
And that has made all the difference.

\u2601 \u2600 \u2602";
        
        var signature = await web3.Eth.Sign.SendRequestAsync(await wallet.GetAddress(), message);
        Debug.Log(signature);
    }

    public async void SendUSDC()
    {

        

        var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
        //Random To Account
        var toAddress = randomWallet.GetAccount(0).Address;
        string gasLimitHex = "055555";
        //TODO: Gas Limit is not parsed properly yet.
        HexBigInteger gasLimit = new HexBigInteger(BigInteger.Parse(gasLimitHex, System.Globalization.NumberStyles.AllowHexSpecifier));

        var USDCData = new
        {
            abi = @" [
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
                ]",
            contractAddress = "0x2791bca1f2de4661ed88a30c99a7a9449aa84174",
            functionData = "'transfer', ['" + toAddress + @"', ethers.utils.parseUnits('5', 18).toHexString()]"
        };


        var value = new HexBigInteger(0);
        TransactionInput transactionInput = new TransactionInput(JsonConvert.SerializeObject(USDCData), toAddress,await wallet.GetAddress(),gasLimit, value);
        await web3.Eth.Transactions.SendTransaction.SendRequestAsync(transactionInput);


    }
    public async void SendNFT()
    {
        try
        {
            
            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            //Random To Account
            var toAddress = randomWallet.GetAccount(0).Address;
            string gasLimitHex = "055555";
            //TODO: Gas Limit is not parsed properly yet.
            HexBigInteger gasLimit = new HexBigInteger(BigInteger.Parse(gasLimitHex, System.Globalization.NumberStyles.AllowHexSpecifier));
            string fromAddress = await wallet.GetAddress();
 
            //var amount = Web3.Convert.ToWei(5, UnitConversion.EthUnit.Gwei);
            var NFTData = new
            {
                abi = @"[
            {
                inputs: [
                {
                    internalType: 'address',
                    name: '_from',
                    type: 'address'
                },
                {
                    internalType: 'address',
                    name: '_to',
                    type: 'address'
                },
                {
                    internalType: 'uint256[]',
                    name: '_ids',
                    type: 'uint256[]'
                },
                {
                    internalType: 'uint256[]',
                    name: '_amounts',
                    type: 'uint256[]'
                },
                {
                    internalType: 'bytes',
                    name: '_data',
                    type: 'bytes'
                }
                ],
            name: 'safeBatchTransferFrom',
            outputs: [],
            stateMutability: 'nonpayable',
            type: 'function'
            }
        ]",
                contractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E",
                functionData = "'safeBatchTransferFrom', ['"+fromAddress+@"', '"+toAddress+@"', ['0x20001'], ['0x64'], []]"
            };

            var value = new HexBigInteger(0);
            TransactionInput transactionInput = new TransactionInput(JsonConvert.SerializeObject(NFTData), toAddress, fromAddress, gasLimit, value);
            await web3.Eth.Transactions.SendTransaction.SendRequestAsync(transactionInput);


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public async void Disconnect()
    {
        try
        {
            await wallet.Disconnect();
            Debug.Log("[DemoDapp] Disconnected.");

            // Return Back to Connect Panel
            DisplayConnectPanel();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    //-------------------abi encoding tests-------------------
    public async void ABITest()
    {
        var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
        //Random To Account
        var toAddress = randomWallet.GetAccount(0).Address;
        var erc20_name = await ERC20.Name(toAddress);
        Debug.Log("erc20 name: "+ erc20_name);
    }
    
    //------------------------------------------------------------------------
    public async void GetSotrageAt()
    {
        HexBigInteger position = null;
        object id = null;
        var signature = await web3.Eth.GetStorageAt.SendRequestAsync(await wallet.GetAddress(), position, id);
    }

    public async void GetEstimatedGas()
    {
        try
        {
            //TODO: will modify this 
            var contractAddress = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270";
            var data = new
            {
                to = contractAddress,
                data = " new ethers.utils.Interface(['function withdraw(uint256 amount)']).encodeFunctionData('withdraw', ['1000000000000000000'])"
            };
            var estimatedGas = await web3.Eth.GasPrice.SendRequestAsync(JsonConvert.SerializeObject(data));
            Debug.Log("estimated gas:" + estimatedGas);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
    //========================Metamask===========================

   
    //UI
    private void DisplayMetamaskPanel()
    {
        HideConnectPanel();
        metamaskPanel.SetActive(true);
       // web3.Client.OverridingRequestInterceptor = new MetamaskInterceptor(metamask);

    }
    private void ConnectMetamask()
    {
        StartCoroutine(metamask.MetamaskConnect());
        bool metamaskInitialized = metamask.IsMetamaskInitialised();
        if(metamaskInitialized)
        {
            DisplayMetamaskPanel();  
        }
    }

    private void MetamaskSignMessage()
    {
        var message = @"1915 Robert Frost
The Road Not Taken

Two roads diverged in a yellow wood,
And sorry I could not travel both
And be one traveler, long I stood
And looked down one as far as I could
To where it bent in the undergrowth

Then took the other, as just as fair,
And having perhaps the better claim,
Because it was grassy and wanted wear
Though as for that the passing there
Had worn them really about the same,

And both that morning equally lay
In leaves no step had trodden black.
Oh, I kept the first for another day!
Yet knowing how way leads on to way,
I doubted if I should ever come back.

I shall be telling this with a sigh
Somewhere ages and ages hence:
Two roads diverged in a wood, and
I took the one less traveled by,
And that has made all the difference.

\u2601 \u2600 \u2602";
        metamask.SignMessageRequest(message);
    }

    public void MetamaskSendTransaction()
    {
        var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
        //Random To Account
        var toAddress = randomWallet.GetAccount(0).Address;
        metamask.TransferRequest(toAddress, "0.000000000000000001");
    }


    //private variables:
    private static Mnemonic exampleMnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
    private string exampleWords = exampleMnemo.ToString();//"ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal";
    private string examplePassword = "password";


}
