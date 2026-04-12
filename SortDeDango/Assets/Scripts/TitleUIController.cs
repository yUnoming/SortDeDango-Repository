using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [SerializeField]
    private Button newGameButton;

    [Tooltip("NewGameボタン押下時のイベント")]
    public event Action onNewGameClicked;

    private void Awake()
    {
        newGameButton.onClick.AddListener(() => onNewGameClicked?.Invoke());
    }
}
