using CardGame.Scripts.Gameplay.PlayerSystem;

namespace CardGame.Scripts.Gameplay.PowerUp
{
    public interface IPowerUpCommand
    {
        void Execute(Player powerUser,  Player targetPlayer = null);
    }
}
