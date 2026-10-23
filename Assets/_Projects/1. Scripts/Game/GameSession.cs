// Runs one high-low round: shuffles the deck, tracks the current card
// and decides when the round ends. Raises events for the UI to pick up

public class GameSession
{
    private const int StartingHealth = 3;

    private IDeck deck;
    private ICard current;
    private int cardCount;
    private int dealt;
    private bool sessionOver;

    public int CardsDealt => dealt;
    public int CardsTotal => cardCount;

    public PlayerData Player { get; }
    public GameStateMachine Fsm { get; } = new();

    public GameSession(int cardCount){
        this.cardCount = cardCount;
        Player = new PlayerData(StartingHealth);
    }

    public void StartNewGame(){
        deck = new Deck();
        deck.Shuffle();
        Player.Reset();

        current = deck.Draw();
        dealt = 1;
        sessionOver = false;

        GameEvents.RaiseCardsRevealed(current, null);
        GameEvents.RaisePointChanged(Player.Point);
        GameEvents.RaiseHealthChanged(Player.Health, Player.MaxHealth);
        GameEvents.RaiseDeckChanged(cardCount - dealt);

        Fsm.ChangeState(new GameplayState(this));
    }

    public void GuessHigher(bool higher){
        if(sessionOver) return;

        if(dealt >= cardCount){
            EndGame(true);
            return;
        }

        ICard next = deck.Draw();
        dealt++;

        bool correct = higher == (next.Value > current.Value);

        if(correct) Player.AddPoint(1);
        else{
            Player.LoseHealth(1);
            if(!Player.IsAlive){
                current = next;
                GameEvents.RaiseCardsRevealed(current, next);
                GameEvents.RaisePointChanged(Player.Point);
                GameEvents.RaiseHealthChanged(Player.Health, Player.MaxHealth);
                GameEvents.RaiseDeckChanged(cardCount - dealt);
                EndGame(false);
                return;
            }
        }

        current = next;

        GameEvents.RaiseCardsRevealed(current, next);
        GameEvents.RaisePointChanged(Player.Point);
        GameEvents.RaiseHealthChanged(Player.Health, Player.MaxHealth);
        GameEvents.RaiseDeckChanged(cardCount - dealt);

        if(dealt >= cardCount) EndGame(true);
    }

    void EndGame(bool won){
        if(sessionOver) return;
        sessionOver = true;
        Fsm.ChangeState(new GameOverState(this, won));
    }
}