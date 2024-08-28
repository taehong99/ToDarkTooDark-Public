using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;
using System;

public class MonsterSpawner : MonoBehaviour
{
    MonsterSpawnPoint[] spawnPoints;

    List<BaseMonster> spawndMonsters;

    public Action allMonsterDied;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<MonsterSpawnPoint>();
    }

    public void SpawnMonsters()
    {
        spawndMonsters = new List<BaseMonster>(); 
        foreach(var point in spawnPoints)
        {
            BaseMonster monster = point.SpawnMonster();
            monster.OnDied += HandleMonsterDied;
            spawndMonsters.Add(monster);
        }
    }


    void HandleMonsterDied(BaseMonster diedMonster)
    {
        if (diedMonster != null)
        {
            diedMonster.OnDied -= HandleMonsterDied; 
        }


        spawndMonsters.Remove(diedMonster);

        if(spawndMonsters.Count == 0)
        {
            allMonsterDied?.Invoke();
        }
    }



    //[ContextMenu("Find SpawnPonts")]
    //void FindAllMonsterSpawnPoints()
    //{
    //    spawnPoints.Clear();
    //    foreach (Transform  child in transform)
    //    {
    //        MonsterSpawnPoint spawnPoint = child.GetComponent<MonsterSpawnPoint>();
    //        if (spawnPoint != null)
    //        {
    //            spawnPoints.Add(spawnPoint);
    //        }
    //        //FindAllMonsterSpawnPoints(child);
    //    }
    //}


}
