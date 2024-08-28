namespace Tae
{
    public interface IInteractable
    {
        void OnInteract(Interactor interactor);
        void ShowUI();
        void HideUI();
    }
}