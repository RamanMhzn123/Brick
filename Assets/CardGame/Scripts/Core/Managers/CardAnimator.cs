using System;
using UnityEngine;
using DG.Tweening;

namespace CardGame.Scripts.Core.Managers
{
    public class CardAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite cardSprite;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private Ease moveEase = Ease.OutQuad; 
        
        public void AnimateCardTransfer(GameObject card, Vector3 startPosition, Vector3 endPosition,
                                        Action onComplete = null)
        {
            card.transform.position = startPosition;
            card.SetActive(true);
            
            card.transform.DOMove(endPosition, moveDuration)
                .SetEase(moveEase)
                .OnStart(() =>
                {
                    // Debug.Log("Card transfer animation started.");
                })
                .OnComplete(() =>
                {
                    // Debug.Log("Card transfer animation completed.");
                })
                .OnComplete(() => {
                    // Debug.Log("Card transfer animation completed.");
                    onComplete?.Invoke();
                });
        }
    }
}