using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CardGame.Scripts.Managers;

namespace CardGame.Scripts.Game_Elements
{
    /// <summary>
    /// Represents a card drop zone that could be:
    /// - Middle stack
    /// - Own player stack
    /// - Other player stack
    /// Handles card placement validation and misplay penalties
    /// </summary>
    public class DropZone : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Player ownerPlayer; // The player who owns this drop zone
        public List<CardUI> faceUpDeck = new(); 
        
        private Player _currentPlayer;
        private CardUI _droppedCard;
    
        public void OnDrop(PointerEventData eventData)
        {
            _droppedCard = eventData.pointerDrag.GetComponent<CardUI>();
            if (_droppedCard == null) return;
            
            _currentPlayer = GameManager.instance.GetCurrentPlayer();
            if (_currentPlayer == null) return;
            
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
                if (ownerPlayer == null) return _droppedCard.GetCardNumber() == 1;
                if (ownerPlayer == _currentPlayer) return true;

                return false;
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
            _droppedCard.SetInteractable(false);
            
            //disable previous card ui in face up stack
            CardUI faceUpCard = GetTopCard(); 
            if (faceUpCard != null)
            {
                faceUpCard.gameObject.SetActive(false);
            }
            
            faceUpDeck.Add(_droppedCard); //add to stack
            
            HandleGameLogicAfterPlacement();
        }
        
        private void HandleGameLogicAfterPlacement()
        {
            if (ownerPlayer == null) // Center stack
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
            else // Own stack
            {
                _currentPlayer.ProcessCardPlay(_droppedCard, false, false);
            }
        }

        private void HandleInvalidDrop()
        {
            bool isPenaltyCase = ownerPlayer == null || _currentPlayer != ownerPlayer;
            
            if (isPenaltyCase)
            {
                GameManager.instance?.GivePenalty(_currentPlayer);
                _droppedCard.SetDropState(true);
                _currentPlayer?.EndTurn();
            }
            else
            {
                _droppedCard.DropCardUI();
                _droppedCard.SetInteractable(true); 
                _droppedCard.SetDropState(false);
            }
        }
        
        public void AddCardToStack(CardUI cardUI)
        {
            if (cardUI == null) return;
            
            cardUI.transform.SetParent(transform);
            faceUpDeck.Add(cardUI);
        }
        
        public CardUI GetTopCard()
        {
            return faceUpDeck.Count == 0 ? null : faceUpDeck[faceUpDeck.Count - 1];
        }
    }
}