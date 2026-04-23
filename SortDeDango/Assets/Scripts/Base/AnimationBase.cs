using System.Collections;
using UnityEngine;

public class AnimationBase : MonoBehaviour
{
    [Tooltip("アニメーション中かどうか")]
    public bool isAnimation { get; protected set; }

    /// <summary>
    /// アニメーション挙動    </summary>
    protected virtual IEnumerator AnimationSequence()
    {
        yield return null;
        isAnimation = false;
    }
    /// <summary>
    /// アニメーション再生    </summary>
    public virtual void Play()
    {
        if (!isAnimation)
        {
            isAnimation = true;
            StartCoroutine(AnimationSequence());
        }
    }
    /// <summary>
    /// アニメーション再生    </summary>
    public virtual IEnumerator PlayCoroutine()
    {
        if (!isAnimation)
        {
            isAnimation = true;
            yield return AnimationSequence();
        }
    }
}
