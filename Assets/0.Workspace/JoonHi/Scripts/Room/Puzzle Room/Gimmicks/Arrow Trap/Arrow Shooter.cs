using System.Collections;
using UnityEngine;
using static Tae.BaseMonster;

public class ArrowShooter : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }

    [Header("Animatior")]
    [SerializeField] Animator animatior;

    [Header("Prefabs")]
    [SerializeField] GameObject Arrow;

    [Header("Spec")]
    [SerializeField] Direction direction;
    [SerializeField] float shootInterval;
    [SerializeField] float shootDuration;
    [SerializeField] int shootLength;
    [SerializeField] Transform telpoint;

    private IEnumerator shootingArrow;
    private bool shooting;
    private bool disable;

    private void Start()
    {
        shooting = false;
        disable = false;
        shootingArrow = ShootRouter();
    }

    public void ShootStart()
    {
        if (shooting == false && disable == false)
        {
            shooting = true;
            StartCoroutine(shootingArrow);
        }
    }

    public void ShootStop()
    {
        if (shooting == true)
        {
            // StopCoroutine(shootingArrow);
            // animator.SetTrigger("");
            shooting = false;
        }
    }

    public void Deactivate()
    {
        disable = true;
        ShootStop();
    }

    private IEnumerator ShootRouter()
    {
        while (shooting == true)
        {
            TeleportArrow telarrow = Instantiate(Arrow,transform).GetComponent<TeleportArrow>();
            telarrow.gameObject.transform.position = this.transform.position;
            telarrow.startpoint = Vector2Int.RoundToInt(transform.position);
            telarrow.direction = direction;
            telarrow.distence = shootLength;
            telarrow.durationtime = shootDuration;
            telarrow.teleportPoint = telpoint.position;
            yield return new WaitForSeconds(shootInterval);
        }
    }
}
