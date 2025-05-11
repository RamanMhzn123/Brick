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
        
        public Card(int number = 0, CardColor cardColor = default, PowerUpType powerUp = default, 
                        Sprite image = null)
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
        None
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