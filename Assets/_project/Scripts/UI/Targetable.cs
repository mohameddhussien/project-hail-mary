using UnityEngine;

public class Targetable : MonoBehaviour
{
    private void OnEnable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.AddTarget(transform);
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.RemoveTarget(transform);
    }
}