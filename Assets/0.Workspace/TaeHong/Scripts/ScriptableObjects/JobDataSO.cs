using UnityEngine;

[CreateAssetMenu(menuName = "Player/JobData", fileName = "JobData")]
public class JobDataSO : ScriptableObject
{
    public PlayerJobData swordsmanData;
    public PlayerJobData archerData;

    public PlayerJobData GetJobData(PlayerJob job)
    {
        switch (job)
        {
            case PlayerJob.Swordsman:
                return swordsmanData;
            case PlayerJob.Archer:
                return archerData;
            default:
                return swordsmanData;
        }
    }
}

