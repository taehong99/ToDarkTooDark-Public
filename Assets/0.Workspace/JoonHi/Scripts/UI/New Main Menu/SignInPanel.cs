using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MainMenuPanelController;

public class SignInPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("InputField")]
    [SerializeField] InputField emailInputField;
    // [SerializeField] InputField passInputField;
    [SerializeField] TMP_InputField passInputField;

    [Header("Buttons")]
    [SerializeField] Button signinButton;
    [SerializeField] Button signUpButton;
    [Space]
    [SerializeField] Button exitbutton;

    [Header("PopUp Panel")]
    [SerializeField] SignUpPanel signUpPanel;

    [Header("Panels")]
    [SerializeField] NewMainMenuPanel mainMenuPanel;
 
    private void Awake()
    {
        signinButton.onClick.AddListener(SignIn);
        signUpButton.onClick.AddListener(SignUp);

        exitbutton.onClick.AddListener(panelController.Quit);
    }

    private void Start()
    {
#if UNITY_EDITOR
        emailInputField.text = "tae@tae.tae";
        passInputField.text = "taetae";
#else
        emailInputField.text = string.Empty;
        passInputField.text = string.Empty;
#endif



        signUpPanel.gameObject.SetActive(false);

        if (FirebaseManager.Auth != null && FirebaseManager.Auth.CurrentUser != null)
        {
            Debug.Log(FirebaseManager.Auth.CurrentUser.UserId);
            panelController.connectionLoadPanel.gameObject.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
            panelController.SetActivePanel(MainPanel.Main);
            panelController.connectionLoadPanel.gameObject.SetActive(false);
        }
    }

    public void SignUp()
    {
        // panelController.SetActivePanel(PanelController.Panel.SignUp);
        signUpPanel.gameObject.SetActive(true);
    }

    public void SignIn()
    {
        SetInteractable(false);

        string email = emailInputField.text;
        string password = passInputField.text;

        if (email == "" || password == "")
        {
            // panelController.ShowInfo("invalid email or password");
            Debug.Log("ID or Password Empty");
            SetInteractable(true);
            return;
        }

        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                // panelController.ShowInfo("SingIn With Email and Password was canceled");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                // panelController.ShowInfo("invalid email or password");
                SetInteractable(true);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
            PhotonNetwork.ConnectUsingSettings();
            panelController.SetActivePanel(MainMenuPanelController.MainPanel.Main);
            LoadUser(result.User.UserId);
            panelController.connectionLoadPanel.SetActive(true);
            SetInteractable(true);
            /* 이메일 확인
            if ( result.User.IsEmailVerified )
            {
                panelController.SetActivePanel(PanelController.Panel.Main);
            }
            else
            {
                // panelController.SetActivePanel(PanelController.Panel.Verify);
            }
            */
        });

    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        signUpButton.interactable = interactable;
        signinButton.interactable = interactable;
    }

    

    private void LoadUser(string userID) //데이터 불러오기
    {
        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userID)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("취소");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("오류");
                    return;
                }
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                Debug.Log(json);

                if(json == null)
                {
                    mainMenuPanel.NewPlayer();
                    return;
                }

                Userdata userData = JsonUtility.FromJson<Userdata>(json);
                Debug.Log(userData.nickName);
                Debug.Log(userData.token);
                Debug.Log(userData.excaliver);

                PhotonNetwork.LocalPlayer.NickName = userData.nickName;
                //mainMenuPanel.Welcome();
                // Userdata userData = JsonUtility.FromJson(json);
            }
            );
    }
}
