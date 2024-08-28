using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CharacterStoryPanel : MonoBehaviour
{
    [Header("Parent Panel")]
    [SerializeField] CharacterStoryMainPanel mainPanel;

    [Header("Buttons")]
    [SerializeField] Button toStorybutton;
    [SerializeField] Button toSkillButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button toLobbyButton;

    [Header("Panels")]
    [SerializeField] GameObject story;
    [SerializeField] GameObject skill;

    [Header("Color")]
    [SerializeField] Color nonSelectedColor;
    [SerializeField] Color selectedColor;

    [Header("Select Object")]
    [SerializeField] RectTransform selectObject;
    [SerializeField] RectTransform initSkillIcon;
    [SerializeField] List<GameObject> skillInfo = new List<GameObject>();
    [SerializeField] List<Text> skillName = new List<Text>();

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        toStorybutton.onClick.AddListener(ToStory);
        toSkillButton.onClick.AddListener(ToSkill);

        exitButton.onClick.AddListener(Close);
        toLobbyButton.onClick.AddListener(BacktoLobby);
    }

    private void OnEnable()
    {
        SetInit();
    }

    private void SetInit()
    {
        ToStory();
        moveSelect(initSkillIcon);
        ChangeSkillinfo(skillInfo[0]);
        ChangeTextColor(skillName[0]);
    }

    private void ToStory()
    {
        story.gameObject.SetActive(true);
        skill.gameObject.SetActive(false);
    }

    private void ToSkill()
    {
        story.gameObject.SetActive(false);
        skill.gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void BacktoLobby()
    {
        mainPanel.Close();
        gameObject.SetActive(false);
    }

    public void moveSelect(RectTransform position)
    {
        selectObject.position = position.position;
    }

    public void ChangeSkillinfo(GameObject skillinfo)
    {
        foreach(GameObject skill in skillInfo)
        {
            skill.SetActive(false);
        }
        skillinfo.SetActive(true);
    }

    public void ChangeTextColor(Text text)
    {
        foreach (Text skill in skillName)
        {
            skill.color = nonSelectedColor;
        }
        text.color = selectedColor;
    }
}
