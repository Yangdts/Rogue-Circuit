using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;


    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private Collider2D[] overlaps;
    private Vector2 direction;
    private float speed = 3f;
    private float jump = 4f;

    private bool grounded;
    private bool climbing;

    public AudioClip jumpSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        overlaps = new Collider2D[4];
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;
        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        //optimized ground detection
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, overlaps);

        for (int i = 0; i < amount; i++) 
        {
            GameObject hit = overlaps[i].gameObject;
            if (hit.layer == LayerMask.NameToLayer("Platform"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.6f);

                Physics2D.IgnoreCollision(collider, overlaps[i], !grounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
            }
        }
    }

    private void Update()
    {
        CheckCollision();

        if (climbing)
        {
            direction.y = Input.GetAxis("Vertical") * speed;
        }
        else if (grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jump;
            audioSource.PlayOneShot(jumpSound);
        }
        else 
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxis("Horizontal") * speed;
        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        //Gravity Manipulation 
        //direction += Physics2D.gravity * Time.deltaTime;
        //precision movement in Player.cs
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if(climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if(direction.x != 0f)
        {
            spriteIndex++;
            if(spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }
            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            enabled = false;
            GameManager.instance.LevelComplete();
        }
        else if(collision.gameObject.CompareTag("Hazard"))
        {
            enabled=false;
            GameManager.instance.LevelFailed();
        }
    }
}