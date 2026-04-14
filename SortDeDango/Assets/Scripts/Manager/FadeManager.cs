using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [Header("設定可")]
    [SerializeField]
    [Tooltip("デフォルトのフェード期間")]
    private float defaultFadeDuration = 1f;
    [Header("設定不可")]
    [SerializeField]
    private Image fadeOverlay;

    [Tooltip("インスタンス")]
    private static FadeManager instance;
    public static FadeManager Instance => instance;
    [Tooltip("フェード実行中かどうか")]
    private bool isFadeRunning;

    private void Awake()
    {
        // 重複確認
        if (instance != null && instance != this)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        // インスタンス化
        else if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// フェード実行    </summary>
    /// <param name="endAlpha">
    /// 最終α値    </param>
    /// <param name="onComplete">
    /// 完了時に発火する内容    </param>
    private IEnumerator FadeRunning(float endAlpha, float fadeDuration, Action onComplete = null, bool isFadeOutAfter = false)
    {
        isFadeRunning = true;

        Color color = fadeOverlay.color;
        float timer = 0f;
        float startAlpha = color.a;
        // フェードの期間だけループ
        while(timer < fadeDuration)
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

        // 設定されているアクション実行
        onComplete?.Invoke();
        yield return new WaitForSecondsRealtime(0.01f);
        if (isFadeOutAfter) StartCoroutine(FadeRunning(0f, fadeDuration));

        isFadeRunning = false;
    }
    /// <summary>
    /// フェードイン(画面が現れる)    </summary>
    /// <param name="onComplete">
    /// 完了時に発火する内容  </param>
    public void FadeIn(float fadeDuration = -1f, Action onComplete = null, bool isFadeOutAfter = false)
    {
        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;
        if (!isFadeRunning) StartCoroutine(FadeRunning(0f, fadeDuration, onComplete, isFadeOutAfter));
    }
    /// <summary>
    /// フェードアウト(画面が消える)    </summary>
    /// <param name="onComplete">
    /// 完了時に発火する内容  </param>
    public void FadeOut(float fadeDuration = -1f, Action onComplete = null, bool isFadeInAfter = false)
    {
        if (fadeDuration < 0f) fadeDuration = defaultFadeDuration;
        StartCoroutine(FadeRunning(1f, fadeDuration, onComplete, isFadeInAfter));
    }
}
