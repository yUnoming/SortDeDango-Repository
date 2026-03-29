using System.Collections.Generic;
using UnityEngine;

public class Dango : MonoBehaviour
{
    [SerializeField, Tooltip("団子の色")]
    private DangoColor dangoColor;
    public DangoColor DangoColor { get { return dangoColor; }}
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField, Header("DangoColorの順番通りにセット")]
    private List<Material> colorMaterials = new List<Material>();

    [Tooltip("現在所属している串")]
    private SkewerController currentSkewer;
    public SkewerController CurrentSkewer => currentSkewer;

    private void OnMouseDown()
    {
        // 団子クリックイベントを通知
        Debug.Log(gameObject.name + " clicked!");
        GameplayController.Instance.OnDangoSelected(this);
    }

    /// <summary>
    /// 団子の色をセット    </summary>
    public void SetColor(DangoColor color)
    {
        dangoColor = color;
        meshRenderer.material = colorMaterials[(int)dangoColor - 1];
    }
    /// <summary>
    /// 現在所属する串を設定    </summary>
    public void SetCurrentSkewer(SkewerController skewer)
    {
        currentSkewer = skewer;
    }
}
