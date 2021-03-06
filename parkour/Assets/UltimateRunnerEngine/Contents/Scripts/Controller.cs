using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class Controller : MonoBehaviour{
    public RuntimeAnimatorController playerRuntimeAnimController;
    public GameObject player;
    public GameObject villainGorup;


    //private ShopWindow shopController;

    private CharacterController characterController;
    private CapsuleCollider characterCollider;
    private GameObject playerController;
    private ColliderTrigger characterColliderTrigger;

    private Animator playerAnimator;
    public Animator villain1Animator, villain2Animator;

    [HideInInspector] public Vector3 currentPosition;

    public float gameSpeed = 1f;

    public GameObject[] effetcs;
    private GameObject effetcsRoot;

    [HideInInspector] public bool isGameOver;

    public static Controller Instance;


    // Use this for initialization
    void Awake(){
        Instance = this;

        characterController = player.GetComponent<CharacterController>();
        playerController = player.transform.Find("playerController").gameObject;
        characterCollider = playerController.GetComponent<CapsuleCollider>();
        playerAnimator = playerController.GetComponent<Animator>();
        
        effetcsRoot = GameObject.Find("EffetcsRoot");

        //Physics.IgnoreCollision(characterCollider, characterController);

        characterColliderTrigger = this.characterCollider.GetComponent<ColliderTrigger>();
        characterColliderTrigger.OnEnter = (ColliderTrigger.OnEnterDelegate) Delegate.Combine(
            this.characterColliderTrigger.OnEnter,
            new ColliderTrigger.OnEnterDelegate(this.OnCharacterColliderEnterTrigger));
        characterColliderTrigger.OnExit = (ColliderTrigger.OnExitDelegate) Delegate.Combine(
            this.characterColliderTrigger.OnExit, new ColliderTrigger.OnExitDelegate(this.OnCharacterColliderExit));


        // Getting Selected Chracter


        Reset();
        
        //镜子姿势
        //Sdktest.Instance.GameActionEvent = OnGameActionEventHandle;
    }

    //private void OnGameActionEventHandle(SdkGameActionVo actionVo)
    //{
    //    if (actionVo.action == "JUMP" && actionVo.grade > 0)
    //    {
    //        Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Up);//跳
    //    }
    //    else if (actionVo.action == "SQUAT" && actionVo.grade > 0)
    //    {
    //        //开始界面,开始游戏
    //        if (GameGlobals.Instance.currentGameState == "OnOpeningScene")
    //        {
    //            GameGlobals.Instance.escapeSceneState.ExecuteAll();
    //        }
    //        else if (GameGlobals.Instance.currentGameState == "onScoreScreenEnter")
    //        {
    //            GameGlobals.Instance.escapeSceneState.ExecuteAll();
    //        }
    //        else if (GameGlobals.Instance.currentGameState == "OnGameRunning")
    //        {
    //            Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Down);//滚
    //        }
    //    }

    //    //根据不同动作进行操作
    //    //Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Left);//左移
    //    //Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Right);//右移
    //    //Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Up);//跳
    //    //Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Down);//滚
    //}


    private float smoothMid = 0.5f;
    private float jumpThreshold = 100;
    private float averageShoulderY = 0;

    private int playerPath = 0; //-1 left  0 center 1 right


    public Slider center_w_slider;
    public Slider tolerance_slider;

    public Persons GetPerson(){
        if (Sdktest.Instance.IsNotifyDownloadOk &&
            MirrorSdkDataModel.Instance.BodynodeData != null &&
            MirrorSdkDataModel.Instance.BodynodeData.persons != null &&
            MirrorSdkDataModel.Instance.BodynodeData.persons.Count > 0){
            var targetPersonId = MirrorSdkDataModel.Instance.BodynodeData.targetPersonId;
            var persons = MirrorSdkDataModel.Instance.BodynodeData.persons;
            var person = persons.Find(p => p.personId == targetPersonId);
            if (person != null){
                return person;
            }
            persons.Sort((a, b) => a.distance.CompareTo(b.distance));
            return persons[0];
        }
        return null;
    }

    TouchController.SwipeDirection UpdateBotSdk(){
        var person = GetPerson();
        if (person != null){
            //上
            float shoulderY = Screen.height - (person.skeletons.leftShoulder.y + person.skeletons.rightShoulder.y) * 0.5f;
            float handsY = Screen.height - (person.skeletons.leftWrist.y + person.skeletons.rightWrist.y) * 0.5f;

            if (handsY > shoulderY + jumpThreshold){
                return TouchController.SwipeDirection.Up;
            }

            //下
            averageShoulderY = Mathf.Lerp(averageShoulderY, shoulderY, Time.deltaTime * 1f);
            if (shoulderY < averageShoulderY - 100){
                averageShoulderY = shoulderY;
                return TouchController.SwipeDirection.Down;
            }

            //左右
            float midX = (person.skeletons.leftShoulder.x + person.skeletons.rightShoulder.x) * 0.5f;
            smoothMid = Mathf.Clamp01(midX / Screen.width);
            
            float halfCW = 0.1f; // center_w_slider.value * 0.5f;
            float tolerance = 0.02f; // tolerance_slider.value;
            switch (playerPath){
                case -1:
                    if (smoothMid > 0.5f - halfCW + tolerance){
                        SetLocal(Local.Center);
                        playerPath = 0;
                    }
                    break;
                case 0:
                    if (smoothMid > 0.5f + halfCW + tolerance){
                        SetLocal(Local.Right);
                        playerPath = 1;
                    }
                    if (smoothMid < 0.5f - halfCW - tolerance){
                        SetLocal(Local.Left);
                        playerPath = -1;
                    }
                    break;
                case 1:
                    if (smoothMid < 0.5f + halfCW - tolerance){
                        SetLocal(Local.Center);
                        playerPath = 0;
                    }
                    break;
            }
        }
        return TouchController.SwipeDirection.Null;
    }

    public enum MoveLR{
        None,
        Left,
        Right
    }

    private MoveLR lastLR = MoveLR.None;
    float lastMoveTime = 0f;

    public void ToBotMove(MoveLR lr){
        //是否与上次相同,一定时间内相同则影响
        if (lr == lastLR && (Time.time - lastMoveTime) < 1.5f){
            return;
        }

        lastLR = lr;
        lastMoveTime = Time.time;

        if (lr == MoveLR.None){
            return;
        }

        switch (lr){
            case MoveLR.Left:
                Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Left); //左移
                break;
            case MoveLR.Right:
                Controller.Instance.HandleSwipe(TouchController.SwipeDirection.Right); //右移
                break;
        }
    }

    public void setCharacter(GameObject cloneCharacter){

        if (!playerController){
            playerController = Instantiate(cloneCharacter, new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (playerController != null){
            playerController.transform.SetParent(player.gameObject.transform);
            playerController.transform.localPosition = new Vector3(0, 0, 0);

            playerController.SetActive(true);

            // Animator
            Animator newPlayerControllerAnimator = playerController.GetComponent<Animator>();
            if (newPlayerControllerAnimator != null){
                newPlayerControllerAnimator.runtimeAnimatorController = playerRuntimeAnimController;
                newPlayerControllerAnimator.applyRootMotion = false;
                playerAnimator = newPlayerControllerAnimator;
                playerAnimator.Play("lookAround", 0, 0);
            }

            // Rigidbody
            Rigidbody newRigidBody = playerController.GetComponent<Rigidbody>();
            if (newRigidBody != null){
                newRigidBody.mass = 1;
                newRigidBody.drag = 0;
                newRigidBody.angularDrag = 0.5f;
                newRigidBody.useGravity = false;
                newRigidBody.isKinematic = true;
                newRigidBody.interpolation = RigidbodyInterpolation.None;
                newRigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }

            // Capsule Collider
            CapsuleCollider newCapsuleCollider = playerController.GetComponent<CapsuleCollider>();
            if (newCapsuleCollider != null){
                newCapsuleCollider.isTrigger = true;
                newCapsuleCollider.center = new Vector3(0, 2.15f, 0.04f);
                newCapsuleCollider.radius = 0.79f;
                newCapsuleCollider.height = 4.2f;
                newCapsuleCollider.direction = 1;

                characterCollider = newCapsuleCollider;
            }


            // On Trigger Object
            ColliderTrigger newPlayerOnTriggerObject = playerController.GetComponent<ColliderTrigger>();
            if (newPlayerOnTriggerObject != null){
                characterColliderTrigger = newPlayerOnTriggerObject;
                characterColliderTrigger.OnEnter = (ColliderTrigger.OnEnterDelegate) Delegate.Combine(
                    this.characterColliderTrigger.OnEnter,
                    new ColliderTrigger.OnEnterDelegate(this.OnCharacterColliderEnterTrigger));
                characterColliderTrigger.OnExit = (ColliderTrigger.OnExitDelegate) Delegate.Combine(
                    this.characterColliderTrigger.OnExit,
                    new ColliderTrigger.OnExitDelegate(this.OnCharacterColliderExit));
            }

            GameGlobals.Instance.playerController = playerController;
        }
    }

    public void Reset(){
        // GameGlobals.Instance.playerController.SetActive(true);
        // playerAnimator.Play("lookAround", 0, 0);
        if (ShopWindow.Instance != null){
            ShopWindowCharacterItem selectedCharacter = ShopWindow.Instance.getSelectedCharacter();
            if (selectedCharacter != null){
                setCharacter(selectedCharacter.getPlayerClone());
            }
        }

        playerX = 0;
        playerZ = 0;

        currentPosition = new Vector3(0, 0, 0);
        player.transform.position = new Vector3(0, 0, 0);

        //startTime = Time.time;
        resetStartTime();
        timeEplased = 0;
        currentLevelSpeed = this.Accelerate(Time.time - startTime);

        trackIndex = 0;
        trackIndexTarget = 0;
        trackIndexPosition = trackIndex;

        trackSpacing = 6f;
        isRolling = false;
        isJumping = false;
        isGameOver = false;

        isOpenTargetLocal = false;
        targetLocal = Local.Center;

        smoothMid = 0.5f;
    }

    #region 控制角色绝对位置

    bool isOpenTargetLocal = false;
    public Local targetLocal = Local.Center;

    public enum Local{
        Left,
        Center,
        Right
    }

    public void SetLocal(Local local){
        isOpenTargetLocal = true;
        targetLocal = local;
    }

    void UpdateLocal(){
        if (changeLaneTrig) return;

        if (isOpenTargetLocal){
            var currLocal = trackIndex == -1 ? Local.Left : trackIndex == 1 ? Local.Right : Local.Center;
            if (currLocal != targetLocal){
                Debug.Log($">> change local:{currLocal} {targetLocal} {isGameOver}");
                switch (currLocal){
                    case Local.Center:
                        HandleSwipe(targetLocal == Local.Left
                            ? TouchController.SwipeDirection.Left
                            : TouchController.SwipeDirection.Right);
                        break;
                    case Local.Left:
                        HandleSwipe(TouchController.SwipeDirection.Right);
                        break;
                    case Local.Right:
                        HandleSwipe(TouchController.SwipeDirection.Left);
                        break;
                }
            }
        }

        //--test
        if (Input.GetKeyUp(KeyCode.J)){
            SetLocal(Local.Left);
        }

        if (Input.GetKeyUp(KeyCode.K)){
            SetLocal(Local.Center);
        }

        if (Input.GetKeyUp(KeyCode.L)){
            SetLocal(Local.Right);
        }
    }

    #endregion


    // Update is called once per frame
    void Update(){
        // Updating Speed
        UpdateAcceleration();

        // Updating Controls
        UpdateControls();

        UpdateLocal();

        timeTotal += Time.deltaTime;
        if (Achievements.Instance)
            Achievements.Instance.updateCalorieTime();
    }


    void FixedUpdate(){
        UpdateFunction();
    }


    private void UpdateFunction(){
        Vector3 peakPosition = currentPosition;

        // Applying Z Movement
        peakPosition = ApplyZMovement(peakPosition);

        // Applying X Movement
        peakPosition = ApplyXMovement(peakPosition);

        // Applying Y Movement
        peakPosition = ApplyYMovement(peakPosition);

        // Move our character!
        characterController.Move(peakPosition);

        playerZ = player.transform.position.z;

        // Move Emeny
        Vector3 delayedPos = Vector3.Lerp(villainGorup.transform.position, player.transform.position,
            Time.deltaTime * 5.0f);
        villainGorup.transform.position = new Vector3(delayedPos.x, delayedPos.y, player.transform.position.z);
    }


    // X MOVEMENT
    private Vector3 ApplyXMovement(Vector3 position){
        Vector3 currentPos = player.transform.position;
        Vector3 nextPos = Vector3.right * playerX;
        return new Vector3((nextPos - currentPos).x, position.y, position.z);
    }


    // Y MOVEMENT
    private Vector3 ApplyYMovement(Vector3 position){
        if ((verticalSpeed < 0f) && characterController.isGrounded){
            verticalSpeed = 0f;
            IsGrounded = true;
            if (isJumping || isFalling){
                isJumping = false;
                isFalling = false;
                IsGrounded = true;

                if (isRolling == false){
                    playerAnimator.Play("run", 0, 0);
                    playVillainAnimation("run");
                }

                doAnEffect(EffetcType.LandingPuff);
                GameGlobals.Instance.audioController.playSound("PlayerLand", false);
            }
        }


        verticalSpeed -= this.gravity * gameSpeed * Time.deltaTime;

        if ((!characterController.isGrounded && !isFalling) &&
            ((verticalSpeed < verticalFallSpeedLimit) && !isRolling)){
            isFalling = true;
            IsGrounded = false;
        }

        Vector3 zero = (Vector3) ((this.verticalSpeed * Time.deltaTime * gameSpeed) * Vector3.up);

        return new Vector3(position.x, zero.y, position.z);
    }

    // Z MOVEMENT
    private Vector3 ApplyZMovement(Vector3 position){
        Vector3 currentPos = player.transform.position;
        playerZ += (currentLevelSpeed * gameSpeed * Time.deltaTime);
        Vector3 nextPos = Vector3.forward * playerZ;

        return new Vector3(position.x, position.y, (nextPos - currentPos).z);
    }


    // Jump ------------------------------------------------------------------------------------------------------------------------------------------
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isFalling;
    private bool IsGrounded;
    [HideInInspector] public float verticalSpeed;
    private float verticalSpeed_jumpTolerance = -30f;
    private float verticalFallSpeedLimit = -1f;
    [HideInInspector] public float gravity = 100f;
    [HideInInspector] public float jumpHeight = 10f;


    public void doJump(){
        bool jumpFlag = (!isJumping && (verticalSpeed <= 0f)) && (this.verticalSpeed > verticalSpeed_jumpTolerance);
        if (characterController.isGrounded || jumpFlag && !getTransitionFromHeight()){
            isJumping = true;
            isFalling = false;
            IsGrounded = false;

            verticalSpeed = this.CalculateJumpVerticalSpeed(this.jumpHeight);

            GameGlobals.Instance.audioController.playSound("PlayerJump", false);
            playerAnimator.Play("jump", 0, 0);
            playVillainAnimation("jump");
        }
    }

    public float CalculateJumpVerticalSpeed(float jumpHeight){
        return Mathf.Sqrt((2f * jumpHeight) * this.gravity);
    }

    private bool getTransitionFromHeight(){
        if (player.transform.position.y > 5.0f){
            return true;
        }

        return false;
    }

    public float JumpLength(){
        return currentLevelSpeed * 2f * this.CalculateJumpVerticalSpeed(jumpHeight) / this.gravity;
    }

    // ------------------------------------------------------------------------------------------------------------------------------------------------


    // Roll -------------------------------------------------------------------------------------------------------------------------------------------

    [HideInInspector] public bool isRolling;
    private Coroutine endRollCoroutine;

    public void doRoll(){
        if (!this.isRolling){
            this.isRolling = true;

            characterController.height = 1.4f;
            characterController.center = new Vector3(0, 0.74f, 0.04f);

            characterCollider.height = 1.4f;
            characterCollider.center = new Vector3(0, 0.74f, 0.04f);


            if (!getTransitionFromHeight()){
                this.verticalSpeed = -this.CalculateJumpVerticalSpeed(this.jumpHeight);
            }
            else{
                this.verticalSpeed = -250f;
            }


            playerAnimator.Play("roll", 0, 0);
            playVillainAnimation("roll");

            GameGlobals.Instance.audioController.playSound("PlayerDuck", false);

            if (endRollCoroutine != null){
                StopCoroutine(endRollCoroutine);
            }

            endRollCoroutine = StartCoroutine(rollDisabler());
        }
    }


    public void EndRoll(){
        if (this.characterController.enabled){
            this.characterController.Move((Vector3) (Vector3.up * 2f));
        }

        characterController.height = 3.5f;
        characterController.center = new Vector3(0, 1.75f, 0.04f);

        characterCollider.height = 3.5f;
        characterCollider.center = new Vector3(0, 1.75f, 0.04f);


        if (this.characterController.enabled){
            this.characterController.Move((Vector3) (Vector3.down * 2f));
        }

        this.isRolling = false;

        if (isJumping == false){
            playerAnimator.Play("run", 0, 0);
            playVillainAnimation("run");
        }
    }

    IEnumerator rollDisabler(){
        yield return new WaitForSeconds(1.8f);
        if (GameGlobals.Instance.isInGamePlay() == false) yield break;
        EndRoll();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------


    // Change Lane------------------------------------------------------------------------------------------------------------------------------------
    public static float laneWidth = 6;
    private Vector3 trackLeft, trackRight;

    [HideInInspector] public int numberOfTracks = 3;
    private float trackSpacing;

    [HideInInspector] public int trackIndex;
    private int trackIndexTarget;
    private float trackIndexPosition;

    private int trackMovement;
    private int trackMovementNext;
    private int trackMovementLast;

    private float characterRotation;

    [HideInInspector] public float characterAngle = 45f;

    private float laneChangeTime = 0.29f;

    private bool changeLaneTrig;

    public void doChangeLane(int movement, float duration){
        if (changeLaneTrig == true) return;

        if (trackMovement != movement){
            ForceChangeTrack(movement, duration);
        }
        else{
            trackMovementNext = movement;
        }
    }

    public void ForceChangeTrack(int movement, float duration){
        changeLaneTrig = true;
        base.StartCoroutine(this.ChangeTrackCoroutine(movement, duration));
    }


    IEnumerator ChangeTrackCoroutine(int move, float duration){
        int newTrackIndex = 0;
        float trackChangeIndexDistance;
        float trackIndexPositionBegin;

        float startX;
        float endX;
        float dir;

        float startRotation;

        bool doStumple = false;

        trackMovement = move;
        trackMovementLast = move;
        trackMovementNext = 0;

        newTrackIndex = trackIndexTarget + move;

        if (newTrackIndex < -1){
            newTrackIndex = -1;
            doStumple = true;
        }
        else if (newTrackIndex > 1){
            newTrackIndex = 1;
            doStumple = true;
        }


        if (doStumple == false){
            trackChangeIndexDistance = Mathf.Abs((float) newTrackIndex - trackIndexPosition);
            trackIndexPositionBegin = trackIndexPosition;
            startX = playerX;
            endX = GetTrackX(newTrackIndex);

            dir = Mathf.Sign((float) newTrackIndex - trackIndexTarget);
            startRotation = this.characterRotation;

            if (move == -1 && IsGrounded == true){
                //playerAnimator.Play("strafeLeft", 0, 0);
            }
            else if (move == 1 && IsGrounded == true){
                //playerAnimator.Play("strafeRight", 0, 0);
            }

            if (move == -1){
                GameGlobals.Instance.audioController.playSound("PlayerLeft", false);
            }
            else if (move == 1){
                GameGlobals.Instance.audioController.playSound("PlayerRight", false);
            }


            trackIndexTarget = newTrackIndex;

            StartCoroutine(ShortTween.Play(trackChangeIndexDistance * duration, (float t) => {
                trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, (float) newTrackIndex, t);
                playerX = Mathf.Lerp(startX, endX, t);
                characterRotation = Bell(t) * dir * characterAngle + Mathf.Lerp(startRotation, 0f, t);
                player.transform.localRotation = Quaternion.Euler(0f, characterRotation, 0f);
            }));
        }
        else{
            if (PlayerPrefs.GetInt("tutorial", 0) == 1){
                if (CriticalModeController.Instance.currentState == CriticalModeController.CriticalModeState.onEnter){
                    if (move == -1){
                        DoStumble(null, StumbleLocation.LeftTrackBorder);
                    }
                    else if (move == 1){
                        DoStumble(null, StumbleLocation.RightTrackBorder);
                    }
                }
            }
        }

        trackMovement = 0;
        trackIndex = newTrackIndex;

        if (trackMovementNext != 0){
            StartCoroutine(ChangeTrackCoroutine(trackMovementNext, duration));
        }


        changeLaneTrig = false;
        yield return 0;
    }

    public float Bell(float x){
        return Mathf.SmoothStep(0f, 1f, 1f - Mathf.Abs(x - 0.5f) / 0.5f);
    }

    public float GetTrackX(int trackIndex){
        switch (trackIndex){
            case -1:
                return -trackSpacing;
            case 0:
                return 0;
            case 1:
                return trackSpacing;
        }

        return 0;
    }


    // -----------------------------------------------------------------------------------------------------------------------------------------------


    // Controllers -----------------------------------------------------------------------------------------------------------------------------------
    public delegate void OnSwipeChanged(TouchController.SwipeDirection swipe);

    public OnSwipeChanged OnSwipeChangedMethod;


    private void UpdateControls(){
        TouchController.SwipeDirection direction = TouchController.SwipeDirection.Null;
#if !UNITY_EDITOR && UNITY_ANDROID
        direction = UpdateBotSdk();
#else
        // Swipe Controls
        direction = GameGlobals.Instance.touchController.getSwipeDirection();
#endif

        if (direction != TouchController.SwipeDirection.Null){
            AutoQuitApp.ins.Living();
            if (direction == TouchController.SwipeDirection.Up){
                HandleSwipe(TouchController.SwipeDirection.Up);
            }

            if (direction == TouchController.SwipeDirection.Down){
                HandleSwipe(TouchController.SwipeDirection.Down);
            }

            if (direction == TouchController.SwipeDirection.Left){
                HandleSwipe(TouchController.SwipeDirection.Left);
            }

            if (direction == TouchController.SwipeDirection.Right){
                HandleSwipe(TouchController.SwipeDirection.Right);
            }
        }


        // Keyboard Controls

        // if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))//Up/ jump
        // {
        //     HandleSwipe(TouchController.SwipeDirection.Up);
        // }
        // else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        // {
        //     HandleSwipe(TouchController.SwipeDirection.Down);
        // }
        // else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))//Left
        // {
        //     HandleSwipe(TouchController.SwipeDirection.Right);
        // }
        // else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))//Right
        // {
        //     HandleSwipe(TouchController.SwipeDirection.Left);
        // }

        if (Input.GetButtonDown("Fire1")){
            CharacterObstacle.onClick(player.transform.position);
        }
    }

    public void HandleSwipe(TouchController.SwipeDirection swipeDir){
        //OnSwipeChangedMethod(swipeDir);

        if (isRolling == true){
            EndRoll();
        }

        switch (swipeDir){
            case TouchController.SwipeDirection.Up:
                GameGlobals.Instance.audioController.stopSound("PlayerDuck", false);

                doJump();
                break;

            case TouchController.SwipeDirection.Down:
                doRoll();
                break;

            case TouchController.SwipeDirection.Left:
                GameGlobals.Instance.audioController.stopSound("PlayerDuck", false);

                doChangeLane(-1, laneChangeTime);
                break;

            case TouchController.SwipeDirection.Right:
                GameGlobals.Instance.audioController.stopSound("PlayerDuck", false);


                doChangeLane(1, laneChangeTime);
                break;
        }
    }


    // ---------------------------------------------------------------------------------------------------

    // SPEED ---------------------------------------------------------------------------------------------
    public class Acceleration{
        public float start = 38;
        public float end = 55;
        public float accelerationTime = 200;
    }

    [HideInInspector] public float currentLevelSpeed;

    public Acceleration speed = new Acceleration();

    private float playerX;
    private float playerZ;
    private float startTime;
    private float timeEplased;
    private float playerZCheck;

    public float timeTotal;

    public void resetStartTime(){
        // TO DOO
        //Debug.Log("Time reset");
        startTime = Time.time;
        timeTotal = 0;

        tutorialWaiting = false;

        if (Achievements.Instance)
            Achievements.Instance.updateCalorieTime(true);
        Debug.Log("------- ResetGame");
    }


    [HideInInspector] public bool playerIsRunning;

    private void UpdateAcceleration(){
        if (playerZ > playerZCheck){
            if (timeEplased > 0){
                startTime += timeEplased;
                timeEplased = 0;
            }

            currentLevelSpeed = this.Accelerate(Time.time - startTime);
            playerIsRunning = true;
        }
        else{
            timeEplased += Time.deltaTime;
            playerIsRunning = false;
        }


        playerZCheck = playerZ;
    }

    public float Accelerate(float t){
        if (t >= this.speed.accelerationTime){
            return this.speed.end;
        }

        return t * (this.speed.end - this.speed.start) / this.speed.accelerationTime + this.speed.start;
    }

    public float NormalizedGameSpeed(){
        return ((this.currentLevelSpeed / this.speed.start) / 2.0f);
    }

    private void playVillainAnimation(string animName){
        if (CriticalModeController.Instance.currentState == CriticalModeController.CriticalModeState.onEnter){
            villain1Animator.Play(animName, 0, 0);
            villain2Animator.Play(animName, 0, 0);
        }
    }
    // ---------------------------------------------------------------------------------------------------


    // COLLIDERS ---------------------------------------------------------------------------------------------

    [HideInInspector] public float obstacleCornerOffset = 18f;

    public enum HitLocation{
        Left = 0,
        XMiddle = 1,
        Right = 2,
        Upper = 3,
        YMiddle = 4,
        Lower = 5,
        Before = 6,
        ZMiddle = 7,
        After = 8
    }


    public enum StumbleLocation{
        LeftSide = 0,
        RightSide = 1,
        FrontLower = 2,
        FrontMiddle = 3,
        FrontUpper = 4,
        FrontLeftCorner = 5,
        FrontRightCorner = 6,
        BackLeftCorner = 7,
        BackRightCorner = 8,
        LeftTrackBorder = 9,
        RightTrackBorder = 10
    }


    private float hitXOffset = 0.5f;
    private float hitMinOffset = 0.32f;
    private float hitMaxOffset = 0.65f;
    private float hitZOffset = 31;


    private int[] analizeHit(Collider collider){
        int[] results = new int[3];

        int resulyX = 0;
        int resulyY = 1;
        int resulyZ = 2;

        Bounds characterBounds = characterCollider.bounds;
        Bounds obstacleBounds = collider.bounds;

        // X ---------------------------------------------------------------------------------

        float colliderXBoundsMin = Mathf.Max(characterBounds.min.x, obstacleBounds.min.x);
        float colliderXBoundsMax = Mathf.Min(characterBounds.max.x, obstacleBounds.max.x);

        float xBoundsTotal = (colliderXBoundsMin + colliderXBoundsMax) * hitXOffset;
        float xDirection = xBoundsTotal - obstacleBounds.min.x;

        if (xDirection <= obstacleBounds.size.x - laneWidth * hitMinOffset){
            resulyX = (xDirection >= laneWidth * hitMinOffset ? (int) HitLocation.XMiddle : (int) HitLocation.Left);
        }
        else{
            resulyX = (int) HitLocation.Right;
        }


        // Y ---------------------------------------------------------------------------------

        float colliderYBoundsMin = Mathf.Max(characterBounds.min.y, obstacleBounds.min.y);
        float colliderYBoundsMax = Mathf.Min(characterBounds.max.y, obstacleBounds.max.y);

        float yBoundsTotal = (colliderYBoundsMin + colliderYBoundsMax) * hitXOffset;
        float yDirection = (yBoundsTotal - characterBounds.min.y) / characterBounds.size.y;

        if (yDirection >= hitMinOffset){
            resulyY = (yDirection >= hitMaxOffset ? (int) HitLocation.Upper : (int) HitLocation.YMiddle);
        }
        else{
            resulyY = (int) HitLocation.Lower;
        }

        // Z ---------------------------------------------------------------------------------

        Vector3 playerPosition = player.transform.position;

        resulyZ = (int) HitLocation.ZMiddle;
        if (playerPosition.z > (obstacleBounds.max.z - (((obstacleBounds.max.z - obstacleBounds.min.z) <= hitZOffset)
                ? ((obstacleBounds.max.z - obstacleBounds.min.z) * hitXOffset)
                : obstacleCornerOffset))){
            resulyZ = (int) HitLocation.After;
        }

        if (playerPosition.z < (obstacleBounds.min.z + obstacleCornerOffset)){
            resulyZ = (int) HitLocation.Before;
        }


        // Result Addresing

        results[0] = resulyX;
        results[1] = resulyY;
        results[2] = resulyZ;
        return results;
    }

    private void OnCharacterColliderEnterTrigger(Collider collider){
        StartCoroutine(OnCharacterColliderEnter(collider));
    }

    private IEnumerator OnCharacterColliderEnter(Collider collider){
        GameObject collidedObject = null;
        TrackObject collidedTrackObject = null;

        if (isGameOver == true) yield break;

        if (collider.transform.parent != null){
            if (collider.transform.parent.gameObject != null){
                collidedObject = collider.transform.parent.gameObject;
                collidedTrackObject = collidedObject.GetComponent<TrackObject>();
            }
        }

        //-------------------------------------------------------------------------------

        switch (collider.tag){
            case "ground":

                //Debug.Log("ground hit");
                break;

            case "duckPlace":
                GameGlobals.Instance.cameraController.duckCamera(CameraController.DuckState.duckStart);
                break;
            case "powerup":

                if (collidedTrackObject != null){
                    // Handle Track Object
                    switch (collidedTrackObject.objectType){
                        case TrackObject.ObjectType.PointsSingle:

                            Coin currentCoin = collidedTrackObject.gameObject.GetComponent<Coin>();
                            if (currentCoin != null){
                                currentCoin.pickUp();
                            }

                            break;
                    }

                    // Powerups and Pickables
                    if (collidedTrackObject.objectGroup == TrackObject.ObjectGroup.PowerUps ||
                        collidedTrackObject.objectGroup == TrackObject.ObjectGroup.Pickables){
                        Powerup powerUp = collidedTrackObject.gameObject.GetComponent<Powerup>();
                        if (powerUp != null){
                            powerUp.pickUp();
                        }
                    }
                }

                break;
            case "obstacle":

                //Debug.Log(trackMovementLast);
                //Debug.Log(collider.name);

                int lane;

                int[] hits = analizeHit(collider);

                HitLocation tx = (HitLocation) hits[0];
                HitLocation ty = (HitLocation) hits[1];
                HitLocation tz = (HitLocation) hits[2];

                float colliderCenter = (collider.bounds.min.x + collider.bounds.max.x) / 2f;
                float x = player.transform.position.x;


                if (x < colliderCenter){
                    lane = 1;
                }
                else if (x > colliderCenter){
                    lane = -1;
                }
                else{
                    lane = 0;
                }

                bool flag = (lane == 0) || (trackMovementLast == lane);
                bool flag2 = characterCollider.bounds.center.z < collider.bounds.min.z;
                bool flag3 = ((tz == HitLocation.Before) && !flag2) && flag;

                // Debug.Log(collider.name + " - flag:" + flag + " flag2:" + flag2 + " flag3:" + flag3);

                // Train Crash
                if (collider.name.Equals("movingMesh") && flag == true && flag2 == false && flag3 == true){
                    this.DoStumble(collidedObject, StumbleLocation.FrontMiddle);
                }


                if ((tz == HitLocation.ZMiddle) || flag3){
                    // SIDES

                    if (trackMovementLast != 0){
                        // Lane Fix
                        doChangeLane(-trackMovementLast, laneChangeTime);
                    }

                    switch (tx){
                        case HitLocation.Left:

                            //Debug.Log(collider.name + " Left Side");
                            this.DoStumble(collidedObject, StumbleLocation.LeftSide);

                            break;

                        case HitLocation.Right:

                            //Debug.Log(collider.name + " Right Side");
                            this.DoStumble(collidedObject, StumbleLocation.RightSide);
                            break;
                    }
                }
                else if ((tx == HitLocation.XMiddle) || (trackMovementLast == 0)){
                    if (tz == HitLocation.Before){
                        if (ty == HitLocation.Lower){
                            //Debug.Log(collider.name + " Front Lower");
                            this.DoStumble(collidedObject, StumbleLocation.FrontLower);
                        }
                        else if (ty == HitLocation.YMiddle){
                            //Debug.Log(collider.name + " Front Middle");
                            this.DoStumble(collidedObject, StumbleLocation.FrontMiddle);
                        }
                        else if (ty == HitLocation.Upper){
                            //Debug.Log(collider.name + " Front Upper");
                            this.DoStumble(collidedObject, StumbleLocation.FrontUpper);
                        }
                    }
                }
                else{
                    if (tx == HitLocation.Left && tz == HitLocation.Before){
                        //Debug.Log(collider.name + " Front Left Corner");
                        this.DoStumble(collidedObject, StumbleLocation.FrontLeftCorner);
                    }

                    if (tx == HitLocation.Right && tz == HitLocation.Before){
                        //Debug.Log(collider.name + " Front Right Corner");
                        this.DoStumble(collidedObject, StumbleLocation.FrontRightCorner);
                    }

                    if (tx == HitLocation.Left && tz == HitLocation.After){
                        ForceChangeTrack(-this.trackMovementLast, 0.5f);

                        //Debug.Log(collider.name + " Back Left Corner");
                        this.DoStumble(collidedObject, StumbleLocation.BackLeftCorner);
                    }

                    if (tx == HitLocation.Right && tz == HitLocation.After){
                        ForceChangeTrack(-this.trackMovementLast, 0.5f);

                        //Debug.Log(collider.name + " Back Right Corner");
                        this.DoStumble(collidedObject, StumbleLocation.BackRightCorner);
                    }
                }

                break;
        }
    }

    private void OnCharacterColliderExit(Collider collider){
        switch (collider.tag){
            case "duckPlace":
                GameGlobals.Instance.cameraController.duckCamera(CameraController.DuckState.duckOver);
                break;
        }
    }

    // Tutorial
    private float tutorialLastZ;

    private bool tutorialWaiting;

    private IEnumerator tutorialRestart(){
        if (tutorialWaiting){
            yield break;
        }

        tutorialWaiting = true;

        yield return new WaitForSeconds(4.0f);

        if (GameGlobals.Instance.isInGamePlay() == false){
            yield break;
        }

        Reset();
        GameGlobals.Instance.achievements.Reset();
        GameGlobals.Instance.cameraController.Reset();

        player.transform.position = new Vector3(0, 0, tutorialLastZ - 150.0f);
        currentPosition = new Vector3(0, 0, tutorialLastZ - 150.0f);
        playerZ = tutorialLastZ - 150.0f;

        player.gameObject.SetActive(true);


        tutorialLastZ = 0;

        playerAnimator.Play("run", 0, 0);
        GameGlobals.Instance.audioController.playSound("PlayerFootSteps", false);
    }

    public void doPlayerRun(){
        if (playerAnimator != null){
            playerAnimator.Play("run", 0, 0);
        }
    }

    public UnityEvent onStumple;
    public bool disableStumpleOnTrackBorders;

    private void DoStumble(GameObject obstacle, StumbleLocation location){
        if (GameGlobals.Instance.isInGamePlay() == false) return;

        // Tutorial Handler ------------------------->

        if (PlayerPrefs.GetInt("tutorial", 0) == 0){
            tutorialLastZ = player.transform.position.z;

            doAnEffect(EffetcType.DeathPuff);

            GameGlobals.Instance.playerController.SetActive(false);

            GameGlobals.Instance.audioController.playSound("PlayerStumple", false);
            GameGlobals.Instance.audioController.playSound("PlayerOuch4", false);
            GameGlobals.Instance.audioController.stopSound("PlayerFootSteps", false);

            GameGlobals.Instance.cameraController.Shake();

            GameGlobals.Instance.audioController.playSoundDelayed("UIError", false, 0.5f);

            if (TutorialController.instance != null){
                TutorialController.instance.hideMessagePanel();
                TutorialController.instance.hideHand();
                TutorialController.instance.showMessage("再来一次!", 1);
            }

            if (GameGlobals.Instance.isInGamePlay() == true){
                StartCoroutine(tutorialRestart());
            }

            return;
        }

        // Tutorial Handler ------------------------->

        if (disableStumpleOnTrackBorders == true){
            if (location == StumbleLocation.LeftTrackBorder){
                return;
            }

            if (location == StumbleLocation.RightTrackBorder){
                return;
            }
        }

        bool isInCriticalState = false;
        if (CriticalModeController.Instance.currentState == CriticalModeController.CriticalModeState.onEnter){
            isInCriticalState = true;
        }
        else{
            isInCriticalState = false;
        }


        //string collidedObstacleID = getObstacleTrackObjectID(obstacle);
        GameGlobals.Instance.cameraController.Shake();
        playVillainAnimation("run");


        bool gameOver = false;

        if (isInCriticalState == false){
            if (onStumple != null){
                onStumple.Invoke();
            }

            GameGlobals.Instance.audioController.playSound("PlayerStumple", false);
            GameGlobals.Instance.audioController.playRandomOuch();
            doAnEffect(EffetcType.StumplePuff);


            switch (location){
                case StumbleLocation.FrontLower:

                    // Jump and passable objects
                    gameOver = true;

                    break;
                case StumbleLocation.FrontMiddle:
                    // -- Game Over
                    gameOver = true;
                    break;
                case StumbleLocation.FrontUpper:
                    // -- Game Over
                    gameOver = true;
                    break;
                case StumbleLocation.LeftSide:
                    gameOver = false;
                    break;
                case StumbleLocation.RightSide:
                    gameOver = false;
                    break;
                case StumbleLocation.LeftTrackBorder:
                    gameOver = false;
                    break;
                case StumbleLocation.RightTrackBorder:
                    gameOver = false;
                    break;
                case StumbleLocation.FrontLeftCorner:
                    gameOver = false;
                    break;
                case StumbleLocation.FrontRightCorner:
                    gameOver = false;
                    break;
                case StumbleLocation.BackLeftCorner:
                    gameOver = false;
                    break;
                case StumbleLocation.BackRightCorner:
                    gameOver = false;
                    break;
            }
        }
        else if (isInCriticalState == true){
            gameOver = true;
        }


        if (gameOver == true){
            doAnEffect(EffetcType.DeathPuff);
            GameGlobals.Instance.audioController.playSound("PlayerOuch4", false);
            GameGlobals.Instance.audioController.playSound("PlayerDeath", false);

            doGameOver(obstacle);
        }
    }

    private string getObstacleTrackObjectID(GameObject obstalce){
        string result = "";

        if (obstalce != null){
            TrackObject tObject = obstalce.GetComponent<TrackObject>();
            if (tObject != null){
                result = tObject.ID;
            }
        }


        return result;
    }

    public UnityEvent onGameOver;

    private void doGameOver(GameObject crashedObstacle){
        isGameOver = true;

        if (onGameOver != null){
            onGameOver.Invoke();
        }

        if (crashedObstacle != null){
            if (crashedObstacle.transform.parent != null){
                TrackObject crashedObstacleElement = crashedObstacle.transform.parent.GetComponent<TrackObject>();

                if (crashedObstacleElement != null){
                    if (crashedObstacleElement.objectGroup == TrackObject.ObjectGroup.CharacterObstacles){
                        CharacterObstacle charObstacle =
                            crashedObstacle.transform.parent.GetComponent<CharacterObstacle>();
                        if (charObstacle != null){
                            charObstacle.onCharacterHit();
                        }
                    }
                }
            }
        }


        //globals.crashedObstacle = crashedObstacle;
    }


    // ---------------------------------------------------------------------------------------------------


    // EFFETCS ---------------------------------------------------------------------------------------------------

    public enum EffetcType{
        None = 0,
        LandingPuff = 1,
        PointPickup = 2,
        DeathPuff = 3,
        StumplePuff = 4,
        PowerupPickUp = 5,
        QuicScore = 6
    }
    
    private Dictionary<EffetcType, List<EffetcController>> effetcControllerPool = new Dictionary<EffetcType, List<EffetcController>>();
    private Transform ecPool;
    private Transform coinPickEffectNode;

    public void recoverAnEffect(EffetcType effect, EffetcController ec){
        ec.gameObject.SetActive(false);
        if(!ecPool) ecPool = new GameObject("EcPool").transform;
        ec.transform.parent = ecPool;
        if(!effetcControllerPool.ContainsKey(effect)){effetcControllerPool.Add(effect, new List<EffetcController>());}
        effetcControllerPool[effect].Add(ec);
    }
    
    public void doAnEffect(EffetcType effetc){
        if (!coinPickEffectNode){
            coinPickEffectNode = new GameObject("CoinPickEffectNode").transform;
            coinPickEffectNode.parent = player.transform;
        }
        if (coinPickEffectNode.childCount >= 5 && effetc != EffetcType.QuicScore) return;
        if(effetc == EffetcType.None) return;
        
        //重构特效创建
        if (!effetcControllerPool.ContainsKey(effetc)){ effetcControllerPool.Add(effetc, new List<EffetcController>()); }
        List<EffetcController> list = effetcControllerPool[effetc];
        EffetcController ec = list.Find(et => !et.gameObject.activeSelf);
        if (!ec){
            GameObject effetcPrefab = null;
            foreach (GameObject eObject in effetcs){
                EffetcController eController = eObject.GetComponent<EffetcController>();
                if (eController != null){
                    switch (effetc){
                        case EffetcType.LandingPuff:
                            if (eController.ID.Equals("puff")){
                                effetcPrefab = eObject;
                            }
                            break;
                        case EffetcType.PointPickup:
                            if (eController.ID.Equals("point")){
                                effetcPrefab = eObject;
                            }
                            break;
                        case EffetcType.PowerupPickUp:
                            if (eController.ID.Equals("powerup")){
                                effetcPrefab = eObject;
                            }
                            break;
                        case EffetcType.DeathPuff:
                            if (eController.ID.Equals("death")){
                                effetcPrefab = eObject;
                            }
                            break;
                        case EffetcType.StumplePuff:
                            if (eController.ID.Equals("stumple")){
                                effetcPrefab = eObject;
                            }
                            break;
                        case EffetcType.QuicScore:
                            if (eController.ID.Equals("quickscore")){
                                effetcPrefab = eObject;
                            }
                            break;
                    }
                }
                if(effetcPrefab) break;
            }
            if (effetcPrefab){
                GameObject createdEffetc = Instantiate(effetcPrefab);
                createdEffetc.transform.rotation = Quaternion.identity;
                ec = createdEffetc.GetComponent<EffetcController>();
            }
        }

        if (ec){
            Vector3 effetcPos = player.transform.position;
             switch (ec.ID){ 
                 case "puff":
                     effetcPos = new Vector3(effetcPos.x, 1.0f, effetcPos.z);
                     break; 
                 case "point":
                        effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z);
                        break; 
                 case "powerup":
                        effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z);
                        break; 
                 case "death":
                     effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z + 3.0f);
                     break; 
                 case "stumple":
                        effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z);
                        break; 
                 case "quickscore":
                        effetcPos = new Vector3(effetcPos.x, effetcPos.y + 4.0f, effetcPos.z + 15.0f);
                        break;
             }
             ec.transform.position = effetcPos;

             if (effetc == EffetcType.QuicScore){
                 int[] points = new int[4]{5, 10, 20, 50};
                 int selectedPoint = points[UnityEngine.Random.Range(0, points.Length)];

                 float pitch = 0;
            
                 switch (selectedPoint){
                     case 5:
                         pitch = 0.9f;
                         break;
                     case 10:
                         pitch = 1.0f;
                         break;
                     case 20:
                         pitch = 1.3f;
                         break;
                     case 50:
                         pitch = 1.6f;
                         break;
                 }

                 GameGlobals.Instance.audioController.playSoundPitched("UIQuestion", pitch);
                 GameGlobals.Instance.achievements.increaseScore(selectedPoint);

                 Transform pointsRoot = ec.transform.Find("pointsGroup");
                 if (pointsRoot != null){
                     foreach (Transform point in pointsRoot){
                         if (point.name.Equals(selectedPoint.ToString())){
                             point.gameObject.SetActive(true);
                         }
                         else{
                             point.gameObject.SetActive(false);
                         }
                     }
                 }
             }
             ec.gameObject.SetActive(true);
             ec.Setup(ec.ID.Equals("point") ? coinPickEffectNode : effetcsRoot.transform, effetc, this);
        }
        
        

        //
        // //---------------
        //
        // GameObject effetcObject = null;
        // Vector3 effetcPos = player.transform.position;
        //
        // foreach (GameObject eObject in effetcs){
        //     EffetcController eController = eObject.GetComponent<EffetcController>();
        //
        //     if (eController != null){
        //         switch (effetc){
        //             case EffetcType.None:
        //                 break;
        //             case EffetcType.LandingPuff:
        //                 if (eController.ID.Equals("puff")){
        //                     effetcObject = eObject;
        //                     effetcPos = new Vector3(effetcPos.x, 1.0f, effetcPos.z);
        //                 }
        //
        //                 break;
        //             case EffetcType.PointPickup:
        //                 if (eController.ID.Equals("point")){
        //                     effetcObject = eObject;
        //                     effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z);
        //                 }
        //
        //                 break;
        //             case EffetcType.PowerupPickUp:
        //                 if (eController.ID.Equals("powerup")){
        //                     effetcObject = eObject;
        //                     effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z);
        //                 }
        //
        //                 break;
        //
        //             case EffetcType.DeathPuff:
        //                 if (eController.ID.Equals("death")){
        //                     effetcObject = eObject;
        //                     effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z + 3.0f);
        //                 }
        //
        //                 break;
        //             case EffetcType.StumplePuff:
        //                 if (eController.ID.Equals("stumple")){
        //                     effetcObject = eObject;
        //                     effetcPos = new Vector3(effetcPos.x, effetcPos.y + 3.0f, effetcPos.z);
        //                 }
        //
        //                 break;
        //             case EffetcType.QuicScore:
        //                 if (eController.ID.Equals("quickscore")){
        //                     effetcObject = eObject;
        //                     effetcPos = new Vector3(effetcPos.x, effetcPos.y + 4.0f, effetcPos.z + 15.0f);
        //                 }
        //
        //                 break;
        //         }
        //     }
        //
        //     if (effetcObject){
        //         break;
        //     }
        // }

        // if (effetcObject != null){
        //     GameObject createdEffetc = Instantiate(effetcObject, effetcPos, Quaternion.identity);
        //     createdEffetc.transform.parent = effetcsRoot.transform;
        //
        //     // Quick Score Points
        //     if (effetc == EffetcType.QuicScore){
        //         int[] points = new int[4]{5, 10, 20, 50};
        //         int selectedPoint = points[UnityEngine.Random.Range(0, points.Length)];
        //
        //         float pitch = 0;
        //
        //
        //         switch (selectedPoint){
        //             case 5:
        //                 pitch = 0.9f;
        //                 break;
        //             case 10:
        //                 pitch = 1.0f;
        //                 break;
        //             case 20:
        //                 pitch = 1.3f;
        //                 break;
        //             case 50:
        //                 pitch = 1.6f;
        //                 break;
        //         }
        //
        //         GameGlobals.Instance.audioController.playSoundPitched("UIQuestion", pitch);
        //         GameGlobals.Instance.achievements.increaseScore(selectedPoint);
        //
        //         Transform pointsRoot = createdEffetc.transform.Find("pointsGroup");
        //         if (pointsRoot != null){
        //             foreach (Transform point in pointsRoot){
        //                 if (point.name.Equals(selectedPoint.ToString())){
        //                     point.gameObject.SetActive(true);
        //                 }
        //                 else{
        //                     point.gameObject.SetActive(false);
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    public void doAnCameraEffect(){
        GameObject effetcObject = null;
        GameObject mainCamera = GameObject.Find("Main Camera");

        foreach (GameObject eObject in effetcs){
            EffetcController eController = eObject.GetComponent<EffetcController>();
            if (eController.ID.Equals("cameraExplosion")){
                effetcObject = eObject;
            }
        }

        if (effetcObject != null){
            UnityEngine.Object newEffetc =
                GameObject.Instantiate(effetcObject, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject a1 = (GameObject) newEffetc;
            a1.transform.parent = mainCamera.transform;
            a1.transform.localPosition = new Vector3(0, -1.0f, 3.0f);
        }
    }

    // ---------------------------------------------------------------------------------------------------
}