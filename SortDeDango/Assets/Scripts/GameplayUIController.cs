using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private TextMeshProUGUI stageNumberTMP;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button undoButton;

    [Tooltip("Restartボタン押下時のイベント")]
    public event Action onRestartClicked;
    [Tooltip("Undoボタン押下時のイベント")]
    public event Action onUndoClicked;


    private void Awake()
    {
        restartButton.onClick.AddListener(() => onRestartClicked?.Invoke());
        undoButton.onClick.AddListener(() => onUndoClicked?.Invoke());
    }

    /// <summary>
    /// UI表示    </summary>
    public void Show() { content.SetActive(true); }
    /// <summary>
    /// UI非表示    </summary>
    public void Hide() { content.SetActive(false); }
    /// <summary>
    /// ステージ番号を設定    </summary>
    public void SetStageNumber(int stageNumber)
    {
        stageNumberTMP.text = $"Stage {stageNumber}";
    }
}
