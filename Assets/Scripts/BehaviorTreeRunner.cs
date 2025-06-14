using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    public BTNode root;

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;
        root?.Tick();
    }
}
