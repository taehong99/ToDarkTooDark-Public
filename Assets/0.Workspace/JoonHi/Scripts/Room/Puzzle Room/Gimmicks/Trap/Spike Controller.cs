using JH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    public List<JH.PressurePlate> pressurePlates = new List<JH.PressurePlate>();
    public List<Spike> spikes = new List<Spike>();
    public int time;

    private IEnumerator timer;

    public void Deactivate()
    {
        foreach (JH.PressurePlate pressurePlate in pressurePlates)
            pressurePlate.Deactivate();
        foreach (Spike spike in spikes)
        {
            spike.Deactivate();
        }
        timer = Timer();
        StartCoroutine(timer);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(time);
        foreach (JH.PressurePlate pressurePlate in pressurePlates)
            pressurePlate.Activate();
    }
}
