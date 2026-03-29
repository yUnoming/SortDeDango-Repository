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
    private TextMeshProUGUI eatActionCountTMP;
    [SerializeField]
    private TextMeshProUGUI eatenDangoCountTMP;
    [SerializeField]
    private TextMeshProUGUI moveCountTMP;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button undoButton;
    [SerializeField]
    private Button eatButton;
    
    [Tooltip("Restartボタン押下時のイベント")]
    public event Action onRestartClicked;
    [Tooltip("Undoボタン押下時のイベント")]
    public event Action onUndoClicked;
    [Tooltip("Eatボタン押下時のイベント")]
    public event Action onEatClicked;


    private void Awake()
    {
        restartButton.onClick.AddListener(() => onRestartClicked?.Invoke());
        undoButton.onClick.AddListener(() => onUndoClicked?.Invoke());
        eatButton.onClick.AddListener(() => onEatClicked?.Invoke());
    }

    /// <summary>
    /// UI表示    </summary>
    public void Show() { content.SetActive(true); }
    /// <summary>
    /// UI非表示    </summary>
    public void Hide() { content.SetActive(false); }
    
    /// <summary>
    /// ステージ番号を更新    </summary>
    public void UpdateStageNumber(int stageNumber)
    {
        stageNumberTMP.text = $"Stage {stageNumber}";
    }
    /// <summary>
    /// 食べる使用回数を更新    </summary>
    /// <param name="remaining">
    /// 残り回数    </param>
    /// <param name="max">
    /// 最大回数    </param>
    public void UpdateEatActionCount(int remaining, int max)
    {
        eatActionCountTMP.text = $"{remaining} / {max}";
    }
    /// <summary>
    /// 食べた団子数を設定    </summary>
    /// <param name="current">
    /// 現在の食べた数    </param>
    /// <param name="target">
    /// 目標数    </param>
    public void UpdateEatenDangoCount(int current, int target)
    {
        eatenDangoCountTMP.text = $"Dango {current} / {target}";
    }
    /// <summary>
    /// 手数の設定    </summary>
    public void UpdateMoveCount(int count)
    {
        moveCountTMP.text = $"Moves {count}";
    }
}
