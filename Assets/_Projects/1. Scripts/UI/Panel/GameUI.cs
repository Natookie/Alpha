using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;

// Wires the menu / gameplay / game-over panels to a GameSession.
// Owns the CardView slots and decides which one gets the next card.
public class GameUI : MonoBehaviour
{
    [Header("GAME TWEAK")]
    [SerializeField] private GameData data;

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
    private List<ICard> visible = new();
    private Coroutine returnToMenuRoutine;

    public GameSession Session => session;

    void Awake(){
        var fsm = new GameStateMachine();
        session = new GameSession(data, fsm, () => new Deck());
        views = new CardView[data.cardShowCount];

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
        for(int i = 0; i < data.cardShowCount; i++){
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

        visible.Clear();
        for(int i = 0; i < views.Length; i++)
            if(views[i] != null) views[i].Reveal(false);

        session.StartNewGame();
    }

    void ShowMainMenu(){
        SetPanel(mainMenuPanel, true);
        SetPanel(gameplayPanel, false);
        SetPanel(gameOverPanel, false);
    }

    // GameSession sends the current card first, then one new card per guess.
    // Keep only the last cardShowCount cards and rebind the slots from the tail.
    void OnCardsRevealed(ICard current, ICard next){
        if(current != null && visible.Count == 0) visible.Add(current);
        if(next != null){
            visible.Add(next);
            while(visible.Count > data.cardShowCount) visible.RemoveAt(0);
        }
        RenderWindow();
    }

    void RenderWindow(){
        for(int i = 0; i < views.Length; i++){
            if(views[i] == null) continue;
            if(i < visible.Count){
                views[i].Bind(visible[i]);
                views[i].Reveal(true);
            }else{
                views[i].Reveal(false);
            }
        }
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
        returnToMenuRoutine = StartCoroutine(ReturnToMenuAfterDelay(data.gameOverDelay));
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