using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    [SerializeField] Button muteOnButton;
    [SerializeField] Button muteOffButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Button saveButton;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        muteOnButton.onClick.AddListener(Mute);
        muteOffButton.onClick.AddListener(Unmute);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        saveButton.onClick.AddListener(ClosePanel);
        closeButton.onClick.AddListener(ClosePanel);
    }

    private void OnDestroy()
    {
        muteOnButton.onClick.RemoveAllListeners();
        muteOffButton.onClick.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
    }

    public void SetVolume(float sliderValue)
    {
        mixer.SetFloat("Volume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }

    public void Mute()
    {
        mixer.SetFloat("Volume", -80); // Mute by setting to -80 dB
    }

    public void Unmute()
    {
        SetVolume(volumeSlider.value);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
