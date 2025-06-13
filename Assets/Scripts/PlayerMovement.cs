using UnityEngine;

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
        // WASD / arrows
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // Space → dash
        if (stats != null
            && stats.dashUnlocked
            && Input.GetKeyDown(KeyCode.Space)
            && Time.time >= lastDashTime + dashCooldown
            && moveInput.sqrMagnitude > 0f)
        {
            lastDashTime = Time.time;
            rb.MovePosition(rb.position + moveInput * dashDistance);
        }
    }

    void FixedUpdate()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
