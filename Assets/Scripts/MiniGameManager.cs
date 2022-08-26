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
        

        
        public void StartGame()
        {
            
            cardLayout.Init();

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