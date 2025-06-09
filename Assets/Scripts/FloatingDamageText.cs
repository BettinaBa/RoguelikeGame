using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public float lifetime = 1f;
    public Vector3 moveVector = new Vector3(0, 1f, 0);
    private TextMeshPro text;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void Setup(int damage)
    {
        if (text != null)
            text.text = damage.ToString();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
    }
}
