using Photon.Pun;
using System.Collections.Generic;
using Tae.Inventory;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

/// <summary>
///  FSM 의 형태로 튜토리얼 흐름을 관리하는 매니저 스크립트
///  
///  ToDo : 하위 TuorialBase의 플로우를 SubState로 구분해 HFSM 의 형태를 띄게 하거나
///        아니면 어짜피 한번 전환 넘어가면 다시 돌아오지 않으니, BT의 형태로 리팩토링 할것.
/// 
/// </summary>
public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance;
    public static TutorialManager Instance { get { return instance; } }

    [SerializeField] List<TutorialBase> tutorials;

    [SerializeField] PopUpUI escPopUP;
    [SerializeField] PopUpUI rewardPopUp;

    [Header("Scene Names")]
    [SerializeField] string mainSceneName;
    [SerializeField] AudioClip tutorialBGM;

    [SerializeField]
    TutorialBase curTutorial = null;
    int curIndex = -1;

    public Canvas PopUpCanvas;
    private PhotonPlayerController player;
    public PhotonPlayerController Player { get { return player; } set { player = value; } }
    public CameraController Cam;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    private void Start()
    {
        StartTutorial();
    }

    void StartTutorial()
    {
        Manager.Game.playerDic.Clear();
        Manager.Game.InitPhaseManager();
        InventoryManager.Instance.gameObject.SetActive(false);

        if (tutorialBGM != null)
            Manager.Sound.PlayBGM(tutorialBGM);

        NextTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        if (curTutorial != null)
        {
            curTutorial.Excute(this);
        }
        else
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Manager.UI.popUpCount == 0)
                Manager.UI.ShowPopUpUI(escPopUP);
            else
                Manager.UI.ClosePopUpUI();
        }
    }

    private void OnDisable()
    {
        Manager.Pool.ClearPool(); // Reset pool
    }

    [ContextMenu("Next Tutorial")] //<- for debug
    public void NextTutorial()
    {
        if (curTutorial != null)
        {
            curTutorial.Exit();
        }

        if (curIndex >= tutorials.Count - 1)
        {
            TutorialFinish();
            return;
        }

        curIndex++;
        curTutorial = tutorials[curIndex];

        curTutorial.Enter();
    }

    private void TutorialFinish()
    {
        curTutorial = null;

        Debug.Log("Tutorial Finish");
        RequireAcountTutorialFlag();
    }

    // 파이어베이스에 튜토리얼 완료 여부 flag 요청, -> tutorial 플래그 값에 따라 분기
    void RequireAcountTutorialFlag()
    {
        string userID = FirebaseManager.Auth.CurrentUser.UserId;

        if (userID == null)
            Manager.Scene.LoadScene(mainSceneName);

        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userID)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("Cancel");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("Faulted");
                    return;
                }
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                if (json == null)
                {
                    Debug.Log("Data Null");
                    Manager.Scene.LoadScene(mainSceneName);
                    return;
                }

                Userdata userData = JsonUtility.FromJson<Userdata>(json);
                if (!userData.tutorial)
                    GiveRewardToAcount(userData);
                else
                    Manager.Scene.LoadScene(mainSceneName);
            }
            );
        /*
        Userdata userdata = await FirebaseManager.Instance.LoadDataAsync();

        if (userdata == null)
            Debug.Log("userData null");

        if(!userdata.tutorial)
            GiveRewardToAcount(userdata);
        else
            Manager.Scene.LoadScene(mainSceneName);*/
    }


    void GiveRewardToAcount(Userdata userdata)
    {
        Manager.UI.ShowPopUpUI(rewardPopUp);

        userdata.tutorial = true;
        userdata.token += 5000000;
        SaveData(userdata);
    }

    bool SaveData(Userdata newUserData)
    {
        bool result = false;

        string userId = FirebaseManager.Auth.CurrentUser.UserId;
        string json = JsonUtility.ToJson(newUserData);
        Debug.Log(json);

        FirebaseManager.DB
            .GetReference("REFPATH")
            .Child(userId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("IsCanceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("IsFaulted");
                    return;
                }

                result = true;
                Debug.Log($"Save Data {result}");
            }
            );
                 return result;
    }
}
