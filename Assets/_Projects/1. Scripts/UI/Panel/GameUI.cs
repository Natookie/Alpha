using System.Collections;
using UnityEngine;
using Nova;

// Wires the menu / gameplay / game-over panels to a GameSession.
// Owns the CardView slots and decides which one gets the next card.

public class GameUI : MonoBehaviour
{
    [Header("GAME TWEAK")]
    [SerializeField] private int cardCount = 10;

    [Header("GAME OVER")]
    [SerializeField] private float gameOverDelay = 2f;

    [Header("CARDS")]
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;

    [Header("PANELS")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("BUTTONS")]
    [SerializeField] private CoreButton startButton;
    [SerializeField] private CoreButton higherButton;
    [SerializeField] private CoreButton lowerButton;
    [SerializeField] private CoreButton restartButton;

    [Header("GAME OVER TEXT")]
    [SerializeField] private TextBlock resultText;
    [SerializeField] private TextBlock cardsDealtText;
    [SerializeField] private TextBlock pointText;

    private GameSession session;
    private CardView[] views;
    private int revealIndex;
    private Coroutine returnToMenuRoutine;

    void Awake(){
        session = new GameSession(cardCount);
        views = new CardView[cardCount];

        SpawnViews();

        if(startButton != null) startButton.OnClick.AddListener(StartGame);
        if(higherButton != null) higherButton.OnClick.AddListener(() => session.GuessHigher(true));
        if(lowerButton != null) lowerButton.OnClick.AddListener(() => session.GuessHigher(false));
        if(restartButton != null) restartButton.OnClick.AddListener(StartGame);

        GameEvents.CardsRevealed += OnCardsRevealed;
        GameEvents.GameEnded += OnGameEnded;
    }

    void Start() => ShowMainMenu();

    void OnDestroy(){
        GameEvents.CardsRevealed -= OnCardsRevealed;
        GameEvents.GameEnded -= OnGameEnded;
    }

    void SpawnViews(){
        if(cardParent == null || cardPrefab == null) return;
        for(int i = 0; i < cardCount; i++){
            GameObject card = Instantiate(cardPrefab, cardParent);
            views[i] = card.GetComponent<CardView>();
            if(views[i] != null) views[i].Reveal(false);
        }
    }

    void StartGame(){
        StopReturnRoutine();

        SetPanel(mainMenuPanel, false);
        SetPanel(gameOverPanel, false);
        SetPanel(gameplayPanel, true);

        revealIndex = 0;
        for(int i = 0; i < views.Length; i++)
            if(views[i] != null) views[i].Reveal(false);

        session.StartNewGame();
    }

    void ShowMainMenu(){
        SetPanel(mainMenuPanel, true);
        SetPanel(gameplayPanel, false);
        SetPanel(gameOverPanel, false);
    }

    void OnCardsRevealed(ICard current, ICard next){
        if(current != null && revealIndex == 0){
            BindSlot(0, current);
            revealIndex = 1;
        }
        if(next != null && revealIndex < views.Length){
            BindSlot(revealIndex, next);
            revealIndex++;
        }
    }

    void BindSlot(int index, ICard card){
        if(index < 0 || index >= views.Length) return;
        CardView view = views[index];
        if(view == null) return;
        view.Bind(card);
        view.Reveal(true);
    }

    void OnGameEnded(bool won){
        int dealt = session.CardsDealt;
        int total = session.CardsTotal;
        int point = session.Player.Point;

        if(resultText != null)     resultText.Text = won ? "You Win!" : "Game Over";
        if(cardsDealtText != null) cardsDealtText.Text = $"Cards Dealt: {dealt} / {total}";
        if(pointText != null)      pointText.Text = $"Point: {point}";

        SetPanel(gameplayPanel, false);
        SetPanel(gameOverPanel, true);

        StopReturnRoutine();
        returnToMenuRoutine = StartCoroutine(ReturnToMenuAfterDelay(gameOverDelay));
    }

    IEnumerator ReturnToMenuAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        returnToMenuRoutine = null;
        ShowMainMenu();
    }

    void StopReturnRoutine(){
        if(returnToMenuRoutine != null){
            StopCoroutine(returnToMenuRoutine);
            returnToMenuRoutine = null;
        }
    }

    void SetPanel(GameObject panel, bool active){
        if(panel != null) panel.SetActive(active);
    }
}