using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace demo
{
    public class DragTarget : MonoBehaviour,IDropHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public void OnDrop(PointerEventData eventData)
        {

            CardDragManager.Instance.DropOnTargetPanel();

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            
            if (CardDragManager.Instance.isDraging)
            {
                
                CardDragManager.Instance.onTargetPanel = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CardDragManager.Instance.isDraging)
            {
                CardDragManager.Instance.onTargetPanel = false;
            }
        }
    }
}