using UnityEditor.Overlays;
using UnityEngine;

public class SkewerSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("生成間隔")]
    private float spawnInterval;
    [SerializeField, Tooltip("生成テーブル")]
    private SkewerSpawnTable spawnTable;
    [SerializeField, Tooltip("串の格納先")]
    Transform skewerRoot;
    [SerializeField, Tooltip("串プレハブ")]
    private GameObject skewerPrefab;
    [SerializeField, Tooltip("団子プレハブ")]
    private GameObject dangoPrefab;

    [Tooltip("生成までの時間計測")]
    private float spawnTimer;

    private void Update()
    {
        if(GameplayManager.currentState == SceneState.Running)
        {
            // 一定時間経過で生成
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                Spawn();
                spawnTimer -= spawnInterval;
            }
        }
    }

    /// <summary>
    /// 串を生成    </summary>
    private void Spawn()
    {
        // 生成番号を乱数取得
        int randomIndex = Random.Range(0, spawnTable.entries.Count);

        // 串の生成・初期設定
        GameObject skewerObj = Instantiate(skewerPrefab);
        SkewerController skewer = skewerObj.GetComponent<SkewerController>();
        skewer.transform.parent = skewerRoot;   // 親の設定

        // 生成するの団子色分のループ
        foreach (DangoColor dangoColor in spawnTable.entries[randomIndex].dangoColors)
        {
            // 団子の生成・初期設定
            GameObject dangoObj = Instantiate(dangoPrefab);
            Dango dango = dangoObj.GetComponent<Dango>();
            dango.SetColor(dangoColor);                     // 団子色の設定
            skewer.AddDango(dango.GetComponent<Dango>());   // 団子の追加
            skewer.SetTopDangoPosition(dango);              // 団子の配置
        }

        // 生成した串を GameplayManager に追加
        GameplayManager.Instance.AddSkewer(skewer);
    }
}
