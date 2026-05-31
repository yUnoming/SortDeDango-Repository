using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUIAnimation : MonoBehaviour
{
    [Header("設定可")]
    [SerializeField, Tooltip("期間")]
    private float fadeDuration = 1f;
    [SerializeField, Tooltip("フェードイン完了時のα値")]
    private float fadeInAlpha = 1f;
    [SerializeField, Tooltip("フェードアウト完了時のα値")]
    private float fadeOutAlpha = 0f;
    [Header("設定不可")]
    [SerializeField]
    private Image fadeOverlay;

    /// <summary>
    /// フェード実行    </summary>
    /// <param name="endAlpha">
    /// 最終α値    </param>
    /// <param name="onComplete">
    /// 完了時に発火する内容    </param>
    private IEnumerator FadeSequence(float endAlpha, float fadeDuration)
    {
        Color color = fadeOverlay.color;
        float timer = 0f;
        float startAlpha = color.a;
        // フェードの期間だけループ
        while (timer < fadeDuration)
        {
            // アルファ値の更新
            color.a = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadeOverlay.color = color;
            // タイム加算
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        color.a = endAlpha;
        fadeOverlay.color = color;
    }
    /// <summary>
    /// フェードイン(画面が現れる)    </summary>
    public void FadeIn()
    {
        StartCoroutine(FadeSequence(fadeInAlpha, fadeDuration));
    }
    /// <summary>
    /// フェードアウト(画面が消える)    </summary>
    /// <param name="onComplete">
    /// 完了時に発火する内容  </param>
    public void FadeOut()
    {
        StartCoroutine(FadeSequence(fadeOutAlpha, fadeDuration));
    }
}
