using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DangoList
{
    [Header("リスト0が一番下の団子")]
    public List<DangoColor> dangoColors = new List<DangoColor>();
}
