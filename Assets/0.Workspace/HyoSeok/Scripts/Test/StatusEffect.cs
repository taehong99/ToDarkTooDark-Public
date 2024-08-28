using UnityEngine;
public enum StatusEffectType { Blind, Silence, Burn, Poison }
public abstract class StatusEffect
{
    public string Name { get; private set; }
    public float Duration { get; private set; }
    public bool IsActive { get; private set; }
    public StatusEffect(string name, float duration)
    {
        Name = name;
        Duration = duration;
        IsActive = true;
    }

    public abstract void ApplyEffect(GameObject target);
    public abstract void RemoveEffect(GameObject target);

    public void Update(float deltaTime)
    {
        if (!IsActive) return;

        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsActive = false;
        }
    }
}