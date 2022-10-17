using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SequenceSharp;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System;

public class DemoManager : MonoBehaviour
{
    [SerializeField] private Wallet wallet;

    [Header("Canvases")]
    [SerializeField] private GameObject connectCanvas;
    [SerializeField] private GameObject welcomeCanvas;


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

    private void OnEnable()
    {
        //wallet initialize
        wallet.onWalletInitialized.AddListener(StartDemo);
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

    }
    private void OnDisable()
    {
        wallet.onWalletInitialized.RemoveListener(StartDemo);
        wallet.onWalletClosed.RemoveListener(DisplayWelcomePanel);

        connectBtn.onClick.RemoveListener(Connect);
        openWalletBtn.onClick.RemoveListener(OpenWallet);
        getAddressBtn.onClick.RemoveListener(GetAddress);
        viewCollectionBtn.onClick.RemoveListener(ViewCollection);
        viewHistoryBtn.onClick.RemoveListener(ViewHistory);
        signMessageBtn.onClick.RemoveListener(SignMessage);
        sendUSDCBtn.onClick.RemoveListener(SendUSDC);
        sendNFTBtn.onClick.RemoveListener(SendNFT);
        disconnectBtn.onClick.RemoveListener(Disconnect);
    }


    /// <summary>
    /// Call after wallet is initialized
    /// </summary>
    public async void StartDemo()
    {
        bool isConnected =  await wallet.IsConnected();
        if (isConnected)
        {
            DisplayWelcomePanel();
 
        }
        else
        {
            DisplayConnectPanel();
        }
    }

    private void DisplayWelcomePanel()
    {
        welcomeCanvas.SetActive(true);
        HideConnectPanel();
    }
    private void HideWelcomePanel()
    {
        welcomeCanvas.SetActive(false);
    }

    private void DisplayConnectPanel()
    {
        connectCanvas.SetActive(true);
        HideWelcomePanel();
    }
    private void HideConnectPanel()
    {
        connectCanvas.SetActive(false);
    }
    public async void Connect()
    {
        try
        {
            HideConnectPanel();

            var connectDetails = await wallet.Connect(new ConnectOptions
            {
                app = "Demo Unity Dapp"
            });
            Debug.Log("[DemoDapp] Connect Details:  " + JsonConvert.SerializeObject(connectDetails, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));

            bool isConnected = await wallet.IsConnected();
            if(isConnected)
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
            HideWelcomePanel();
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
            HideWelcomePanel();
            string accountAddress = await wallet.GetAddress();
            Debug.Log("[DemoDapp] accountAddress " + accountAddress);
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
            HideWelcomePanel();
            string accountAddress = await wallet.GetAddress();
            Debug.Log("[DemoDapp] accountAddress " + accountAddress);
            GetTokenBalancesArgs tokenBalancesArgs = new GetTokenBalancesArgs(accountAddress, true);
            BlockChainType blockChainType = BlockChainType.Polygon;

            Indexer.GetTokenBalances(blockChainType, tokenBalancesArgs, (tokenBalances) =>
            {
                Debug.Log(tokenBalances);
            });

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
            //TODO:
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
            HideWelcomePanel();
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
            HideWelcomePanel();
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
            HideWelcomePanel();
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
            data: new ethers.utils.Interface(ERC_1155_ABI).encodeFunctionData('safeBatchTransferFrom', [fromAddress, toAddress, ['0x20001'], ['0x64']])
        }

        const txnResp = await signer.sendTransactionBatch([tx], 4);

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
