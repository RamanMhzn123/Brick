using System;
using System.Collections;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Scripts.UI
{
    public class PowerUpUIManager : MonoBehaviour
    {
        public GameObject playerSelectionPanel; // Reference to the player selection panel
        public GameObject powerUpUsedAnimation;
        
        public Button playerButtonPrefab; // Prefab for player buttons
        public Transform buttonContainer; // Parent object for player buttons

        private Action<Player> _onPlayerSelected; // Callback for player selection
        private Player _currentPlayer; // The player who is selecting a target

        public static PowerUpUIManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            powerUpUsedAnimation.SetActive(false);
            playerSelectionPanel.SetActive(false);
        }
        
        public void ShowPowerUI(Player currentPlayer, PowerUpType powerUpType, Action<Player> onPlayerSelected)
        {
            _currentPlayer = currentPlayer;
            _onPlayerSelected = onPlayerSelected;

            StartCoroutine(ShowPowerUpEffectThenSelection(powerUpType));
        }
        
        private IEnumerator ShowPowerUpEffectThenSelection(PowerUpType powerUpType)
        {
            powerUpUsedAnimation.SetActive(true);
            
            yield return new WaitForSeconds(1);

            powerUpUsedAnimation.SetActive(false);

            if (powerUpType == PowerUpType.Block || powerUpType == PowerUpType.Knockout)
            {
                ShowPlayerSelection();
            }
            else
            {
                _onPlayerSelected?.Invoke(null);
            }
        }

        private void ShowPlayerSelection()
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var player in GameManager.instance.allPlayers)
            {
                if (player != _currentPlayer)
                {
                    Button playerButton = Instantiate(playerButtonPrefab, buttonContainer);
                    playerButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Player {player.id}";
                    playerButton.onClick.AddListener(() => OnPlayerSelected(player));
                }
            }
            
            playerSelectionPanel.SetActive(true);
        }
        
        private void OnPlayerSelected(Player selectedPlayer)
        {
            _onPlayerSelected?.Invoke(selectedPlayer);
            playerSelectionPanel.SetActive(false);
        }
    }
}