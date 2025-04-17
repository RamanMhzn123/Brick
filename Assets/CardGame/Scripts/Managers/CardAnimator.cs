using CardGame.Scripts.Game_Elements;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Create moving cards
/// </summary>
public class CardAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Sprite cardSprite;
    [SerializeField] private float moveDuration = 1f; // Duration of the card movement
    [SerializeField] private float flipDuration = 0.5f; // Duration of the card flip
    [SerializeField] private Ease moveEase = Ease.OutQuad; // Easing for movement
    [SerializeField] private Ease flipEase = Ease.InOutQuad; // Easing for flipping
    
    /// <summary>
    /// Animates the card transfer from one position to another.
    /// </summary>
    /// <param name="startPosition">The starting position of the card.</param>
    /// <param name="endPosition">The destination position of the card.</param>
    public void AnimateCardTransfer(Vector3 startPosition, Vector3 endPosition)
    {
        // Set the card's initial position
        // card.transform.position = startPosition;
        //
        // // Animate the card movement
        // card.transform.DOMove(endPosition, moveDuration)
        //     .SetEase(moveEase)
        //     .OnStart(() =>
        //     {
        //         Debug.Log("Card transfer animation started.");
        //     })
        //     .OnComplete(() =>
        //     {
        //         Debug.Log("Card transfer animation completed.");
        //     });
        //
        // // Optional: Animate the card flip
        // AnimateCardFlip(card);
    }

    /// <summary>
    /// Animates the card flipping to show its face.
    /// </summary>
    /// <param name="card">The card UI to animate.</param>
    private void AnimateCardFlip(CardUI card)
    {
        // Flip the card using scaling
        card.transform.DOScaleX(0, flipDuration / 2)
            .SetEase(flipEase)
            .OnComplete(() =>
            {
                // Change the card's sprite to its face
                // card.ShowFace();

                // Scale back to normal size
                card.transform.DOScaleX(1, flipDuration / 2)
                    .SetEase(flipEase);
            });
    }
}