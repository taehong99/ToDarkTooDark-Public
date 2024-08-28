using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class DeveloperNotesPopUpPanel : MonoBehaviour
    {
        [SerializeField] Button CloseButton;

        private void Awake()
        {
            CloseButton.onClick.AddListener(Close);
        }

        public void ShowDeveloperNotes()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}