using UnityEngine;

public class Effect : MonoBehaviour
{
    protected ParticleSystem effect;

    protected virtual void Awake()
    {
        effect = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 一回だけエフェクト再生    </summary>
    public virtual void PlayOnce()
    {
        effect.Play();
        Destroy(effect.main.duration);
    }
    /// <summary>
    /// エフェクト再生    </summary>
    public virtual void Play()
    {
        effect.Play();
    }
    /// <summary>
    /// エフェクト削除 </summary>
    /// <param name="t">
    /// 削除までの期間 </param>
    public virtual void Destroy(float t = 0)
    {
        Destroy(effect, t);
    }

}
