using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class AttendancePopUpPanel : MonoBehaviour
    {
        [SerializeField] Button CloseButton;

        private void Awake()
        {
            CloseButton.onClick.AddListener(Close);
        }

        public void ShowAttendance()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}