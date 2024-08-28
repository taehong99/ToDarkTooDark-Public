using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using Tae;

public enum Monster { Skeleton, EliteOrc, Slime, ArmoredOrc, Orc, ArmoredSkeleton, PhantomKnight }
public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField] Monster typeToSpawn;
    public Sprite curSprite;
    public Sprite skeletonSprite;
    public Sprite eliteOrcSprite;
    public Sprite slimeSprite;
    public Sprite armoredOrcSprite;
    public Sprite orcSprite;
    public Sprite armoredSkeletonSprite;
    public Sprite phantomKnightSprite;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Disable sprite preview in game
        spriteRenderer.enabled = false;
    }

    public BaseMonster SpawnMonster()
    {
        if (!PhotonNetwork.IsConnected)
            return OfflineSpawn();

        if (!PhotonNetwork.IsMasterClient)
            return null;

        if (typeToSpawn == Monster.Skeleton)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/Skeleton", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.Slime)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/Slime", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.Orc)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/Orc", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.ArmoredSkeleton)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/ArmoredSkeleton", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if(typeToSpawn == Monster.ArmoredOrc)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/ArmoredOrc", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.EliteOrc)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/EliteOrc", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.PhantomKnight)
        {
            BaseMonster monster = PhotonNetwork.InstantiateRoomObject("Monsters/PhantomKnight", transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }

        return null;
    }

    BaseMonster OfflineSpawn()
    {
        if (typeToSpawn == Monster.Skeleton)
        {
            GameObject mob = Resources.Load("Monsters/TutorialSkeleton") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.Slime)
        {
            GameObject mob = Resources.Load("Monsters/TutorialSlime") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.Orc)
        {
            GameObject mob = Resources.Load("Monsters/Orc") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.ArmoredSkeleton)
        {
            GameObject mob = Resources.Load("Monsters/ArmoredSkeleton") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.ArmoredOrc)
        {
            GameObject mob = Resources.Load("Monsters/ArmoredOrc") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.EliteOrc)
        {
            GameObject mob = Resources.Load("Monsters/EliteOrc") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }
        else if (typeToSpawn == Monster.PhantomKnight)
        {
            GameObject mob = Resources.Load("Monsters/PhantomKnight") as GameObject;
            BaseMonster monster = Instantiate(mob, transform.position, Quaternion.identity).GetComponent<BaseMonster>();
            return monster;
        }

        return null;
    }

    public void UpdateSprite()
    {
        if (typeToSpawn == Monster.Skeleton)
        {
            curSprite = skeletonSprite;
            spriteRenderer.sprite = skeletonSprite;
        }
        else if(typeToSpawn == Monster.EliteOrc)
        {
            curSprite = eliteOrcSprite;
            spriteRenderer.sprite = eliteOrcSprite;
        }
        else if (typeToSpawn == Monster.Slime)
        {
            curSprite = slimeSprite;
            spriteRenderer.sprite = slimeSprite;
        }
        else if (typeToSpawn == Monster.ArmoredOrc)
        {
            curSprite = armoredOrcSprite;
            spriteRenderer.sprite = armoredOrcSprite;
        }
        else if (typeToSpawn == Monster.Orc)
        {
            curSprite = orcSprite;
            spriteRenderer.sprite = orcSprite;
        }
        else if (typeToSpawn == Monster.ArmoredSkeleton)
        {
            curSprite = armoredSkeletonSprite;
            spriteRenderer.sprite = armoredSkeletonSprite;
        }
        else if (typeToSpawn == Monster.PhantomKnight)
        {
            curSprite = phantomKnightSprite;
            spriteRenderer.sprite = phantomKnightSprite;
        }
    }
}