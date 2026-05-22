using UnityEngine;
using UnityEngine.EventSystems;

public class UndoButton : ButtonBase
{
    [Tooltip("前フレームの押下状態")]
    private bool previousInteractable;

    private void LateUpdate()
    {
        previousInteractable = button.interactable;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && (button.interactable || previousInteractable)) AudioManager.Instance.PlaySE(buttonClickSE, false);
        else AudioManager.Instance.PlaySE(buttonClickInvalidSE, false);
    }
}
