using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField]
    private SkewerController skewer1;
    [SerializeField]
    private SkewerController skewer2;

    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            skewer2.AddDango(skewer1.GetTopDango());
            skewer1.RemoveTopDango();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            skewer1.AddDango(skewer2.GetTopDango());
            skewer2.RemoveTopDango();
        }
    }
}