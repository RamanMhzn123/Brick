using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGame.Scripts.Card_Creation_Logic;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Gameplay;
using CardGame.Scripts.Gameplay.PlayerSystem;
using UnityEngine.Events;

namespace CardGame.Scripts.Core.Managers
{
    public class PenaltyManager : MonoBehaviour
    {
        public UnityEvent onPenaltyApplied;
        
        private List<Player> _allPlayers;
        private List<DropZone> _centerDropZones;
        private CardAnimator _cardAnimator;
        private CardPoolManager _cardPoolManager;

        private CardUI _playedCard;
        private Player _penaltyPlayer;
        
        private Coroutine _penaltyCoroutine;
        
        public void Initialize(List<Player> players, CardAnimator cardAnimator, CardPoolManager cardPoolManager)
        {
            _allPlayers = players;
            _centerDropZones = GameManager.instance.centerZone;
            
            _cardAnimator = cardAnimator;
            _cardPoolManager = cardPoolManager;
        }
        
        public bool DetectWrongPlay(Player penaltyPlayer, CardUI droppedCard)
        {
            _playedCard = droppedCard;
            _penaltyPlayer = penaltyPlayer;
            
            if (CheckCenterZone())
                return true;
            
            return CheckOtherPlayers();
        }
        
        private bool CheckCenterZone()
        {
            DropZone colorZone = null;
            foreach (DropZone zone in _centerDropZones)
            {
                if (zone.zoneColor != _playedCard.GetCardColor()) continue;
                
                colorZone = zone;
                break;
            }

            if (!colorZone) return false;
            
            CardUI topCard = colorZone.GetTopCard();
                
            if (IsInvalidPlay(topCard, true))
            {
                StartCoroutine(C_ApplyPenalty(_penaltyPlayer));
                return true;
            }

            return false;
        }
        
        private bool CheckOtherPlayers()
        {
            foreach (Player player in _allPlayers)
            {
                if (player == _penaltyPlayer) continue;
        
                CardUI topCard = player.GetTopFaceUpCard();

                if (IsInvalidPlay(topCard, false))
                {
                    StartCoroutine(C_ApplyPenalty(_penaltyPlayer));
                    return true;
                }
            }
            return false;
        }
        
        private bool IsInvalidPlay(CardUI topCard, bool isCenterZone)
        {
            if (!topCard)
            {
                return isCenterZone && _playedCard.GetCardNumber() == 1;
            }

            return _playedCard.GetCardNumber() == topCard.GetCardNumber() + 1;
        }
        
        public void GivePenalty(Player penaltyPlayer)
        {
            if (_penaltyCoroutine != null)
            {
                StopCoroutine(_penaltyCoroutine);
                _penaltyCoroutine = null;
            }
            
            _penaltyCoroutine = StartCoroutine(C_ApplyPenalty(penaltyPlayer));
        }
        
        private IEnumerator C_ApplyPenalty(Player penalizedPlayer)
        {
            if (!penalizedPlayer) yield break;

            List<CardUI> activeCardBacks = new List<CardUI>();
            List<IEnumerator> transfers = new List<IEnumerator>();

            try
            {
                foreach (Player player in _allPlayers)
                {
                    if (player == penalizedPlayer) continue;

                    if (!player.TryGetPenaltyCard(out CardUI penaltyCard)) continue;
                    
                    CardUI cardBack = _cardPoolManager.GetBackCardUI();
                    cardBack.transform.position = player.GetCardSpawnPosition();
                    activeCardBacks.Add(cardBack);

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
            
            onPenaltyApplied?.Invoke();
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

        private void ReturnAllVisualsToPool(List<CardUI> visuals)
        {
            foreach (CardUI visual in visuals)
            {
                if (visual.gameObject.activeInHierarchy)
                {
                    _cardPoolManager.ReturnCardBack(visual);
                }
            }
        }
    }
}