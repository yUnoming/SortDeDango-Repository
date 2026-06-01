using UnityEngine;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject content;

    /// <summary>
    /// UI表示    </summary>
    public void Show() { content.SetActive(true); }
    /// <summary>
    /// UI非表示    </summary>
    public void Hide() { content.SetActive(false); }
    /// <summary>
    /// 表示状態の切り替え    </summary>
    public void ToggleDisplay()
    {
        content.SetActive(!content.activeSelf);
    }
}
