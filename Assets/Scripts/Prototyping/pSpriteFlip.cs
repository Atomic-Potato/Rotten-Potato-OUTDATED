using UnityEngine;

public class pSpriteFlip : MonoBehaviour {
    [SerializeField] SpriteRenderer sp;
    [SerializeField] Rigidbody2D rb;

    private void Update() {
        if(rb.velocity.x > 0)
            sp.flipX = false;
        else if (rb.velocity.x < 0)
            sp.flipX = true;
    }
}