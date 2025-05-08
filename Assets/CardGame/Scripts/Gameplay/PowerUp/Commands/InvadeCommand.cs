using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using UnityEngine;
using CardGame.Scripts.Managers;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.PowerHandler.PowerUp
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