using UnityEngine;

public class SkewerController : MonoBehaviour
{
    [Tooltip("串の現在の状態")]
    private SkewerState currentState;

    /// <summary>
    /// 状態を変更    </summary>
    /// <param name="nextState">
    /// 変更先の状態  </param>
    public void ChangeState(SkewerState nextState){ currentState = nextState; }
}
