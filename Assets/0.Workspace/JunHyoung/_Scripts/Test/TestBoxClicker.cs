using ItemLootSystem;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class TestBoxClicker : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] BaseBox box;

    public void OnPointerClick(PointerEventData eventData)
    {
        box.OnInteract(null);
    }
}
