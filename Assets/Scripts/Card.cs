using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
namespace demo
{

    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Image imgHolder = null;

        [SerializeField] private float scaleFactor = 0.5f;
        public Vector3 originalPos;
        public Vector3 originalScale;
        public int originalIndex;
        public CardProperties cardProperties;

        
        private bool isAttacking = false;
       

        public void Init(CardAsset cardAsset)
        {

            imgHolder.sprite = cardAsset.imgHolder.sprite;
            cardProperties = cardAsset._cardProperties;
            
            RectTransform imgRect = imgHolder.GetComponent<RectTransform>();
            imgRect.sizeDelta = new Vector2(imgHolder.sprite.texture.width * scaleFactor, imgHolder.sprite.texture.height * scaleFactor);
            originalIndex = transform.GetSiblingIndex();

        }

        public void Attack()
        {
            isAttacking = true;
            DG.Tweening.Sequence attacksq = DOTween.Sequence();
            attacksq.Append(
                transform.DOMove(MiniGameManager.Instance.attackTransform.position, 0.1f)
            );
            attacksq.Join(
                transform.DOScale(4.0f, 0.1f)
                );
            attacksq.Append(
                transform.DOMove(MiniGameManager.Instance.enemyTransform.position, 0.2f)
            );
            attacksq.Join(
                transform.DOScale(0.1f, 0.2f)
            );
            attacksq.Join(
                imgHolder.DOFade(0f, 0.2f)
                ); ;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {

            transform.DOLocalMoveY(100, 0.3f);
            transform.SetAsLastSibling();
            transform.DOScale(1.5f, 0.3f);

        }

      

        public void OnPointerClick(PointerEventData eventData)
        {
            Attack();
            MiniGameManager.Instance.EnemyOnAttack();
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            if (!isAttacking)
            {
                transform.DOMove(originalPos, 0.3f);
                transform.DOScale(originalScale, 0.3f);
                transform.SetSiblingIndex(originalIndex);
            }

        }
    }
}
