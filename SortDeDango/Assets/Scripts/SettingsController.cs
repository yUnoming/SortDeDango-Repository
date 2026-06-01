using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider bgmVolumeSlider;
    [SerializeField]
    private Slider seVolumeSlider;

    private void Awake()
    {
        masterVolumeSlider.onValueChanged.AddListener((value) => SetAudioVolume(AudioType.Master, value));
        bgmVolumeSlider.onValueChanged.AddListener((value) => SetAudioVolume(AudioType.BGM, value));
        seVolumeSlider.onValueChanged.AddListener((value) => SetAudioVolume(AudioType.SE, value));
    }
    
    /// <summary>
    /// 音量ボリュームを設定    </summary>
    public void SetAudioVolume(AudioType type, float volume)
    {
        AudioManager.Instance.SetVolume(type, volume);
    }
}
