using System.IO;
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

    private const string SettingsFilePath = "setting.json";
    private SettingsData settingsData;

    private void Awake()
    {
        Setup();

        masterVolumeSlider.onValueChanged.AddListener((value) => SetAudioVolume(AudioType.Master, value));
        bgmVolumeSlider.onValueChanged.AddListener((value) => SetAudioVolume(AudioType.BGM, value));
        seVolumeSlider.onValueChanged.AddListener((value) => SetAudioVolume(AudioType.SE, value));
    }
    
    private void Setup()
    {
        // 設定内容の読み込み
        settingsData = Load();
        // 読み込んだ設定内容を反映
        masterVolumeSlider.value = settingsData.masterVolume;
        bgmVolumeSlider.value = settingsData.bgmVolume;
        seVolumeSlider.value = settingsData.seVolume;
    }

    /// <summary>
    /// 音量ボリュームを設定    </summary>
    public void SetAudioVolume(AudioType type, float volume)
    {
        AudioManager.Instance.SetVolume(type, volume);
    }
    /// <summary>
    /// 設定内容の保存    </summary>
    public void Save()
    {
        settingsData.masterVolume = masterVolumeSlider.value;
        settingsData.bgmVolume = bgmVolumeSlider.value;
        settingsData.seVolume = seVolumeSlider.value;

        string json = JsonUtility.ToJson(settingsData);
        string path = Path.Combine(Application.persistentDataPath, SettingsFilePath);
        File.WriteAllText(path, json);
    }
    /// <summary>
    /// 設定内容のロード    </summary>
    public SettingsData Load()
    {
        SettingsData loadedData = new SettingsData();
        string path = Path.Combine(Application.persistentDataPath, SettingsFilePath);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            loadedData = JsonUtility.FromJson<SettingsData>(json);
        }

        return loadedData;
    }

}
