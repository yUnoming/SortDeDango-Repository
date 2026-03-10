using UnityEngine;

public class Dango : MonoBehaviour
{
    [SerializeField, Tooltip("団子の色")]
    private DangoColor dangoColor;

    public void SetColor(DangoColor color){ dangoColor = color; }
}
