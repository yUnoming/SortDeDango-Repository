using UnityEngine;
using UnityEngine.EventSystems;

public class EatButton : ButtonBase
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && !button.interactable)
            AudioManager.Instance.PlaySE(buttonClickInvalidSE, false);
    }
}
