using UnityEngine;

public class Dango : MonoBehaviour
{
    [SerializeField, Tooltip("団子の色")]
    private DangoColor color;
    public DangoColor Color { get { return color; } set { color = value; } }
}
