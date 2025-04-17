using UnityEngine;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.PowerHandler.PowerUp
{
    public class CoverCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            Debug.Log($"{powerUser.name} has used cover power up.");
            powerUser.coverTurns++;
        }
    }
}