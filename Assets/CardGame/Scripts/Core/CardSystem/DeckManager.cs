using System.Collections.Generic;
using UnityEngine;
using CardGame.Scripts.Card_Creation_Logic;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Gameplay.PlayerSystem;
using CardGame.Scripts.Managers;

namespace CardGame.Scripts.Core.CardSystem
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] private CardSprites cardSprites;
        public CardColor[] colors;
        public int totalColorCards;
        
        private readonly List<Card> _deck = new();
        private readonly List<CardUI> _deckUI = new();
        private List<Player> _players;
    
        public void InitializeDeck(CardPoolManager cardPoolManager)
        {
            CardFactory.Initialize(cardSprites);
            
            foreach (CardColor color in colors)
            {
                for (int i = 1; i <= totalColorCards; i++)
                {
                    Card cardData = CardFactory.CreateCard(i, color);
                    _deck.Add(cardData);
                    
                    CardUI cardUI = cardPoolManager.GetCardUI(cardData);
                    _deckUI.Add(cardUI);
                }
            }
            
            ShuffleDeck();
        }
    
        /// <summary>
        /// Fisher Yates algorithm for shuffling cards
        /// </summary>
        private void ShuffleDeck()
        {
            for (int i = _deck.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1); // Random index from 0 to i
                (_deck[i], _deck[j]) = (_deck[j], _deck[i]); // Swap elements
            }
        }
        
        public void DealCards()
        {
            _players = GameManager.instance.allPlayers;
        
            int cardsPerPlayer = _deck.Count / _players.Count;

            for (int i = 0; i < cardsPerPlayer; i++)
            {
                foreach (Player player in _players)
                {
                    if (_deck.Count == 0) return; // Stop if the deck runs out of cards

                    CardUI drawnCard = _deckUI[_deck.Count-1]; // Take from the top
                    player.AddCard(drawnCard);
                    _deck.RemoveAt(_deck.Count - 1);
                }
            }
        }
    }
}