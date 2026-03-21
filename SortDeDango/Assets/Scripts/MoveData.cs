using UnityEngine;

public class MoveData
{
    [Tooltip("移動元の串")]
    public SkewerController from;
    [Tooltip("移動先の串")]
    public SkewerController to;
    [Tooltip("移動させた団子")]
    public Dango dango;

    public MoveData(SkewerController from, SkewerController to, Dango dango)
    {
        this.from = from;
        this.to = to;
        this.dango = dango;
    }
}
