using System.Collections.Generic;
using UnityEngine;

public class Dango : MonoBehaviour
{
    [SerializeField, Tooltip("団子の色")]
    private DangoColor color;
    public DangoColor Color { get { return color; }}
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField, Header("DangoColorの順番通りにセット")]
    private List<Material> colorMaterials = new List<Material>();

    /// <summary>
    /// 団子の色をセット    </summary>
    public void SetColor(DangoColor newColor)
    {
        color = newColor;
        meshRenderer.material = colorMaterials[(int)color - 1];
    }
}
