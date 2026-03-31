using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("串の間隔")]
    private float skewerSpacing;
    [SerializeField, Tooltip("串の格納先")]
    Transform skewerRoot;
    [SerializeField, Tooltip("串プレハブ")]
    private GameObject skewerPrefab;
    [SerializeField, Tooltip("団子プレハブ")]
    private GameObject dangoPrefab;

    private List<SkewerController> generatedSkewers = new List<SkewerController>();

    /// <summary>
    /// ステージ生成    </summary>
    /// <param name="stageData">
    /// 生成に使用するステージデータ    </param>
    /// <returns>
    /// 生成した串リスト   </returns>
    public List<SkewerController> Generate(StageData stageData)
    {
        generatedSkewers.Clear();
        int totalSkewers = stageData.totalSkewers;
        float firstSkewerPositionX = -skewerSpacing * (totalSkewers - 1) * 0.5f; // 一番目の串の座標

        // 串の総数分のループ
        for(int skewerIndex = 0; skewerIndex < totalSkewers; skewerIndex++)
        {
            //=====
            // 串の生成・初期設定
            GameObject skewerObj = Instantiate(skewerPrefab);
            SkewerController skewer = skewerObj.GetComponent<SkewerController>();
            // 座標セット
            skewer.transform.parent = skewerRoot;
            skewer.transform.position = new Vector3(
                firstSkewerPositionX + skewerSpacing * skewerIndex,
                skewer.transform.position.y,
                skewer.transform.position.z
                );

            // 団子リストに設定された団子色分のループ
            foreach (DangoColor dangoColor in stageData.dangoLists[skewerIndex].dangoColors)
            {
                //=====
                // 団子の生成・初期設定
                GameObject dangoObj = Instantiate(dangoPrefab);
                Dango dango = dangoObj.GetComponent<Dango>();
                dango.SetColor(dangoColor);                     // 団子色の設定
                skewer.AddDango(dango.GetComponent<Dango>());   // 団子の追加
                skewer.SetTopDangoPosition(dango);              // 団子の配置
            }
            generatedSkewers.Add(skewer);
        }
        return generatedSkewers;
    }
    /// <summary>
    /// ステージ再生成    </summary>
    public void Regenerate(StageData stageData)
    {
        // 串の総数分のループ
        for (int skewerIndex = 0; skewerIndex < stageData.totalSkewers; skewerIndex++)
        {
            // 団子リストに設定された団子色分のループ
            foreach (DangoColor dangoColor in stageData.dangoLists[skewerIndex].dangoColors)
            {
                //=====
                // 団子の生成・初期設定
                GameObject dangoObj = Instantiate(dangoPrefab);
                Dango dango = dangoObj.GetComponent<Dango>();
                dango.SetColor(dangoColor);                                             // 団子色の設定
                generatedSkewers[skewerIndex].AddDango(dango.GetComponent<Dango>());    // 団子の追加
                generatedSkewers[skewerIndex].SetTopDangoPosition(dango);               // 団子の配置
            }
        }
    }
}
