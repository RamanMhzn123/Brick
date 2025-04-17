using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.Scripts.Managers;
using CardGame.Scripts.UI;

namespace CardGame.Scripts.Game_Elements
{
    public class Player : MonoBehaviour
    {
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
        public event Action OnPlayerTurnFinished;
        
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
            
            passButton.interactable = false;
            timerUI.gameObject.SetActive(isMyTurn);
            drawCardButton.interactable = isMyTurn;
            
            if (isMyTurn)
            {
                timerUI.StartTimer();
            }

            // drawCardButton.interactable = isMyTurn && faceDownDeck.Count > 0;
            
            GetTopFaceUpCard()?.SetInteractable(isMyTurn);
            
            if (powerUpData.IsCovered && !isMyTurn)
            {
                ReducePowerUp(PowerUpType.Cover);
            }
        }
        
        private void DrawCard()
        {
            if (faceDownDeck.Count == 0 && !TryFlipFaceUpDeck())
            {
                return;
            }

            passButton.interactable = true;
            drawCardButton.interactable = false;
            
            timerUI.ResetTimer();
            timerUI.StartTimer();
            
            GetTopFaceUpCard()?.SetActiveAndInteractable(false);
            
            CardUI drawnCard = GetFaceDownCard();
            drawnCard?.SetActiveAndInteractable(true);
            faceUpZone.AddCardToStack(drawnCard);
            UpdatePlayerUI();
        }
        
        private bool TryFlipFaceUpDeck()
        {
            if (faceUpZone.faceUpDeck.Count == 0)
            {
                GameManager.instance.GameOver(this);
                return false; // No cards to flip, player wins
            }
            
            faceUpZone.faceUpDeck.Reverse(); 
            
            foreach (CardUI cardUI in faceUpZone.faceUpDeck)
            {
                cardUI.SetInteractable(true);
                faceDownDeck.Add(cardUI);
                GameManager.instance.ReturnCardToPool(cardUI);
            }
            
            faceUpZone.faceUpDeck.Clear();
    
            Debug.Log($"Player {id} flipped {faceUpZone.faceUpDeck.Count} cards from the drop zone to their hand.");
            return true;
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
        /// When no card to give from hand, take from face up stack
        /// Remove card from stack
        /// After no card = winner
        /// </summary>
        /// <returns></returns>
        public CardUI GetBottomFaceUpCard()
        {
            if (faceUpZone.faceUpDeck.Count == 0)
            {
                Debug.Log("No cards to return from the face up stack");
                return null;
            }
            
            CardUI cardUI = faceUpZone.faceUpDeck[0];
            faceUpZone.faceUpDeck.RemoveAt(0);
            UpdatePlayerUI();

            if (faceUpZone.faceUpDeck.Count == 0)
            {
                GameManager.instance.GameOver(this);
            }
            
            return cardUI;
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
        
        /// <summary>
        /// Change/End Turn
        /// </summary>
        public void EndTurn() 
        {
            GetTopFaceUpCard()?.SetInteractable(false);
            timerUI.StopTimer();
            OnPlayerTurnFinished?.Invoke();
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
                // drawCardButton.interactable = true;
                GetTopFaceUpCard()?.SetActiveAndInteractable(true);
            }
            OnCardPlayed?.Invoke(card, isCenter, isSuccess);
        }
        
        public void UpdatePlayerUI()
        {
            playerUI.UpdateDisplay(GetFaceUpDeckCount(), faceDownDeck.Count);
        }
        
        public CardUI GetTopFaceUpCard() => faceUpZone.GetTopCard();
        public void RemoveCard(CardUI cardUI) => faceUpZone.faceUpDeck.Remove(cardUI);
        private int GetFaceUpDeckCount() => faceUpZone.faceUpDeck.Count;
        
        public void ReducePowerUp(PowerUpType powerUpType)
        {
            powerUpData.ReducePowerUpTurns(powerUpType);
        }
    }
}