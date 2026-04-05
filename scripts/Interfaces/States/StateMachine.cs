namespace tinySwords.scripts;

public class StateMachine
{
    public IState _currentState { get; private set; }
    
    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update(double delta)
    {
        _currentState?.Update(delta);
    }
}