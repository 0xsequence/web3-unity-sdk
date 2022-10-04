using System.Collections;
using System.Collections.Generic;
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
        connectBtn.onClick.AddListener(() => wallet.ExecuteSequenceJS(""));
        /*        connectAndAuthBtn.onClick.AddListener(Sequence.Instance.Connect);
                connectWithSettingsBtn.onClick.AddListener(Sequence.Instance.Connect);
                disconnectBtn.onClick.AddListener(Sequence.Instance.Disconnect);
                openWalletBtn.onClick.AddListener(Sequence.Instance.OpenWallet);
                openWalletWithSettingsBtn.onClick.AddListener(Sequence.Instance.OpenWalletWithSettings);
                closeWalletBtn.onClick.AddListener(Sequence.Instance.CloseWallet);
                isConnectedBtn.onClick.AddListener(Sequence.Instance.IsConnected);
                isOpenedBtn.onClick.AddListener(Sequence.Instance.IsOpened);
                defaultChainBtn.onClick.AddListener(Sequence.Instance.GetDefaultChainID);
                authChainBtn.onClick.AddListener(Sequence.Instance.GetAuthChainID);*/

        //signing
        /*        chainIDBtn.onClick.AddListener(Sequence.Instance.GetChainID);
                networksBtn.onClick.AddListener(Sequence.Instance.GetNetworks);
                getAccountsBtn.onClick.AddListener(Sequence.Instance.GetAccounts);
                getBalanceBtn.onClick.AddListener(Sequence.Instance.GetBalance);
                getWalletStateBtn.onClick.AddListener(Sequence.Instance.GetWalletState);*/

        //simulation
        /*        estimateUnwrapGasBtn.onClick.AddListener(Sequence.Instance.EstimateUnwrapGas);*/
        //transaction
        /*        sendOnDefaultChainBtn.onClick.AddListener(Sequence.Instance.SendETH);
                sendOnAuthChainBtn.onClick.AddListener(Sequence.Instance.SendETHSidechain);
                sendDAIBtn.onClick.AddListener(Sequence.Instance.SendDAI);
                sendERC1155Btn.onClick.AddListener(Sequence.Instance.Send1155Tokens);
                sendOnRinkebyBtn.onClick.AddListener(Sequence.Instance.SendRinkebyUSDC);*/
        //various
        /*        contractExampleBtn.onClick.AddListener(Sequence.Instance.ContractExample);
                fetchTokenBalanceAndMetadataBtn.onClick.AddListener(Sequence.Instance.FetchTokenBalances);*/
    }


}
