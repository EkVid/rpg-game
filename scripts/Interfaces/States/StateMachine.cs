namespace tinySwords.scripts;

public class StateMachine
{
    private IState _currentState;
    
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