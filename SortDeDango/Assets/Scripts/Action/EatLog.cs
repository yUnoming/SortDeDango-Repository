using System.Collections.Generic;
using UnityEngine;

public class EatLog : ActionLog
{
    public SkewerController skewer;
    public List<int> matchingDangoIndices = new List<int>();
    public List<Dango> eatenDangos = new List<Dango>();

    public EatLog(SkewerController skewer, List<int> matchingDangoIndices, List<Dango> eatenDangos)
    {
        this.type = ActionType.Eat;
        this.skewer = skewer;
        this.matchingDangoIndices = matchingDangoIndices;
        this.eatenDangos = eatenDangos;
    }
}
