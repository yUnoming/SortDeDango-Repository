using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button stageSelectButton;

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
    /// セーブデータの有無でボタンの入力受付を更新    </summary>
    /// <param name="isAvailable">
    /// セーブデータの有無   </param>
    public void UpdateButtonInteractableBySaveData(bool isAvailable)
    {
        continueButton.interactable = isAvailable;
        stageSelectButton.interactable = isAvailable;
    }
}