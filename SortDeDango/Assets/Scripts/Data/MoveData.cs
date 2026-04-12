using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
    [Tooltip("移動元の串")]
    public SkewerController from;
    [Tooltip("移動先の串")]
    public SkewerController to;
    [Tooltip("移動させた団子")]
    public List<Dango> dangoList;

    public MoveData(SkewerController from, SkewerController to)
    {
        this.from = from;
        this.to = to;
        this.dangoList = new List<Dango>();
    }
}
