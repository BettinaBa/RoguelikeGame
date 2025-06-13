using UnityEngine;
using UnityEngine.InputSystem;

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
        // Gather movement input from the new Input System
        moveInput = Vector2.zero;
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y += 1f;
        }

        var gamepad = Gamepad.current;
        if (gamepad != null)
            moveInput += gamepad.leftStick.ReadValue();

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        // Space → dash
        if (stats != null
            && stats.dashUnlocked
            && (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
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
