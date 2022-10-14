using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo
{

    public class CardLayout : MonoBehaviour
    {
        private int cardNum;
        public float arc = 30;
        private List<RectTransform> cardTransforms;

        [SerializeField] private RectTransform deckRoot = null;
        [SerializeField] private GameObject cardTemplate = null;

        private void Awake()
        {
            cardTransforms = new List<RectTransform>();
        }
        public void Init()
        {

            foreach (CardAsset cardAsset in MiniGameManager.Instance.GetCardDeck())
            {
                Card newCard = Instantiate(cardTemplate, deckRoot.transform).GetComponent<Card>();

                newCard.Init(cardAsset);
                cardTransforms.Add(newCard.GetComponent<RectTransform>());

            }
            SetLayout();
        }
        public void SetLayout()
        {

            cardNum = transform.childCount;
            float startingRot = arc / 2f;
            float deltaRot = -arc / (float)cardNum;
            float width = transform.GetComponent<RectTransform>().rect.width;
           

            Vector2 startingPos = new Vector2(0, 0);//new Vector2(-width / 2f + offset, 0);

            float deltaPos = width / 10; //hardcoded value pwease change later 

            int i = -cardNum/2;
            foreach (RectTransform child in cardTransforms)
            {
                float rot = 0 + i * deltaRot;
                child.Rotate(new Vector3(0, 0, rot));
                child.localPosition = startingPos + new Vector2(i * deltaPos, 0);
                //Debug.Log(child.localPosition);
                child.GetComponent<Card>().originalPos = child.position;
                child.GetComponent<Card>().originalScale = transform.localScale;

                i++;
            }

        }
    }
}