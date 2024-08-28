using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    static DialogueManager instance;
    public static DialogueManager Instance { get { return instance; }}

    //[SerializeField] DialogueCanvas manager1;
    //[SerializeField] DialogueCanvas manager2;

    [SerializeField] List<DialogueCanvas> dialogueCanvases = new List<DialogueCanvas>();

    [SerializeField] Button nextButton;

    [SerializeField]  DialogueSequence dialogueSequence;

    public event Action OnSequnceFinsh;

    /*************************************************
    *                 Unity Event
    *************************************************/

    private void Awake()
    {
        CreateInstnace();

        nextButton.onClick.AddListener(NextButtonClick);
        DisableNextButton();
        DisableDialogues();
        timeTemp = Time.timeScale;

        for (int i = 0; i < dialogueCanvases.Count; i++)
        {
            dialogueCanvases[i].OnTypeFinish += EnableNextButton;
        }
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(NextButtonClick);

        for (int i = 0; i < dialogueCanvases.Count; i++)
        {
            dialogueCanvases[i].OnTypeFinish -= EnableNextButton;   
        }
    }


    void CreateInstnace()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("Create Instance");
        }
        else
            Destroy(gameObject);
    }



    /*************************************************
    *                   Methods
    *************************************************/


    public void SetDialogueSequence(DialogueSequence dialogueSequence)
    {
       this.dialogueSequence = Instantiate(dialogueSequence);
       EnableDialogues();
       NextButtonClick();
    }

    float timeTemp;
    void NextButtonClick()
    {
       
        Time.timeScale = 0f;
        DialogueData dialogue = dialogueSequence.NextDialogue();

        if (dialogue == null) //현재 대화 시퀀스 종료시 처리할 로직 추가할것
        {
            FinishSequence();
            return;
        }

        int canvasIndex = (int) dialogue.teller;

        ForcusDialogueCanvas(canvasIndex);
        dialogueCanvases[canvasIndex].PrintDialogue(dialogue);

        DisableNextButton();
    }

    void FinishSequence()
    {
        Time.timeScale = timeTemp;
        DisableDialogues();
        DisableNextButton();
        OnSequnceFinsh?.Invoke();
        OnSequnceFinsh = null; // 모든 대화는 일회성으로 이뤄지기 때문에 이벤트 초기화
    }

    public void DisableNPCCanvas() // 마지막 탈출 시퀀스에서 쓰기 위한 일회용 메서드...
    {
        dialogueCanvases[1]
        .gameObject.SetActive(false);
    }

    void DisableNextButton()
    {
        nextButton.gameObject.SetActive(false);
    }

    void EnableNextButton()
    {
        nextButton.gameObject.SetActive(true);
    }

    public void DisableDialogues()
    {
        for(int i = 0; i < dialogueCanvases.Count; i++){
            dialogueCanvases[i].gameObject.SetActive(false);
        }
    }

    public void EnableDialogues()
    {
        for (int i = 0; i < dialogueCanvases.Count; i++)
        {
            dialogueCanvases[i].gameObject.SetActive(true);
        }
    }


    void ForcusDialogueCanvas(int canvasIndex)
    {

        // Activate target Canvas
        dialogueCanvases[canvasIndex].GetCanvas().sortingOrder = 0;
        dialogueCanvases[canvasIndex].enabled = true;

        // Disable all other Canvases
        for (int i = 0; i < dialogueCanvases.Count; i++)
        {
            if (i != canvasIndex)
            {
                dialogueCanvases[i].GetCanvas().sortingOrder = -1;
                dialogueCanvases[i].enabled = false;
            }
        }
    }

}
