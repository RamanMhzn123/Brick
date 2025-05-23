using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class InvadeCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            foreach (Player player in GameManager.instance.allPlayers)
            {
                if (player == powerUser) continue;
                if(player.IsCovered()) continue;
                
                CardUI cardToGive = powerUser.GetFaceDownCard();
                player.AddCardToBottom(cardToGive);
            }
        }
    }
}