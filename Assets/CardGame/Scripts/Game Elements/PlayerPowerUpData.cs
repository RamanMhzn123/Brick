using CardGame.Scripts.Managers;
using UnityEngine;

namespace CardGame.Scripts.Game_Elements
{
    [System.Serializable]
    public class PlayerPowerUpData
    {
        public int coverTurns;
        public int blockedTurns;
        public int replayTurns;

        public bool IsCovered => coverTurns > 0;
        public bool IsBlocked => blockedTurns > 0;
        public bool IsReplay => replayTurns > 0;
    
        public void ApplyPowerUp(PowerUpType powerUpType)
        {
            switch (powerUpType)
            {
                case PowerUpType.Cover:
                    coverTurns ++;
                    break;
                case PowerUpType.Block:
                    blockedTurns ++;
                    break;
                case PowerUpType.Replay:
                    replayTurns ++;
                    break;
                default:
                    Debug.LogWarning($"Unknown power-up type: {powerUpType}");
                    break;
            }
        }

        public void ReducePowerUpTurns(PowerUpType powerType)
        {
            if (powerType == PowerUpType.Cover)
            {
                if (coverTurns <= 0) return;
                coverTurns--;
                if (coverTurns == 0)
                {
                    Debug.Log("Cover power-up expired.");
                }
            }
            else if (powerType == PowerUpType.Block)
            {
                if (blockedTurns <= 0) return;
                blockedTurns--;
                if (blockedTurns == 0)
                {
                    Debug.Log("Block power-up expired.");
                }
            }
            else if (powerType == PowerUpType.Replay)
            {
                if (replayTurns <= 0) return;
                replayTurns--;
                if (replayTurns == 0)
                {
                    Debug.Log("Replay power-up expired.");
                }
            }
            else
            {
                Debug.LogWarning($"Unknown power-up type: {powerType}");
            }
        }
    }
}