using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ThrowItemPopUpUI : BaseUI
{
    private Slider countSlider;
    private TextMeshProUGUI sliderValue;
    private Button cancelButton;
    private Button throwButton;
    private UnityAction<int> throwItemsEvent;

    public void Init()
    {
        base.Awake();

        countSlider = GetUI<Slider>("CountSlider");
        sliderValue = GetUI<TextMeshProUGUI>("SliderValue");
        cancelButton = GetUI<Button>("CancelButton");
        throwButton = GetUI<Button>("ThrowButton");

        OnSliderValueChanged(countSlider.minValue);
        countSlider.onValueChanged.AddListener(OnSliderValueChanged);
        throwButton.onClick.AddListener(OnThrow);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnEnable()
    {
        countSlider.value = 1;
    }

    private void OnDisable()
    {
        // Unsubscribe
        throwItemsEvent = null;
    }

    private void OnDestroy()
    {
        // Unsubscribe
        throwButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }

    public void PopUp(int maxValue, UnityAction<int> onThrowAction)
    {
        // Change maxValue
        countSlider.maxValue = maxValue;

        // delegates subscribe to events
        throwItemsEvent += onThrowAction;

        // Display UI
        gameObject.SetActive(true);
    }

    private void OnSliderValueChanged(float value)
    {
        sliderValue.text = value.ToString();
    }

    private void OnThrow()
    {
        throwItemsEvent?.Invoke((int)countSlider.value);
        gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
