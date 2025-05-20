using UnityEngine;

namespace CardGame.Scripts.UI
{ 
    public class GameUIManager : MonoBehaviour
    {
        public GameObject gameOverPanel;
        
        public void ShowGameOverScreen()
        {
            gameOverPanel.SetActive(true);
        }
    }
}
