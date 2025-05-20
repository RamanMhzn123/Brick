using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class BlockCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            if (!targetPlayer) return;
            
            if (targetPlayer.powerUpData.IsCovered)
            {
                GameManager.instance.GivePenalty(powerUser);
                return;
            }
            
            targetPlayer.blockedTurns++;
        }
    }
}