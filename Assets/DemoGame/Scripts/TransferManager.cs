using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace demo
{
    public class TransferManager : MonoBehaviour
    {
        [SerializeField]private GameObject TransferPanel;
        [SerializeField] private Button rejectBtn;
        [SerializeField] private Button confirmBtn;
        private BlockChainType blockChainType;//network
        private DateTime requestTime;
        public static TransferManager Instance { get; private set; }
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
            TransferPanel.SetActive(false);
        }
        private void OnEnable()
        {
            rejectBtn.onClick.AddListener(OnReject);
        }
        private void OnDisable()
        {
            rejectBtn.onClick.RemoveListener(OnReject);
        }
        public void ShowTransferPanel()
        {
            
            TransferPanel.SetActive(true);
            LoadTransferPanelContent();
        }

        private void OnReject()
        {
            TransferPanel.SetActive(false);
        }

        private void LoadTransferPanelContent()
        {
            Debug.Log("Load Transfer Panel Content");

        }
    }
}