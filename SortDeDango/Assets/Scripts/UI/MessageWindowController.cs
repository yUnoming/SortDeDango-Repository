using TMPro;
using UnityEngine;

public class MessageWindowController : MonoBehaviour
{
    [SerializeField]
    private GameObject windowPanel;
    [SerializeField]
    private TextMeshProUGUI messageText;

    /// <summary>
    /// ウィンドウを開く    </summary>
    public void ShowWindow() { windowPanel.SetActive(true); }
    /// <summary>
    /// ウィンドウを閉じる    </summary>
    public void CloseWindow() { windowPanel.SetActive(false); }
    /// <summary>
    /// 表示する文字列のセット    </summary>
    public void SetMessage(string message) { messageText.text = message; }
}
