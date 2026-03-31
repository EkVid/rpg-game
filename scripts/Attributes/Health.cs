using System;
using Godot;

namespace tinySwords.scripts;

public class Health
{
    public float Hitpoints{ get; set; } = 100f;
    public event Action OnDeath;
    
    public void TakeDamage(float damage)
    {
        Hitpoints -= damage;
        if (Hitpoints <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        Hitpoints += amount;
        if (Hitpoints >= 100)
        {
            Hitpoints = 100;
        }
    }

    public void Reset(float hitpoints)
    {
        Hitpoints = hitpoints;
    }
    

}