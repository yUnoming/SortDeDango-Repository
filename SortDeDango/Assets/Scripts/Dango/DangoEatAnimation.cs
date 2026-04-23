using System.Collections;
using UnityEngine;

public class DangoEatAnimation : AnimationBase
{
    [SerializeField, Tooltip("拡大値")]
    Vector3 scaleUp;
    [SerializeField, Tooltip("縮小値")]
    Vector3 scaleDown;
    [SerializeField, Tooltip("拡大に掛ける時間")]
    float durationUp;
    [SerializeField, Tooltip("縮小に掛ける時間")]
    float durationDown;

    /// <summary>
    /// アニメーション挙動    </summary>
    protected override IEnumerator AnimationSequence()
    {
        Transform target = gameObject.transform;
        Vector3 startScale = target.localScale;

        // 開始～拡大
        yield return ScaleOverTime(target, startScale, scaleUp, durationUp);
        // 拡大～縮小
        yield return ScaleOverTime(target, scaleUp, scaleDown, durationDown);
        // 削除
        Delete(target, startScale);

        isAnimation = false;
    }
    /// <summary>
    /// 時間を掛けてスケール    </summary>
    private IEnumerator ScaleOverTime(Transform target, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (GameplayManager.Instance.isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            target.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        target.localScale = endScale;   // 終了スケールへ
    }
    /// <summary>
    /// 疑似的に削除    </summary>
    private void Delete(Transform target, Vector3 startScale)
    {
        transform.SetParent(null, false);
        target.position = new Vector3(0, -50, 0);
        target.localScale = startScale;
    }
}
