using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SequenceSharp;
using Newtonsoft.Json;
using UnityEngine.UI;

using System;

public class DemoManager : MonoBehaviour
{
    [SerializeField] private Wallet wallet;

    [Header("Canvases")]
    [SerializeField] private GameObject connectCanvas;
    [SerializeField] private GameObject welcomeCanvas;
    [SerializeField] private GameObject addressCanvas;
    [SerializeField] private GameObject collectionCanvas;


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


    [Header("Collection")]
    [SerializeField] private Collection m_collection;
    [Header("AccountAddress")]
    [SerializeField] private AccountAddress m_address;

    private bool m_connected = false; //For UI Only

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
    }
    private void OnEnable()
    {
        //wallet initialize
        if(wallet.readyToConnect)
        {
            StartDemo();
        } else
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
            string accountAddress = await wallet.GetAddress();//to test"0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
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
            DisplayCollectionPanel(tokenBalances);
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
            string accountAddress = await wallet.GetAddress();//to test"0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
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
        try
        {
            await wallet.ExecuteSequenceJS(@"
                const wallet = sequence.getWallet();

                console.log('signing message...');
                const signer = wallet.getSigner();

                const message = `1915 Robert Frost
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
Two roads diverged in a wood, and I—
I took the one less traveled by,
And that has made all the difference.

\u2601 \u2600 \u2602`

            // sign
            const sig = await signer.signMessage(message);
            console.log('signature:', sig);

            // validate
            const isValidHex = await wallet.utils.isValidMessageSignature(
                await wallet.getAddress(),
                ethers.utils.hexlify(ethers.utils.toUtf8Bytes(message)),
                sig,
                await signer.getChainId()
            )
            console.log('isValidHex?', isValidHex);

            const isValid = await wallet.utils.isValidMessageSignature(await wallet.getAddress(), message, sig, await signer.getChainId());
            console.log('isValid?', isValid);
            if (!isValid) throw new Error('sig invalid');
            ");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void SendUSDC()
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
    }
    public async void SendNFT()
    {
        try
        {
            var txnResponse = await wallet.ExecuteSequenceJS(@"
            const ERC_1155_ABI = [
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
        ];
        const signer = seq.getWallet().getSigner();
        const fromAddress = await seq.getWallet().getAddress();
        const toAddress = ethers.Wallet.createRandom().address;


        const skyweaverContractAddress = '0x631998e91476DA5B870D741192fc5Cbc55F5a52E'; // (Skyweaver address on prod)
        //tx : sequence.transactions.Transaction
        const tx = {
            delegateCall: false,
            revertOnError: false,
            gasLimit: '0x55555',
            to: skyweaverContractAddress,
            value: 0,
            data: new ethers.utils.Interface(ERC_1155_ABI).encodeFunctionData('safeBatchTransferFrom', [fromAddress, toAddress, ['0x20001'], ['0x64'], []])
        }

        const txnResp = await signer.sendTransactionBatch([tx]);

        return txnResponse; ");
            Debug.Log("[DemoDapp] txnResponse: " + txnResponse);

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


}
