using System.Collections.Generic;
using UnityEngine;
using CardGame.Scripts.Game_Elements;

namespace CardGame.Scripts.Managers
{
    public static class CardFactory
    {
        private static CardSprites _sprites;
        
        private static readonly Dictionary<CardColor, PowerUpType> ColorPowerMapping = new()
        {
            { CardColor.Red, PowerUpType.Block },       // Red = Block
            { CardColor.Blue, PowerUpType.Replay },     // Blue = Replay Power
            { CardColor.Purple, PowerUpType.Invade },   // Purple = Invade
            { CardColor.Green, PowerUpType.Cover },     // Green = Cover
            { CardColor.Yellow, PowerUpType.Knockout }  // Yellow = Knockout
        };
        
        public static void Initialize(CardSprites sprites)
        {
            _sprites = sprites;
            _sprites.Initialize();
        }
        
        public static Card CreateCard(int number, CardColor color)
        {                 
            PowerUpType assignedPower = PowerUpType.None;
            Sprite cardSprite;
            
            if (number == 3 || number == 6 || number == 9)
            {
                assignedPower = ColorPowerMapping[color];
                cardSprite = _sprites.GetPowerUpSprite(assignedPower);
            }else
            {
                cardSprite = _sprites.GetBaseSprite(color);
            }
            
            return new Card(number, color, assignedPower, cardSprite);
        }

        /// <summary>
        /// For back card sprite with different color
        /// </summary>
        /// <returns></returns>
        public static Card CreateBackCard()
        {
            Sprite cardSprite = _sprites.backCardSprite;
            return new Card(image:cardSprite);
        }
    }
}