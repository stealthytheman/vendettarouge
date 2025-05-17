using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float moveX;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");


        animator.SetBool("isWalking", moveX != 0);

        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX < 0;
        }
    }

    void FixedUpdate()
    {
        Vector2 targetPosition = rb.position + new Vector2(moveX * moveSpeed * Time.fixedDeltaTime, 0);
        rb.MovePosition(targetPosition);
    }      
}

