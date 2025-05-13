using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.PowerHandler
{
    public interface IPowerUpCommand
    {
        void Execute(Player powerUser,  Player targetPlayer = null);
    }
}
