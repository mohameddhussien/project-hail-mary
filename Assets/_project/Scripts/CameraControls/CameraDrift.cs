using UnityEngine;

public class CameraDrift : MonoBehaviour
{
	void Update()
	{
		transform.position = new Vector3(
			Mathf.Sin(Time.time * 0.2f) * 0.2f, Mathf.Cos(Time.time * 0.2f) * 0.2f,
			-10
		);
	}
}