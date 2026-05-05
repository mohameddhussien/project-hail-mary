using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Vector3 normal = Vector3.one, hover = Vector3.one * 1.1f;

	public void OnPointerEnter(PointerEventData eventData)
	{
		transform.localScale = hover;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		transform.localScale = normal;
	}
}