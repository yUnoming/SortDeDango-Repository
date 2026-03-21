using System;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private Button nextButton;

    [Tooltip("Nextボタン押下時のイベント")]
    public event Action OnNextClicked;

    private void Awake()
    {
        nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());
    }

    /// <summary>
    /// UI表示    </summary>
    public void Show() { content.SetActive(true); }
    /// <summary>
    /// UI非表示    </summary>
    public void Hide() { content.SetActive(false); }
}
