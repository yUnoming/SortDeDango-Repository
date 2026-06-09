using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [Tooltip("設定データ")]
    private SettingsData settingsData;
    public SettingsData SettingsData => settingsData;

    private void Awake()
    {
        // 設定内容の読み込み
        settingsData = Load();
        // 読み込んだ設定内容を反映
        SetAudioVolume(AudioType.Master, settingsData.masterVolume);
        SetAudioVolume(AudioType.BGM, settingsData.bgmVolume);
        SetAudioVolume(AudioType.SE, settingsData.seVolume);
    }

    /// <summary>
    /// 音量ボリュームを設定    </summary>
    public void SetAudioVolume(AudioType type, float volume)
    {
        // 実際の音量ボリュームに設定
        AudioManager.Instance?.SetVolume(type, volume);
        // 設定データに保存
        switch (type)
        {
            case AudioType.Master: settingsData.masterVolume = volume;  break;
            case AudioType.BGM: settingsData.bgmVolume = volume;    break;
            case AudioType.SE: settingsData.seVolume = volume;  break;
        }
    }
    /// <summary>
    /// 設定内容の保存    </summary>
    public void Save()
    {
        SaveDataManager.Instance.Save<SettingsData>(settingsData);
    }
    /// <summary>
    /// 設定内容の読み込み    </summary>
    public SettingsData Load()
    {
        return SaveDataManager.Instance.Get<SettingsData>();
    }

}
