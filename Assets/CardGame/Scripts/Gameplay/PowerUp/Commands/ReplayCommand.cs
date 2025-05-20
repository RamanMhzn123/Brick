using UnityEngine;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
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