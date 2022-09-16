using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class Sequence : MonoBehaviour
{
    #region javascript functions from jslib
    //Connection 
    [DllImport("__Internal")]
    private static extern void TestFromJS();

    [DllImport("__Internal")]
    private static extern void ConnectFromJS();

   [DllImport("__Internal")]
    private static extern void DisconnectFromJS();
   [DllImport("__Internal")]
    private static extern void OpenWalletFromJS();
   [DllImport("__Internal")]
    private static extern void OpenWalletWithSettingsFromJS();
   [DllImport("__Internal")]
    private static extern void CloseWalletFromJS();
   [DllImport("__Internal")]
    private static extern void IsConnectedFromJS();
   [DllImport("__Internal")]
    private static extern void IsOpenedFromJS();
   [DllImport("__Internal")]
    private static extern void GetDefaultChainIDFromJS();
   [DllImport("__Internal")]
    private static extern void GetAuthChainIDFromJS();

    //State
   [DllImport("__Internal")]
    private static extern void GetChainIDFromJS();
   [DllImport("__Internal")]
    private static extern void GetNetworksFromJS();
   [DllImport("__Internal")]
    private static extern void GetAccountsFromJS();
   [DllImport("__Internal")]
    private static extern void GetBalanceFromJS();
   [DllImport("__Internal")]
    private static extern void GetWalletStateFromJS();

    //Signing
   [DllImport("__Internal")]
    private static extern void SignMessageFromJS();
   [DllImport("__Internal")]
    private static extern void SignTypedDataFromJS();
   [DllImport("__Internal")]
    private static extern void SignAuthMessageFromJS();
   [DllImport("__Internal")]
    private static extern void SignETHAuthFromJS();

    //Simulation
   [DllImport("__Internal")]
    private static extern void EstimateUnwrapGasFromJS();
    
    //Transactions
   [DllImport("__Internal")]
    private static extern void SendETHFromJS();
   [DllImport("__Internal")]
    private static extern void SendETHSidechainFromJS();
   [DllImport("__Internal")]
    private static extern void SendDAIFromJS();
   [DllImport("__Internal")]
    private static extern void Send1155TokensFromJS();
   [DllImport("__Internal")]
    private static extern void SendRinkebyUSDCFromJS();

    //Various
   [DllImport("__Internal")]
    private static extern void ContractExampleFromJS();
   [DllImport("__Internal")]
    private static extern void FetchTokenBalancesFromJS();
    #endregion

    //Singleton
    public static Sequence Instance { get; private set; }
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

    #region delegates
    public event Action<Sequence> OnUpdate;
    #endregion
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Log from Sequence.cs:");
        
    }

    public void Test()
    {
        TestFromJS();
    }
    public void Connect()
    {
        ConnectFromJS();
    }
    public void Disconnect()
    {
        DisconnectFromJS();
    }
    public void OpenWallet()
    {
        OpenWalletFromJS();
    }
    public void OpenWalletWithSettings()
    {
        OpenWalletWithSettingsFromJS();
    }
    public void CloseWallet()
    {
        CloseWalletFromJS();
    }
    public void IsConnected()
    {
        IsConnectedFromJS();
    }
    public void IsOpened()
    {
        IsOpenedFromJS();
    }
    public void GetDefaultChainID()
    {
        GetDefaultChainIDFromJS();
    }

    public void GetAuthChainID()
    {
        GetAuthChainIDFromJS();
    }


    //State
    public void GetChainID()
    {
        GetChainIDFromJS();
    }

    public void GetNetworks()
    {
        GetNetworksFromJS();
    }
    public void GetAccounts()
    {
        GetAccountsFromJS();
    }
    public void GetBalance()
    {
        GetBalanceFromJS();
    }
    public void GetWalletState()
    {
        GetWalletStateFromJS();
    }



    //Signing
    public void SignMessage()
    {
        SignAuthMessageFromJS();
    }
    public void SignTypedData()
    {
        SignTypedDataFromJS();
    }
    public void SignAuthMessage()
    {
        SignAuthMessageFromJS();
    }
    public void SignETHAuth()
    {
        SignETHAuthFromJS();
    }
    //Simulation
    public void EstimateUnwrapGas()
    {
        EstimateUnwrapGasFromJS();
    }
    //Transaction
    public void SendETH()
    {
        SendETHFromJS();
    }
    public void SendETHSidechain()
    {
        SendETHSidechainFromJS();
    }
    public void SendDAI()
    {
        SendDAIFromJS();
    }
    public void Send1155Tokens()
    {
        Send1155TokensFromJS();
    }
    public void SendRinkebyUSDC()
    {
        SendRinkebyUSDCFromJS();
    }
    //various
    public void ContractExample()
    {
        ContractExampleFromJS();

    }
    
    public void FetchTokenBalances()
    {
        FetchTokenBalancesFromJS();
    }

}
