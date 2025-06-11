using UnityEngine;
using UnityEngine.InputSystem;      // ← add this

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;       // ← replace moveDirection

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ← New Input System callback:
    public void OnMove(InputValue value)
    {
        // this gets called automatically when your "Move" action fires
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // same GameOver check you had
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        // apply movement
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}