using System;
using UnityEngine;

namespace CardGame.Scripts.Game_Elements
{
    [Serializable]
    public class Card
    {
        public int number;
        public Sprite image;
        public CardColor cardColor;
        public PowerUpType powerUp;
        
        public Card(int number, CardColor cardColor, PowerUpType powerUp, Sprite image)
        {
            this.number = number;
            this.image = image;
            this.cardColor = cardColor;
            this.powerUp = powerUp;
        }
    }
    
    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
    }
    
    public enum PowerUpType
    {
        None,   
        Block,   
        Replay,
        Invade,
        Cover,
        Knockout
    }
}