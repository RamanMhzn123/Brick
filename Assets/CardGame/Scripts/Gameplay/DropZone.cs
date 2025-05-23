using System.Collections.Generic;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Gameplay.PlayerSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame.Scripts.Gameplay
{
    /// <summary>
    /// Represents a card drop zone that could be:
    /// - Middle stack
    /// - All player stack
    /// Handles card placement validation and misplay penalties
    /// </summary>
    public class DropZone : MonoBehaviour, IDropHandler
    {
		public CardColor zoneColor = CardColor.None;
        [SerializeField] private Player ownerPlayer; // The player who owns this drop zone
        public List<CardUI> faceUpDeck = new(); 
        
        private Player _currentPlayer;
        private CardUI _droppedCard;
    
        public void OnDrop(PointerEventData eventData)
        {
            _droppedCard = eventData.pointerDrag.GetComponent<CardUI>();
            if (!_droppedCard) return;
            
            _currentPlayer = GameManager.instance.GetCurrentPlayer();
            if (!_currentPlayer) return;
            
            if (_droppedCard.CardData == null) return;
            
            HandleCardDrop();
        }

        private void HandleCardDrop()
        {
            if (faceUpDeck.Contains(_droppedCard)) return;
            
            if (IsValidCardPlacement())
            {
                PlaceCardInZone();
            }
            else
            {
                HandleInvalidDrop();
            }
        }
        
        // Validate the card placement
        // Central stack: Card must be the next number AND match the color
        // Player stack: Card only needs to be the next number (color doesn't matter)
        private bool IsValidCardPlacement()
        {
            if (faceUpDeck.Count == 0)
            {
                if (!ownerPlayer)
                {
                    return _droppedCard.GetCardNumber() == 1 && _droppedCard.GetCardColor() ==  zoneColor;
                }
                
                return ownerPlayer == _currentPlayer;
            }

            if (ownerPlayer == _currentPlayer)
            {
                return true;
            }
            
            return ValidateStandardPlacementRules();
        }
        
        private bool ValidateStandardPlacementRules()
        {
            Card topCard = faceUpDeck[faceUpDeck.Count - 1].CardData;
            bool isNextInSequence = _droppedCard.GetCardNumber() == topCard.number + 1;

            if (!isNextInSequence)
            {
                return false;
            }
            
            // Center stack requires matching color
            if (ownerPlayer == null)
            {
                return _droppedCard.GetCardColor() == topCard.cardColor;
            }

            // Opponent's stack can't be covered if they have power-up
            return !ownerPlayer.powerUpData.IsCovered;
        }
        
        private void PlaceCardInZone()
        {
            _droppedCard.DropCardUI(transform);
            _droppedCard.SetDrop(true);
            _droppedCard.SetInteractable(false);
            
            //disable previous card ui in face up stack
            CardUI faceUpCard = GetTopCard(); 
            if (faceUpCard)
            {
                faceUpCard.gameObject.SetActive(false);
            }
            
            faceUpDeck.Add(_droppedCard); //add to stack
            
            HandleGameLogicAfterPlacement();
        }
        
        private void HandleGameLogicAfterPlacement()
        {
            if (!ownerPlayer) // Center stack
            {
                _currentPlayer.RemoveCard(_droppedCard);
                _currentPlayer.ProcessCardPlay(_droppedCard, true, true);
            }
            else if (_currentPlayer != ownerPlayer) // Opponent's stack
            {
                ownerPlayer.UpdatePlayerUI();
                _currentPlayer.RemoveCard(_droppedCard);
                _currentPlayer.ProcessCardPlay(_droppedCard, false, true);
            }
            else // Own stack (this won't be called)
            {
                _currentPlayer.ProcessCardPlay(_droppedCard, false, false);
            }
        }

        /// <summary>
        /// Handle wrong move or didn't drop card
        /// </summary>
        private void HandleInvalidDrop()
        {
            bool isPenaltyCase = !ownerPlayer || _currentPlayer != ownerPlayer;

            if (!isPenaltyCase) return;
            
            GameManager.instance?.CheckForPenalty(_currentPlayer, _droppedCard);

            _droppedCard.DropCardUI();
            _droppedCard.SetDrop(true);
            _droppedCard.SetInteractable(false);
        }
        
        public void AddCardToStack(CardUI cardUI)
        {
            if (!cardUI) return;
            
            cardUI.transform.SetParent(transform);
            faceUpDeck.Add(cardUI);
        }
        
        /// <summary>
        /// Retrieves the top card from the face-up deck.
        /// </summary>
        /// <returns>returns null if the face up deck is empty</returns>
        public CardUI GetTopCard()
        {
            return faceUpDeck.Count != 0 ? faceUpDeck[faceUpDeck.Count - 1] : null;
        }
    }
}