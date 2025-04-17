using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.PowerHandler
{
    public interface IPowerUpCommand
    {
        void Execute(Player powerUser,  Player targetPlayer = null);
    }
}
