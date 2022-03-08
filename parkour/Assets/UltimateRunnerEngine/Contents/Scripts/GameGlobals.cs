using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class GameGlobals : MonoBehaviour
{


    // Singleton
    private static GameGlobals instance = null;
    public static GameGlobals Instance
    {
        get
        {
            return instance;
        }
    }


    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject playerController;
    [HideInInspector]
    public Controller controller;
    [HideInInspector]
    public CameraController cameraController;
    [HideInInspector]
    public Camera mainCamera;
    [HideInInspector]
    public TrackGenerator trackGenerator;
    [HideInInspector]
    public Achievements achievements;
    [HideInInspector]
    public PowerupController powerupController;
    [HideInInspector]
    public AudioController audioController;
    [HideInInspector]
    public TouchController touchController;

    // Updated by GameState Component
    string _currentGameState;

    const int ActionTime = 3000;
    const int ActionId = 1005;
    [HideInInspector]
    public string currentGameState
    {
        get { return _currentGameState; }
        set {
            if (value != _currentGameState)
            {
                _currentGameState = value;
                if (_currentGameState == "onScoreScreenEnter") {
                    HideCurCalorie();
                }
                if (_currentGameState == "OnGameRunning" && PlayerPrefs.GetInt("tutorial", 0) == 1) {
                    ShowCurCalorie();
                }
                Debug.Log("currentGameState:" + value);
            }
        }
    }

    public void ShowCurCalorie() {
        curCalorie.gameObject.SetActive(true);
        curCalorie.PlayForward();
    }

    private void HideCurCalorie() {
        curCalorie.PlayReverse();
    }

    public float bendingX;
    public float bendingY;

    //开始游戏时间
    //public DateTime starTime;


    private TweenFOV cameraFovEffetc;
    public GameObject highScorePanel;
    public GameObject messageBoxPanel;
    public GameObject tutorialPanel;
    public GameObject RankPanelList;
    public GameObject RankMe;
    public GameObject rankLost;

    [HideInInspector]
    public bool gameAutoStart;
    public GameState openingSceneState;
    public GameState escapeSceneState;
    public GameState pauseGameState;
    public GameState resumeGameState;
    public GameState villainCheerState;
    public GameState highScoreState;

    public GameState onScoreScreenExit;
    

    public TweenPosition curCalorie;

    // Auto starts game when player hits replay button at the score scene
    public void checkAutoStart()
    {
        if (gameAutoStart == false)
        {
            if (openingSceneState != null)
            {
                openingSceneState.ExecuteAll();
            }
        }
        else
        {
            if (escapeSceneState != null)
            {
                escapeSceneState.ExecuteAll();
            }
            gameAutoStart = false;
        }
    }

    public void setGameAutoStart()
    {
        gameAutoStart = true;
    }

    private void Awake()
    {


        // Singleton
        instance = this;


        // Begin
        Application.targetFrameRate = 60;

        highScorePanel.SetActive(false);

        // First Start Defaulu Settings
        if (PlayerPrefs.GetInt("firstStart", 0) == 0)
        {
            PlayerPrefs.SetInt("firstStart", 1);
            PlayerPrefs.SetInt("rate_complete", 0);
            PlayerPrefs.SetInt("rate_peak", 16);

            PlayerPrefs.SetInt("audio", 1);
            PlayerPrefs.SetInt("highscore", 0);
            PlayerPrefs.SetInt("ads_unitypeak", 5);
            PlayerPrefs.SetInt("ads_chartboostpeak", 0);
        }


        // Assing Bending Values
        Shader.SetGlobalFloat("_V_CW_X_Bend_Size", bendingX);
        Shader.SetGlobalFloat("_V_CW_Y_Bend_Size", bendingY);
        Shader.SetGlobalFloat("_V_CW_Z_Bend_Size", 0);
        Shader.SetGlobalFloat("_V_CW_Z_Bend_Bias", 0);
        Shader.SetGlobalFloat("_V_CW_Camera_Bend_Offset", 0);



        player = GameObject.Find("Player");
        playerController = GameObject.Find("playerController");

        controller = GetComponent<Controller>();
        audioController = GetComponent<AudioController>();

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraFovEffetc = GameObject.Find("Main Camera").GetComponent<TweenFOV>();

        cameraController = GetComponent<CameraController>();

        trackGenerator = GetComponent<TrackGenerator>();
        achievements = GetComponent<Achievements>();
        powerupController = GetComponent<PowerupController>();
        touchController = GetComponent<TouchController>();


        Sdktest.Instance.GameRankingListEvent = OnGameRankingHandle;

    }

    #region 游戏控制匹配,modify

    bool GameActionCountDown = false;
    string currGameActionId = "";
    //int currGameActionTime = 0;
    Action currGameActionHandle = null;
    const float dt_time = 1 / 2f;
    const float dt = 1f * dt_time;
    Dictionary<string, GameActionTimeBarVo> pairs = new Dictionary<string, GameActionTimeBarVo>();
    private void OnGameActionMatchStop()
    {
        GameActionCountDown = false;
        currGameActionId = "";
        currGameActionHandle = null;
        pairs.Clear();
    }
    public class GameActionTimeBarVo
    {
        public float time_now = 0;
        public float time_last = 0;
    }
    private void OnGameActionMatchFinsh()
    {
        currGameActionHandle?.Invoke();
        OnGameActionMatchStop();
    }

    void UpdateGameAddPos()
    {
        if (!GameActionCountDown) return;

        if (!pairs.ContainsKey(currGameActionId)) return;
        //Debug.Log("--UpdateGameAddPos");
        float deTime_jia = Time.deltaTime * 0.8f * dt_time;
        float deTime_jian = deTime_jia * 2;
        if (pairs[currGameActionId].time_now > pairs[currGameActionId].time_last)
        {
            pairs[currGameActionId].time_last += deTime_jia;

            if (pairs[currGameActionId].time_last > pairs[currGameActionId].time_now)
            {
                pairs[currGameActionId].time_last = pairs[currGameActionId].time_now;
            }
        }
        else
        {
            pairs[currGameActionId].time_last -= deTime_jian;
            pairs[currGameActionId].time_now -= deTime_jian;

            pairs[currGameActionId].time_last = Mathf.Max(0, pairs[currGameActionId].time_last);
            pairs[currGameActionId].time_now = Mathf.Max(0, pairs[currGameActionId].time_now);
        }
        //float wuJinHeight = Mathf.Clamp(pairs[currGameActionId].time_last * Img_WuJinBar_Size.y, 0, Img_WuJinBar_Size.y);
        //Img_WuJinBar.sizeDelta = new Vector2(Img_WuJinBar_Size.x, wuJinHeight);

        if (pairs[currGameActionId].time_last >= 1)
        {
            OnGameActionMatchFinsh();
        }
    }


    private void OnGameRankingHandle(SdkRankRoot data) {
        if (data == null || data.topList == null) {
            RankPanelList.SetActive(false);
            RankMe.SetActive(false);
            rankLost.SetActive(true);
            return;
        } else {
            RankPanelList.SetActive(true);
            RankMe.SetActive(true);
            rankLost.SetActive(false);
        }
        //显示数据
        for (int i = 0; i < RankPanelList.transform.childCount; i++)
        {
            var item = RankPanelList.transform.GetChild(i);
            if (data.topList != null && data.topList.Count > i)
            {
                item.gameObject.SetActive(true);
                SetRankData(item.gameObject, data.topList[i]);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
        //我的数据
        SetRankData(RankMe, data.userData, true);
    }

    private void SetRankData(GameObject item, SdkRankTopListItem data, bool isme = false)
    {
        var lbRank = item.transform.Find("lbRank").GetComponent<Text>();
        var lbName = item.transform.Find("lbName").GetComponent<Text>();
        var lbScore = item.transform.Find("lbScore").GetComponent<Text>();
        if(isme && data.rank > 500){
            lbRank.text = "未上榜";
        }else{
            lbRank.text = data.rank.ToString();
        }
        lbName.Ellipsis(data.name);
        lbScore.text = (data.topCalorie * 0.001f).ToString("F1") + "千卡";
    }

    private void ResetRankPannel()
    {
        RankPanelList.SetActive(false);
        RankMe.SetActive(false);
        rankLost.SetActive(true);
    }
    #endregion

    private void FixedUpdate()
    {
        //handleBendDirection();
    }

    private int backButtonDelay;

    private void Update()
    {
        if (backButtonDelay <= 0)
        {
            if (Input.GetKey(KeyCode.P))
            {
                backButtonDelay = 50;
                Sdktest.Instance.BotSdkOrder("PauseGame");
            }
            if (Input.GetKey(KeyCode.C))
            {
                backButtonDelay = 50;
                Sdktest.Instance.BotSdkOrder("ContinueGame");
            }
            if (Input.GetKey(KeyCode.S))
            {
                backButtonDelay = 50;
                Sdktest.Instance.BotSdkOrder("StartGame");
            }
        }

        if (backButtonDelay > 0)
        {
            backButtonDelay--;
        }

        UpdateGameAddPos();
        
    }
    


    private void OnApplicationQuit()
    {

        // Reset bending for design
        Shader.SetGlobalFloat("_V_CW_X_Bend_Size", 0);
        Shader.SetGlobalFloat("_V_CW_Y_Bend_Size", 0);
        Shader.SetGlobalFloat("_V_CW_Z_Bend_Size", 0);
        Shader.SetGlobalFloat("_V_CW_Z_Bend_Bias", 0);
        Shader.SetGlobalFloat("_V_CW_Camera_Bend_Offset", 0);

    }

    public void onGameStart()
    {

    }

    public void doCameraFovEffect()
    {

        if (cameraFovEffetc != null)
        {
            cameraFovEffetc.ResetToBeginning();
            cameraFovEffetc.PlayForward();
        }
    }

    public void ResetGame()
    {

        isHighScoreRecorded = false;
        controller.Reset();
        trackGenerator.Reset();
        //cameraController.Reset();
        achievements.Reset();
        powerupController.Reset();
        CharacterObstacle.Reset();
        CriticalModeController.Instance.Reset();

        Sign.ShuffleSings();

        ResetRankPannel();


        //游戏重新开始？
        //starTime = DateTime.Now;
        //if (Achievements.Instance)
        //    Achievements.Instance.updateCalorieTime(0, true);
        //Debug.Log("------- ResetGame");
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void OverGame()
    {
        //提交
    }


    public void handlePause(bool isPaused)
    {

        MovingObstacle.handlePauseAll(isPaused);
        CharacterObstacle.hanlePause(isPaused);

    }

    public void handleDeath()
    {
        CharacterObstacle.handleDeath();
    }


    public bool isInGamePlay()
    {
        if (GameGlobals.Instance.currentGameState == null) return false;

        if (GameGlobals.Instance.currentGameState.Equals("OnGameRunning"))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool isInDeathScene()
    {
        /*
        if (gameFSM.ActiveStateName == "PreDeath" || gameFSM.ActiveStateName == "Death Scene")
        {
            return true;
        }
        else
        {
            return false;
        }
        */

        return false;
    }

    public bool isInCriticalMode()
    {
        //Debug.Log(villainsFSM.ActiveStateName);

        /*
        if (villainsFSM.ActiveStateName == "Listening")
        {
            return false;
        }
        else
        {
            return true;
        }
        */
        return false;
    }

    // Global Player Handlers ------------------------------------------------------------------------------------------------------------------------------------------------------

    public void playPlayerAnimaiton(string animName)
    {

        if (playerController != null)
        {
            Animator playerAnimator = playerController.GetComponent<Animator>();
            playerAnimator.Play(animName, 0, 0);
        }

    }

    public void movePlayerToDefaultArea()
    {
        if (playerController != null)
        {
            //ShopWindow.SetLayerOnAllRecursive(playerController.gameObject, "Default");
            playerController.transform.parent = player.transform;
            playerController.transform.localPosition = new Vector3(0, 0, 0);
            playerController.transform.localRotation = new Quaternion(0, 0, 0, 0);
            playerController.transform.localScale = new Vector3(1, 1, 1);

        }
    }

    public void movePlayerToHighScoreArea()
    {
        Transform highScoreArea = GameObject.Find("HighScrore").transform.Find("playerHighScoreHolder");

        if (playerController != null)
        {
            //ShopWindow.SetLayerOnAllRecursive(playerController.gameObject, "UI");

            playerController.transform.parent = highScoreArea;
            playerController.transform.localPosition = new Vector3(0, 80.0f, -75.0f);
            playerController.transform.localRotation = new Quaternion(0, 180.0f, 0, 0);
            playerController.transform.localScale = new Vector3(50, 50, 50);

            Animator playerAnimator = playerController.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.Play("hiphop", 0, 0);
            }

        }
    }



    public void disableMovingObstacles()
    {
        MovingObstacle.ActivateResumeMotion();
        MovingCoin.ActivateResumeMotion();
    }

    public void setGroundedPlayer()
    {
        StartCoroutine(movePlayerToLastPosition());
    }


    private IEnumerator movePlayerToLastPosition()
    {

        int posTimer = 0;
        float duration = 60;

        GameObject villains = GameObject.Find("villians");
        CharacterController characterController = player.GetComponent<CharacterController>();

        float destPos = 0;
        switch (controller.trackIndex)
        {
            case -1:
                destPos = -6.0f;
                break;
            case 0:
                destPos = 0;
                break;
            case 1:
                destPos = 6.0f;
                break;
        }


        while (true)
        {

            if (posTimer >= duration)
            {
                break;
            }

            controller.verticalSpeed -= controller.gravity * Time.deltaTime;
            Vector3 yPos = (Vector3)((controller.verticalSpeed * Time.deltaTime) * Vector3.up);


            float delta = 1.0f / duration * posTimer;


            Vector3 currentPos = player.transform.position;
            Vector3 nextPos = Vector3.right * Mathf.Lerp(player.transform.position.x, destPos, delta);
            Vector3 xpos = new Vector3((nextPos - currentPos).x, 0, 0);

            characterController.Move(yPos);
            characterController.Move(xpos);

            villains.transform.position = player.transform.position;

            posTimer++;

            yield return new WaitForSeconds(Time.fixedDeltaTime);

        }



    }




    // -------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void goToMainMenu()
    {
    }

    [HideInInspector]
    public bool isHighScoreRecorded;
    public void checkHighScoreRecorded()
    {
        //关闭高分面板
        //if (isHighScoreRecorded == false)
        //{
        villainCheerState.ExecuteAll();
        //}
        //else
        //{
        //highScorePanel.SetActive(true);
        //    highScoreState.ExecuteAll();
        //}
    }


    public void testDialog()
    {
        DialogWindow.Instance.showDialog(DialogWindow.DialogType.YesNo, "NABER LA ?", testResult);
    }

    public void testResult(DialogWindow.DialogResult result)
    {
        Debug.Log("DIALOG GAPANDI " + result);
    }



    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void StartGame()
    {
        setGameAutoStart();
        onScoreScreenExit.ExecuteAll();
    }
}
