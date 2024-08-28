using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new List<StatusEffect>();
    [SerializeField] PlayerStates playerStates;
    public void AddEffect(StatusEffect effect)
    {
        if (playerStates.IsUsedItem)
        {
            activeEffects.Add(effect);
            effect.ApplyEffect(gameObject);
        }
    }
    public void DebuffRemoveEfffect()
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            if(activeEffects[i].Name == "디버프")
            {
                activeEffects.RemoveAt(i);
            }
        }
    }
    public void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].Update(deltaTime);
            if (!activeEffects[i].IsActive)
            {

                Debug.Log('2');
                activeEffects[i].RemoveEffect(gameObject);
                activeEffects.RemoveAt(i);
            }
        }
    }
}