using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class MonsterModel : MonoBehaviour
    {
        [SerializeField] BaseMonster monster;

        private void Attack01Frame()
        {
            monster.Attack01Frame();
        }

        private void Attack02Frame()
        {
            monster.Attack02Frame();
        }

        private void Attack03Frame()
        {
            monster.Attack03Frame();
        }
    }
}