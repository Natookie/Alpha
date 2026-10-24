// Runs one high-low round: shuffles the deck, tracks the current card
// and decides when the round ends. Raises events for the UI to pick up

public class GameSession
{
    private readonly GameData data;
    private readonly System.Func<IDeck> deckFactory;

    private IDeck deck;
    private ICard current;
    private int dealt;
    private bool sessionOver;

    public PlayerData Player { get; }
    public GameStateMachine Fsm { get; }

    public int CardsDealt => dealt;
    public int CardsTotal => deck == null ? 0 : deck.Remaining + dealt;

    //Cheat Sheet
    public ICard PeekNext(int offset) => deck == null ? null : deck.Peek(offset);
    public ICard CurrentCard => current;
    public int RemainingCards => deck == null ? 0 : deck.Remaining;

    public GameSession(GameData data, GameStateMachine fsm, System.Func<IDeck> deckFactory){
        this.data = data;
        Fsm = fsm;
        this.deckFactory = deckFactory;
        Player = new PlayerData(data.startingHealth);
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
        GameEvents.RaiseDeckChanged(deck.Remaining);

        Fsm.ChangeState(new GameplayState(this));
    }

    public void GuessHigher(bool higher){
        if(sessionOver) return;
        if(deck.IsEmpty){ EndGame(true); return; }

        ICard next = deck.Draw();
        dealt++;

        bool correct = higher == IsHigher(next, current);

        if(correct) Player.AddPoint(data.pointsPerCorrectGuess);
        else{
            Player.LoseHealth(data.damagePerWrongGuess);
            if(!Player.IsAlive){
                current = next;
                Reveal(next);
                EndGame(false);
                return;
            }
        }

        current = next;
        Reveal(next);

        if(deck.IsEmpty) EndGame(true);
    }

    void Reveal(ICard next){
        GameEvents.RaiseCardsRevealed(current, next);
        GameEvents.RaisePointChanged(Player.Point);
        GameEvents.RaiseHealthChanged(Player.Health, Player.MaxHealth);
        GameEvents.RaiseDeckChanged(deck.Remaining);
    }

    void EndGame(bool won){
        if(sessionOver) return;
        sessionOver = true;
        Fsm.ChangeState(new GameOverState(this, won));
    }

    bool IsHigher(ICard a, ICard b){
        int ra = CardModel.CompareValue(a, data);
        int rb = CardModel.CompareValue(b, data);
        if(ra != rb) return ra > rb;
        return CardModel.SuitWeight(a.Suit) > CardModel.SuitWeight(b.Suit);
    }

    public bool OptimalMoveIsHigher(){
        if(current == null || deck == null) return false;
        ICard next = PeekNext(0);
        if(next == null) return false;
        return IsHigher(next, current);
    }
}