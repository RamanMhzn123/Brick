// using System;
// using UnityEngine;
//
// namespace CardGame.Scripts.Game_Elements
// {
//     [Serializable]
//     public class Card
//     {
//         public int number;
//         public Color color;
//         public PowerUpType powerUp;
//         
//         public Card(int number, Color color, PowerUpType powerUp = PowerUpType.None)
//         {
//             this.number = number;
//             this.color = color;
//             this.powerUp = powerUp;
//         }
//     }
//     
//     public enum PowerUpType
//     {
//         None,    // No power-up
//         Block,   // Block player from playing card
//     }
// }
// using TMPro;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// namespace CardGame.Scripts.Game_Elements
// {
//     /// <summary>
//     /// Drag and drop
//     /// Set card UI
//     /// </summary>
//     [RequireComponent(typeof(CanvasGroup))]
//     public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
//     {
//         private RectTransform _rectTransform;
//         private CanvasGroup _canvasGroup;
//         private Transform _originalParent;
//
//         [Header("Visuals")] 
//         public Image image;
//         public TextMeshProUGUI text;
//
//         public Card _card;
//         private bool _isDropped;
//
//         private void Awake()
//         {
//             _rectTransform = GetComponent<RectTransform>();
//             _canvasGroup = GetComponent<CanvasGroup>();
//         }
//
//         public void Initialize(Card card)
//         {
//             _card = card;
//             image.color = card.color;
//             text.text = card.number.ToString();
//         }
//
//         #region Drag handling
//
//         public void OnBeginDrag(PointerEventData eventData)
//         {
//             _isDropped = false;
//             _originalParent = transform.parent;
//             transform.SetParent(_originalParent.root);  // Bring to top layer
//             
//             ActivateCardUI(false);
//         }
//
//         public void OnDrag(PointerEventData eventData)
//         {
//             _rectTransform.anchoredPosition += eventData.delta / transform.lossyScale.x; // Move with cursor
//         }
//
//         /// <summary>
//         /// If the card was dropped on a valid drop zone:
//         /// Move it to the drop zone.
//         /// Call _gameManager.OnCardPlayed(); to handle turn logic.
//         /// If the card was dropped somewhere invalid:
//         /// Return it to its original position in the player's hand.
//         /// Reset visual effects (alpha & raycasts).
//         /// </summary>
//         /// <param name="eventData"></param>
//         public void OnEndDrag(PointerEventData eventData)
//         {
//             if (_isDropped) return;
//
//             Debug.Log("Card dropped, checking for valid drop zone...");
//
//             // If not dropped in a valid place, return to original hand
//             ActivateCardUI(true);
//             
//             transform.SetParent(_originalParent); // Move back to player's hand
//             
//             Debug.Log("Card returned to hand.");
//         }
//
//         #endregion
//
//         public void DropCardUI(Transform parent = null)
//         {
//             if (parent == null)
//             {
//                 transform.SetParent(_originalParent);
//             }
//             else
//             {
//                 transform.SetParent(parent);
//                 _isDropped = true;
//             }
//         }
//
//         public void ActivateCardUI(bool isActive)
//         {
//             _canvasGroup.blocksRaycasts = isActive;
//             _canvasGroup.alpha = isActive ? 1 : 0.5f;
//         }
//
//         public int GetCardNumber()
//         {
//             return _card.number;
//         }
//         
//         public Color GetCardColor()
//         {
//             return _card.color;
//         }
//         
//         
//     }
// }
// using System.Collections.Generic;
// using CardGame.Scripts.Managers;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// namespace CardGame.Scripts.Game_Elements
// {
//     /// <summary>
//     /// Middle stacking drop zone
//     /// Own player stack drop zone
//     /// Other player stack drop zone
//     /// </summary>
//     public class DropZone : MonoBehaviour, IDropHandler
//     {
//         [SerializeField] private Player ownerPlayer; // The player who owns this drop zone
//         public List<CardUI> cardStack = new(); // Stack for this zone
//         
//         [Header("Current State")]
//         private Player _currentPlayer;
//         private CardUI _droppedCard;
//
//         public void OnDrop(PointerEventData eventData)
//         {
//             _droppedCard = eventData.pointerDrag.GetComponent<CardUI>();
//             if (_droppedCard == null)
//             {
//                 Debug.LogWarning("No CardUI provided.");
//                 return;
//             }
//             
//             _currentPlayer = GameManager.Instance.GetCurrentPlayer();
//             if (_currentPlayer == null)
//             {
//                 Debug.LogError("Current player is not set.");
//                 return;
//             }
//             
//             Card cardData = _droppedCard._card;
//             if (cardData == null)
//             {
//                 Debug.LogError("Card data is missing.");
//                 return;
//             }
//             
//             OnCardDropped();
//         }
//
//         private void OnCardDropped()
//         {
//             if (cardStack.Contains(_droppedCard))
//             {
//                 Debug.Log("Card is already in this drop zone. No action taken.");
//                 return;
//             }
//             
//             if (IsValidCardPlacement())
//             {
//                 PlaceCardInZone();
//             }
//             else
//             {
//                 ReturnCardToPlayerHand();
//             }
//         }
//         
//         // Validate the card placement
//         // Central stack: Card must be the next number AND match the color
//         // Player stack: Card only needs to be the next number (color doesn't matter)
//         private bool IsValidCardPlacement()
//         {
//             if (cardStack.Count == 0)
//             {
//                 if (ownerPlayer == null) return _droppedCard.GetCardNumber() == 1;
//                 if (ownerPlayer == _currentPlayer) return true;
//
//                 return false;
//             }
//
//             if (ownerPlayer == _currentPlayer)
//             {
//                 return true;
//             }
//
//             Card topCard = cardStack[cardStack.Count - 1]._card;
//             bool isNextInSequence = _droppedCard.GetCardNumber() == topCard.number + 1;
//
//             if (ownerPlayer == null) return isNextInSequence && _droppedCard.GetCardColor()== topCard.color;
//
//             return isNextInSequence;
//         }
//
//         // Set the card's parent to the drop zone
//         // Add the card to the stack
//         // Disable interaction with the card
//         private void PlaceCardInZone()
//         {
//             _droppedCard.DropCardUI(transform); 
//             cardStack.Add(_droppedCard); 
//             _droppedCard.ActivateCardUI(false); 
//
//             if (ownerPlayer == null)
//             {
//                 Debug.Log("Card added to central drop zone.");
//                 RepeatTurn();
//             }
//             else if (_currentPlayer == ownerPlayer)
//             {
//                 Debug.Log("Card added to player's own drop zone.");
//                 GameManager.Instance.OnCardPlayed(); // Change turn after a valid drop
//             }
//             else
//             {
//                 Debug.Log("Card added to another player's drop zone.");
//                 RepeatTurn();
//             }
//         }
//         
//         private void RepeatTurn()
//         {
//             _currentPlayer.RemoveCardFromStack(_droppedCard); // Remove the card from the current player's stack
//             _currentPlayer.ActivatePlayer(true); // Allow the current player to continue their turn
//
//             if (_currentPlayer.IsStackEmpty() && _currentPlayer.playerDeck.Count == 0) //check winner
//             {
//                 GameManager.Instance.GameOver(_currentPlayer);
//             }
//         }
//
//         private void ReturnCardToPlayerHand()
//         {
//             Debug.Log($"Invalid Drop: Cannot place {_droppedCard.GetCardNumber()} here.");
//
//             // Return the card to the player's hand
//             _droppedCard.DropCardUI(); // Reset the card's position
//             _droppedCard.ActivateCardUI(true); // Re-enable interaction
//         }
//         
//         public void ActivateStackTopCard(bool isActive)
//         {
//             _currentPlayer = GameManager.Instance.GetCurrentPlayer();
//             
//             if (_currentPlayer != ownerPlayer || cardStack.Count == 0)
//             {
//                 Debug.Log(ownerPlayer.id +":"+isActive);
//                 return;
//             };
//             
//             var topCardUI = cardStack[cardStack.Count - 1];
//             topCardUI.ActivateCardUI(isActive); // Enable interaction with the top card
//             
//             Debug.Log($"Top card: {topCardUI.GetCardNumber()}{topCardUI.GetCardColor()} id {isActive}");
//         }
//     }
// }
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace CardGame.Scripts.Game_Elements
// {
//     public class Player : MonoBehaviour 
//     {
//         public int id;
//         public Button drawCardButton;
//         
//         [SerializeField] private Transform showArea;
//         [SerializeField] private DropZone dropZone;
//         
//         [Header("Player Card Details")]
//         public List<Card> playerDeck = new(); // player cards
//
//         public void InitializePlayer(int playerId)
//         {
//             id = playerId;
//         }
//         
//         // draw from player deck
//         public Card DrawFromHand(GameObject card)
//         {
//             card.transform.SetParent(showArea);
//             
//             int i = playerDeck.Count - 1;
//             
//             Card drawnCard = playerDeck[i];
//             playerDeck.RemoveAt(i);
//             return drawnCard;
//         }
//         
//         /// <summary>
//         /// Add card to player hand from deck manager
//         /// </summary>
//         /// <param name="card"></param>
//         public void PlaceCard(Card card)
//         {
//             playerDeck.Add(card);
//         }
//
//         public void ActivatePlayer(bool isActive)
//         {
//             if (playerDeck.Count == 0)
//             {
//                 drawCardButton.interactable = false;
//             }
//             
//             drawCardButton.interactable = isActive;
//             dropZone.ActivateStackTopCard(isActive);
//         }
//
//         public void RemoveCardFromStack(CardUI cardUI)
//         {
//             dropZone.cardStack.Remove(cardUI);
//         }
//
//         public bool IsStackEmpty()
//         {
//             return dropZone.cardStack.Count == 0;
//         }
//     }
// }
// using System.Collections.Generic;
// using CardGame.Scripts.Game_Elements;
// using UnityEngine;
//
// namespace CardGame.Scripts.Managers
// {
//     public class DeckManager : MonoBehaviour
//     {
//
//         public Color[] colors;
//         public int totalColorCards;
//         
//         private readonly List<Card> _deck = new();
//         private List<Player> _players;
//     
//         private void Start()
//         {
//             InitializeDeck();
//             ShuffleDeck();
//         }
//     
//         private void InitializeDeck()
//         {
//             foreach (Color color in colors)
//             {
//                 for (int i = 1; i <= totalColorCards; i++)
//                 {
//                     _deck.Add(new Card(i, color));
//                 }
//             }
//         }
//     
//         /// <summary>
//         /// Fisher Yates algorithm for shuffling cards
//         /// </summary>
//         private void ShuffleDeck()
//         {
//             int n = _deck.Count;
//     
//             for (int i = n - 1; i > 0; i--)
//             {
//                 int j = Random.Range(0, i + 1); // Random index from 0 to i
//                 (_deck[i], _deck[j]) = (_deck[j], _deck[i]); // Swap elements
//             }
//         }
//     
//         /// <summary>
//         /// Deal Card
//         /// </summary>
//         // [Button]
//         public void DealCards()
//         {
//             _players = GameManager.Instance.allPlayers;
//         
//             int cardsPerPlayer = _deck.Count / _players.Count;
//
//             for (int i = 0; i < cardsPerPlayer; i++)
//             {
//                 for (int j = 0; j < _players.Count; j++)
//                 {
//                     if (_deck.Count == 0) return; // Stop if deck runs out of cards
//
//                     Card drawnCard = _deck[_deck.Count-1]; // Take from the top
//                     _players[j].PlaceCard(drawnCard);
//                     _deck.RemoveAt(_deck.Count - 1);
//                 }
//             }
//         }
//     }
// }
// using System.Collections.Generic;
// using CardGame.Scripts.Game_Elements;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace CardGame.Scripts.Managers
// {
//     public class GameManager : MonoBehaviour
//     {
//         [SerializeField] private TurnManager turnManager;
//         
//         [Header("Player Settings")]
//         public List<Transform> twoPlayerPositions; //position maybe different based on no. of player
//         public Player playerPrefab;
//         public int playerCount;
//         public List<Player> allPlayers;
//         private Player _currentPlayer;
//         
//         [Header("Card Setting")]
//         public GameObject cardUIPrefab;
//         
//         [Header("Events")]
//         public UnityEvent onPlayerInitialized;
//         public UnityEvent<Player> onGameOver;
//         
//         public static GameManager Instance;
//
//         [Header("Card Pooling")]
//         public GameObject cardPrefab; // The card prefab to pool
//         public int cardPoolSize = 20; // Initial size of the card pool
//         public ObjectPoolManager _cardPool;
//
//         public void Awake()
//         {
//             Instance = this;
//         }
//
//         public void Start()
//         {
//             InitializeCardPool();
//             InitializePlayers();
//         }
//         
//         /// <summary>
//         /// Initialize the card object pool.
//         /// </summary>
//         private void InitializeCardPool()
//         {
//             _cardPool.prefab = cardPrefab;
//             _cardPool.initialPoolSize = cardPoolSize;
//         }
//         
//         private void InitializePlayers()
//         {
//             for (int i = 0; i < playerCount; i++)
//             {
//                 // TODO: if its up need to rotate, based on positions
//                 Player player = Instantiate(playerPrefab, twoPlayerPositions[i]).GetComponent<Player>();
//                 player.gameObject.name = "Player" + i;
//                 
//                 player.InitializePlayer(i);
//                 player.drawCardButton.onClick.AddListener(SetCard);
//                 
//                 allPlayers.Add(player);
//             }
//             
//             onPlayerInitialized?.Invoke();
//         }
//
//         /// <summary>
//         /// Draw card
//         /// </summary>
//         private void SetCard()
//         {
//             if(_currentPlayer.playerDeck.Count == 0) //flip the stack/slot then draw
//             {
//                 CheckForWinner();
//                 return;
//             }
//             
//             GameObject cardObject = GetCardFromPool();
//             CardUI cardUI = cardObject.GetComponent<CardUI>(); // Get the CardUI component
//             Card card = _currentPlayer.DrawFromHand(cardObject);
//             
//             cardUI.Initialize(card);
//             _currentPlayer.ActivatePlayer(false);
//         }
//         
//         /// <summary>
//         /// Get a card from the pool.
//         /// </summary>
//         private GameObject GetCardFromPool()
//         {
//             return _cardPool.GetObject();
//         }
//         
//         public void DiscardCard(GameObject card)
//         {
//             // Reset the card's state (if needed)
//             card.transform.SetParent(null); // Remove from any parent
//             card.SetActive(false); // Deactivate the card
//
//             // Return the card to the pool
//             ReturnCardToPool(card);
//         }
//         
//         /// <summary>
//         /// Return a card to the pool.
//         /// </summary>
//         private void ReturnCardToPool(GameObject card)
//         {
//             _cardPool.ReturnObject(card);
//         }
//         
//         public void OnCardPlayed()
//         {
//             Debug.Log($"Player {_currentPlayer.id} played a card.");
//             turnManager.EndTurn();
//         }
//
//         public Player GetCurrentPlayer()
//         {
//             return _currentPlayer;
//         }
//
//         public void SetCurrentPlayer(Player player)
//         {
//             _currentPlayer = player;
//         }
//         
//         /// <summary>
//         /// Check if the current player has won the game
//         /// </summary>
//         private void CheckForWinner()
//         {
//             if (_currentPlayer.playerDeck.Count == 0) // TODO: && _currentPlayer.IsStackEmpty()
//             {
//                 Debug.Log($"Player {_currentPlayer.id} has won the game!");
//                 // GameOver(_currentPlayer);
//             }
//         }
//         
//         /// <summary>
//         /// Handle game over when a player wins
//         /// </summary>
//         /// <param name="winner">The player who won the game</param>
//         public void GameOver(Player winner)
//         {
//             Debug.Log($"Game Over! Player {winner.id} is the winner!");
//
//             // Notify listeners that the game is over
//             onGameOver?.Invoke(winner);
//
//             // Disable card drawing for all players
//             foreach (var player in allPlayers)
//             {
//                 player.drawCardButton.onClick.RemoveListener(SetCard);
//             }
//         }
//
//         #region Power-Up Effects
//
//         public void SkipNextTurn()
//         {
//             // Logic to skip the next player's turn
//             Debug.Log("Next player's turn skipped!");
//         }
//
//         #endregion
//     }
// }
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace CardGame.Scripts.Managers
// {
//     public class ObjectPoolManager : MonoBehaviour
//     {
//         public GameObject prefab; // The prefab to pool
//         public int initialPoolSize = 10; // Initial number of objects in the pool
//
//         private Queue<GameObject> _pool = new();
//
//         private void Start()
//         {
//             InitializePool();
//         }
//
//         /// <summary>
//         /// Initialize the object pool with a set number of objects.
//         /// </summary>
//         private void InitializePool()
//         {
//             for (int i = 0; i < initialPoolSize; i++)
//             {
//                 CreateNewObject();
//             }
//         }
//
//         /// <summary>
//         /// Create a new object and add it to the pool.
//         /// </summary>
//         private void CreateNewObject()
//         {
//             GameObject obj = Instantiate(prefab, transform);
//             obj.SetActive(false);
//             _pool.Enqueue(obj);
//         }
//
//         /// <summary>
//         /// Get an object from the pool.
//         /// </summary>
//         public GameObject GetObject()
//         {
//             if (_pool.Count == 0)
//             {
//                 CreateNewObject(); // Create a new object if the pool is empty
//             }
//
//             GameObject obj = _pool.Dequeue();
//             obj.SetActive(true);
//             return obj;
//         }
//
//         /// <summary>
//         /// Return an object to the pool.
//         /// </summary>
//         public void ReturnObject(GameObject obj)
//         {
//             obj.SetActive(false);
//             _pool.Enqueue(obj);
//         }
//     }
// }
//
// using System.Collections.Generic;
// using CardGame.Scripts.Game_Elements;
// using UnityEngine;
//
// namespace CardGame.Scripts.Managers
// {
//     public class TurnManager : MonoBehaviour
//     {
//         public int _currentPlayerIndex;
//         private List<Player> _players;
//         private Player _currentPlayer;
//
//         public void StartGame()
//         {
//             _players = GameManager.Instance.allPlayers;
//             
//             if (_players.Count == 0) return;
//             
//             _currentPlayerIndex = 0;
//             StartTurn();
//         }
//
//         private void StartTurn()
//         {
//             _currentPlayer = _players[_currentPlayerIndex];
//             GameManager.Instance.SetCurrentPlayer(_currentPlayer);
//             
//             // Activate only the current player
//             foreach (var p in _players)
//             {
//                 p.ActivatePlayer(_currentPlayer == p);
//             }
//         }
//
//         public void EndTurn()
//         {
//             // Move to the next player
//             _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
//             StartTurn();
//         }
//     }
// }
