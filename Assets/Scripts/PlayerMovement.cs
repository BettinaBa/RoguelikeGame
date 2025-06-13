using UnityEngine;
using UnityEngine.InputSystem;  // ← if you're using the new Input System

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Base move speed of the player; will be multiplied by permanent upgrades.")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Apply any permanent speed-multiplier upgrades from PlayerPrefs:
        //   Meta_SpeedMult defaults to 1f, then gets multiplied each purchase in UpgradeMenuManager
        float speedMult = PlayerPrefs.GetFloat("Meta_SpeedMult", 1f);
        moveSpeed *= speedMult;
    }

    // New Input System callback (if you're using it):
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
