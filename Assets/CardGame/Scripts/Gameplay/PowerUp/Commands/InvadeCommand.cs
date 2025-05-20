using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Gameplay.PlayerSystem;
using UnityEngine;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class InvadeCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            Debug.Log($"{powerUser.name} has used invade power up.");

            foreach (Player player in GameManager.instance.allPlayers)
            {
                if (player == powerUser) continue;
                
                CardUI cardToGive = powerUser.GetFaceDownCard();
                player.AddCardToBottom(cardToGive);
                Debug.Log($"{powerUser.name} gave {cardToGive.CardData.number} to {player.name}");
            }
        }
    }
}