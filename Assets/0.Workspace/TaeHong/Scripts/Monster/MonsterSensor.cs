using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class MonsterSensor : MonoBehaviour
    {
        private BaseMonster monster;

        private void Awake()
        {
            monster = GetComponentInParent<BaseMonster>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            monster.OnSensorTriggerEnter(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            monster.OnSensorTriggerStay(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            monster.OnSensorTriggerExit(collision);
        }
    }
}