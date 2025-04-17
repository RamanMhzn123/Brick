using UnityEngine;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.PowerHandler.PowerUp
{
    public class BlockCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            Debug.Log($"{powerUser.name} has used block power up.");

            if (targetPlayer == null)
            {
                Debug.LogError("BlockCommand: Target player is null.");
                return;
            }
            
            if (targetPlayer.powerUpData.IsCovered)
            {
                Debug.Log($"Cannot block the Player {targetPlayer.id}. Player has a cover. Penalty!!!");
                return;
            }
            
            targetPlayer.blockedTurns++;
        }
    }
}