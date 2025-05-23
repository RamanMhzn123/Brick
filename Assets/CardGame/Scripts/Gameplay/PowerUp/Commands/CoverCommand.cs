using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class CoverCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            powerUser.GetPlayerUI().SetCoverImage();
            powerUser.GivePowerUp(PowerUpType.Cover);
        }
    }
}