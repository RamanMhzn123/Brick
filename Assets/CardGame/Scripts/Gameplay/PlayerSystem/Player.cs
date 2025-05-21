using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Core.Managers;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.UI;

namespace CardGame.Scripts.Gameplay.PlayerSystem
{
    public class Player : MonoBehaviour
    {
        public RectTransform deckTransform;
        [Header("Player Properties")]
        public int id;
        public PlayerPowerUpData  powerUpData = new();
        public int coverTurns;
        public int blockedTurns;
        public int replayTurns;
        
        [Header("UI References")]
        [SerializeField] private Button drawCardButton;
        [SerializeField] private Button passButton;
        [SerializeField] private TimerUI timerUI;
        [SerializeField] private PlayerUI playerUI;
        
        [Header("Player Card")]
        [SerializeField] private DropZone faceUpZone; //player bench
        [SerializeField] private List<CardUI> faceDownDeck = new(); //player hand

        public event Action<CardUI, bool, bool> OnCardPlayed;
        
        private void Awake()
        {
            TurnManager.Instance.OnTurnChanged += HandleTurnChange;
            drawCardButton.onClick.AddListener(DrawCard);
            passButton.onClick.AddListener(EndTurn);
        }
        
        private void OnDestroy()
        {
            TurnManager.Instance.OnTurnChanged -= HandleTurnChange;
            drawCardButton.onClick.RemoveListener(DrawCard);
            passButton.onClick.RemoveListener(EndTurn);
        }
        
        public void InitializePlayer(int playerId)
        {
            id = playerId;
            UpdatePlayerUI();
        }
        
        private void HandleTurnChange(Player currentPlayer)
        {
            bool isMyTurn = this == currentPlayer;
            UpdateTurnUI(isMyTurn);
            
            GetTopFaceUpCard()?.SetInteractable(isMyTurn);
            
            if (powerUpData.IsCovered && !isMyTurn)
            {
                ReducePowerUp(PowerUpType.Cover);
            }
        }
        
        /// <summary>
        /// Draws card from deck to zone
        /// Penalty for misplay
        /// </summary>
        private void DrawCard()
        {
            if (!CanDrawCard())
            {
                return;
            }

            UpdateButtonStates();
            StartTimer();
            DrawAndMoveCard();
            CheckForPenalty();
        }
        
        /// <summary>
        /// Checks if a card can be drawn from the deck,
        /// including flipping the face-up deck if necessary.
        /// </summary>
        private bool CanDrawCard()
        {
            if (faceDownDeck.Count != 0) return true;
            
            if (faceUpZone.faceUpDeck.Count == 0)
            {
                GameManager.instance.GameOver(this);
                return false;
            }
                
            List<CardUI> cardsToFlip = new List<CardUI>(faceUpZone.faceUpDeck);
            faceUpZone.faceUpDeck.Clear();
            
            cardsToFlip.Reverse();
            
            foreach (CardUI cardUI in cardsToFlip)
            {
                cardUI.SetInteractable(true);
                faceDownDeck.Add(cardUI);
                GameManager.instance.ReturnCardToPool(cardUI);
            }
            
            Debug.Log($"Player {id} flipped {faceUpZone.faceUpDeck.Count} cards...");
            return true;
        }
        
        /// <summary>
        /// Handles updating the button and timer states for the player's turn.
        /// </summary>
        private void UpdateTurnUI(bool isMyTurn)
        {
            playerUI.HighlightPlayer(isMyTurn);
            drawCardButton.interactable = isMyTurn;
            passButton.interactable = false;
            
            timerUI.gameObject.SetActive(isMyTurn);

            if (isMyTurn)
            {
                timerUI.StartTimer();
            }
            else
            {
                timerUI.StopTimer();
            }
        }
          
        /// <summary>
        /// Updates the button states for the draw and pass buttons
        /// based on the turn state.
        /// </summary>
        private void UpdateButtonStates()
        {
            drawCardButton.interactable = false;
            passButton.interactable = true;
        }
        
        /// <summary>
        /// Starts the timer for the player's turn.
        /// </summary>
        private void StartTimer()
        {
            timerUI.ResetTimer();
            timerUI.StartTimer();
        }
        
        /// <summary>
        /// Draws a card from the hand or face-up deck and
        /// adds it to the play area.
        /// </summary>
        private void DrawAndMoveCard()
        {
            GetTopFaceUpCard()?.SetActiveAndInteractable(false);
    
            CardUI drawnCard = GetFaceDownCard();
            drawnCard?.SetActiveAndInteractable(true);
            faceUpZone.AddCardToStack(drawnCard);
            UpdatePlayerUI();
        }
        
        /// <summary>
        /// Checks if a penalty condition is triggered after drawing a card,
        /// and ends the turn if necessary.
        /// </summary>
        private void CheckForPenalty()
        {
            if (faceUpZone.faceUpDeck.Count <= 1) return;
            
            CardUI previousCard = faceUpZone.faceUpDeck[faceUpZone.faceUpDeck.Count - 2];
            if (GameManager.instance.CheckForPenalty(this, previousCard))
            {
                Debug.Log("Player got Penalty");
            }
        }
        
        public CardUI GetFaceDownCard()
        {
            if (faceDownDeck.Count == 0) return null;
            
            int lastIndex  = faceDownDeck.Count - 1;
            CardUI card = faceDownDeck[lastIndex];
            faceDownDeck.RemoveAt(lastIndex);
            UpdatePlayerUI();
            return card;
        }
        
        /// <summary>
        /// Add card to player hand from deck manager
        /// </summary>
        /// <param name="card"></param>
        public void AddCard(CardUI card)
        {
            faceDownDeck.Add(card);
            UpdatePlayerUI();
        }
        
        public void AddCardToBottom(CardUI card)
        {
            faceDownDeck.Insert(0, card);
            UpdatePlayerUI();
        }
        
        public void ProcessCardPlay(CardUI card, bool isCenter, bool isSuccess)
        {
            UpdatePlayerUI();
            
            if (isSuccess)
            {
                if (HasPlayerWon())
                {
                    GameManager.instance.GameOver(this);
                    return;
                }
                
                // drawCardButton.interactable = true;
                GetTopFaceUpCard()?.SetActiveAndInteractable(true);
            }
            OnCardPlayed?.Invoke(card, isCenter, isSuccess);
        }

        private bool HasPlayerWon()
        {
            return faceUpZone.faceUpDeck.Count == 0 && faceDownDeck.Count == 0;
        }
        
        /// <summary>
        /// Change/End Turn
        /// </summary>
        public void EndTurn() 
        {
            timerUI.StopTimer();
            passButton.interactable = false;
			
            CardUI activeCard  = GetTopFaceUpCard();
            activeCard?.SetInteractable(false);
            
            OnCardPlayed?.Invoke(activeCard, false, false);
        }
        
        public void UpdatePlayerUI()
        {
            playerUI.UpdateDisplay(faceUpZone.faceUpDeck.Count, faceDownDeck.Count);
        }
        
        public CardUI GetTopFaceUpCard()
        {
            if (!faceUpZone.GetTopCard())
            {
                //TODO: what to do?
            }
            
            return faceUpZone.GetTopCard();
        }

        public void RemoveCard(CardUI cardUI) => faceUpZone.faceUpDeck.Remove(cardUI);
        
        public void ReducePowerUp(PowerUpType powerUpType)
        {
            powerUpData.ReducePowerUpTurns(powerUpType);
        }

        public void StopTimer()
        {
            timerUI.StopTimer();
        }

        public void RestartTimer()
        {
            timerUI.ResetTimer();
        }

        public PlayerUI GetPlayerUI()
        {
            return playerUI;
        }
        
        #region Penalty related

        public bool TryGetPenaltyCard(out CardUI penaltyCard)
        {
            penaltyCard = GetFaceDownCard();
            if (penaltyCard) return true;

            penaltyCard = GetBottomFaceUpCard();
            return penaltyCard;
        }
        
        /// <summary>
        /// When no card to give from hand, take from face up stack
        /// Remove card from stack
        /// After no card = winner
        /// </summary>
        /// <returns></returns>
        private CardUI GetBottomFaceUpCard()
        {
            if (faceUpZone.faceUpDeck.Count == 0) return null;
            
            CardUI cardUI = faceUpZone.faceUpDeck[0];
            faceUpZone.faceUpDeck.RemoveAt(0);
            UpdatePlayerUI();

            if (faceUpZone.faceUpDeck.Count == 0)
            {
                GameManager.instance.GameOver(this);
            }
            
            return cardUI;
        }
        
        public Vector3 GetCardSpawnPosition() // deck position
        {
            return deckTransform.position;
        }
        
        #endregion
    }
}