using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Rigidbody2D rb;
    private float baseSpeed = 2.5f;
    private bool movingRight = true;

    private float maxMultiplier = 2f;
    private float difficultyRampTime = 120f;

    private LayerMask wallLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
    private void FixedUpdate()
    {
        float speedMultiplier = GetSpeedMultiplier();
        Vector2 targetVelocity = new Vector2((movingRight ? baseSpeed : -baseSpeed) * speedMultiplier, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, 20f * Time.fixedDeltaTime);
    }

    private float GetSpeedMultiplier()
    {
        float gameTime = GameManager.instance?.totalPlayTime ?? 0f;
        return Mathf.Min(1f + (gameTime / difficultyRampTime * (maxMultiplier - 1f)), maxMultiplier);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform")) {
            movingRight = !movingRight;
            rb.AddForce(collision.transform.right * baseSpeed * 0.4f, ForceMode2D.Impulse);
        }
        else if (wallLayer == (wallLayer | (1 << collision.gameObject.layer)))
        {
            rb.velocity = new Vector2(movingRight ? baseSpeed : -baseSpeed, 0);
        }
    }

}
