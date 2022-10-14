using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo
{
    [System.Serializable]
    public class CardDragManager : MonoBehaviour
    {
        [SerializeField] private Transform originalPanel;
        [SerializeField] private Transform targetPanel;
        [SerializeField] private Transform targetRoot;
        [SerializeField] private Transform dragTransform;
        private List<Transform> cardToDragList =  new List<Transform>();
        private Vector3 startDragPos;
        public static CardDragManager Instance { get; private set; }

        public bool isDraging = false;
        public bool onTargetPanel = false;
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

        public void AddToList(Transform cardTransform)
        {
            if (!cardToDragList.Contains(cardTransform)){
                cardToDragList.Add(cardTransform);
            }
            
        }

        public void RemoveFromList(Transform cardTransform)
        {
            if (cardToDragList.Contains(cardTransform))
            {
                cardToDragList.Remove(cardTransform);
            }
        }
        public void BeginDrag()
        {
            //record mouse position
            startDragPos = Input.mousePosition;
            isDraging = true;
        }

        public void Drag()
        {
            isDraging = true;
            //calculate mouse distance and move the whole list with it
            foreach(Transform cardTransform in cardToDragList )
            {
                cardTransform.position += Input.mousePosition - startDragPos;
                cardTransform.SetParent(dragTransform);
            }
            startDragPos = Input.mousePosition;
        }

        public void EndDrag()
        {
            
            if(!onTargetPanel)
            {
                foreach (Transform cardTransform in cardToDragList)
                {
                    cardTransform.SetParent(originalPanel);
                }
            }


            isDraging = false;
        }
        public void DropOnTargetPanel()
        {
            foreach (Transform cardTransform in cardToDragList)
            {
                cardTransform.SetParent(targetRoot);
            }

            cardToDragList.Clear();
            TransferManager.Instance.ShowTransferPanel();
        }
        public void ClearCardToDragList()
        {
            //destroy cards and empty list

        }
    }
}