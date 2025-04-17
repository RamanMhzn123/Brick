using System.Collections.Generic;
using CardGame.Scripts.Game_Elements;
using UnityEngine;

namespace CardGame.Scripts.Managers
{
    /// <summary>
    /// Detect wrong play/miss plays after every turn
    /// </summary>
    public class PenaltyManager
    {
        private List<Player> _allPlayers;
        private CardAnimator _cardAnimator;

        public void Initialize(List<Player> players, CardAnimator cardAnimator)
        {
            _allPlayers = players;
            _cardAnimator = cardAnimator;
        }

        /// <summary>
        /// Played Card by current player
        /// Check all others player stack
        /// TODO : Check center also
        /// </summary>
        public bool DetectWrongPlay(Player currentPlayer, CardUI droppedCard)
        {
            int number = droppedCard.GetCardNumber();
            
            foreach (Player player in _allPlayers)
            {
                if (player == currentPlayer) continue;
                
                CardUI cardUI = player.GetTopFaceUpCard();
                if (cardUI != null && number == cardUI.GetCardNumber() + 1)
                {
                    ApplyPenalty(currentPlayer);
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Applies a penalty to the specified player.
        /// The player receives a card from each other player's top of their hand deck or bottom of their face-up deck.
        /// </summary>
        /// <param name="penalizedPlayer">The player who is being penalized.</param>
        public void ApplyPenalty(Player penalizedPlayer)
        {
            if (penalizedPlayer == null) return;

            foreach (Player player in _allPlayers)
            {
                if (player == penalizedPlayer) continue;

                CardUI penaltyCard = player.GetFaceDownCard();

                if (penaltyCard == null)
                {
                    CardUI bottomFaceUpCard = player.GetBottomFaceUpCard();
                    if (bottomFaceUpCard != null)
                    {
                        
                        penaltyCard = bottomFaceUpCard;
                    }
                }

                if (penaltyCard != null)
                {
                    PlayCardAnimation();
                    
                    penalizedPlayer.AddCardToBottom(penaltyCard);
                    Debug.Log($"Player {penalizedPlayer.id} received a card from Player {player.id}.");
                }
                else
                {
                    Debug.LogWarning($"Player {player.id} has no cards to give as a penalty.");
                }
            }
        }

        private void PlayCardAnimation()
        {
            // Create a CardUI instance for the penalty card
            // CardUI penaltyCardUI = CreateCardUI(penaltyCard);
            //
            // // Get the start and end positions for the animation
            // Vector3 startPosition = player.GetDeckPosition(); // Position of the giving player's deck
            // Vector3 endPosition = penalizedPlayer.GetDeckPosition(); // Position of the receiving player's deck
            // // Animate the card transfer
            // _cardAnimator.AnimateCardTransfer(startPosition, endPosition);
        }
        
        /// <summary>
        /// Pre-create animate card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private CardUI CreateCardUI(Card card)
        {
            // Instantiate a CardUI prefab and set its data
            // CardUI cardUI = Instantiate(cardUIPrefab).GetComponent<CardUI>();
            // cardUI.Initialize(card);
            // return cardUI;
            return null;
        }
        
    }
}