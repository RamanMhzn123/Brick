using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using CardGame.Scripts.Card_Creation_Logic;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.Core.Managers
{
    public class PenaltyManager : MonoBehaviour
    {
        private List<Player> _allPlayers;
        private CardAnimator _cardAnimator;
        private CardPoolManager _cardPoolManager;

        public void Initialize(List<Player> players, CardAnimator cardAnimator, CardPoolManager cardPoolManager)
        {
            _allPlayers = players;
            _cardAnimator = cardAnimator;
            _cardPoolManager = cardPoolManager;
        }
        
        public bool DetectWrongPlay(Player currentPlayer, CardUI droppedCard)
        {
            int number = droppedCard.GetCardNumber();

            foreach (var player in _allPlayers)
            {
                if (player == currentPlayer) continue;

                var cardUI = player.GetTopFaceUpCard();
                if (cardUI != null && number == cardUI.GetCardNumber() + 1)
                {
                    StartCoroutine(ApplyPenalty(currentPlayer));
                    return true;
                }
            }

            return false;
        }

        [Button]
        public void TestPenalty()
        {
            StartCoroutine(ApplyPenalty(_allPlayers[0]));
        }
        
        private IEnumerator ApplyPenalty(Player penalizedPlayer)
        {
            if (!penalizedPlayer) yield break;

            List<CardUI> activeCardBacks = new List<CardUI>();
            List<IEnumerator> transfers = new List<IEnumerator>();

            try
            {
                foreach (var player in _allPlayers)
                {
                    if (player == penalizedPlayer) continue;

                    if (!player.TryGetPenaltyCard(out CardUI penaltyCard)) continue;
                    
                    CardUI cardBack = _cardPoolManager.GetBackCardUI();
                    cardBack.transform.position = player.GetCardSpawnPosition();
                    activeCardBacks.Add(cardBack);

                    // Create and execute transfer
                    transfers.Add(ExecuteCardTransfer(
                        cardBack,
                        penaltyCard,
                        player,
                        penalizedPlayer
                    ));
                }

                yield return StartCoroutine(WaitForAllTransfers(transfers));
            }
            finally
            {
                ReturnAllVisualsToPool(activeCardBacks);
            }
        }

        private void ReturnAllVisualsToPool(List<CardUI> visuals)
        {
            foreach (var visual in visuals)
                if (visual.gameObject.activeInHierarchy)
                    _cardPoolManager.ReturnCardBack(visual);
        }
        
        private IEnumerator ExecuteCardTransfer(CardUI cardBack, CardUI penaltyCard, 
            Player fromPlayer, Player toPlayer)
        {
            bool animationComplete = false;
            
            _cardAnimator.AnimateCardTransfer(
                cardBack.gameObject,
                fromPlayer.GetCardSpawnPosition(),
                toPlayer.GetCardSpawnPosition(),
                () => animationComplete = true
            );
            
            yield return new WaitUntil(() => animationComplete);
            toPlayer.AddCardToBottom(penaltyCard);
        }
        
        private IEnumerator WaitForAllTransfers(List<IEnumerator> transfers)
        {
            foreach (var transfer in transfers)
            {
                yield return StartCoroutine(transfer);
            }
        }
    }
}