using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistoryUnit : MonoBehaviour
{
    [SerializeField] private TMP_Text timeStampText;
    [SerializeField] private TMP_Text tokenNameText;
    [SerializeField] private TMP_Text tokenCountText;

    public void SetUnit(string timeStamp, string tokenName, string tokenCount)
    {
        timeStampText.text = timeStamp;
        tokenNameText.text = tokenName;
        tokenCountText.text = tokenCount;
    }
}
