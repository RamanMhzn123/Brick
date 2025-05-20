using CardGame.Scripts.Gameplay.PlayerSystem;
using UnityEngine;

namespace CardGame.Scripts.Gameplay.PowerUp.Commands
{
    public class CoverCommand : IPowerUpCommand
    {
        public void Execute(Player powerUser, Player targetPlayer)
        {
            powerUser.GetPlayerUI().SetCoverImage();
            powerUser.coverTurns++;
        }
    }
}