using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// シーンの種類    </summary>
public enum SceneType
{
    None,
    /// <summary>
    /// 起動処理(初期設定・初期ロードなど)    </summary>
    Boot = 0,
    /// <summary>
    /// タイトル    </summary>
    Title = 1,
    /// <summary>
    ///     </summary>
    Select = 10,
    /// <summary>
    /// ゲームのプレイ部分    </summary>
    Gameplay = 20,
    /// <summary>
    /// リザルト    </summary>
    Result = 30,
    /// <summary>
    /// 終了    </summary>
    Quit = 100,
}
/// <summary>
/// シーンの状態    </summary>
public enum SceneState
{
    /// <summary>
    /// 特になし    </summary>
    None,
    /// <summary>
    /// 初期化    </summary>
    Init,
    /// <summary>
    /// 開始    </summary>
    Start,
    /// <summary>
    /// 実行中    </summary>
    Running,
    /// <summary>
    /// 終了    </summary>
    End,
    /// <summary>
    /// 停止    </summary>
    Uninit,
}

public class SceneManagerBase : MonoBehaviour
{
    /// <summary>
    /// 現在アクティブなシーンマネージャー    </summary>
    public static SceneManagerBase activeManager;
    /// <summary>
    /// 現在のシーン状態    </summary>
    public static  SceneState currentState = SceneState.Init;
    /// <summary>
    /// 現在のシーン名    </summary>
    public static string sceneName;

    private SceneType tmpNextSceneType;
    private bool tmpIsLoadScene;
    private string tmpLoadSceneName;
    private bool isWaitSceneChange; // シーン遷移の待機状態かどうか

    private void OnEnable() { activeManager = this; }
    protected void Update()
    {
        // 現在のシーン状態によって遷移
        switch (currentState)
        {
            case SceneState.Init: StateInit(); break;
            case SceneState.Start: StateStart(); break;
            case SceneState.Running: StateRunning(); break;
            case SceneState.End: StateEnd(); break;
            case SceneState.Uninit: StateUninit(); break;
        }
    }

    /// <summary>
    /// シーンマネージャーの切り替え    </summary>
    /// <param name="nextSceneType">
    /// 切り替えるシーンマネージャーに対応したシーンの種類    </param>
    private void ChangeSceneManager(SceneType nextSceneType)
    {
        // 現在のシーンマネージャーの初期化
        enabled = false;
        currentState = SceneState.Init;
        // 切り替え
        switch (nextSceneType)
        {
            case SceneType.Title: GetComponent<SceneManagerBase<TitleManager>>().enabled = true; break;
            case SceneType.Gameplay: GetComponent<SceneManagerBase<GameplayManager>>().enabled = true; break;
            case SceneType.Result: GetComponent<SceneManagerBase<ResultManager>>().enabled = true; break;
        }
    }
    /// <summary>
    /// シーンをロード    </summary>
    /// <param name="loadSceneName">
    /// ロードするシーン名    </param>
    private void LoadScene(string loadSceneName)
    {
        sceneName = loadSceneName;
        SceneManager.LoadScene(loadSceneName);
    }
    /// <summary>
    /// シーン名を取得    </summary>
    /// <param name="sceneType">
    /// 取得するシーンの種類    </param>
    private string GetSceneName(SceneType sceneType)
    {
        // 各シーンタイプに適切なシーン名を設定
        switch(sceneType)
        {
            case SceneType.Title: return "TitleScene";
            case SceneType.Select: return "StageSelectScene";
            case SceneType.Gameplay: return "GameplayScene";
            case SceneType.Result: return "ResultScene";
        }
        return null;
    }
    /// <summary>
    /// アプリケーション終了    </summary>
    private void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        // ゲームを終了
        Application.Quit();
#endif
    }

    protected virtual void StateInit() { currentState++; }
    protected virtual void StateStart() { currentState++; }
    protected virtual void StateRunning() { currentState++; }
    protected virtual void StateEnd() { currentState++; }
    /// <summary>
    /// オーバーライドした際、必ず最後にbase.StateUninit()を呼び出すこと！！    </summary>
    protected virtual void StateUninit()
    {
        // シーン遷移待ち状態であれば、シーン遷移
        if (isWaitSceneChange)
            ChangeScene(tmpNextSceneType, tmpIsLoadScene, tmpLoadSceneName);
    }

    /// <summary>
    /// シーン遷移    </summary>
    /// <param name="nextSceneType">
    /// 遷移先シーンの種類    </param>
    /// <param name="isLoadScene">
    /// シーンをロードするかどうか    </param>
    /// <param name="loadSceneName">
    /// ロードするシーン名(入力が無ければ、シーンの種類名 + Scene)    </param>
    public void ChangeScene(SceneType nextSceneType, bool isLoadScene = true, string loadSceneName = "")
    {
        // 値のコピー
        tmpNextSceneType = nextSceneType;
        tmpIsLoadScene = isLoadScene;
        tmpLoadSceneName = loadSceneName;
        // 後処理状態であれば、シーン遷移
        if (currentState == SceneState.Uninit)
        {
            if (tmpNextSceneType == SceneType.Quit) QuitApplication();
            else
            {
                // マネージャーの切り替え
                ChangeSceneManager(tmpNextSceneType);
                // 対象シーンのロード
                if (tmpIsLoadScene)
                {
                    if (tmpLoadSceneName != "") LoadScene(tmpLoadSceneName);
                    else LoadScene(GetSceneName(tmpNextSceneType));
                }
            }
            isWaitSceneChange = false;
        }
        // 後処理状態でなければ、待機状態にしてシーン状態遷移
        else
        {
            isWaitSceneChange = true;
            currentState++;
        }
    }
}
/// <summary>
/// 継承時の記述方式 TestSceneManager : SceneManagerBase<TestSceneManager>    </summary>
public abstract class SceneManagerBase<T> : SceneManagerBase where T : SceneManagerBase<T>
{
    /// <summary>
    /// インスタンス    </summary>
    private static T instance;
    public static T Instance
    { 
        get
        {
            // インスタンスが無ければ、グローバルマネージャーをシーン内に生成
            if (instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("GlobalManagersVariant");
                GameObject obj = Instantiate(prefab);
                instance = obj.GetComponentInChildren<T>();
            }
            return instance;
        }
    }

    protected void Awake()
    {
        // 重複確認
        if (instance != null && instance != this)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        // インスタンス化
        else if (instance == null)
        {
            instance = this as T;
            sceneName = SceneManager.GetActiveScene().name;
        }
    }
    protected virtual void Start(){}
}