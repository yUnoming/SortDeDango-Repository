using UnityEditor.Profiling;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField]
    private SkewerController skewer1;
    [SerializeField]
    private SkewerController skewer2;

    private static GameplayController instance;
    public static GameplayController Instance { get { return instance; } }
    
    [Tooltip("現在選択中の串")]
    private SkewerController selectingSkewer;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
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

    public void OnSelectedSkewer(SkewerController skewer)
    {
        if (selectingSkewer == null)
        {
            selectingSkewer = skewer;
            return;
        }
        else
        {
            skewer.AddDango(selectingSkewer.GetTopDango());
            selectingSkewer.RemoveTopDango();
        }

        selectingSkewer = null;
    }
}