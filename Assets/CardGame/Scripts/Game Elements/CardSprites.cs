using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Scripts.Game_Elements
{
    [CreateAssetMenu(fileName = "CardSprites", menuName = "Card Game/Card Sprites")]
    public class CardSprites : ScriptableObject
    {
        [System.Serializable]
        public class ColorSpritePair
        {
            public CardColor color;
            public Sprite sprite;
        }

        [System.Serializable]
        public class PowerUpSpritePair
        {
            public PowerUpType powerUp;
            public Sprite sprite;
        }

        private Sprite _fallbackSprite;
        
        public List<ColorSpritePair> baseColorSprites;
        public List<PowerUpSpritePair> powerUpSprites;
        
        // Runtime dictionaries for fast lookup
        private Dictionary<CardColor, Sprite> _baseSpriteDict;
        private Dictionary<PowerUpType, Sprite> _powerUpSpriteDict;

        // Call this once at game start to initialize dictionaries
        public void Initialize()
        {
            _baseSpriteDict = new Dictionary<CardColor, Sprite>();
            foreach (var pair in baseColorSprites)
            {
                _baseSpriteDict[pair.color] = pair.sprite;
            }

            _powerUpSpriteDict = new Dictionary<PowerUpType, Sprite>();
            foreach (var pair in powerUpSprites)
            {
                _powerUpSpriteDict[pair.powerUp] = pair.sprite;
            }
        }

        // Fast sprite access methods
        public Sprite GetBaseSprite(CardColor color)
        {
            if (_baseSpriteDict.TryGetValue(color, out var sprite) && sprite != null)
                return sprite;
            
            return _fallbackSprite;
        }

        public Sprite GetPowerUpSprite(PowerUpType powerUp)
        {
            if (_powerUpSpriteDict.TryGetValue(powerUp, out var sprite) && sprite != null)
                return sprite;
            
            return _fallbackSprite;
        }
    }
}