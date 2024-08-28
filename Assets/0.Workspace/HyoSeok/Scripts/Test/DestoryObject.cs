using System;
using UnityEngine;

public class DestoryObject : MonoBehaviour, IDamageable
{
    int health = 1;
    int maxHealth;
    public int Health { get { return health; } set { health = value; } }
    public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    public GameObject LastHitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action<int> healthChangedEvent;
    public event Action diedEvent;


    public void TakeDamage(int amount)
    {

        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void ApplyStun(float duration)
    {
    }

    public void Heal(float amount, bool isPercent)
    {
    }

    public void TakeFixedDamage(int amount)
    {
    }
}
