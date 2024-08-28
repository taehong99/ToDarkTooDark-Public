using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class ShopPopUpPanel : MonoBehaviour
    {
        [SerializeField] Button CloseButton;

        private void Awake()
        {
            CloseButton.onClick.AddListener(Close);
        }

        public void ShowShop()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}