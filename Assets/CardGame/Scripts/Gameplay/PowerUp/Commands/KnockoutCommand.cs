using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class KnockoutCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            if (!powerUser || !targetPlayer)
            {
                return;
            }

            if (targetPlayer.IsCovered())
            {
                GameManager.instance.GivePenalty(powerUser);
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                CardUI card = powerUser.GetFaceDownCard();
                
                if (card)
                {
                    targetPlayer.AddCardToBottom(card);
                }
                else
                {
                    GameManager.instance.GameOver(powerUser);
                    break;
                }
            }
        }
    }
}