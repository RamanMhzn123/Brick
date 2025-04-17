using System;
using System.Collections.Generic;
using UnityEngine;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.PowerHandler;

namespace CardGame.Scripts.Managers
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }
    
        public event Action<Player> OnTurnChanged;
    
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
                player.OnPlayerTurnFinished += ChangeTurn;
            }
            
            _currentPlayer = _players[_currentPlayerIndex];
            OnTurnChanged?.Invoke(_currentPlayer);
        }

        /// <summary>
        /// Check for penalty before next player turn
        /// </summary>
        private bool CheckPenalty()
        {
            return GameManager.instance.CheckForPenalty(_currentPlayer, _currentCard);
        }

        private void EndTurn(CardUI droppedCard, bool isCenter, bool isSuccess)
        {
            _currentCard = droppedCard;
            
            if (!isSuccess)  //card dropped in own
            {
                HandleUnsuccessfulPlay();
                return;
            }

            if (!isCenter) //card dropped in other player
            {
                OnTurnChanged?.Invoke(_currentPlayer);
                return;
            }
            
            if (droppedCard.GetPowerUp() == PowerUpType.None) //card dropped in center
            {
                OnTurnChanged?.Invoke(_currentPlayer);
                return;
            }

            _powerUpManager.ApplyPowerUpEffect(droppedCard.GetPowerUp(), _currentPlayer);
            OnTurnChanged?.Invoke(_currentPlayer); //invoke here?
        }

        private void ChangeTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _currentPlayer = _players[_currentPlayerIndex];
            OnTurnChanged?.Invoke(_currentPlayer);
        }
        
        private void HandleUnsuccessfulPlay()
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
        
        private void AdvanceTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _currentPlayer = _players[_currentPlayerIndex];

            if(_currentPlayer.blockedTurns > 0)
            {
                Debug.Log($"Player {_currentPlayer.id} is blocked and cannot play this turn.");
                _currentPlayer.ReducePowerUp(PowerUpType.Block);

                _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
                _currentPlayer = _players[_currentPlayerIndex];
                OnTurnChanged?.Invoke(_currentPlayer);
            }
            else
            {
                OnTurnChanged?.Invoke(_currentPlayer);
            }
        }
    }
}