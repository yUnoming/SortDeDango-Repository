using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    Master,
    BGM,
    SE,
}
public class AudioManager : MonoBehaviour
{
    [Header("設定可能")]
    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)]
    private float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)]
    private float seVolume = 1f;

    [Header("設定不可")]
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private List<AudioSource> d2Sources;
    [SerializeField]
    private List<AudioSource> d3Sources;

    /// <summary>
    /// インスタンス    </summary>
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            // インスタンスが無ければ、グローバルマネージャーをシーン内に生成
            if (instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("GlobalManagersVariant");
                GameObject obj = Instantiate(prefab);
                instance = obj.GetComponentInChildren<AudioManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 重複確認
        if (instance != null && instance != this)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        // インスタンス化
        else if (instance == null) instance = this;
    }
    private void Start()
    {
        // 音量セット
        SetVolume(AudioType.Master, masterVolume);
        SetVolume(AudioType.BGM, bgmVolume);
        SetVolume(AudioType.SE, seVolume);
    }

    /// <summary>
    /// 利用可能な2Dソースを取得    </summary>
    /// <returns></returns>
    private AudioSource GetAvailable2DSource()
    {
        foreach(AudioSource source in d2Sources)
        {
            if(!source.isPlaying) return source;
        }
        return null;
    }
    /// <summary>
    /// 利用可能な3Dソースを取得    </summary>
    /// <returns></returns>
    private AudioSource GetAvailable3DSource()
    {
        foreach (AudioSource source in d3Sources)
        {
            if (!source.isPlaying) return source;
        }
        return null;
    }

    /// <summary>
    /// BGMを再生    </summary>
    /// <param name="data">
    /// 再生する音声データ    </param>
    public void PlayBGM(AudioData data)
    {
        if(bgmSource.clip != data.clip)
        {
            bgmSource.clip = data.clip;
            bgmSource.volume = data.volume;
            bgmSource.loop = data.isLoop;
            bgmSource.Play();
        }
    }
    /// <summary>
    /// BGMを停止    </summary>
    public void StopBGM(){bgmSource.Stop(); bgmSource.clip = null; }
    /// <summary>
    /// SEを再生(2Dがメイン)    </summary>
    /// <param name="data">
    /// 再生する音声データ    </param>
    public void PlaySE(AudioData data)
    {
        // 再生可能であれば、再生
        AudioSource source = GetAvailable2DSource();
        if(source != null)
        {
            source.clip = data.clip;
            source.volume = data.volume;
            source.loop = data.isLoop;
            source.Play();
        }
        else Debug.Log("2D用SEソースが足りていません。");
    }
    /// <summary>
    /// SEを再生(3Dがメイン)    </summary>
    /// <param name="data">
    /// 再生する音声データ    </param>
    /// <param name="position">
    /// 再生する位置    </param>
    public void PlaySE(AudioData data, Vector3 position)
    {
        // 再生可能であれば、再生
        AudioSource source = GetAvailable3DSource();
        if (source != null)
        {
            source.gameObject.transform.position = position;
            source.clip = data.clip;
            source.volume = data.volume;
            source.loop = data.isLoop;
            source.Play();
        }
        else Debug.Log("3D用SEソースが足りていません。");
    }

    /// <summary>
    /// 音量ボリュームを設定    </summary>
    /// <param name="type">
    /// 設定する音声タイプ    </param>
    /// <param name="volume">
    /// 設定するボリューム値    </param>
    public void SetVolume(AudioType type, float volume)
    {
        // デシベル変換後、AudioMixerにセット
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(type.ToString(), dB);
    }
}
