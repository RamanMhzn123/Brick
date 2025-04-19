using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace CardGame.Scripts.Game_Elements
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Transform _originalParent;
        private Vector3 _originalPosition;
        
        [Header("Visuals")]
        public Image image;
        public TextMeshProUGUI text;
        public Card CardData { get; private set; }
        
        private bool _isDropped;
        private bool _isHovering;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Initialize(Card card)
        {
            CardData = card;
            image.sprite = CardData.image;
            text.text = CardData.number.ToString();
        }
        
        #region Drag handling

        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalParent = transform.parent;
            _originalPosition = transform.position;
            transform.SetParent(_originalParent.root);
            SetInteractable(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / transform.lossyScale.x;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if(!_isDropped) // if card is just hovering not dropped
            {
                transform.SetParent(_originalParent);
                transform.position = _originalPosition;
                SetInteractable(true);
            }
        }
        
        public void DropCardUI(Transform parent = null)
        {
            if (parent == null)
            {
                transform.SetParent(_originalParent);
            }
            else
            {
                transform.SetParent(parent);
            }
        }
        
        public void SetInteractable(bool isActive)
        {
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.alpha = isActive ? 1 : 0.5f;
        }
        
        public void SetActiveAndInteractable(bool isActive)
        {
            gameObject.SetActive(isActive);
            SetInteractable(isActive);
        }

        #endregion
        
        # region CardData Utils
        public int GetCardNumber()
        {
            return CardData.number;
        }
        
        public CardColor GetCardColor()
        {
            return CardData.cardColor;
        }
        
        public PowerUpType GetPowerUp()
        {
            return CardData.powerUp;
        }
        
        #endregion
        
        public void PrepareForPool()
        {
            // _isDropped = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public void SetDrop(bool isDropped)
        {
            _isDropped = isDropped;
        }
    }
}