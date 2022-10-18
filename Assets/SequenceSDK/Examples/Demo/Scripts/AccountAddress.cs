using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AccountAddress : MonoBehaviour
{
    public TMP_Text m_accountAddressText;

    public void DisplayAccountAddress(string accountAddress)
    {
        m_accountAddressText.text = "Hi Player \n" + accountAddress;
    }
}
