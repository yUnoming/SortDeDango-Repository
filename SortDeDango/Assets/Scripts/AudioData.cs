using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "AudioData")]
public class AudioData : ScriptableObject
{
    [Tooltip("音源")]
    public AudioClip clip;
    [Range(0f, 1f), Tooltip("音量")]
    public float volume = 1f;
    [Tooltip("ループの有無")]
    public bool isLoop;
}