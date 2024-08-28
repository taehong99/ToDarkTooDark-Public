using System.Collections.Generic;
using UnityEngine;

public class TutorialPuzzleRoom : MonoBehaviour
{
    [SerializeField] protected List<PressurePlate> pressurePlates;

    [SerializeField] GameObject brige;
    [SerializeField] Collider2D brigeColider;

    protected int activatePlateCnt;

    private void Awake()
    {
        for (int i = 0; i < pressurePlates.Count; i++)
        {
            pressurePlates[i].roomEvent.AddListener(PlatePressed);
        }

        activatePlateCnt = 0;
    }

    public void PlatePressed(bool active)
    {
        activatePlateCnt++;
        PlatePressCheck();
    }

    protected virtual void PlatePressCheck()
    {
        brige.gameObject.SetActive(activatePlateCnt >= pressurePlates.Count);
        brigeColider.enabled = !brige.gameObject.activeSelf;
    }
}
