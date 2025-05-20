using System;
using System.Collections.Generic;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Gameplay.PlayerSystem;
using CardGame.Scripts.Gameplay.PowerUp.Commands;
using CardGame.Scripts.UI;
using UnityEngine;

namespace CardGame.Scripts.Gameplay.PowerUp
{
    public class PowerUpManager : MonoBehaviour
    {
        private PowerUpType _currentPowerUpType;
        private Player _powerUser;
        private Dictionary<PowerUpType, IPowerUpCommand> _powerUpCommands;
        private static Action _onPowerUpApplied;
        
        public void Initialize()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            _powerUpCommands = new Dictionary<PowerUpType, IPowerUpCommand>
            {
                { PowerUpType.Block, new BlockCommand() },
                { PowerUpType.Replay, new ReplayCommand() },
                { PowerUpType.Invade, new InvadeCommand() },
                { PowerUpType.Cover, new CoverCommand() },
                { PowerUpType.Knockout, new KnockoutCommand() },
            };
        }
        
        public void ApplyPowerUpEffect(PowerUpType powerUpType, Player powerUser, Action onPowerUpApplied)
        {
            _currentPowerUpType = powerUpType;
            _powerUser = powerUser;
            _onPowerUpApplied = onPowerUpApplied;
            
            PowerUpUIManager.instance.ShowPowerUI(powerUser, powerUpType, ExecutePowerUpEffect);
        }
        
        private void ExecutePowerUpEffect(Player targetPlayer = null)
        {
            if (_powerUpCommands.TryGetValue(_currentPowerUpType, out IPowerUpCommand command))
            {
                command.Execute(_powerUser, targetPlayer);
                _onPowerUpApplied?.Invoke();
            }
            else
            {
                Debug.LogWarning($"No command found for power-up: {_currentPowerUpType}");
            }
        }
    }
}