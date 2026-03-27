namespace tinySwords.scripts.enemies;

public partial class Enemy: Character
{

    public override void _Ready()
    {
        base._Ready();
        Speed = 200.0f;
        Hitpoints = 50f;
        AttackPower = 20f;
        AttackDelay = 10f;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
    }

    private void HandleAttack()
    {
        
    }
}