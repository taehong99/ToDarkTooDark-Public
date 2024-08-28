using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class ExitPopUpPanel : MonoBehaviour
    {
        // [SerializeField] TMP_Text infoText;  // 나중에 개그요소로 사용한다면...?
        [SerializeField] Button YesButton;
        [SerializeField] Button NoButton;
        [SerializeField] Button CloseButton;

        private void Awake()
        {
            YesButton.onClick.AddListener(Quit);
            NoButton.onClick.AddListener(Close);
            CloseButton.onClick.AddListener(Close);
        }

        public void ShowExit()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Quit()
        {
            gameObject.SetActive(false);
            Application.Quit();
            Debug.Log("GameQuit");
        }
    }
}