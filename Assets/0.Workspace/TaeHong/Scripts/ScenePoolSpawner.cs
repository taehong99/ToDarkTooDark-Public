using UnityEngine;
using System;

/// <summary>
/// 현제 씬에 필요한 오브젝트 풀을 생성해주는 스크립트
/// 에디터에 PoolInfo[] 배열안에 원하는 PooledObject랑 사이즈를 정해줄 수 있음
/// </summary>

public class ScenePoolSpawner : MonoBehaviour
{
    [SerializeField] PoolInfo[] poolInfos;

    private void Awake()
    {
        foreach(PoolInfo poolInfo in poolInfos)
        {
            Manager.Pool.CreatePool(poolInfo.prefab, poolInfo.size, poolInfo.capacity);
        }
    }
}

[Serializable]
public struct PoolInfo
{
    public PooledObject prefab;
    public int size;
    public int capacity;

    public PoolInfo(PooledObject prefab, int size, int capacity)
    {
        this.prefab = prefab;
        this.size = size;
        this.capacity = capacity;
    }
}