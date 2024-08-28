using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class SettingPopUpPanel : MonoBehaviour
    {
        [SerializeField] Button CloseButton;

        private void Awake()
        {
            CloseButton.onClick.AddListener(Close);
        }

        public void ShowSetting()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}