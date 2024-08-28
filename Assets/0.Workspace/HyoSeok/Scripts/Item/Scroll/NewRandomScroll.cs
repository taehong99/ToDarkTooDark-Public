using UnityEngine;

[CreateAssetMenu(fileName = "NewRandomScroll", menuName = "Item/RandomScroll")]
public class NewRandomScroll : BaseScrollItem
{
    RandomScroll player;
    protected override void UseScroll(GameObject user)
    {
        player = user.GetComponentInChildren<RandomScroll>();
        if (player == null)
            return;
        player.UseItem();
    }


}
