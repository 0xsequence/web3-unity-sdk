using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SequenceSharp;

[System.Serializable]
public class Network
{
    public string name;
    public int chainID;
}

[System.Serializable]
public class NetworkDropdown : MonoBehaviour
{
    public TMP_Dropdown _dropdown;
    //Set in Inspector
    [Header("Network")]
    public List<Network> networks = new List<Network>();
    [Header("TESTNET")]
    public List<Network> testnetNetworks = new List<Network>();

    void Awake()
    {
 
        _dropdown.options.Clear();

        foreach(Network network in networks)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData() { text = network.name });
            
        }
        _dropdown.onValueChanged.AddListener(delegate { NetworkSelected(_dropdown); });

        //initialize to polygon
        _dropdown.SetValueWithoutNotify(1);
        
    }

    /// <summary>
    /// Select Network
    /// </summary>
    /// <param name="dropdown"></param>
    void NetworkSelected(TMP_Dropdown dropdown)
    {
        var chainId = networks[dropdown.value].chainID;

        DemoManager.Instance.ChangeNetwork(chainId);
        
    }

    
    
}
