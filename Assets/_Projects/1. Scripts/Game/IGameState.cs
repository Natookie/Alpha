// Called by GameState on state transitions.
public interface IGameState
{
    void Enter();
    void Exit();
}