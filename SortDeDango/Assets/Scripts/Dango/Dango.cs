using System.Collections.Generic;
using UnityEngine;

public class Dango : MonoBehaviour
{
    [SerializeField, Tooltip("団子の色")]
    private DangoColor dangoColor;
    public DangoColor DangoColor { get { return dangoColor; } }
    [SerializeField, Header("DangoColorの順番通りにセット")]
    private List<Material> colorMaterials = new List<Material>();
    [SerializeField]
    private MeshRenderer modelMeshRenderer;
    [SerializeField]
    private MeshRenderer outlineMeshRenderer;
    [SerializeField]
    private GameObject dangoEatEffect;

    [Tooltip("現在所属している串")]
    private SkewerController currentSkewer;
    public SkewerController CurrentSkewer => currentSkewer;

    /// <summary>
    /// 団子の色をセット    </summary>
    public void SetColor(DangoColor color)
    {
        dangoColor = color;
        modelMeshRenderer.material = colorMaterials[(int)dangoColor - 1];
    }
    /// <summary>
    /// 現在所属する串を設定    </summary>
    public void SetCurrentSkewer(SkewerController skewer)
    {
        currentSkewer = skewer;
    }
    /// <summary>
    /// アウトラインの表示切り替え    </summary>
    /// <param name="isVisual">
    /// 表示するかどうか    </param>
    public void SetOutlineVisible(bool isVisible)
    {
        outlineMeshRenderer.enabled = isVisible;
    }

    /// <summary>
    /// 団子を食べた時のエフェクト再生    </summary>
    public void PlayEatEffect()
    {
        GameObject obj = Instantiate(dangoEatEffect, this.transform.position, Quaternion.identity);
        obj.GetComponent<DangoEatEffect>().Play(colorMaterials[(int)dangoColor - 1]);
    }
}
