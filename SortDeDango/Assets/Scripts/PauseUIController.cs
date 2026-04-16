using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private Button continueButton;

    private void Awake()
    {
        PauseButton pauseButton = FindAnyObjectByType<PauseButton>();
        pauseButton.onClick.AddListener(OnPause);
        continueButton.onClick.AddListener(OnPause);
    }

    /// <summary>
    /// ポーズする    </summary>
    private void OnPause()
    {
        content.SetActive(!content.activeSelf);
        GameplayManager.Instance.TogglePause();
    }
}
