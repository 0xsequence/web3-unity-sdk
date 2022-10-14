using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace demo
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private Button playBtn;
        [SerializeField] private Button transferBtn;
        [Header("player asset")]
        [SerializeField] private GameObject playerAssetPanelGo;
        [Header("Mini Game")]
        [SerializeField] private GameObject miniGamePanelGo;
        [Header("Transfer ")]
        [SerializeField] private GameObject friendAssetPanelGo;
        public static GameManager Instance { get; private set; }
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
        private void Start()
        {
            playerAssetPanelGo.SetActive(true);
            miniGamePanelGo.SetActive(false);
            friendAssetPanelGo.SetActive(false);
        }
        private void OnEnable()
        {
            playBtn.onClick.AddListener(StartMiniGame);
            transferBtn.onClick.AddListener(ShowFriendAssets);

        }
        private void OnDisable()
        {
            playBtn.onClick.RemoveListener(StartMiniGame);
            transferBtn.onClick.RemoveListener(ShowFriendAssets);
        }

        private void StartMiniGame()
        {
            miniGamePanelGo.SetActive(true);
            
            MiniGameManager.Instance.StartGame();
            playerAssetPanelGo.SetActive(false);
            HideButtons();
        }


        private void ShowFriendAssets()
        {
            friendAssetPanelGo.SetActive(true);
            HideButtons();
        }

        private void HideButtons()
        {
            playBtn.gameObject.SetActive(false);
            transferBtn.gameObject.SetActive(false);
        }


    }
}
