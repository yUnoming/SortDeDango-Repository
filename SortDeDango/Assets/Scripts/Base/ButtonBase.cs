using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, Tooltip("クリックSE")]
    protected AudioData buttonClickSE;
    [SerializeField, Tooltip("クリック失敗SE")]
    protected AudioData buttonClickInvalidSE;
    [SerializeField]
    protected Button button;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && button.interactable) AudioManager.Instance.PlaySE(buttonClickSE, false);
        else AudioManager.Instance.PlaySE(buttonClickInvalidSE, false);
    }
}