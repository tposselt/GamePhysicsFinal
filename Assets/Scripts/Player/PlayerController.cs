using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public enum PlayerState
{
    GROUND,
    DEAD
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float speed = 1f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deceleration = 10f;
    [SerializeField] float airControl = 0.5f;       // Multiplier for acceleration while in air (0-1)

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float lowJumpMultiplier = 2f;  // Multiplier for gravity when jump button is released early (controls minimum jump height)
    [SerializeField] float fallMultiplier = 2.5f;   // Increases gravity when falling for better feel
    [SerializeField] float coyoteTime = 0.2f;       // Time in seconds character can jump after leaving ground

    [Header("Ground Detection")]
    [SerializeField] float castDistance = 0.1f;     // How far to check for ground below the character
    [SerializeField] ContactFilter2D groundFilter;  // Layer and collision settings for ground detection

    [Header("Components")]
    [SerializeField] Animator animator;             // Reference to character's animator
    [SerializeField] SpriteRenderer spriteRenderer; // Reference to character's sprite renderer
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text youDiedText;
    //[SerializeField] ScoreBoard deathScreen;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject leftTeleporter;
    [SerializeField] GameObject rightTeleporter;
    [SerializeField] BoolEventChannelSO teleportEvent;
    [SerializeField] EventChannelSO resetEvent;
    [SerializeField] GameObject magnetHitbox;

    int score;
    int regression;

    Rigidbody2D rb;
    Magnet magnet;
    Vector3 magnetPull;
    float magnetMultiplier = 20;

    public const int FACE_LEFT = -1;
    public const int FACE_RIGHT = 1;

    int facing = FACE_RIGHT;
    float currentSpeed = 0f; // Current horizontal speed after smoothing
    public int Facing => facing;

    public Rigidbody2D RB => rb;

    // Ground collision tracking
    RaycastHit2D[] raycastHits = new RaycastHit2D[5]; // Buffer for ground collision results
    int groundHits = 0;                              // Number of ground collisions detected
    bool isGrounded = false;                         // Whether character is currently on ground
    float coyoteTimer = 0f;                          // Timer for coyote time jump window
    float respawnTimer = 5f;

    // Jump state tracking
    bool jumpButtonReleased = true;                  // Whether jump button has been released since last press

    // Movement input
    Vector2 direction;                              // Current input direction (typically from -1 to 1)

    PlayerState state = PlayerState.GROUND;

    public void OnMove(Vector2 v) => direction = v;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        magnet = magnetHitbox.GetComponent<Magnet>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) Magnet();
        else if (Input.GetMouseButtonUp(0)) magnetPull = Vector3.zero;
        UpdateGroundCollision();
        UpdateFacing();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        scoreText.text = "Score: " + score;

        switch (state)
        {
            case PlayerState.GROUND:
                {
                    // Calculate target speed based on input direction
                    float targetSpeed = direction.x * speed;

                    // Apply acceleration based on grounded state
                    // Reduces control in the air by using the airControl multiplier
                    float accelRate = isGrounded ? acceleration : acceleration * airControl;

                    // Apply movement with smoothing
                    if (Mathf.Abs(targetSpeed) > 0.01f)
                    {
                        // Accelerating towards target speed
                        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);
                    }
                    else
                    {
                        // Decelerating to stop
                        currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
                    }

                    // Apply horizontal velocity
                    rb.linearVelocityX = currentSpeed + magnetPull.x;

                    // Apply variable jump height physics
                    if (rb.linearVelocityY < 0)
                    {
                        // We're falling - apply increased gravity for snappier falls
                        rb.linearVelocityY += Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                    }
                    else if (rb.linearVelocityY > 0 && jumpButtonReleased)
                    {
                        // We're rising but the jump button was released early
                        // Apply extra gravity to cut the jump short (creates variable jump height)
                        rb.linearVelocityY += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
                    }

                    rb.linearVelocityY += magnetPull.y;
                    break;
                }
            case PlayerState.DEAD:
                {
                    if (respawnTimer > 0) respawnTimer -= Time.deltaTime;
                    else
                    {
                        score = 0;
                        regression = 0;
                        resetEvent.Raise();
                        state = PlayerState.GROUND;
                        rb.linearVelocity = Vector2.zero;
                        rb.rotation = 0;
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                        transform.position = new Vector3(0, -2.4f, 0);
                        youDiedText.text = "";
                        respawnTimer = 5;
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// Update character's facing direction based on movement input.
    /// </summary>
    void UpdateFacing()
    {
        // Only flip if we're moving and facing the opposite direction
        if (direction.x != 0 && Mathf.Sign(direction.x) != facing)
        {
            FlipDirection();
        }
    }

    /// <summary>
    /// Check for ground collision and update grounded state.
    /// Also handles landing logic and timer updates.
    /// </summary>
    void UpdateGroundCollision()
    {
        // Cast a ray downward to detect ground
        groundHits = rb.Cast(Vector2.down, groundFilter, raycastHits, castDistance);

        // Update grounded state
        bool wasGrounded = isGrounded;
        isGrounded = (groundHits > 0);

        // Handle landing (transitioning from in-air to grounded)
        if (isGrounded && !wasGrounded && rb.linearVelocityY < 0)
        {
            // Stop jump phase and vertical momentum when landing
            rb.linearVelocityY = 0;

            // Reset jump animation trigger to prevent retriggering
            animator?.ResetTrigger("Jump");
        }

        // Reset coyote timer when grounded
        if (isGrounded) coyoteTimer = coyoteTime;

        // Update timers
        coyoteTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Update animator parameters based on character state.
    /// </summary>
    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("InAir", !isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(direction.x));
        animator.SetFloat("VelocityY", rb.linearVelocityY);
    }

    /// <summary>
    /// Handle jump input. Called when jump button is pressed.
    /// </summary>
    public void OnJump()
    {
        switch (state)
        {
            case PlayerState.GROUND:
                {
                    if (coyoteTimer > 0)
                    {
                        // First jump - using coyote time for better feel
                        jumpButtonReleased = false;
                        ExecuteJump();
                    }
                    break;
                }
        }
    }

    public void OnJumpRelease()
    {
        jumpButtonReleased = true;  // This flag is used in FixedUpdate to apply the lowJumpMultiplier
    }

    private void ExecuteJump()
    {
        // Calculate jump velocity using physics formula:
        // v = sqrt(2 * g * h) where g is gravity and h is desired height
        float jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight * rb.gravityScale);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);

        // Trigger jump animation if animator exists
        animator?.SetTrigger("Jump");
    }

    public void Magnet()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - transform.position);
        direction.Normalize();

        float degrees = (float) (Mathf.Rad2Deg * Math.Atan2(direction.y, direction.x));
        if (degrees < 0) degrees += 360;

        magnetHitbox.transform.rotation = Quaternion.Euler(0, 0, degrees + (Mathf.Sign(degrees) * 90));

        magnetPull = magnet.CalculateMagnetPull() * magnetMultiplier;
    }

    public void OnDeath()
    {
        state = PlayerState.DEAD;
        rb.constraints = RigidbodyConstraints2D.None;
        youDiedText.text = "YOU DIED";
    }

    /// <summary>
    /// Flip the character's facing direction by updating the sprite.
    /// </summary>
    private void FlipDirection()
    {
        facing *= -1;  // Toggle between 1 and -1
        if (spriteRenderer != null)
            spriteRenderer.flipX = (facing == -1);  // Flip sprite when facing left
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        Rigidbody2D collisionRb;
        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out collisionRb))
        {
            if (Math.Abs(collisionRb.linearVelocity.magnitude - rb.linearVelocity.magnitude) > 1) OnDeath();
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Laser") OnDeath();
        else if (collision.gameObject == leftTeleporter)
        {
            teleportEvent.Raise(true);
            regression++;
        }
        else if (collision.gameObject == rightTeleporter)
        {
            teleportEvent.Raise(false);
            if (regression > 0) regression--;
            else score++;
        }
    }
}