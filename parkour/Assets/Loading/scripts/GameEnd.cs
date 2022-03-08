using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{

    // Use this for initialization

	public Text coinsText,distancetext,bestDistanceText;
	public Button homeBtn;
	public GameObject mask;



	private float coins, distance;

	
	public Vector3[] originalPositions;


	void OnEnable() {

		Sdktest.Instance.GameState = E_GameState.GameOver;


		mask.SetActive(false);
		homeBtn.onClick.AddListener(OpenHomeView);
		
		//FbButton.transform.Translate(40,0,0);
		coinsText.text="0";
		distancetext.text="0";
		iTween.ValueTo(gameObject,iTween.Hash("from",coins,"to",GamePlayController.collectedCoinsCounts,"time",1,"easetype",iTween.EaseType.easeInOutCubic,
			"onupdate","changeCoinText","delay", 0f,"oncomplete","startDistanceCount") );

		iTween.ColorTo(coinsText.gameObject,iTween.Hash("color",Color.red,"time",1.0f,"delay", 0f ));

		 
		PlayerPrefs.SetInt("TotalCoins",PlayerPrefs.GetInt("TotalCoins",0 ) + GamePlayController.collectedCoinsCounts) ;
		//to stop bgsounds on GameoVer
		SoundController.Static.BgSoundsObj.SetActive (false);
		//SoundController.Static.PlayBIKECrashSound ();
	}
	
	private void OpenHomeView() {
		SceneManager.LoadScene("MainMenu");
	}
	private void OpenHomeView(string msg) {
		if (msg == "StartGame" && gameObject.activeSelf == true) {
			SceneManager.LoadScene("MainMenu");
		}
	}


	void BestDistance()
	{
		if (PlayerPrefs.GetFloat ("BestDistance", 0) < GamePlayController.distanceTravelled) {
			//iTween.MoveTo(newBestDistance,iTween.Hash("islocal",true,"position",new Vector3(-0.03848858f,0,-2),"time",0.5f));
			
			//iTween.ShakePosition(Camera.main.gameObject, new Vector3 (2.3f, 2.3f,2.5f),0.8f);
			iTween.PunchRotation (Camera.main.gameObject, iTween.Hash ("amount", new Vector3 (0.3f, 0.3f, 0.3f), "time", 0.5f));

			PlayerPrefs.SetFloat("BestDistance",GamePlayController.distanceTravelled );
		}
		bestDistanceText.text= ""+Mathf.RoundToInt(PlayerPrefs.GetFloat ("BestDistance", 0))+"m";
		Invoke ("showButtons", 0.5f);
	}
	void changeCoinText(float newValue)
	{
		coinsText.text = ""+ Mathf.RoundToInt(  newValue );
		SoundController.Static.playCoinHit();
	}

	void startDistanceCount()
	{
		iTween.ValueTo(gameObject,iTween.Hash("from",0,"to", GamePlayController.distanceTravelled,"time",1,"easetype",iTween.EaseType.easeInOutCubic,
		                                      "onupdate","changeDistanceText","oncomplete","BestDistance") );


		SoundController.Static.PlayPowerPickUp();

		iTween.ColorTo(distancetext.gameObject,Color.red,1.0f);
	}

	void changeDistanceText(float newValue)
	{
		SoundController.Static.playCoinHit();
		distancetext.text = ""+ Mathf.RoundToInt(  newValue )+"千卡";

	}
	 
	void showButtons()
	{
		SoundController.Static.PlaySlider();
		
		//iTween.MoveTo(FbButton,iTween.Hash("position",originalPositions[2],"time",0.5f,"easetype",iTween.EaseType.easeInOutBounce,"islocal",true,"delay",1.3f ) );
	}
}
