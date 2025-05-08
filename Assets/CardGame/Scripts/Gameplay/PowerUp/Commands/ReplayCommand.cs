using UnityEngine;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.PowerHandler.PowerUp
{
    public class ReplayCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayerr)
        {
            Debug.Log($"{powerUser.name} has used replay power up.");
            powerUser.replayTurns++;
        }
    }
}