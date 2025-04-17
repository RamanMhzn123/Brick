using System;
using System.Collections.Generic;
using UnityEngine;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.PowerHandler.PowerUp;
using CardGame.Scripts.UI;

namespace CardGame.Scripts.PowerHandler
{
    public class PowerUpManager : MonoBehaviour
    {
        private PowerUpType _currentPowerUpType;
        private Player _powerUser;
        private Dictionary<PowerUpType, IPowerUpCommand> _powerUpCommands;
        public static Action<PowerUpType> OnPowerUpUsed;
        
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
        
        public void ApplyPowerUpEffect(PowerUpType powerUpType, Player powerUser)
        {
            _currentPowerUpType = powerUpType;
            _powerUser = powerUser;
            
            PowerUpUIManager.instance.ShowPowerUI(powerUser, powerUpType, ExecutePowerUpEffect);
        }
        
        private void ExecutePowerUpEffect(Player targetPlayer = null)
        {
            if (_powerUpCommands.TryGetValue(_currentPowerUpType, out IPowerUpCommand command))
            {
                command.Execute(_powerUser, targetPlayer);
            }
            else
            {
                Debug.LogWarning($"No command found for power-up: {_currentPowerUpType}");
            }
        }
    }
}