// Kicks off the first round as soon as the game starts
// Menu panel is shown/hidden by GameUI, not by this state
public class MainMenuState : IGameState
{
    private readonly GameSession _session;
    public MainMenuState(GameSession session) => _session = session;
    public void Enter() => _session.StartNewGame();
    public void Exit() { }
}

// Nothing to do. Guesses go straight through GameSession
// this state  exists just so the FSM has something to point at while a round is live
public class GameplayState : IGameState
{
    private readonly GameSession _session;
    public GameplayState(GameSession session) => _session = session;
    public void Enter() { }
    public void Exit() { }
}

// Fires the game-over event so the UI can show the result screen
public class GameOverState : IGameState
{
    private readonly GameSession session;
    private readonly bool won;

    public GameOverState(GameSession session, bool won){
        this.session = session;
        this.won = won;
    }

    public void Enter() => GameEvents.RaiseGameEnded(won);
    public void Exit(){}
}