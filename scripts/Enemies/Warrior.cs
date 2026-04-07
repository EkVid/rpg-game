using Godot;

public partial class Warrior : Enemy
{
    public override void _Ready()
    {
        base._Ready();
        
        ChaseBox.BodyEntered += (Node2D body) =>
        {
            if (body is Player)
                PlayerInRange = true;
        };
        
        ChaseBox.BodyExited += (Node2D body) =>
        {
            if (body is Player)
                PlayerInRange = false;
        };
    
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float distanceToPlayer = (Player.GlobalPosition - GlobalPosition).Length();
        float distanceToSpawn = (GlobalPosition - SpawnPoint).Length();
        
        if (IsDead) return;
        
        if (PlayerInRange && Player != null)
        {
            Direction = (Player.GlobalPosition - GlobalPosition).Normalized();

            if (distanceToPlayer <= AttackDistance)
            {
                // enter attack state
                if (StateMachine._currentState != AttackState)
                    StateMachine.ChangeState(AttackState);
            }
            else
            {
                // enter run state
                if (StateMachine._currentState != RunState) 
                    StateMachine.ChangeState(RunState);
            }
        }
        else
        {
            if (distanceToSpawn > JitterPrevention)
            {
                // enter run state
                Direction = GlobalPosition.DirectionTo(SpawnPoint);
                if (StateMachine._currentState != RunState)
                    StateMachine.ChangeState(RunState);
            }
            else
            {
                // enter idle state
                GlobalPosition = SpawnPoint;
                Direction = Vector2.Zero;
                if(StateMachine._currentState != IdleState)
                    StateMachine.ChangeState(IdleState);
            }
        }
        
        StateMachine.Update(delta);
    }
}
