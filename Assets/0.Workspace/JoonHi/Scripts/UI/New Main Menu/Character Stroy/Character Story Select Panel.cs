using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStorySelectPanel : MonoBehaviour
{
    [Header("Data")]
    public CharacterInfoData charadata;
    
    [Header("UI")]
    [SerializeField] Text charName;
    [SerializeField] Image charImage;

    [Header("Buttons")]
    [SerializeField] Button selectButton;

    [Header("PopUp Panel")]
    [SerializeField] CharacterStoryPanel storyPanel;

    private void Awake()
    {
        charName.text = charadata.CharacterName;
        charImage.sprite = charadata.CharacterSprite;
    }

    private void Start()
    {
        selectButton.onClick.AddListener(ShowStroy);
    }

    private void ShowStroy()
    {
        storyPanel.gameObject.SetActive(true);
    }
}
