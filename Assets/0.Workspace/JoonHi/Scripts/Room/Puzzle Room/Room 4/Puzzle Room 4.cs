using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PuzzleRoom4 : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] GameObject treasurePrefab;

        [Header("Treasure Point")]
        [SerializeField] Transform treasure1;
        [SerializeField] Transform treasure2;

        [Header("Traps")]
        [SerializeField] List<SawTrap> saws = new List<SawTrap>();
        [SerializeField] List<ArrowShooter> statue = new List<ArrowShooter>();
        [SerializeField] List<PressurePlate> pressurePlate = new List<PressurePlate>();
        [SerializeField] List<GameObject> wall = new List<GameObject>();

        private List<GameObject> PlayerList = new List<GameObject>();

        private void Start()
        {
            GameObject treasurePrefab1 = Instantiate(treasurePrefab, treasure1);
            GameObject treasurePrefab2 = Instantiate(treasurePrefab, treasure2);

            pressurePlate[0].Pressed.AddListener(statue[0].Deactivate);
            pressurePlate[0].Pressed.AddListener(pressurePlate[0].Deactivate);
            pressurePlate[1].Pressed.AddListener(statue[1].Deactivate);
            pressurePlate[1].Pressed.AddListener(pressurePlate[1].Deactivate);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (PlayerList.Contains(collision.gameObject) == false)
                    PlayerList.Add(collision.gameObject);
                foreach (SawTrap saw in saws)
                {
                    saw.SawStart();
                }
                foreach(ArrowShooter statue in statue)
                {
                    statue.ShootStart();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerList.Remove(collision.gameObject);
                if (PlayerList.Count == 0)
                {
                    foreach (SawTrap saw in saws)
                    {
                        saw.SawStop();
                    }
                    foreach (ArrowShooter statue in statue)
                    {
                        statue.ShootStop();
                    }
                }
            }
        }
    }
}