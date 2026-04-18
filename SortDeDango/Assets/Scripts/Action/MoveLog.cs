using System.Collections.Generic;
using UnityEngine;

public class MoveLog : ActionLog
{
    [Tooltip("移動元の串")]
    public SkewerController from;
    [Tooltip("移動先の串")]
    public SkewerController to;
    [Tooltip("移動させた団子")]
    public List<Dango> movedDangos = new List<Dango>();

    public MoveLog(SkewerController from, SkewerController to, List<Dango> movedDangos)
    {
        this.type = ActionType.Move;
        this.from = from;
        this.to = to;
        this.movedDangos = movedDangos;
    }
}
