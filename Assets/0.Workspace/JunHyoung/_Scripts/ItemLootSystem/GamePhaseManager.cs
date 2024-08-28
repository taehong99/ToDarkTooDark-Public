using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public enum GamePhase
{
    Phase1 = 0, // 0 sec ~ 1 min 59 sec
    Phase2 = 120, // 2 min ~ 3 min 59 sec
    Phase3 = 240, // 4 min ~ 5 mim 59 sec
    Phase4 = 360, // 6 min ~ 6 min 59 sec
    Phase5 = 420 //  7 min ~             
}

//게임 페이즈를 관리하는 객체, 게임 플레이 시작 타이밍에 생성,초기화 시켜서 사용할 것.
public class GamePhaseManager : MonoBehaviourPun, IPunObservable
{
#if UNITY_EDITOR
    public bool isDebugging;
    public GamePhase currentPhase = GamePhase.Phase1; // for Debuging at Inspector
    public GamePhase CurrentPhase { get => currentPhase; private set { currentPhase = value; OnPhaseChanged?.Invoke(currentPhase); } }
#else
    private GamePhase currentPhase = GamePhase.Phase1;  // 실제로는 Set 함부로 못하게
    public GamePhase CurrentPhase { get => currentPhase; private set { currentPhase = value; OnPhaseChanged?.Invoke(currentPhase); } }
#endif

    private float startTime = 0;
    public float elapsedTime;

    public UnityAction<GamePhase> OnPhaseChanged;

    public void StartTimer()
    {
        currentPhase = GamePhase.Phase1;
        startTime = Time.time;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (isDebugging) // Debugging 켜져있으면 강제 페이즈 전환 가능
            return;
#endif

        if (startTime == 0)
            return;

        UpdateGamePhase();
    }

    private void UpdateGamePhase()
    {
        // 마스터만 시간/페이즈 관리
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;

        elapsedTime = Time.time - startTime;

        if (Manager.Game.Phase == GamePhase.Phase5)
            return;

        GamePhase tempPhase;

        if (elapsedTime < (float) GamePhase.Phase2)
        {
            tempPhase = GamePhase.Phase1;
        }
        else if (elapsedTime < (float) GamePhase.Phase3)
        {
            tempPhase = GamePhase.Phase2;
        }
        else if (elapsedTime < (float) GamePhase.Phase4)
        {
            tempPhase = GamePhase.Phase3;
        }
        else if (elapsedTime < (float) GamePhase.Phase5)
        {
            tempPhase = GamePhase.Phase4;
        }
        else
        {
            tempPhase = GamePhase.Phase5;
        }

        if (tempPhase != currentPhase)
        {
            currentPhase = tempPhase;
            photonView.RPC("SyncPhase", RpcTarget.AllViaServer, GetPhaseNumber(tempPhase));
        }
             
    }

    //누군가 엑스칼리버 획득시 이벤트에 델리게이트 걸어서 호출
    [ContextMenu("Phase5")]
    public void ChangePhase5()
    {
        CurrentPhase = GamePhase.Phase5;
    }

    // For Debug
    [ContextMenu("Phase2")]
    void ChangePhase2()
    {
        CurrentPhase = GamePhase.Phase2;
    }

    [ContextMenu("Phase3")]
    void ChangePhase3()
    {
        CurrentPhase = GamePhase.Phase3;
    }


    [ContextMenu("Phase4")]
    void ChangePhase4()
    {
        CurrentPhase = GamePhase.Phase4;
    }

    GamePhase GetPhaseFromInt(int num)
    {
        switch (num)
        {
            case 1:
                return GamePhase.Phase1;
            case 2:
                return GamePhase.Phase2;
            case 3:
                return GamePhase.Phase3;
            case 4:
                return GamePhase.Phase4;
            case 5:
                return GamePhase.Phase5;
            default:
                Debug.LogWarning("INVALID PHASE NUMBER");
                return GamePhase.Phase1;
        }
    }

    int GetPhaseNumber(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Phase1:
                return 1;
            case GamePhase.Phase2:
                return 2;
            case GamePhase.Phase3:
                return 3;
            case GamePhase.Phase4:
                return 4;
            case GamePhase.Phase5:
                return 5;
            default:
                Debug.LogWarning("INVALID PHASE");
                return -1;
        }
    }

    [PunRPC]
    public void SyncPhase(int phaseNumber)
    {
        CurrentPhase = GetPhaseFromInt(phaseNumber);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(elapsedTime);
        }
        else
        {
            elapsedTime = (float) stream.ReceiveNext();
        }
    }
}


