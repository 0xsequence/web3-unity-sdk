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
    [SerializeField] private GameObject historyCanvas;
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
    [Header("History")]
    [SerializeField] private GameObject historyUnitPrefab;
    [SerializeField] private HistoryUI historyUI;
    [SerializeField] private Transform historyScroll;

    [Header("Collection")]
    [SerializeField] private Collection m_collection;
    [Header("AccountAddress")]
    [SerializeField] private AccountAddress m_address;

    private bool m_connected = false; //For UI Only

    private Web3 web3 = new Web3();
    private SequenceInterceptor _sequenceInterceptor;
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

        _sequenceInterceptor = new SequenceInterceptor(wallet, 137);
        web3.Client.OverridingRequestInterceptor = _sequenceInterceptor;
        
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

        testingBtn.onClick.AddListener(SendTetherUSD);

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
            HideHistoryPanel();
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

    private void DisplayHistoryPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            historyCanvas.SetActive(true);
            HideWelcomePanel();
            HideConnectPanel();
            HideCollectionPanel();
            HideAddressPanel();
        });
    }

    public void HideHistoryPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            historyCanvas.SetActive(false);
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
            HideHistoryPanel();

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
                HideHistoryPanel();

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
            HideHistoryPanel();
        });
    }
    public void HideConnectPanel()
    {
        MainThread.wkr.AddJob(() =>
        {
            connectCanvas.SetActive(false);
        });
    }

    public void ChangeNetwork(int chainId)
    {
        _sequenceInterceptor.chainID = chainId;
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
            await wallet.OpenWallet("wallet",null,null);
                
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
            string accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            var tokenBalances = await Indexer.FetchMultiplePages(async (pageNumber) =>
            {
                GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, true, new Page
                {
                    page = pageNumber
                });
                var balances = await Indexer.GetTokenBalances(Chain.Polygon, tokenBalancesArgs);

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
                    var balances = await Indexer.GetTokenBalances(Chain.Polygon, tokenBalancesArgs);

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
        DisplayHistoryPanel();
        historyUI.ClearHistories();
        try
        {   
            string accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";// await wallet.GetAddress(); //to test"0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            var transactions = await Indexer.FetchMultiplePages(async (pageNumber) =>
            {
                var args = new GetTransactionHistoryArgs(new TransactionHistoryFilter
                {
                    accountAddress = accountAddress
                }, new Page
                {
                    page = pageNumber
                });
                var history = await Indexer.GetTransactionHistory(Chain.Polygon, args);
                
                foreach (var transaction in history.transactions)
                {
                    
                    foreach(var transfer in transaction.transfers)
                    {
                        
                        // Try to get token name, but got a "missing revert data in call exception" from ether.js for some contract address.
                        string name = "";
                        switch (transfer.contractType)
                        {
                            case ContractType.ERC20:
                                name = await ERC20.Name(transfer.contractAddress);
                                break;
                            case ContractType.ERC721:
                                name = await ERC721.Name(transfer.contractAddress);
                                break;
                            default:
                                break;
                        }

                        GameObject unitGO = Instantiate(historyUnitPrefab);
                        unitGO.transform.SetParent(historyScroll);
                        unitGO.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);
                        HistoryUnit historyUnit = unitGO.GetComponent<HistoryUnit>();
                        historyUnit.SetUnit(transaction.timestamp, name, transfer.tokenIds.Length.ToString());
                        historyUI.AddToHistoryList(historyUnit);
                        /*foreach(var tokenId in transfer.tokenIds)
                        {
                            Debug.Log("tokenId: "+ tokenId);
                        }*/
                    }
                    
                }
                return (history.page, history.transactions);
            }, 9999);

            
        }
        catch
        {
            
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
        var abi = @" [
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
                ]";

        var contractAddress = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
        var contract = web3.Eth.GetContract(abi, contractAddress);
        var transferFunction = contract.GetFunction("transfer");
        var senderAddress =await wallet.GetAddress();

        var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
        //Random To Account
        var newAddress = randomWallet.GetAccount(0).Address;

        var amountToSend = 0;//?
        var gas = await transferFunction.EstimateGasAsync(senderAddress, null, null, newAddress, amountToSend);
        var value = new HexBigInteger(0);
        var receiptAmountSend =
            await transferFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, value, null, newAddress,
                amountToSend);
        Debug.Log("[Sequence] ReceiptAmountSend:" + receiptAmountSend);


    }

    //Issue:
    //https://forum.openzeppelin.com/t/cannot-estimate-gas/22245
    public async void SendNFT()
    {
        try
        {
            //Contract Address
            var contractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";
            switch ((int)_sequenceInterceptor.chainID)
            {
                case 1:
                    contractAddress = "0x5c891d76584b46bC7F1E700169a76569Bb77d2Db";
                    break;
                case 137:
                    contractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";
                    break;
                case 56:
                    contractAddress = "0x3d24C45565834377b59fCeAA6864D6C25144aD6c";
                    break;
                case 42161:
                    contractAddress = "0x9e22a752C4eD3D6C5DaB0123ED6A1C7eCF575200";
                    break;
                case 42170:
                    contractAddress = "0x61A119dAA01179dbAa33780D42E75b960e57f6B7";
                    break;
                case 10:
                    contractAddress = "0xfa14e1157f35e1dad95dc3f822a9d18c40e360e2";
                    break;
                case 43114:
                    contractAddress = "0xDb05DA15A08B59e6227f2d5047723243Be5b8525";
                    break;
                case 100:
                    contractAddress = "0x4330e8d1Cf427F878636d2614Dc13c356A71Be14";
                    break;
                //Testnet:
                case 5:
                    contractAddress = "0xff02b7d59975E76F67B63b20b813a9Ec0f6AbD60";
                    break;
                case 80001:
                    contractAddress = "0xD2b0e73725918EA5F6ec25361D96D04977a86F88";
                    break;
                case 97:
                    contractAddress = "0x35c847059890178486841aE31d8C5f8d2C22Aea9";
                    break;
                default:
                    Debug.Log("[Sequence] Incorrect Chain Id");
                    break;
            }

            var abi = @"[
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
        ]";
            
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var transferFunction = contract.GetFunction("safeBatchTransferFrom");
            var senderAddress = await wallet.GetAddress();

            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            //Random To Account
            var newAddress = randomWallet.GetAccount(0).Address;

            var amountToSend = 0;//?
            var gas = await transferFunction.EstimateGasAsync(senderAddress, null, null, newAddress, amountToSend);
            var value = new HexBigInteger(0);
            var receiptAmountSend =
                await transferFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, value, null, newAddress,
                    amountToSend);
            Debug.Log("[Sequence] ReceiptAmountSend:" + receiptAmountSend);


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public async void SendNFT(string contractAddress)
    {
        try
        {


            var abi = @"[
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
        ]";

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var transferFunction = contract.GetFunction("safeBatchTransferFrom");
            var senderAddress = await wallet.GetAddress();

            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            //Random To Account
            var newAddress = randomWallet.GetAccount(0).Address;

            var amountToSend = 0;//?
            var gas = await transferFunction.EstimateGasAsync(senderAddress, null, null, newAddress, amountToSend);
            var value = new HexBigInteger(0);
            var receiptAmountSend =
                await transferFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, value, null, newAddress,
                    amountToSend);
            Debug.Log("[Sequence] ReceiptAmountSend:" + receiptAmountSend);


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public async void SendTetherUSD()
    {
        var abi = @" [
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
                ]";
        var contractAddress = "0xdAC17F958D2ee523a2206206994597C13D831ec7";
        var contract = web3.Eth.GetContract(abi, contractAddress);
        var transferFunction = contract.GetFunction("transfer");
        var senderAddress = await wallet.GetAddress();

        var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
        //Random To Account
        var newAddress = randomWallet.GetAccount(0).Address;

        var amountToSend = 0;//?

        var networkId = await web3.Net.Version.SendRequestAsync();
        Debug.Log("networkID:" + networkId);

        var gas = await transferFunction.EstimateGasAsync(senderAddress, null, null, newAddress, amountToSend);
        var value = new HexBigInteger(0);
        var receiptAmountSend =
            await transferFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, value, null, newAddress,
                amountToSend);
        Debug.Log("[Sequence] ReceiptAmountSend:" + receiptAmountSend);
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
        //ERC20Example erc20example = FindObjectOfType<ERC20Example>();
        //erc20example.ERC20Examples();
        //ERC721Example erc721example = FindObjectOfType<ERC721Example>();
        //erc721example.ERC721Examples();
        ERC1155Example erc1155example = FindObjectOfType<ERC1155Example>();
        erc1155example.ERC1155Examples();
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
            var abi = @" [
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
                ]";
            var contractAddress = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var transferFunction = contract.GetFunction("transfer");
            var senderAddress = await wallet.GetAddress();

            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            //Random To Account
            var newAddress = randomWallet.GetAccount(0).Address;

            var amountToSend = 0;
            var value =new HexBigInteger(0);
            var gas = await transferFunction.EstimateGasAsync(senderAddress, null, value, newAddress, amountToSend);
            Debug.Log("[Sequence Estimated Gas: ]"+ gas.ToString());
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
