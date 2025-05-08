using UnityEngine;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.Card_Creation_Logic
{
    public class CardPoolManager : ObjectPoolManager
    {
        [Header("Pool Configuration")]
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject backCardPrefab;

        [SerializeField] private int cardPoolSize = 20;
        [SerializeField] private int backCardPoolSize = 4;

        private const string CardPoolKey = "card_pool";
        private const string BackCardPoolKey = "back_card_pool";
        
        public void InitializeCardPool()
        {
            if (cardPrefab == null)
            {
                Debug.LogError("Card prefab is not assigned!");
                return;
            }
            
            CreatePool(CardPoolKey, cardPrefab, cardPoolSize);
            CreatePool(BackCardPoolKey, backCardPrefab, backCardPoolSize);
        }
        
        public CardUI GetCardUI(Card cardData, Transform customParent = null)
        {
            CardUI cardUI = GetObject(CardPoolKey).GetComponentInChildren<CardUI>();
            
            if (cardUI == null) return null;
            
            cardUI.transform.SetParent(customParent ?? poolParent);
            
            cardUI.Initialize(cardData);
            return cardUI;
        }

        public void ReturnCardUI(CardUI cardUI, bool resetParent = true)
        {
            if (cardUI == null) return;
            
            cardUI.PrepareForPool();
            
            if (resetParent)
            {
                cardUI.transform.SetParent(poolParent);
            }
            
            ReturnObject(CardPoolKey, cardUI.gameObject);
        }

        public CardUI GetBackCardUI(Transform customParent = null)
        {
            CardUI cardUI = GetObject(BackCardPoolKey).GetComponentInChildren<CardUI>();
            
            if (!cardUI) return null;
            
            cardUI.transform.SetParent(customParent ?? poolParent);
            return cardUI;
        }
        
        public void ReturnCardBack(CardUI cardUI, bool resetParent = true)
        {
            if (!cardUI) return;
            
            cardUI.PrepareForPool();
            
            if (resetParent)
            {
                cardUI.transform.SetParent(poolParent);
            }
            
            ReturnObject(CardPoolKey, cardUI.gameObject);
        }
    }
}