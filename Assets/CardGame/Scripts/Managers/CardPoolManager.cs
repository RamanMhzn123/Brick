using UnityEngine;
using CardGame.Scripts.Game_Elements;
using UnityEngine.Serialization;

namespace CardGame.Scripts.Managers
{
    public class CardPoolManager : ObjectPoolManager<CardUI>
    {
        [Header("Pool Configuration")]
        [SerializeField] private CardUI cardPrefab;
        [SerializeField] private Transform poolParent;
        [SerializeField] private int initialPoolSize = 20;
        
        private const string CardPoolKey = "card_pool";

        public void InitializeCardPool()
        {
            if (cardPrefab == null)
            {
                Debug.LogError("Card prefab is not assigned!");
                return;
            }

            // Create parent if not assigned
            if (poolParent == null)
            {
                poolParent = new GameObject("CardPool").transform;
                poolParent.SetParent(transform);
            }
            
            CreatePool(CardPoolKey, cardPrefab, initialPoolSize);
        }
        
        public CardUI GetCardUI(Card cardData, Transform customParent = null)
        {
            CardUI cardUI = GetObject(CardPoolKey);
            
            if (cardUI == null)
            {
                Debug.LogWarning("Failed to get card from pool");
                return null;
            }
            
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
    }
}