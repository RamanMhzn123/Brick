namespace CardGame.Scripts.Game_Elements
{
    public interface IPlayer
    {
        void AddCard(CardUI card);
        void EndTurn();
    }
}