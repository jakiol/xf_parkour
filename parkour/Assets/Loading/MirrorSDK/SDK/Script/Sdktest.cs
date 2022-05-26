using Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sdktest : MonoBehaviour
{

    static Sdktest _instance;
    public static Sdktest Instance
    {
        get
        {
#if UNITY_EDITOR
            if (_instance == null)
            {
                _instance = new Sdktest();
            }
#endif
            return _instance;
        }
        set { _instance = value; }
    }

    public BoxLineManage boxLineManage;
    public TextAsset textAsset;

    public Text textTips;

    bool IsUpdate = false;

    private string sessionId;

    public bool IsNotifyDownloadOk
    {
        get; private set;
    }
#if UNITY_ANDROID && !UNITY_EDITOR
        = false;
#else
        = true;
#endif

    AndroidJavaObject jo_AndroidTool;
    AndroidJavaObject jo_UnityActivity;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        sessionId = Guid.NewGuid().ToString("N");

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo_UnityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");

        if (jo_UnityActivity != null) {
            Debug.Log("已获取 UnityActivity");
        }
        else {
            Debug.Log("未能获取 UnityActivity");
        }

        AndroidJavaClass AndroidToolClass = new AndroidJavaClass("com.xinfan.forest.AndroidTool");
        jo_AndroidTool = AndroidToolClass.GetStatic<AndroidJavaObject>("INSTANCE");
        if (jo_AndroidTool != null) {
            Debug.Log("已获取 jo_AndroidTool");
            jo_AndroidTool.Call("registerBodyListener");
        }
        else {
            Debug.Log("未能获取 jo_AndroidTool");
        }
#endif

        //SetSdkData(textAsset.text);
    }

    // Use this for initialization
    void Start()
    {


    }


    //public void IsUpdate_True() {
    //    IsUpdate = true;
    //}
    //public void IsUpdate_False() {
    //    IsUpdate = false;
    //}


    /// <summary>
    /// 设置 Sdk 的 Json 数据
    /// </summary>
    /// <param name="msg"></param>
    private void SetSdkDataMsg(string msg)
    {
        MirrorSdkDataModel.Instance.UpdateBodynodeMsg(msg);
    }



    public void SendAndroid()
    {
        jo_AndroidTool.Call("DebLogTest");
    }

    ///// <summary>
    ///// 测试代码 - 头框数据更新
    ///// </summary>
    //private void SendAndroidUpdateNodeData() {
    //    jo_AndroidTool.Call("UpdateNodeData");
    //}


    public void UnityClick()
    {
        float val = 10.1f;
        UnityClick(val, val);
    }

    public void UnityClick(float x, float y)
    {
        Debug.LogError("发送点击事件");
        jo_AndroidTool.Call("UnityClick", x, y);
    }

    public void TestInitSdkData()
    {
        //Debug.Log("TestInitSdkData");
#if UNITY_ANDROID && UNITY_EDITOR
        if (textAsset != null)
        {
            SetSdkDataMsg(textAsset.text);
        }
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("TestInitSdkData");
#endif

    }

    // 去查找确认是一代还是二代镜子
    public void checkOneOrMini()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if(jo_AndroidTool != null){
            jo_AndroidTool.Call("checkOneOrMini");
        }
#endif


    }

    // 监听返回的镜子宽度，以此确定是几代镜子
    public void sendClientWidth(string width)
    {
        int screenWidth = int.Parse(width);
        Debug.Log("返回的屏幕宽度数据是:: " + screenWidth);
        UserData.screenWidth = screenWidth;
    }



    /// <summary>
    /// 开启Body数据
    /// </summary>
    public void registerBodyListener()
    {
        Debug.Log("Unity - 开启Body数据");
#if UNITY_ANDROID && !UNITY_EDITOR
        if(jo_AndroidTool != null){
            MirrorSdkDataModel.Instance.IsOpenUpdateData = true;
            jo_AndroidTool.Call("registerBodyListener");
        }
#endif
    }

    /// <summary>
    /// 关闭Body数据
    /// </summary>
    public void unregisterBodyListener()
    {
        Debug.Log("Unity - 关闭Body数据");
#if UNITY_ANDROID && !UNITY_EDITOR
        if(jo_AndroidTool != null){
            MirrorSdkDataModel.Instance.IsOpenUpdateData = false;
            jo_AndroidTool.Call("unregisterBodyListener");
        }
#endif
    }


    /// <summary>
    /// 安卓Debug数据
    /// </summary>
    /// <param name="str"></param>
    public void AndroidDebugEvent(string str)
    {
        Debug.Log("安卓消息：" + str);
    }


    /// <summary>
    /// 墨镜骨骼数据
    /// </summary>
    /// <param name="msg"></param>
    public void AndroidBodynodeEvent(string msg)
    {
        Debug.Log("Unity - 魔镜骨骼消息：" );//+ msg);
        SetSdkDataMsg(msg);
    }

    public void IsOpenBoxLine()
    {
        MirrorSdkDataModel.Instance.isOpenBoxLine = !MirrorSdkDataModel.Instance.isOpenBoxLine;
    }

    public void IsRightHand()
    {
        MirrorSdkDataModel.Instance.isRightHand = !MirrorSdkDataModel.Instance.isRightHand;
    }



    public void UnityAction()
    {
        jo_AndroidTool.Call("UnityAction");
    }


    public void notifyDownload()
    {
        Debug.Log("Unity - 模型下载");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("notifyDownload");
#endif
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    public void AndroidActionEvent(string msg)
    {
        Debug.Log("魔镜动作检测：" + msg);
        MirrorSdkDataModel.Instance.UpdateActionMsg(msg);
    }

    /// <summary>
    /// 接收到姿势匹配数据
    /// </summary>
    public Action<SdkActionMatchData> GameAddPosEvent;

    public void AddPoseVal(string msg)
    {
        Debug.Log("Unity - 接收到姿势匹配数据，：" + msg);

        try
        {
            SdkActionMatchData sdkGameActionVo = JsonConvert.DeserializeObject<SdkActionMatchData>(msg);
            MirrorSdkDataModel.Instance.AddPoseVal(sdkGameActionVo);

            GameAddPosEvent?.Invoke(sdkGameActionVo);
        }
        catch
        {
            Debug.Log("接收到姿势匹配数据 错误 " + msg);
        }

        //if (float.TryParse(msg, out float val)) {
        //    MirrorSdkDataModel.Instance.AddPoseVal(val);
        //}
        //else {
        //    Debug.Log("PoseVal 错误：" + msg);
        //}
    }


    /// <summary>
    /// 通用动作事件
    /// </summary>
    public Action<SdkGameActionVo> GameActionEvent;

    public void GameAction(string msg)
    {

        Debug.Log("Unity - 接收到通用动作匹配，" + msg);

        try
        {
            SdkGameActionVo sdkGameActionVo = JsonConvert.DeserializeObject<SdkGameActionVo>(msg);
            GameActionEvent?.Invoke(sdkGameActionVo);

        }
        catch
        {
            Debug.Log("Unity - 姿势检测返回数据错误，" + msg);
        }

    }


    int tipsNum = 0;
    public E_GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            tipsNum++;
            if(textTips) textTips.text = gameState.ToString() + " - " + tipsNum;
        }
    }
    E_GameState gameState = E_GameState.None;

    public void BotSdkOrder(string msg)
    {
        Debug.Log("Unity - 接收到语音识别，" + msg);

        switch (msg)
        {
            case "StartGame"://开始游戏
                if (GameGlobals.Instance.currentGameState == "OnOpeningScene" ||
                    GameGlobals.Instance.currentGameState == "onScoreScreenEnter")
                {
                    GameGlobals.Instance.StartGame();
                }
           
                break;
            case "PauseGame"://暂停
                if (GameGlobals.Instance.currentGameState == "OnGameRunning") {
                    GameGlobals.Instance.pauseGameState.ExecuteAll();
                }

                if (GameGlobals.Instance.currentGameState == "onScoreScreenEnter" || GameGlobals.Instance.currentGameState == "OnOpeningScene")
                {
                    TTS.ins.playTutorial(7);
                }
                break;
            case "ContinueGame"://继续
                if (GameGlobals.Instance.currentGameState == "onPauseGame") {
                    GameGlobals.Instance.resumeGameState.ExecuteAll();
                }
                break;
            case "QuitGame":
                Application.Quit();
                break;
        }

        // switch (GameState)
        // {
        //     case E_GameState.Home:
        //         home(msg);
        //         break;
        //     case E_GameState.Gameing:
        //         game(msg);
        //         break;
        //     case E_GameState.GameOver:
        //         gameOver(msg);
        //         break;
        // }
    }


    public Action<float, float> GameUpdateTotalCalorieEvent;

    //public void AllCalorie(string msg) {
    //    Debug.Log("Unity - 累计卡路里 Unity - 累计卡路里 " + msg);

    //    try {
    //        SdkCaloriesDataRoot dataRoot = JsonConvert.DeserializeObject<SdkCaloriesDataRoot>(msg);

    //        float Calorie_Last = GameConfig.Instance.Calorie;
    //        int Calorie_Now = dataRoot.userData.totalCalorie;
    //        GameConfig.Instance.Calorie = Calorie_Now;
    //        ConfigDataManage.Instance.UpdatePoseList(Calorie_Now);
    //        GameUpdateTotalCalorieEvent?.Invoke(Calorie_Last, Calorie_Now);
    //    }
    //    catch (Exception ex) {
    //        Debug.LogError("Unity - 累计卡路里失败 - " + ex.Message);
    //    }
    //}


    public Action<SdkRankRoot> GameRankingListEvent;

    public void GameRankingList(string msg)
    {
        Debug.Log("Unity - 游戏排行榜 - " + msg);

        SdkRankRoot rankRoot;
        try
        {
            rankRoot = JsonConvert.DeserializeObject<SdkRankRoot>(msg);
        }
        catch (Exception ex)
        {
            rankRoot = new SdkRankRoot()
            {
                topList = new List<SdkRankTopListItem>(),
                type = "game_top_list",
                userData = new SdkRankUserData()
            };
            Debug.LogError("Unity - 获取游戏排行榜失败 - " + ex.Message);
        }
        Debug.Log("GameRankingListEvent has listener:" + GameRankingListEvent != null);
        GameRankingListEvent?.Invoke(rankRoot);
    }



    private void home(string msg)
    {
        switch (msg)
        {
            case "StartGame":
                UnityEngine.SceneManagement.SceneManager.LoadScene("dubaicity");
                break;
            case "QuitGame":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }

    }

    private void game(string msg)
    {
        switch (msg)
        {
            case "StartGame":
                UnityEngine.SceneManagement.SceneManager.LoadScene("dubaicity");
                break;
            case "QuitGame":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    private void gameOver(string msg)
    {
        switch (msg)
        {
            case "StartGame":
                UnityEngine.SceneManagement.SceneManager.LoadScene("dubaicity");
                break;
            case "QuitGame":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    public void sendNotifyDownloadOk()
    {
        IsNotifyDownloadOk = true;
        Debug.Log("Unity - 模型下载 - Ok");
    }


    void Update()
    {

        MirrorSdkDataModel.Instance.UpdateBodynodeData();
        MirrorSdkDataModel.Instance.UpdateActionData();

        if (MirrorSdkDataModel.Instance.IsOpenUpdateData && MirrorSdkDataModel.Instance.isOpenBoxLine)
        {
            boxLineManage.SetData(MirrorSdkDataModel.Instance.BodynodeData);
        }
        else
        {
            boxLineManage.SetData(null);
        }

    }

    private void LateUpdate()
    {
        MirrorSdkDataModel.Instance.ClearData();
    }



    /// <summary>
    /// 开启游戏匹配
    /// </summary>
    public void enableGameActionMatch(int id)
    {
        return;
        Debug.Log("Unity - 开启游戏匹配");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("enableGameActionMatch", id);
#endif
    }

    /// <summary>
    /// 开启游戏事件的监听
    /// </summary>
    public void registerGameActionListener()
    {
        return;
        Debug.Log("Unity - 开启游戏事件的监听");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("registerGameActionListener");
#endif
    }

    /// <summary>
    /// 开启具体动作的匹配
    /// </summary>
    /// <param name="id">动作ID</param>
    /// <param name="time">持续时间，毫秒</param>
    public void resetGameActionMatch(int id, int time)
    {
        Debug.Log(string.Format("Unity - 开启具体动作的匹配，id：{0}，time：{1}", id, time));

        //SdkActionMatchData matchData = new SdkActionMatchData();
        //matchData.actionId = id;
        //matchData.grade = 1;

        //string sdkGameActionVo = JsonConvert.SerializeObject(matchData);
        //AddPoseVal(sdkGameActionVo);

#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("resetGameActionMatch", id, time);
#endif
    }

    /// <summary>
    /// 关闭游戏事件的监听
    /// </summary>
    public void unRegisterGameActionListener()
    {
        return;
        Debug.Log("Unity - 关闭游戏事件的监听");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("unRegisterGameActionListener");
#endif
    }

    /// <summary>
    /// 关闭游戏匹配
    /// </summary>
    public void unEnableGameActionMatch(int id)
    {
        return;
        Debug.Log("Unity - 关闭游戏匹配");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("unEnableGameActionMatch", id);
#endif
    }


    /// <summary>
    /// 开启通用动作匹配
    /// </summary>
    public void OpenResetGameActionList()
    {
        return;
        Debug.Log("Unity - 开启通用动作匹配");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("OpenResetGameActionList");
#endif
    }
    /// <summary>
    /// 关闭通用动作匹配
    /// </summary>
    public void CloseResetGameActionList()
    {
        return;
        Debug.Log("Unity - 关闭通用动作匹配");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("CloseResetGameActionList");
#endif
    }

    /// <summary>
    /// 提交卡路里
    /// </summary>
    /// <param name="calorie">总卡路里</param>
    /// <param name="duration">游戏时长</param>
    /// <param name="extra">其他信息</param>
    public void updateGameData(int calorie, int duration, string extra = "")
    {
        AutoQuitApp.ins.Living();
        Debug.LogFormat("更新卡路里，session {0} , calorie {1} , duration {2} , extra {3}", sessionId, calorie, duration, extra);
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("updateGameData", sessionId, calorie, duration, extra);
#endif
    }


    //    public Action<SdkRankRoot> GameRankingListEvent;

    /// <summary>
    /// 更新单局记录
    /// </summary>
    /// <param name="calorie"></param>
    /// <param name="num"></param>
    public void updateDataAndRequestRankingList(int calorie, int num)
    {
        Debug.LogFormat("更新单局记录，calorie {0} , num {1} ", calorie, num);
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("updateDataAndRequestRankingList", calorie, num);
#endif
#if UNITY_EDITOR
        SdkRankRoot rankRoot = new SdkRankRoot()
        {
            topList = new List<SdkRankTopListItem>() {
                new SdkRankTopListItem(){
                    avatar="",
                    id="1",
                    name="1",
                    rank=1,
                    topCalorie=1
                }
            },
            type = "game_top_list",
            userData = new SdkRankUserData()
        };
        GameRankingListEvent?.Invoke(rankRoot);
#endif
    }

    //public Action<float, float> GameUpdateTotalCalorieEvent;

    /// <summary>
    /// 获取累计卡路里
    /// </summary>
    public void fetchGameTotalCalorie()
    {
        Debug.LogFormat("获取累计卡路里");
#if UNITY_ANDROID && !UNITY_EDITOR
        jo_AndroidTool.Call("fetchGameTotalCalorie");
#endif


#if UNITY_EDITOR
        GameUpdateTotalCalorieEvent?.Invoke(0, 10000);
#endif
    }
    

}

public enum E_GameState
{
    None,
    Home,
    Gameing,
    GameOver
}