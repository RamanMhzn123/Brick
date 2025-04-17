using UnityEngine;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.PowerHandler.PowerUp
{
    public class KnockoutCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            Debug.Log($"{powerUser.name} has used knockout power up.");
            
            if (powerUser == null || targetPlayer == null)
            {
                Debug.LogError("KnockoutCommand: Power user or target player is null.");
                return;
            }

            if (targetPlayer.powerUpData.IsCovered)
            {
                Debug.Log($"Cannot knockout the Player {targetPlayer.id}. Player has a cover. Penalty!!!");
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                CardUI card = powerUser.GetFaceDownCard();
                if (card != null)
                {
                    targetPlayer.AddCardToBottom(card);
                    Debug.Log($"Player {targetPlayer.id} received {card.CardData.number} from Player {powerUser.id}.");
                }
                else
                {
                    Debug.LogWarning($"Player {powerUser.id} has no more cards to transfer.");
                    break;
                }
            }
        }
    }
}