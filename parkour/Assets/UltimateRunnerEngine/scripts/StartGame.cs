using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {

    public TweenPosition tweenPosition;
    public Image fill;
    public float wtime = 2f;

    public AudioClip tts;
    public bool playTTS = true;
    
    private float fillAmount = 0;
    private float actionTime = 0;
    private bool uiDone = false;


    // Start is called before the first frame update
    void Start() {
        Sdktest.Instance.GameState = E_GameState.Home;
        Sdktest.Instance.GameActionEvent += OnGameActionEvent;
        
        tweenPosition.AddOnFinished(() => {
            if (tweenPosition.isForward) {
                //tweenPosition UI 入场结束
                uiDone = true;
                if (playTTS)
                {
                    TTS.ins.PlayTTS(tts);
                }
                AutoQuitApp.ins.Living();
            } else {
                //tweenPosition UI 出场结束
                uiDone = false;
                AutoQuitApp.ins.Living();
            }
            fillAmount = 0;
            actionTime = 0;
        });
    }

    private void DoStartGame() {
        TTS.ins.playJumpSquat();
        GameGlobals.Instance.StartGame();
    }


    private void OnGameActionEvent(SdkGameActionVo actionVo) {
        if (uiDone && actionVo.action == "ARMS_RAISED") {
            actionTime = 1.1f;
        }
    }
    
    private void Update() {
        if (uiDone) {
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y < 500) {
                actionTime = 1.1f;
            }
            actionTime -= Time.deltaTime;
            fillAmount += actionTime > 0 ? Time.deltaTime : -Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, wtime);
            fill.fillAmount = fillAmount / wtime;
            if (fill.fillAmount >= 0.99f) {
                uiDone = false;
                DoStartGame();
            }
        }
    }

    private void OnDestroy() {
        Sdktest.Instance.GameActionEvent -= OnGameActionEvent;
    }
}
