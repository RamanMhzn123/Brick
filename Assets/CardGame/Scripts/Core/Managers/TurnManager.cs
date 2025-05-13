using System;
using System.Collections.Generic;
using UnityEngine;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Gameplay.PlayerSystem;
using CardGame.Scripts.PowerHandler;

namespace CardGame.Scripts.Core.Managers
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }
    
        public event Action<Player> OnTurnChanged; //issue
    
        private int _currentPlayerIndex;
        private List<Player> _players = new();
        private Player _currentPlayer;
        private CardUI _currentCard;
        private PowerUpType _currentPowerUp;
        private PowerUpManager _powerUpManager;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void Initialize(List<Player> players, PowerUpManager powerUpManager)
        {
            _players = players;
            _powerUpManager = powerUpManager;

            foreach (var player in _players)
            {
                player.OnCardPlayed += EndTurn;
            }
            
            _currentPlayer = _players[_currentPlayerIndex];
            OnTurnChanged?.Invoke(_currentPlayer);
        }
        
        private void EndTurn(CardUI droppedCard, bool isCenter, bool isSuccess)
        {
            _currentCard = droppedCard;
            
            if (!isSuccess)  //card dropped in own
            {
                HandleUnsuccessfulPlay();
                return;
            }

            if (!isCenter) //card dropped in another player
            {
                OnTurnChanged?.Invoke(_currentPlayer);
                return;
            }
            
            if (droppedCard.GetPowerUp() == PowerUpType.None) //card dropped in a center
            {
                OnTurnChanged?.Invoke(_currentPlayer);
                return;
            }

            _powerUpManager.ApplyPowerUpEffect(droppedCard.GetPowerUp(), _currentPlayer);
            OnTurnChanged?.Invoke(_currentPlayer); //invoke here?
        }
        
        private void HandleUnsuccessfulPlay()
        {
            //check penalty
            if (CheckPenalty())
            {
                AdvanceTurn();
            }
            else
            {
                if (_currentPlayer.replayTurns > 0)
                {
                    _currentPlayer.ReducePowerUp(PowerUpType.Replay);
                
                    if (!CheckPenalty())
                    {
                        OnTurnChanged?.Invoke(_currentPlayer);
                    }
                    else
                    {
                        AdvanceTurn();
                    }
                }
                else
                {
                    AdvanceTurn();
                }
            }
        }
        
        /// <summary>
        /// Check for penalty before the next player turn
        /// </summary>
        private bool CheckPenalty()
        {
            return GameManager.instance.CheckForPenalty(_currentPlayer, _currentCard);
        }
        
        private void AdvanceTurn()
        {
            MoveToNextPlayer();

            if(_currentPlayer.blockedTurns > 0)
            {
                _currentPlayer.ReducePowerUp(PowerUpType.Block);
               MoveToNextPlayer();
                OnTurnChanged?.Invoke(_currentPlayer);
            }
            else
            {
                OnTurnChanged?.Invoke(_currentPlayer);
            }
        }
        
        private void MoveToNextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _currentPlayer = _players[_currentPlayerIndex];
        }
    }
}