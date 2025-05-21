using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CardGame.Scripts.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public TextMeshProUGUI faceDownDeckText;
        public TextMeshProUGUI faceUpDeckText;
        public Image highlightImage;
        public Image mainImage;
        
        public void UpdateDisplay(int up, int down)
        {
            faceUpDeckText.text = up.ToString();
            faceDownDeckText.text = down.ToString();
        }

        public void SetCoverImage()
        {
            mainImage.color = Color.green;
        }

        public void SetDefaultImage()
        {
            mainImage.color = Color.black ;
        }

        public void HighlightPlayer(bool isMyTurn)
        {
            highlightImage.gameObject.SetActive(isMyTurn);
        }
    }
}