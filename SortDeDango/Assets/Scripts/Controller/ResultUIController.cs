using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private TextMeshProUGUI moveCountTMP;
    [SerializeField]
    private TextMeshProUGUI minMoveCountTMP;
    [SerializeField]
    private TextMeshProUGUI resultMessageTMP;
    [SerializeField]
    private GameObject cherryBlossomPetals;

    [Tooltip("Nextボタン押下時のイベント")]
    public event Action onNextClicked;

    private void Awake()
    {
        nextButton.onClick.AddListener(() => onNextClicked?.Invoke());
        Hide();
    }

    /// <summary>
    /// UI表示    </summary>
    public void Show() { content.SetActive(true); }
    /// <summary>
    /// UI非表示    </summary>
    public void Hide() { content.SetActive(false); }

    /// <summary>
    /// リザルト表示    </summary>
    /// <param name="result">
    /// リザルト情報    </param>
    public void ShowResult(ResultData result)
    {
        moveCountTMP.text = $"{result.moveCount}";
        minMoveCountTMP.text = $"{result.minMoveCount}";

        if (result.IsMinMoveCleared())
        {
            resultMessageTMP.text = "Perfect Move!!";
            cherryBlossomPetals.SetActive(true);
        }
        else if (result.moveCount > result.minMoveCount) resultMessageTMP.text = $"{result.moveCount - result.minMoveCount} move away from perfect!";
        else resultMessageTMP.text = "You are smarter than the developer!!";
    }
}
