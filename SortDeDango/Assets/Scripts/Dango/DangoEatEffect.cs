using UnityEngine;

public class DangoEatEffect : Effect
{
    /// <summary>
    /// エフェクト再生    </summary>
    /// <param name="material">
    /// エフェクトの色指定用のマテリアル    </param>
    public void Play(Material material)
    {
        // パーティクルの色を変更してから再生
        ParticleSystem.MainModule main = effect.main;
        main.startColor = new ParticleSystem.MinMaxGradient(material.color);
        base.Play();
    }
}
