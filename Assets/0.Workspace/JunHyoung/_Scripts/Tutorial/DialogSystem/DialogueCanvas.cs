using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueCanvas : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{ 
    [Header("Setting")]
    [SerializeField] float term = 0.01f;
    [SerializeField] float doubleSpeedterm  = 0.002f;
    [Range(0, 1), SerializeField] float blurValue = 0.5f;
    float curTerm;

    [Space(15), Header("Components")]
    [SerializeField] Canvas actorCanvas;
    [SerializeField] TextMeshProUGUI actorName;
    [SerializeField] TextMeshProUGUI actorDialogue;
    [SerializeField] WaveTextEffect waveEffect;

    [SerializeField] RectTransform portraitRect;
    [SerializeField] Image actorPortrait;
    [SerializeField] Image backGround;

    [Space(10),Header("Button"),SerializeField] Button skipButton;

    public event Action OnTypeFinish;
    public Canvas GetCanvas() { return actorCanvas; }

    Coroutine typeRoutine;
    Coroutine portraitMoveRoutine;

    /*************************************************
    *                 Unity Event
    *************************************************/

    // Init States and Subscribe Events

    private void Awake()
    {
        curTerm = term;
        skipButton.onClick.AddListener(SkipDialogue);
    }

    private void OnDestroy()
    {
        OnTypeFinish = null;
        skipButton.onClick.RemoveListener(SkipDialogue);
    }


    // Manage UI Alpha value 
    
    void OnEnable()
    {
        if (actorPortrait.sprite != null)
            actorPortrait.color = Color.white;
        else
            actorPortrait.color = Color.clear;
        backGround.color = new Color(backGround.color.r, backGround.color.g, backGround.color.b, 1);
        actorDialogue.color = Color.black;
        actorName.color = Color.black;

        waveEffect.enabled = true;
    }

    void OnDisable()
    {
        if (actorPortrait.sprite != null)
            actorPortrait.color = new Color(1f, 1f, 1f, blurValue);
        else
            actorPortrait.color = Color.clear;
        actorDialogue.color = new Color(0f,0f, 0f, blurValue);
        backGround.color =new Color(backGround.color.r, backGround.color.g, backGround.color.b, blurValue);
        actorName.color = new Color(0f, 0f, 0f, blurValue);

        waveEffect.enabled = false;

    }


    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            SkipDialogue();
        }
    }
    /*************************************************
    *                    Methods
    *************************************************/
    [ContextMenu("typing text")]
    public void TypingTest()
    {
        if(typeRoutine == null)
            typeRoutine = StartCoroutine(TypingText(actorDialogue, actorDialogue.text, true));
    }

    public void PrintDialogue(DialogueData dialogue)
    {
        actorName.text = dialogue.teller.ToString();
        
        if(dialogue.sprite != null)
        {
            actorPortrait.sprite = dialogue.sprite;
            actorPortrait.color = Color.white;
        }

        if (typeRoutine == null)
            typeRoutine = StartCoroutine(TypingText(actorDialogue, dialogue.script, true));
    }

    public void SkipDialogue()
    {
        actorDialogue.maxVisibleCharacters = actorDialogue.text.Length;
        OnTypeFinish?.Invoke();
        ClearRoutine();
    }

    IEnumerator TypingText(TextMeshProUGUI targetUI, string text , bool movePortrait = false)
    {
        targetUI.text = text;
        targetUI.maxVisibleCharacters = 0;

        if (portraitMoveRoutine == null && movePortrait)
            portraitMoveRoutine = StartCoroutine(MovePortrait());


        while (targetUI.maxVisibleCharacters <= text.Length)
        {
            targetUI.maxVisibleCharacters++;
            yield return YieldCache.WaitForSecondsRealTime(curTerm);
            //yield return new WaitForSecondsRealtime(curTerm);
        }
        OnTypeFinish?.Invoke();
        ClearRoutine();
    }

    IEnumerator MovePortrait()
    {
        Vector2 originalPosition = portraitRect.anchoredPosition;
        float speed = 1f;
        float range = 20f;

        while (true)
        {
            float newY = Mathf.PingPong(Time.unscaledTime *speed / curTerm, range * 2) - range + originalPosition.y;
            portraitRect.anchoredPosition = new Vector2(originalPosition.x, newY);
            yield return null;
        }
    }


    public void TypingTextDouble()
    {
        curTerm = doubleSpeedterm;
    }

    void ClearRoutine()
    {
        StopAllCoroutines();
        typeRoutine = null;
        portraitMoveRoutine = null;
    }

    /*************************************************
     *           IPointer Click Interface
     *************************************************/

    // 좌클릭시 스킵, 우클릭시 배속

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SkipDialogue();
        }

        if(eventData.button == PointerEventData.InputButton.Right)
        {
            curTerm = doubleSpeedterm;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        curTerm = term;
    }
}
