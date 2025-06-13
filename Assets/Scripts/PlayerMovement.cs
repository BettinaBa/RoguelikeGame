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
        // WASD via explicit key checks in case axes aren't configured
        moveInput = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) moveInput.x -= 1f;
        if (Input.GetKey(KeyCode.D)) moveInput.x += 1f;
        if (Input.GetKey(KeyCode.S)) moveInput.y -= 1f;
        if (Input.GetKey(KeyCode.W)) moveInput.y += 1f;
        moveInput.Normalize();

        // Space → dash
        if (stats != null
            && stats.dashUnlocked
            && Input.GetKeyDown(KeyCode.Space)
            && Time.time >= lastDashTime + dashCooldown)
        {
            lastDashTime = Time.time;
            if (moveInput.sqrMagnitude > 0f)
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
