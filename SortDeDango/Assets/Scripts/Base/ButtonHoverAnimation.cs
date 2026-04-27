using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, Tooltip("ホバー時の拡大サイズ")]
    private Vector3 hoverScaleUp = Vector3.one;
    [SerializeField]
    private Button button;

    [Tooltip("通常サイズ")]
    private Vector3 defaultScale = Vector3.one;
    [Tooltip("ホバー状態かどうか")]
    public bool isHovered { get; private set; }

    private void Awake()
    {
        defaultScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(button.interactable)
        {
            isHovered = true;
            transform.localScale = hoverScaleUp;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            isHovered = false;
            transform.localScale = defaultScale;
        }
    }
}
