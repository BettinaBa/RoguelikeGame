using UnityEngine;
using UnityEngine.InputSystem; // for InputValue events

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Walk speed (multiplied by permanent upgrades).")]
    public float moveSpeed = 5f;

    [Header("Dash Settings")]
    [Tooltip("Dash distance in units.")]
    public float dashDistance = 5f;
    [Tooltip("Seconds between dashes.")]
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.right; // remembers last non-zero input
    private float lastDashTime = -Mathf.Infinity;
    private PlayerStats stats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        // Apply permanent speed multiplier from Meta_SpeedMult
        float metaMult = PlayerPrefs.GetFloat("Meta_SpeedMult", 1f);
        moveSpeed *= metaMult;
    }

    void Update()
    {
        // Legacy Input fallback (WASD)
        Vector2 legacy = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) legacy.x -= 1f;
        if (Input.GetKey(KeyCode.D)) legacy.x += 1f;
        if (Input.GetKey(KeyCode.S)) legacy.y -= 1f;
        if (Input.GetKey(KeyCode.W)) legacy.y += 1f;

        if (legacy.sqrMagnitude > 0f)
        {
            moveInput = legacy.normalized;
            lastMoveDir = moveInput;
        }
    }

    // Input System callback
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        moveInput.Normalize();
        if (moveInput.sqrMagnitude > 0f)
            lastMoveDir = moveInput;
    }

    // Input System callback for Jump (used as Dash)
    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;
        if (stats == null || !stats.dashUnlocked) return;
        if (Time.time < lastDashTime + dashCooldown) return;

        lastDashTime = Time.time;
        Vector2 dir = moveInput.sqrMagnitude > 0f ? moveInput : lastMoveDir;
        rb.MovePosition(rb.position + dir.normalized * dashDistance);
    }

    void FixedUpdate()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
