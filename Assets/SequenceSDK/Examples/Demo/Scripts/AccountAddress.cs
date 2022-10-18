using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AccountAddress : MonoBehaviour
{
    public TMP_Text m_accountAddressText;
    public Button m_backButton;

    private void OnEnable()
    {
        m_backButton.onClick.AddListener(BackToWelcomePanel);
    }
    private void OnDisable()
    {
        m_backButton.onClick.RemoveListener(BackToWelcomePanel);
    }
    public void DisplayAccountAddress(string accountAddress)
    {
        m_accountAddressText.text = "Hi Player \n" + accountAddress;
    }
    public void BackToWelcomePanel()
    {
        DemoManager.Instance.HideAddressPanel();
        DemoManager.Instance.DisplayWelcomePanel();
    }
}
