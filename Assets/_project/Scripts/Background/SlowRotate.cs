using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 2 * Time.deltaTime, 0);
    }
}
