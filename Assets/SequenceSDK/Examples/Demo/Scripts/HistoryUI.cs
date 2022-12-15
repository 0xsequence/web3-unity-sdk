using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HistoryUI : MonoBehaviour
{
    public Button m_backButton;
    List<HistoryUnit> _historyUnits = new List<HistoryUnit>();

    private void OnEnable()
    {
        m_backButton.onClick.AddListener(BackToWelcomePanel);
    }
    private void OnDisable()
    {
        m_backButton.onClick.RemoveListener(BackToWelcomePanel);
    }

    public void BackToWelcomePanel()
    {
        DemoManager.Instance.HideHistoryPanel();
        DemoManager.Instance.DisplayWelcomePanel();
    }

    public void AddToHistoryList(HistoryUnit history)
    {
        _historyUnits.Add(history);

    }

    public void ClearHistories()
    {
        foreach (var history in _historyUnits)
        {
            Destroy(history.gameObject);

        }
        _historyUnits.Clear();
    }
}
