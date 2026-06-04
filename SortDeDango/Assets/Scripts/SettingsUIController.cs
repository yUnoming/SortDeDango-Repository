using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private SettingsController controller;
    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider bgmVolumeSlider;
    [SerializeField]
    private Slider seVolumeSlider;

    private void Awake()
    {
        masterVolumeSlider.onValueChanged.AddListener((value) => controller.SetAudioVolume(AudioType.Master, value));
        bgmVolumeSlider.onValueChanged.AddListener((value) => controller.SetAudioVolume(AudioType.BGM, value));
        seVolumeSlider.onValueChanged.AddListener((value) => controller.SetAudioVolume(AudioType.SE, value));
    }
    /// <summary>
    /// UI更新    </summary>
    private void UpdateUI()
    {
        masterVolumeSlider.value = controller.SettingsData.masterVolume;
        bgmVolumeSlider.value = controller.SettingsData.bgmVolume;
        seVolumeSlider.value = controller.SettingsData.seVolume;
    }

    /// <summary>
    /// UI表示    </summary>
    public void Show()
    {
        UpdateUI();
        content.SetActive(true);
    }
    /// <summary>
    /// UI非表示    </summary>
    public void Hide() { content.SetActive(false); }
}
