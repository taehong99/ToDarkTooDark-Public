using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportArrow : MonoBehaviour
{
    public Vector2Int startpoint;
    public ArrowShooter.Direction direction;
    public int distence;
    public float durationtime;
    public Vector2 teleportPoint;

    private Vector2Int endpoint;
    private Vector2 speed;

    void Start()
    {
        switch(direction)
        {
            case ArrowShooter.Direction.Up:
                endpoint = startpoint + new Vector2Int(0, distence);
                speed = new Vector2(0, distence/durationtime);
                break;
            case ArrowShooter.Direction.Down:
                transform.rotation = Quaternion.Euler(0,0,180);
                endpoint = startpoint + new Vector2Int(0, -distence);
                speed = new Vector2(0, -distence/durationtime);
                break;
            case ArrowShooter.Direction.Left:
                transform.rotation = Quaternion.Euler(0,0,90);
                endpoint = startpoint + new Vector2Int(-distence, 0);
                speed = new Vector2(-distence/durationtime, 0);
                break;
            case ArrowShooter.Direction.Right:
                transform.rotation = Quaternion.Euler(0,0,-90);
                endpoint = startpoint + new Vector2Int(distence, 0);
                speed = new Vector2(distence/durationtime, 0);
                break;
        }
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x + speed.x * Time.deltaTime, transform.position.y + speed.y * Time.deltaTime, transform.position.z);
        if (Vector2Int.RoundToInt(transform.position) == endpoint)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") /*&& collision.TryGetComponent(out PhotonView photonView)*/)
        {
            collision.gameObject.transform.position = teleportPoint;
            Destroy(gameObject);
        }
    }
}
