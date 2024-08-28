using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFollowNPC : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;

    Transform user; 


    void OnEnable()
    {
        FindUser();
    }



    // Update is called once per frame
    void Update()
    {
        if(user != null)
            FollowUser();
    }

    void FindUser()
    {
        //Transform user = FindAnyObjectByType<PhotonPlayerController>().GetComponent<Transform>();

        user = TutorialManager.Instance.Player.transform;
    }


    void FollowUser()
    {
        if(transform.position.x < user.position.x)
            renderer.flipX = true;
        else
            renderer.flipX = false;
    }
}
