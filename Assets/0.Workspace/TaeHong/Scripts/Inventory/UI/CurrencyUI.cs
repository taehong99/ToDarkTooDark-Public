using UnityEngine;
using UnityEngine.UI;
using Tae.Inventory;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] Text goldCount;
    [SerializeField] Text bronzeKeyCount;
    [SerializeField] Text silverKeyCount;
    [SerializeField] Text goldKeyCount;

    public void Init()
    {
        InventoryManager.Instance.goldCountChangedEvent += UpdateGoldCount;
        InventoryManager.Instance.keyCountChangedEvent += UpdateKeyCount;
    }

    public void OnDestroy()
    {
        InventoryManager.Instance.goldCountChangedEvent -= UpdateGoldCount;
        InventoryManager.Instance.keyCountChangedEvent -= UpdateKeyCount;
    }

    private void UpdateGoldCount(int count)
    {
        goldCount.text = count.ToString();
    }

    private void UpdateKeyCount()
    {
        bronzeKeyCount.text = InventoryManager.Instance.BronzeKeyCount.ToString();
        silverKeyCount.text = InventoryManager.Instance.SilverKeyCount.ToString();
        goldKeyCount.text = InventoryManager.Instance.GoldKeyCount.ToString();
    }
}
