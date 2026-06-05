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
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private GameObject menuButtonGroup;
    [SerializeField]
    private float menuButtonGroupDown;

    [Tooltip("NewGameボタン押下時のイベント")]
    public event Action onNewGameClicked;
    [Tooltip("Continueボタン押下時のイベント")]
    public event Action onContinueClicked;

    private void Awake()
    {
        newGameButton.onClick.AddListener(() => onNewGameClicked?.Invoke());
        continueButton.onClick.AddListener(() => onContinueClicked?.Invoke());

#if !UNITY_STANDALONE
        // PC向けビルドでなければ、終了ボタンを非表示
        quitButton.gameObject.SetActive(false);
        menuButtonGroup.transform.position = menuButtonGroup.transform.position + Vector3.down * menuButtonGroupDown;
#endif
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