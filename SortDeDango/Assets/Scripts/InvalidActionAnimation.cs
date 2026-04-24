using System.Collections;
using UnityEngine;

public class InvalidActionAnimation : AnimationBase
{
    [SerializeField, Tooltip("一番右の座標")]
    private float farRight;
    [SerializeField, Tooltip("一番左の座標")]
    private float farLeft;
    [SerializeField, Tooltip("端から端への移動に掛ける時間")]
    private float duration;

    /// <summary>
    /// アニメーション挙動    </summary>
    protected override IEnumerator AnimationSequence()
    {
        Vector3 startPos = transform.position;
        Vector3 farRightPos = startPos + Vector3.right * farRight;
        Vector3 farLeftPos = startPos - Vector3.right * farLeft;

        // 開始位置～右端
        yield return MoveOverTime(transform, startPos, farRightPos, duration / 2);
        // 右端～左端
        yield return MoveOverTime(transform, farRightPos, farLeftPos, duration);
        // 左端～開始位置
        yield return MoveOverTime(transform, farLeftPos, startPos, duration / 2);

        isAnimation = false;
    }
    /// <summary>
    /// 一定時間を掛けて移動    </summary>
    /// <param name="target">
    /// 移動対象    </param>
    /// <param name="from">
    /// 開始座標    </param>
    /// <param name="to">
    /// 終了座標    </param>
    /// <param name="duration">
    /// 移動に掛かる時間    </param>
    private IEnumerator MoveOverTime(Transform target, Vector3 from, Vector3 to, float duration)
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
            target.position = Vector3.Lerp(from, to, t);

            yield return null;
        }

        target.position = to;   // 終了座標へ
    }
}
