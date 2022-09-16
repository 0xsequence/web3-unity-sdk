using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace demo
{
    public class MiniGameManager : MonoBehaviour
    {
        public List<CardAsset> cardAssetDeck;
        public static MiniGameManager Instance { get; private set; }

        [SerializeField] private CardLayout cardLayout;
        [SerializeField] private Button backBtn;

        public Transform enemyTransform;
        public Transform attackTransform;

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

            cardAssetDeck = new List<CardAsset>();
        }

        private void OnEnable()
        {
            backBtn.onClick.AddListener(BackToMenu);
        }

        private void OnDisable()
        {

            backBtn.onClick.RemoveListener(BackToMenu);
        }

        public void StartGame()
        {
            
            cardLayout.Init();

        }

        public void BackToMenu()
        {
            cardAssetDeck.Clear();
            //game manager control what panel to show
        }

        public void AddCardToDeck(CardAsset cardAsset)
        {
            
            if(!cardAssetDeck.Contains(cardAsset))
            {
                cardAssetDeck.Add(cardAsset);
            }
        }

        public void RemoveCardFromDeck(CardAsset cardAsset)
        {
            if (cardAssetDeck.Contains(cardAsset))
            {
                cardAssetDeck.Remove(cardAsset);
            }
        }
        public List<CardAsset> GetCardDeck()
        {
            return cardAssetDeck;
        }

        public void EnemyOnAttack()
        {
            //TODO: Enemy Anim
            enemyTransform.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InFlash, 12, 0).SetAutoKill(false);
        }
    }


}