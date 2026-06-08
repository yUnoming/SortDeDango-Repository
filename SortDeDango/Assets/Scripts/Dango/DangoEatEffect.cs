using UnityEngine;

public class DangoEatEffect : EffectBase
{
    /// <summary>
    /// エフェクト再生    </summary>
    /// <param name="material">
    /// エフェクトの色指定用のマテリアル    </param>
    public void PlayOnce(Material material)
    {
        // パーティクルの色を変更してから再生
        ParticleSystem.MainModule main = effect.main;
        main.startColor = new ParticleSystem.MinMaxGradient(material.color);
        base.PlayOnce();
    }
}
