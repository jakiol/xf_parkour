using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{

    private GameObject handIcon;
    private TweenPosition handIconSwipeLRAnim;
    private TweenPosition handIconRollAnim;
    private TweenPosition handIconJumpAnim;
    private TweenPosition handIconTapAnim;

    private GameObject tutorialTriggers;
  
  
    // Message
    private TweenPosition messageBoardTween;
    private Text lblMessage;

    public static TutorialController instance;

    private void Awake()
    {
      
        instance = this;

        GameGlobals.Instance.messageBoxPanel.SetActive(true);
        GameGlobals.Instance.tutorialPanel.SetActive(true);


        handIcon = GameObject.Find("sprHand");
        handIcon.SetActive(false);

      
  

        GameGlobals.Instance.achievements.scoresLock = true;

        tutorialTriggers = this.transform.Find("tutorialTriggers").gameObject;

        // Message
        messageBoardTween = GameObject.Find("MessageBoxPanel").GetComponent<TweenPosition>();
        messageBoardTween.ResetToBeginning();
        lblMessage = GameObject.Find("lblMessage").GetComponent<Text>();
        GameGlobals.Instance.messageBoxPanel.SetActive(false);

        // Anims
 
 
        handIconSwipeLRAnim = handIcon.AddComponent<TweenPosition>();
        handIconSwipeLRAnim.enabled = false;
        handIconSwipeLRAnim.from = new Vector3(120, 0, 0);
        handIconSwipeLRAnim.to = new Vector3(-120, 0, 0);
        handIconSwipeLRAnim.duration = 1.0f;
        handIconSwipeLRAnim.ignoreTimeScale = true;

        handIconRollAnim = handIcon.AddComponent<TweenPosition>();
        handIconRollAnim.enabled = false;
        handIconRollAnim.from = new Vector3(0, 120, 0);
        handIconRollAnim.to = new Vector3(0, -120, 0);
        handIconRollAnim.duration = 1.0f;
        handIconRollAnim.ignoreTimeScale = true;

        handIconJumpAnim = handIcon.AddComponent<TweenPosition>();
        handIconJumpAnim.enabled = false;
        handIconJumpAnim.from = new Vector3(0, 0, 0);
        handIconJumpAnim.to = new Vector3(0, 90, 0);
        handIconJumpAnim.duration = 1.0f;
        handIconJumpAnim.ignoreTimeScale = true;

        handIconTapAnim = handIcon.AddComponent<TweenPosition>();
        handIconTapAnim.enabled = false;
        //handIconTapAnim.delay = 0.7f;
        handIconTapAnim.from = new Vector3(0, 0, 0);
        handIconTapAnim.to = new Vector3(0, -50, 0);
        handIconTapAnim.duration = 0.3f;
        handIconTapAnim.ignoreTimeScale = true;

        if (tutorialTriggers != null)
        {

            foreach (Transform trigger in tutorialTriggers.transform)
            {

                TutorialTrigger onTriggerObject = trigger.GetComponent<TutorialTrigger>();
                if (onTriggerObject != null)
                {
                    onTriggerObject.OnEnter = (TutorialTrigger.OnEnterDelegate)Delegate.Combine(onTriggerObject.OnEnter, new TutorialTrigger.OnEnterDelegate(this.OnPlayerEnter));
                    onTriggerObject.OnExit = (TutorialTrigger.OnExitDelegate)Delegate.Combine(onTriggerObject.OnExit, new TutorialTrigger.OnExitDelegate(this.OnPlayerExit));
                }
           

            }
        
        }




    }



    private bool triggerLock;

    private void OnPlayerEnter(GameObject self,Collider collider)
    {

        if (triggerLock == true) return;
        if (collider == null) return;

        GameGlobals.Instance.messageBoxPanel.SetActive(true);

        if (collider.name.Equals("Player") && GameGlobals.Instance.isInGamePlay() == true)
        {

            switch (self.gameObject.name)
            {
                case "welcome":

                    lblMessage.text = "欢迎进入教学";
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(0);
                    break;

                case "learn":

                    lblMessage.text = "让我们学习\n新的动作吧.";
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(1);
                    break;

                case "swipeInfo":

                    lblMessage.text = "左右移动\n躲避障碍物";
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(2);
                    break;

                case "handSwipe":
                    
                    handIcon.SetActive(true);

                    handIconSwipeLRAnim.ResetToBeginning();
                    handIconSwipeLRAnim.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UIWhistle", false);
                    StartCoroutine(handHider());
                    break;

                // case "great1":
                //
                //     handIcon.SetActive(false);
                //     lblMessage.text = "漂亮！";
                //     messageBoardTween.PlayForward();
                //     GameGlobals.Instance.audioController.playSound("UICharacterSelect", false);
                //     StartCoroutine(messageHider());
                //     break;

                case "rollInfo":

                    lblMessage.text = "蹲下躲过\n高障碍物";
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(3);
                    break;

                case "handRoll":

                    handIcon.SetActive(true);
                    handIconRollAnim.ResetToBeginning();
                    handIconRollAnim.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UIWhistle", false);
                    StartCoroutine(handHider());
                    break;


                // case "great2":
                //
                //     handIcon.SetActive(false);
                //     lblMessage.text = "太棒了！";
                //     handIconRollAnim.ResetToBeginning();
                //     messageBoardTween.PlayForward();
                //     GameGlobals.Instance.audioController.playSound("UICharacterSelect", false);
                //     StartCoroutine(messageHider());
                //     break;

                case "jumpInfo":

                    lblMessage.text = "跨过\n低障碍物";
                    handIconRollAnim.ResetToBeginning();
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(4);
                    break;

                case "handJump":

                    handIcon.SetActive(true);
                    handIconJumpAnim.ResetToBeginning();
                    handIconJumpAnim.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UIWhistle", false);
                    StartCoroutine(handHider());
                    break;

                // case "great3":
                //
                //     handIcon.SetActive(false);
                //     lblMessage.text = "帅呆了！";
                //     handIconRollAnim.ResetToBeginning();
                //     messageBoardTween.PlayForward();
                //     GameGlobals.Instance.audioController.playSound("UICharacterSelect", false);
                //     StartCoroutine(messageHider());
                //     break;

                case "combatMode":

                    lblMessage.text = "一个坏家伙\n在那边等你呢";
                    handIconRollAnim.ResetToBeginning();
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    break;

                case "karate":

                    lblMessage.text = "蹲下翻滚\n打败敌人";
                    handIconRollAnim.ResetToBeginning();
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UITap", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(5);
                    break;

                case "handTap":

                    handIcon.SetActive(true);
                    handIconTapAnim.ResetToBeginning();
                    handIconTapAnim.PlayForward();
                    GameGlobals.Instance.audioController.playSound("UIWhistle", false);
                    StartCoroutine(handHider());

                    break;

                // case "great4":
                //
                //     handIcon.SetActive(false);
                //     lblMessage.text = "WOW~\n简直完美！";
                //     handIconRollAnim.ResetToBeginning();
                //     messageBoardTween.PlayForward();
                //     GameGlobals.Instance.audioController.playSound("UICharacterUnlock", false);
                //     StartCoroutine(messageHider());
                //     break;

                case "ready":
                    GameGlobals.Instance.ShowCurCalorie();
                    
                    PlayerPrefs.SetInt("tutorial", 1);
                    GameGlobals.Instance.achievements.scoresLock = false;

                    lblMessage.text = "学会啦\n快去冒险吧";
                    handIconRollAnim.ResetToBeginning();
                    messageBoardTween.PlayForward();
                    GameGlobals.Instance.audioController.playSound("PowerupLetterComplete", false);
                    StartCoroutine(messageHider());
                    TTS.ins.playTutorial(6);
                    break;

            }


            triggerLock = true;
        }

     

    }

    public void showMessage(string msg,float delay)
    {
        StartCoroutine(msgJob(msg, delay));
    }

    private IEnumerator msgJob(string msg, float delay)
    {
        yield return new WaitForSeconds(delay);

        lblMessage.text = msg;
        messageBoardTween.PlayForward();
        StartCoroutine(messageHider());

    }

    public void hideMessagePanel()
    {
        messageBoardTween.PlayReverse();
    }

    public void hideHand()
    {
        //handIconAlphaAnim.PlayReverse();
    }

    private IEnumerator messageHider()
    {
        yield return new WaitForSeconds(2f);
        messageBoardTween.PlayReverse();
    
    }

    private IEnumerator handHider()
    {
        yield return new WaitForSeconds(0.8f);
        //handIconAlphaAnim.PlayReverse();
    

    }

    private void OnPlayerExit(GameObject self, Collider collider)
    {

        if (collider == null) return;

        if (collider.name.Equals("Player") && GameGlobals.Instance.isInGamePlay() == true)
        {
            triggerLock = false;
        }


    }

}

