using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace demo
{
    public struct Artist
    {
        public string name;
    }
    public struct Trait
    {
        public string name;
    }
    public struct CardProperties
    {
        public Artist artist;
        public string attachment;
        public int baseCardId;
        public string cardType;
        public string element;
        public int health;
        public int mana;
        public int power;
        public string prism;
        public Trait trait;
        public string type;

    }
    public class CardAsset : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Image imgHolder = null;
        public CardProperties _cardProperties;
        [SerializeField] private float scaleFactor = 0.8f;
        private CanvasGroup canvasGroup;
        private bool isSelected;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public void InitCardAsset(Sprite sprite, CardProperties cardProperties)
        {
            imgHolder.sprite = sprite;
            RectTransform imgRect = imgHolder.GetComponent<RectTransform>();
            
            imgRect.sizeDelta = new Vector2(sprite.texture.width * scaleFactor, sprite.texture.height * scaleFactor);
            
            imgRect.localPosition = new Vector2(100 ,200);//hard coded value , change later
            _cardProperties = cardProperties;
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
            CardDragManager.Instance.BeginDrag();
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            
            CardDragManager.Instance.Drag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
            CardDragManager.Instance.EndDrag();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isSelected)
            {
                isSelected = true;
                imgHolder.color = Color.gray;
                CardDragManager.Instance.AddToList(this.transform);
                MiniGameManager.Instance.AddCardToDeck(this);
            }
            else
            {
                isSelected = false;
                imgHolder.color = Color.white;
                CardDragManager.Instance.RemoveFromList(this.transform);
                MiniGameManager.Instance.RemoveCardFromDeck(this);
            }

        }
    }
}
