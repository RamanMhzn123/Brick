using TMPro;
using UnityEngine;

namespace CardGame.Scripts.Game_Elements
{
    public class PlayerUI : MonoBehaviour
    {
        public TextMeshProUGUI faceDownDeckText;
        public TextMeshProUGUI faceUpDeckText;

        public void UpdateDisplay(int up, int down)
        {
            faceUpDeckText.text = up.ToString();
            faceDownDeckText.text = down.ToString();
        }
    }
}