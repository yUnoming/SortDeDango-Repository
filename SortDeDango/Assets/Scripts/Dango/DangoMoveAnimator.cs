using System.Collections;
using UnityEngine;

public class DangoMoveAnimator : MonoBehaviour
{
    [SerializeField, Tooltip("持ち上げる高さ")]
    private float liftHeight = 1f;
    [SerializeField, Tooltip("持ち上げに掛かる時間")]
    private float liftDuration = 0.2f;
    [SerializeField, Tooltip("移動に掛かる時間")]
    private float moveDuration = 0.2f;
    [SerializeField, Tooltip("落とすのにかかる時間")]
    private float dropDuration = 0.2f;

    [Tooltip("アニメーション中かどうか")]
    private bool isAnimation;
    public bool IsAnimation {  get { return isAnimation; } }

    /// <summary>
    /// アニメーション挙動    </summary>
    /// <param name="myTransform">
    /// 自身のTransform    </param>
    /// <param name="endPos">
    /// アニメーションの最終到達地点    </param>
    private IEnumerator AnimationSequence(Transform myTransform, Vector3 endPos)
    {
        Vector3 startPos = myTransform.position;
        Vector3 liftedStartPos = startPos + Vector3.up * liftHeight;
        Vector3 liftedEndPos = endPos + Vector3.up * liftHeight;

        // 持ち上げ
        yield return MoveOverTime(myTransform, startPos, liftedStartPos, liftDuration);
        // 移動
        yield return MoveOverTime(myTransform, liftedStartPos, liftedEndPos, moveDuration);
        // 落とす
        yield return MoveOverTime(myTransform, liftedEndPos, endPos, dropDuration);

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
        while(elapsedTime < duration)
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

    /// <summary>
    /// アニメーション再生    </summary>
    /// <param name="myTransform">
    /// 自身のTransform    </param>
    /// <param name="endPos">
    /// アニメーションの最終到達地点    </param>
    public IEnumerator PlayAnimation(Transform myTransform, Vector3 endPos)
    {
        if(!isAnimation)
        {
            isAnimation = true;
            yield return StartCoroutine(AnimationSequence(myTransform, endPos));
        }
    }

}
