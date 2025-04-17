using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CardGame.Scripts.Game_Elements;
using CardGame.Scripts.PowerHandler;

namespace CardGame.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
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
        private PenaltyManager _penaltyManager;
        
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
            
            _penaltyManager = new PenaltyManager();
            _penaltyManager.Initialize(allPlayers, cardAnimator);
        }
        
        /// <summary>
        /// Initialize the card object pool.
        /// </summary>
        private void InitializePlayers()
        {
            for (int i = 0; i < playerCount; i++)
            {
                // TODO: if its up need to rotate, based on positions
                Player player = Instantiate(playerPrefab).GetComponent<Player>();
                player.transform.SetParent(twoPlayerPositions[i]);
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
        
        /// <summary>
        /// Handle game over when a player wins
        /// </summary>
        /// <param name="winner">The player who won the game</param>
        public void GameOver(Player winner)
        {
            Debug.Log($"Game Over! Player {winner.id} is the winner!");

            // Notify listeners that the game is over
            onGameOver?.Invoke(winner);
        }

        #region Penalty

        public void GivePenalty(Player penaltyPlayer)
        {
            _penaltyManager.ApplyPenalty(penaltyPlayer);
        }

        public bool CheckForPenalty(Player currentPlayer, CardUI droppedCard)
        {
            return _penaltyManager.DetectWrongPlay(currentPlayer, droppedCard);
        }

        #endregion
        
    }
}