using UnityEngine;

public interface IKnockbackable
{
    void GetKnockedBack(Vector2 direction, float distance, float duration);
}
