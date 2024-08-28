using System.Collections;
using Tae;
using UnityEngine;

public class DebufController : MonoBehaviour
{
    [Header("PlayerHealth")]
    [SerializeField] PlayerHealth playerHealth;

    #region Lava

    [Header("Lava Spec")]
    [SerializeField] float damagePercentage;
    [SerializeField] int dotDamageTime;

    private IEnumerator continuousDotDamage;
    private IEnumerator timer;

    public void OnLava()
    {
        if (timer != null)
        {
            if (timer != null)
            {
                StopCoroutine(timer);
                timer = null;
            }
        }
        if (continuousDotDamage == null)
        {
            continuousDotDamage = DotDamageRoutine(damagePercentage, playerHealth);
            StartCoroutine(continuousDotDamage);
        }
    }

    public void LavaTimerStart()
    {
        timer = Timer(dotDamageTime);
        StartCoroutine(timer);
        // Debug.Log("tiemr start");
    }

    IEnumerator DotDamageRoutine(float precent, PlayerHealth player)
    {
        // Debug.Log("dotdamagestart");
        while (true)
        {
            if (player.Health <= 0)
                break;
            player.TakeFixedDamage((int) (player.MaxHealth * precent));
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Timer(int time)
    {
        yield return new WaitForSeconds(time);
        if (continuousDotDamage != null)
            StopCoroutine(continuousDotDamage);
        continuousDotDamage = null;
        // Debug.Log("stop");
    }
    #endregion
}
