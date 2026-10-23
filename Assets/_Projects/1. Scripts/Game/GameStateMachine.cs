// Holds the active state. ChangeState exits the current one first, then enters the next
public class GameStateMachine
{
    public IGameState Current { get; private set; }

    public void ChangeState(IGameState next){
        Current?.Exit();
        Current = next;
        Current.Enter();
    }
}