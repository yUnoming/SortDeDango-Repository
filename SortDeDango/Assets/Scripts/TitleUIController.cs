using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private Button continueButton;

    [Tooltip("NewGameボタン押下時のイベント")]
    public event Action onNewGameClicked;
    [Tooltip("Continueボタン押下時のイベント")]
    public event Action onContinueClicked;

    private void Awake()
    {
        newGameButton.onClick.AddListener(() => onNewGameClicked?.Invoke());
        continueButton.onClick.AddListener(() => onContinueClicked?.Invoke());
    }
    
    /// <summary>
    /// Continueボタンの入力受付状態を設定    </summary>
    /// <param name="isInteractable">
    /// 入力を受け付けるかどうか    </param>
    public void SetContinueButtonInteractable(bool isInteractable) { continueButton.interactable = isInteractable; }
}
