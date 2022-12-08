using NBitcoin;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using SequenceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class DemoManager : MonoBehaviour
{
    [SerializeField]
    private SequenceSharp.Wallet wallet;

    public bool isTestingAddress;

    [Header("Canvases")]
    [SerializeField]
    private GameObject connectCanvas;

    [SerializeField]
    private GameObject welcomeCanvas;

    [SerializeField]
    private GameObject addressCanvas;

    [SerializeField]
    private GameObject collectionCanvas;

    [SerializeField]
    private GameObject historyCanvas;

    public DemoUIManager uiManager;

    [Header("Connection")]
    [SerializeField]
    private Button connectBtn;

    [Header("Welcome Panel")]
    [SerializeField]
    private Button openWalletBtn;

    [SerializeField]
    private Button getAddressBtn;

    [SerializeField]
    private Button viewCollectionBtn;

    [SerializeField]
    private Button viewHistoryBtn;

    [SerializeField]
    private Button signMessageBtn;

    [SerializeField]
    private Button sendUSDCBtn;

    [SerializeField]
    private Button sendNFTBtn;

    [SerializeField]
    private Button erc20Button;

    [SerializeField]
    private Button erc721Button;

    [SerializeField]
    private Button erc1155Button;

    [SerializeField]
    private Button disconnectBtn;

    [Header("Wallet")]
    [SerializeField]
    private Button closeWalletBtn;

    [Header("History")]
    [SerializeField]
    private GameObject historyUnitPrefab;

    [SerializeField]
    private HistoryUI historyUI;

    [SerializeField]
    private Transform historyScroll;

    [Header("Collection")]
    [SerializeField]
    private Collection m_collection;

    [Header("AccountAddress")]
    [SerializeField]
    private AccountAddress m_address;

    private bool m_connected = false; //For UI Only

    public Web3 web3 = new Web3();
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
        erc20Button.onClick.AddListener(ERC20AbiExample);
        erc721Button.onClick.AddListener(ERC721AbiExample);
        erc1155Button.onClick.AddListener(ERC1155AbiExample);
        disconnectBtn.onClick.AddListener(Disconnect);

        closeWalletBtn.onClick.AddListener(CloseWallet);
    }

    /// <summary>
    /// Call after wallet is initialized
    /// </summary>
    public async void StartDemo()
    {
        bool isConnected = await wallet.IsConnected();
        m_connected = isConnected;

        if (isConnected)
        {
            DisplayWelcomePanel();
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
            uiManager.HideLoadingPanel();
            uiManager.SetStyle();
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
            uiManager.HideLoadingPanel();
            uiManager.SetStyle();
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
        MainThread.wkr.AddJob(async () =>
        {
            collectionCanvas.SetActive(true);
            //uiManager.EnableCollectionPanel();

            HideConnectPanel();
            HideWelcomePanel();
            HideAddressPanel();
            HideHistoryPanel();

            await m_collection.RetriveContractInfoData(tokenBalances);
            uiManager.HideLoadingPanel();
            //Loading Panel will be hidden after all tokens are retrieved
            uiManager.SetStyle();
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
                uiManager.HideLoadingPanel();
                uiManager.SetStyle();
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
            uiManager.HideLoadingPanel();
            uiManager.SetStyle();
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

            var connectDetails = await wallet.Connect(
                new ConnectOptions { app = "Demo Unity Dapp" }
            );
            Debug.Log(
                "[DemoDapp] Connect Details:  "
                    + JsonConvert.SerializeObject(
                        connectDetails,
                        Formatting.Indented,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                    )
            );

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
            await wallet.OpenWallet("wallet", null, null);

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
        HideWelcomePanel();
        uiManager.ShowLoadingPanel();
        string accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        if (!isTestingAddress)
        {
            accountAddress = await wallet.GetAddress();
        }
        var tokenBalances = await Indexer.FetchMultiplePages(
            async (pageNumber) =>
            {
                GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(
                    accountAddress,
                    true,
                    new Page { page = pageNumber }
                );
                var balances = await Indexer.GetTokenBalances(Chain.Polygon, tokenBalancesArgs);

                return (balances.page, balances.balances);
            },
            9999
        );
        List<TokenBalance> tokenBalanceList = new List<TokenBalance>();
        foreach (var tokenBalance in tokenBalances)
        {
            var tokenBalanceWithContract = await Indexer.FetchMultiplePages(
                async (pageNumber) =>
                {
                    GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(
                        accountAddress,
                        tokenBalance.contractAddress,
                        true,
                        new Page { page = pageNumber }
                    );
                    var balances = await Indexer.GetTokenBalances(
                        Chain.Polygon,
                        tokenBalancesArgs
                    );

                    return (balances.page, balances.balances);
                },
                9999
            );
            tokenBalanceList.AddRange(tokenBalanceWithContract);
        }

        DisplayCollectionPanel(tokenBalanceList.ToArray());

    }

    public async void ViewHistory()
    {
        HideWelcomePanel();
        uiManager.ShowLoadingPanel();

        historyUI.ClearHistories();


        string accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        if (!isTestingAddress)
        {
            accountAddress = await web3.GetAddress();
        }
        var chainID = await web3.Eth.ChainId.SendRequestAsync();

        var transactions = await Indexer.FetchMultiplePages(
            async (pageNumber) =>
            {
                var args = new GetTransactionHistoryArgs(
                    new TransactionHistoryFilter { accountAddress = accountAddress },
                    new Page { page = pageNumber }
                );
                var history = await Indexer.GetTransactionHistory(chainID, args);

                var txsWithNames = history.transactions.SelectMany(tx =>
                    tx.transfers.Select(t => (
                        name: getTokenName(web3, t.contractType, t.contractAddress, t.tokenIds != null ? t.tokenIds[0] : null),
                        t, tx.timestamp
                    ))
                ).ToArray();

                return (history.page, txsWithNames);
            },
            10
        );
        var txNames = await Task.WhenAll(transactions.Select(t => t.name).ToArray());
        var txsWithNames = txNames.Zip(transactions.Select(t => (t.t, t.timestamp)), (name, t) => (name, t.t, t.timestamp));
        foreach (var (name, t, timestamp) in txsWithNames)
        {
            GameObject unitGO = Instantiate(historyUnitPrefab);
            unitGO.transform.SetParent(historyScroll);
            unitGO.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);
            HistoryUnit historyUnit = unitGO.GetComponent<HistoryUnit>();
            historyUnit.SetUnit(
                timestamp,
                name,
                t.tokenIds.Length.ToString()
            );
            historyUI.AddToHistoryList(historyUnit);
        }
        uiManager.HideLoadingPanel();
        DisplayHistoryPanel();

    }

    public async void SignMessage()
    {
        var message =
            @"1915 Robert Frost
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
        var abi =
            @" [
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

        var zero = new HexBigInteger(0);
        var receiptAmountSend = await transferFunction.SendTransactionAndWaitForReceiptAsync(
            senderAddress,
            zero,
            zero,
            null,
            newAddress,
            0
        );
        Debug.Log("[Sequence] sent:" + receiptAmountSend);
    }

    public async void SendNFT()
    {
        try
        {
            //Contract Address
            var contractAddress = "";
            BigInteger[] tokenIds = new BigInteger[1];

            switch ((int)_sequenceInterceptor.chainID)
            {
                case 1:
                    contractAddress = "0x697ed880c89C55a004003318690981d708E0d9ed";
                    tokenIds[0] = 4605;
                    break;
                case 137:
                    contractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";
                    tokenIds[0] = 16646145;
                    break;
                case 56:
                    contractAddress = "0xa23b58EA2eE75A30075B3D071444Da35d4947Cc9";
                    tokenIds[0] = 81690;
                    break;
                case 42161:
                    contractAddress = "0x925f6348cFD83D3cc0e77830F7A176A33b550111";
                    tokenIds[0] = 66277;
                    break;
                case 42170:
                    contractAddress = "0x1B3e11E78D082eF0acA690d0B918aEbb216E20E5";
                    tokenIds[0] = 999;
                    break;
                case 10:
                    contractAddress = "0xfA14e1157F35E1dAD95dC3F822A9d18c40e360E2";
                    tokenIds[0] = 619046;
                    break;
                case 43114:
                    contractAddress = "";
                    tokenIds[0] = 0;
                    break;
                case 100:
                    contractAddress = "0xf0A93Ad0184cF1e5f29d7b5579358C99b9010F17";
                    tokenIds[0] = 7957;
                    break;
                //Testnet:
                case 5:
                    contractAddress = "0xff02b7d59975E76F67B63b20b813a9Ec0f6AbD60";
                    tokenIds[0] = 0;
                    break;
                case 80001:
                    contractAddress = "0xF399feCf01C3b8f115807c2f4E8117c57a570f36";
                    tokenIds[0] = 16695550197660;
                    break;
                case 97:
                    contractAddress = "0x7246611e05A926D0214d70fD5207f988fECaAa68";
                    tokenIds[0] = 1;
                    break;
                default:
                    Debug.Log("[Sequence] Incorrect Chain Id");
                    break;
            }

            var abi =
                @"[
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
            var safeBatchTransferFunction = contract.GetFunction("safeBatchTransferFrom");
            var senderAddress = await wallet.GetAddress();

            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            //Random To Account
            var newAddress = randomWallet.GetAccount(0).Address;
            var zero = new HexBigInteger(0);
            BigInteger[] amounts = { 1 };
            Byte[] bytes = { };
            BigInteger value = new HexBigInteger(1);

            var transactionResp =
                await safeBatchTransferFunction.SendTransactionAndWaitForReceiptAsync(
                    senderAddress,
                    zero,
                    zero,
                    null,
                    senderAddress,
                    newAddress,
                    tokenIds,
                    amounts,
                    bytes
                );

            Debug.Log("[Sequence] ReceiptAmountSend:" + transactionResp);
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
            var abi =
                @"[
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
            var safeBatchTransferFunction = contract.GetFunction("safeBatchTransferFrom");
            var senderAddress = await wallet.GetAddress();

            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            //Random To Account
            var newAddress = randomWallet.GetAccount(0).Address;

            var zero = new HexBigInteger(0);
            var receiptAmountSend =
                await safeBatchTransferFunction.SendTransactionAndWaitForReceiptAsync(
                    senderAddress,
                    zero,
                    zero,
                    null,
                    newAddress,
                    0
                );
            Debug.Log("[Sequence] ReceiptAmountSend:" + receiptAmountSend);
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

    public async void ERC20AbiExample()
    {
        var chainID = await web3.Eth.ChainId.SendRequestAsync();
        var contractAddress = exampleERC20Address(chainID);
        var erc20 = new ERC20(web3, contractAddress);

        Debug.Log("[Sequence] ERC20 Token Example:");
        Debug.Log($"Using ERC20 address ${contractAddress} on chain ${chainID}.");
        var name = await erc20.Name();
        Debug.Log("name: " + name);
        var symbol = await erc20.Symbol();
        Debug.Log("symbol: " + symbol);
        var decimals = await erc20.Decimals();
        Debug.Log("decimals: " + decimals);
        var totalSupply = await erc20.TotalSupply();
        Debug.Log("totalSupply: " + totalSupply);
        // More methods available in the ERC20 ABI :)
    }

    public async void ERC721AbiExample()
    {
        var chainID = await web3.Eth.ChainId.SendRequestAsync();
        var contractAddress = exampleERC721Address(chainID);
        var erc721 = new ERC721(web3, contractAddress);

        Debug.Log("[Sequence] ERC721 Token Example:");
        Debug.Log($"Using ERC721 address ${contractAddress} on chain ${chainID}.");
        Debug.Log("name: " + await erc721.Name());
        Debug.Log("symbol: " + await erc721.Symbol());
        Debug.Log("Token URI of token ID 1: " + await erc721.TokenURI(BigInteger.One));
        Debug.Log("Owner of token ID 1: " + await erc721.OwnerOf(BigInteger.One));
        // More methods available in the ERC721 ABI :)
    }
    public async void ERC1155AbiExample()
    {
        var chainID = await web3.Eth.ChainId.SendRequestAsync();
        var contractAddress = exampleERC1155Address(chainID);
        var erc1155 = new ERC1155(web3, contractAddress);

        Debug.Log("[Sequence] ERC1155 Token Example:");
        Debug.Log($"Using ERC1155 address ${contractAddress} on chain ${chainID}.");
        Debug.Log("URI of token ID 1: " + await erc1155.URI(BigInteger.One));
        // More methods available in the ERC1155 ABI :)
    }

    public async void GetEstimatedGas()
    {
        try
        {
            var abi =
                @" [
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
            var value = new HexBigInteger(0);
            var gas = await transferFunction.EstimateGasAsync(
                senderAddress,
                null,
                value,
                newAddress,
                amountToSend
            );
            Debug.Log("[Sequence Estimated Gas: ]" + gas.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public static Mnemonic exampleMnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
    public static string exampleWords = exampleMnemo.ToString(); // "ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal"
    public static string examplePassword = "password";
    public static Nethereum.HdWallet.Wallet exampleWallet = new Nethereum.HdWallet.Wallet(
        exampleWords,
        examplePassword
    );
    public static string exampleToAccount = exampleWallet.GetAccount(0).Address;

    /// <summary>
    /// Returns the address of USDC on a given chain ID.
    /// Returns USDC-like tokens on testnets, if there's no official USDC deployment there.
    /// </summary>
    public static string exampleERC20Address(BigInteger chainID)
    {
        switch (chainID)
        {
            // Mainnets
            case var _ when chainID == Chain.Ethereum:
                return "0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48";
            case var _ when chainID == Chain.Polygon:
                return "0x2791Bca1f2de4661ED88A30C99A7a9449Aa84174";
            case var _ when chainID == Chain.BNBSmartChain:
                return "0x8AC76a51cc950d9822D68b83fE1Ad97B32Cd580d";
            case var _ when chainID == Chain.ArbitrumOne:
                return "0xFF970A61A04b1cA14834A43f5dE4533eBDDB5CC8";
            case var _ when chainID == Chain.ArbitrumNova:
                return "0x750ba8b76187092B0D1E87E28daaf484d1b5273b";
            case var _ when chainID == Chain.Optimism:
                return "0x7F5c764cBc14f9669B88837ca1490cCa17c31607";
            case var _ when chainID == Chain.Avalanche:
                return "0xB97EF9Ef8734C71904D8002F8b6Bc66Dd9c48a6E";
            case var _ when chainID == Chain.Gnosis:
                return "0xddafbb505ad214d7b80b1f830fccc89b60fb7a83";

            // Testnets
            case var _ when chainID == Chain.TestnetGoerli:
                return "0x07865c6e87b9f70255377e024ace6630c1eaa37f";
            case var _ when chainID == Chain.TestnetPolygonMumbai:
                return "0x0fa8781a83e46826621b3bc094ea2a0212e71b23";
            case var _ when chainID == Chain.TestnetBNBSmartChain:
                return "0x64544969ed7EBf5f083679233325356EbE738930";
            case var _ when chainID == Chain.TestnetAvalancheFuji:
                return "0xAF82969ECF299c1f1Bb5e1D12dDAcc9027431160";
            default:
                throw new ArgumentException("Unsupported chain ID: " + chainID);
        }
    }

    /// <summary>
    /// Returns the address of an arbitrary ERC721 token on a given chain ID.
    /// </summary>
    public static string exampleERC721Address(BigInteger chainID)
    {
        switch (chainID)
        {
            // Mainnets
            case var _ when chainID == Chain.Ethereum:
                return "0x7492E30d60D96c58ED0f0DC2FE536098C620C4c0";
            case var _ when chainID == Chain.Polygon:
                return "0xC36442b4a4522E871399CD717aBDD847Ab11FE88";
            case var _ when chainID == Chain.BNBSmartChain:
                return "0x2B09d47D550061f995A3b5C6F0Fd58005215D7c8";
            case var _ when chainID == Chain.ArbitrumOne:
                return "0xfAe39eC09730CA0F14262A636D2d7C5539353752";
            case var _ when chainID == Chain.ArbitrumNova:
                return "0x67F39eA8b7a41e4EBd1dF85fc33b413dcbD6D1bB";
            case var _ when chainID == Chain.Optimism:
                return "0xC36442b4a4522E871399CD717aBDD847Ab11FE88";
            case var _ when chainID == Chain.Avalanche:
                return "0x3025C5c2aA6eb7364555aAC0074292195701bBD6";
            case var _ when chainID == Chain.Gnosis:
                return "0x22C1f6050E56d2876009903609a2cC3fEf83B415";

            // Testnets
            case var _ when chainID == Chain.TestnetGoerli:
                return "0x084297b12f204adb74c689be08302fa3f12db8a7";
            case var _ when chainID == Chain.TestnetPolygonMumbai:
                return "0x757b1BD7C12B81b52650463e7753d7f5D0565C0e";
            case var _ when chainID == Chain.TestnetBNBSmartChain:
                return "0x7b56E60eA3d1D8d44d94899c1d491f07Cfa64357";
            case var _ when chainID == Chain.TestnetAvalancheFuji:
                return "0xdb0f219C120E1c0C9d4f144Db34Bc5B43B8da1DC";
            default:
                throw new ArgumentException("Unsupported chain ID: " + chainID);
        }
    }
    /// <summary>
    /// Returns the address of an arbitrary ERC1155 token on a given chain ID.
    /// </summary>
    public static string exampleERC1155Address(BigInteger chainID)
    {
        switch (chainID)
        {
            // Mainnets
            case var _ when chainID == Chain.Ethereum:
                return "0x495f947276749Ce646f68AC8c248420045cb7b5e";
            case var _ when chainID == Chain.Polygon:
                return "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";
            case var _ when chainID == Chain.BNBSmartChain:
                return "0x2DFEb752222ccceCB9BC0a934b02C3A86f633900";
            case var _ when chainID == Chain.ArbitrumOne:
                return "0xF3d00A2559d84De7aC093443bcaAdA5f4eE4165C";
            case var _ when chainID == Chain.ArbitrumNova:
                return "0xdAe44EAb390c3aa63ee5868c4166a09e35515058";
            case var _ when chainID == Chain.Optimism:
                return "0xfB1951b7EeF8E7613D3b09424fB4aEf805c16267";
            case var _ when chainID == Chain.Avalanche:
                return "0xa695ea0C90D89a1463A53Fa7a02168Bc46FbBF7e";
            case var _ when chainID == Chain.Gnosis:
                return "0xbD0AFf2785a5B752B421459aD6eDf13632d509De";

            // Testnets
            case var _ when chainID == Chain.TestnetGoerli:
                return "0x45d78213BD303ae89aea98B540065C53D67c88dA";
            case var _ when chainID == Chain.TestnetPolygonMumbai:
                return "0x34EdacAfe12D97868B628e600fE38E14C2338D76";
            case var _ when chainID == Chain.TestnetBNBSmartChain:
                return "0x838BA6f26A185245392A5EC6F6f12A380e1FE432";
            case var _ when chainID == Chain.TestnetAvalancheFuji:
                return "0xfA9214AEe59a6631A400DC039808457524dE70A2";
            default:
                throw new ArgumentException("Unsupported chain ID: " + chainID);
        }
    }
    private async Task<string> getTokenName(Web3 web3, ContractType contractType, string address, BigInteger? tokenID)
    {
        var n = "Unknown Token";
        try
        {
            // Try to get token name, but got a "missing revert data in call exception" from ether.js for some contract address.
            switch (contractType)
            {
                case ContractType.ERC20:
                    n = await new ERC20(web3, address).Name();
                    break;
                case ContractType.ERC721:
                    n = await new ERC721(web3, address).Name();
                    break;
                case ContractType.ERC1155:
                    n = await new ERC1155(web3, address).URI((BigInteger)tokenID);
                    break;
                default:
                    break;
            }
        }
        catch
        {
            Debug.LogWarning("Failed to get token name for contract address: " + address);
        }
        return n;
    }
}
