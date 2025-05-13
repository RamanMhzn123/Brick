using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CardGame.Scripts.Card_Creation_Logic;
using CardGame.Scripts.Core.CardSystem;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.Gameplay;
using CardGame.Scripts.Gameplay.PlayerSystem;
using CardGame.Scripts.PowerHandler;

namespace CardGame.Scripts.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public List<DropZone> centerZone;

        [Header("Player Settings")]
        public List<RectTransform> twoPlayerPositions; //position maybe different based on no. of player
        public Player playerPrefab;
        public int playerCount;
        public List<Player> allPlayers;
        private Player _currentPlayer;
        
        [Header("Events")]
        public UnityEvent<Player> onGameOver;
        public static GameManager instance;
        
        [Header("Managers")]
        [SerializeField] private PowerUpManager powerUpManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CardPoolManager cardPoolManager;
        [SerializeField] private CardAnimator cardAnimator;
        [SerializeField] private PenaltyManager penaltyManager;
        
        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            cardPoolManager.InitializeCardPool();
            deckManager.InitializeDeck(cardPoolManager);
            
            InitializePlayers();
            deckManager.DealCards();
            
            powerUpManager.Initialize();
            
            TurnManager.Instance.OnTurnChanged += player =>{_currentPlayer = player;};
            TurnManager.Instance.Initialize(allPlayers, powerUpManager);
            
            penaltyManager.Initialize(allPlayers, cardAnimator, cardPoolManager);
        }
        
        /// <summary>
        /// Initialize the card object pool.
        /// </summary>
        private void InitializePlayers()
        {
            for (int i = 0; i < playerCount; i++)
            {
                Player player = Instantiate(playerPrefab,twoPlayerPositions[i])
                                .GetComponent<Player>();
                player.gameObject.name = "Player" + i;
                player.InitializePlayer(i);
                allPlayers.Add(player);
            }
            
            _currentPlayer = allPlayers[0];
        }
        
        /// <summary>
        /// Return a card to the pool.
        /// </summary>
        public void ReturnCardToPool(CardUI card)
        {
            cardPoolManager.ReturnCardUI(card);
        }

        public Player GetCurrentPlayer()
        {
            return _currentPlayer;
        }
        
        /// <summary>
        /// Check if the current player has won the game
        /// </summary>
        private void CheckForWinner()
        {
            Debug.Log($"Player {_currentPlayer.id} has won the game!");
            // GameOver(_currentPlayer);
        }
        
        public void GameOver(Player winner)
        {
            onGameOver?.Invoke(winner);
        }
        
        #region Penalty

        public void GivePenalty(Player penaltyPlayer)
        {
        }

        public bool CheckForPenalty(Player currentPlayer, CardUI droppedCard)
        {
            return penaltyManager.DetectWrongPlay(currentPlayer, droppedCard);
        }

        #endregion
    }
}