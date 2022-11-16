using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SequenceSharp;

[System.Serializable]
public class NetworkDropdown : MonoBehaviour
{
    private Wallet _wallet;
    public TMP_Dropdown _dropdown;
    //Set in Inspector
    public List<ProviderConfig> networks = new List<ProviderConfig>();
    public Button initializeButton;
    private ProviderConfig chosenProviderConfig = null;

    public TMP_Text selectionText;
    public GameObject networkCanvas;
    void Awake()
    {
        _wallet = FindObjectOfType<Wallet>();
        chosenProviderConfig = networks[0]; //default polygon
        //set up dropdowns
        _dropdown.options.Clear();
        foreach(ProviderConfig network in networks)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData() { text = network.defaultNetworkId });
            
        }

        _dropdown.onValueChanged.AddListener(delegate { NetworkSelected(_dropdown); });
        initializeButton.onClick.AddListener(InitializeWallet);
    }

    /// <summary>
    /// Select Network
    /// </summary>
    /// <param name="dropdown"></param>
    void NetworkSelected(TMP_Dropdown dropdown)
    {
        chosenProviderConfig = networks[dropdown.value];

        if(dropdown.value  == 2)
        {
            //"testnet"
            selectionText.text = "NOTE: to use mumbai, first go to https://sequence.app and click on 'Enable Testnet'.";
        }
        else
        {
            selectionText.text = "";
        }
    }

    void InitializeWallet()
    {
        
        _wallet.Initialize(chosenProviderConfig);
        networkCanvas.SetActive(false);
    }

    
}
