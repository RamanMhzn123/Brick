using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class ReplayCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayerr)
        {
            powerUser.GivePowerUp(PowerUpType.Replay);
        }
    }
}