using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("押した際に下げる量")]
    private float pressDownValue;
    [SerializeField]
    private Shadow shadow;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.position += Vector3.down * pressDownValue;
        shadow.enabled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.position += Vector3.up * pressDownValue;
        shadow.enabled = true;
    }
}