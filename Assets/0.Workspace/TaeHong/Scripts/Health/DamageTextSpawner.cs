using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] GameObject damageTextPrefab;
        [SerializeField] Transform parent;

        public void SpawnText(Color color, int amount)
        {
            GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            damageText.GetComponent<DamageText>().Spawn(color, amount, parent);
        }

        public void SpawnMultText(Color color, int amount)
        {
            GameObject damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            damageText.GetComponent<DamageText>().SpawnMultiple(color, amount, parent);
        }
    }
}