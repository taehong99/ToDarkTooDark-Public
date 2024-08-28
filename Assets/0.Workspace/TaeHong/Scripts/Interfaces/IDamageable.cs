using System;
using UnityEngine;

public interface IDamageable
{
    // Properties
    int Health { get; set; }
    int MaxHealth { get; set; }
    GameObject LastHitter { get; set; }

    // Events
    event Action<int> healthChangedEvent;
    event Action diedEvent;

    // Methods
    void TakeDamage(int amount);
    void ApplyStun(float duration);
    void Heal(float amount, bool isPercent);
    void TakeFixedDamage(int amount);
}
