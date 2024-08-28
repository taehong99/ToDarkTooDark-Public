using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("InputField")]
    [SerializeField] InputField emailInputField;
    [SerializeField] TMP_InputField passInputField;
    [SerializeField] TMP_InputField confirmInputField;
    [SerializeField] TMP_InputField nicknameInputField;

    [Header("Buttons")]
    [SerializeField] Button cancelButton;
    [SerializeField] Button signUpButton;

    [Header("Texts")]
    [SerializeField] GameObject UcanUseThisID;
    [SerializeField] GameObject allreadyID;
    [SerializeField] GameObject UcanUseThisPW;
    [SerializeField] GameObject ShortPW;
    [SerializeField] GameObject SamePW;
    [SerializeField] GameObject NotSamePW;
    [SerializeField] GameObject UcanUseThisNickName;
    [SerializeField] GameObject allreadyNickName;
    [SerializeField] GameObject TooLongNickName;

    [Header("Panel")]
    [SerializeField] ConfirmPanel confirmPanel;
    [SerializeField] ConfirmPanel failPanel;

    private bool canUseID;
    private bool canUsePW;
    private bool pwMatch;
    private bool canUseNickname;

    private DatabaseReference reference;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Cancel);
        signUpButton.onClick.AddListener(SignUp);

        passInputField.onEndEdit.AddListener(PasswordCheck);
        confirmInputField.onEndEdit.AddListener(PasswordCheck);
    }

    private void Start()
    {
        UcanUseThisID.SetActive(false);
        allreadyID.SetActive(false);
        UcanUseThisPW.SetActive(false);
        ShortPW.SetActive(false);
        SamePW.SetActive(false);
        NotSamePW.SetActive(false);
        UcanUseThisNickName.SetActive(false);
        allreadyNickName.SetActive(false);
        TooLongNickName.SetActive(false);
    }

    public void SignUp()
    {
        string email = emailInputField.text;
        string password = passInputField.text;
        string confirm = confirmInputField.text;
        string nickname = nicknameInputField.text;

        if (password.Length == 0 || password.Length < 6)
        {
            failPanel.Show();
            return;
        }

        if (password != confirm)
        {
            pwMatch = false;
            failPanel.Show();
            return;
        }

        FirebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                // panelController.ShowInfo("CreateUserWithEmailAndPasswordAsync canceled");
                failPanel.Show();
                SetInteractable(true);
                return;
            }
            else if (task.IsFaulted)
            {
                // panelController.ShowInfo($"CreateUserWithEmailAndPasswordAsync failed : {task.Exception.Message}");
                failPanel.Show();
                SetInteractable(true);
                return;
            }

            confirmPanel.Show();
            SetInteractable(true);
            gameObject.SetActive(false);
        });
    }

    public void Cancel()
    {
        emailInputField.text = "";
        passInputField.text = "";
        confirmInputField.text = "";

        this.gameObject.SetActive(false);
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        confirmInputField.interactable = interactable;
        cancelButton.interactable = interactable;
        signUpButton.interactable = interactable;
    }

    public void PasswordCheck(string buf)
    {
        if(passInputField.text.Length == 0 && confirmInputField.text.Length == 0)
        {
            SamePW.SetActive(false);
            NotSamePW.SetActive(false);
            return;
        }

        if(passInputField.text.Length == 0)
        {
            UcanUseThisPW.SetActive(false);
            ShortPW.SetActive(false);
        }
        else if(passInputField.text.Length < 6 && passInputField.text.Length > 0)
        {
            UcanUseThisPW.SetActive(false);
            ShortPW.SetActive(true);
        }
        else if(passInputField.text.Length >= 6)
        {
            UcanUseThisPW.SetActive(true);
            ShortPW.SetActive(false);
        }

        if(confirmInputField.text.Length == 0 || (passInputField.text.Length < 6 && passInputField.text.Length > 0) || passInputField.text.Length == 0)
        {
            SamePW.SetActive(false);
            NotSamePW.SetActive(false);
        }
        else if(passInputField.text == confirmInputField.text)
        {
            SamePW.SetActive(true);
            NotSamePW.SetActive(false);
        }
        else 
        {
            SamePW.SetActive(false);
            NotSamePW.SetActive(true);
        }
    }
}
