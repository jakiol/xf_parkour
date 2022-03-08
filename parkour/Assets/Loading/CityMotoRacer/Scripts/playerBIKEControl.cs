 using UnityEngine;
using System.Collections;
using System;


public class playerBIKEControl : MonoBehaviour
{
	public float turnSpeed;
	public float carSpeed;
	public float tilt;
	public float[] limits;
	public Transform carBody;
	public Transform[] wheelObjs;
	public float wheelSpeed;
	public static event EventHandler gameEnded;
	public static event EventHandler switchOnMagnetPower;
	public static event EventHandler switchOFFMagnetPower;
	public float magnetPowerTime=3.0f;
	private float nextFire;
	public static float isDoubleSpeed = 1;
	Transform thisTrans ;
	public GameObject particleParent;
	public Transform frontAnchor, backAnchor ;
	//public Transform bike;
	 
	public static Vector3 thisPosition ;
	bool isbikefalldown = true ;
	public Quaternion  Back_RotationTarget;
 
	Quaternion back_currentRotation;
	 

	float brakeSpeed = 0; 
	BIKECamera bikeScript ;
	Camera mainCamera;
	public enum playerstates{
		playerLive,Playerdead
	};
	public	playerstates currentState;

	#region enum for Player Animations

 
	#endregion


	void OnEnable()
	{
		if (UnityEngine.Random.Range (-4, 4) > 0)
						rotateDir = 1;
				else
						rotateDir = -1;
	 
		//print ("Nitrou"+ PlayerPrefs.GetInt ("NitrousCount", 0));
		#if UNITY_WEBPLAYER || UNITY_EDITOR
		tilt = tilt*2; 
		#endif
		thisTrans = transform;
		mainCamera = Camera.main;
		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
			if(t.name.Contains("Effect") )
			{
				particleParent = t.gameObject;
			}
				}

		isDoubleSpeed = 1;


		bikeScript = Camera.main.GetComponent<BIKECamera> ();
		bikeScript.targetTrans = thisTrans;

		
		#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8
		// even blob shadows is also taking 5 draw calls ,so we are going disable it 
//		foreach(Transform t in transform.GetComponentsInChildren<Transform>() )
//		{ if(t.name.Contains("Shadow") ) {
//				Debug.Log(t.name);
//				Destroy(t.gameObject);
//			}
//		}
		#endif

	}

	void LiveUpdate(){
		backAnchor.rotation = Quaternion.Lerp (backAnchor.rotation, back_currentRotation,   0.1f );
		
		
		if(isDoubleSpeed == 1 ) {
			hideNitrousParticle();
			UIControl.Static.neddleIndex=0;
		}
		else  showNitrousParticle ();	
		}



	public float rotateDir   ;
	void Update ()
	{

		switch (currentState) {
		case playerstates.playerLive:
			LiveUpdate();
			playerIFixedUpdate();
			break;
		case playerstates.Playerdead:
			 
				 
			 if( isbikefalldown )
			{
				  if( !isUsingNitrous){
					iTween.RotateTo( backAnchor.gameObject,new Vector3(0,50*rotateDir,70*rotateDir),1.0f) ;
					
					iTween.MoveTo(backAnchor.gameObject,thisTrans.position+new Vector3(10 * rotateDir ,0,40)  ,2.0f ) ;

				}
				else {

					iTween.RotateTo( backAnchor.gameObject,new Vector3( 180*rotateDir,0,0),1.0f) ;
					
					iTween.MoveTo(backAnchor.gameObject,thisTrans.position+new Vector3(10 * rotateDir ,15 ,40)  ,2.0f ) ;
					Invoke("bringBikeToGround",0.3f);
				}
					
			 isbikefalldown= false;
			}
		
			break;

		}
		 
	}

	void bringBikeToGround()
	{
		//iTween.RotateTo( backAnchor.gameObject,new Vector3( 270*rotateDir,0,0),1.50f) ;
		iTween.MoveTo(backAnchor.gameObject,thisTrans.position+new Vector3(0 ,90 * rotateDir,60)  ,1.5f ) ; 
		isbikefalldown = true;
		isUsingNitrous = false;
	}

 


	void OnTriggerEnter( Collider c )
	{

		  if( c.tag.Contains("coin" ) )
		{
			 
				coinControl coinScript = c.gameObject.GetComponent<coinControl>() as coinControl ;
				 coinScript.moveToPlayer = true;
		    	Destroy( c );
			  
			    GamePlayController.collectedCoinsCounts++;

		}
		else if( c.GetComponent<Collider>().name.Contains("Magnet" ) )
		{
			Destroy(c.gameObject);
			if(switchOnMagnetPower != null) switchOnMagnetPower(null,null);
			Invoke("EndMagnetPower",magnetPowerTime + PlayerPrefs.GetInt ("MagnetCount", 0));
			SoundController.Static.PlayPowerPickUp();
		}
		else if( c.GetComponent<Collider>().name.Contains("InstantNitrous" ) )
		{
			Destroy(c.gameObject);

			NitrousIndicator.NitrousCount+=30;//+ PlayerPrefs.GetInt ("NitrousCount", 0); // set to nitrous    + PlayerPrefs.GetInt ("NitrousCount", 0);
			NitrousIndicator.Static.UpdateNitrousDisplay();
			SoundController.Static.PlayPowerPickUp();
		}

	}
	void EndMagnetPower( )
	{
		if (switchOFFMagnetPower != null)
						switchOFFMagnetPower (null, null);
	 }

	void OnCollisionEnter(Collision incomingCollision)
	{
		string incTag = incomingCollision.collider.tag;

		if (incTag.Contains ("trafficCar")) {
		 
			carSpeed=0;
			wheelSpeed=0;
			isDoubleSpeed=0;
			turnSpeed=0;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			isDoubleSpeed = 1;
			GamePlayController.isGameEnded = true;
			if(gameEnded != null) gameEnded(null,null);
			iTween.ShakePosition(Camera.main.gameObject,new Vector3(1,1,1),0.6f);
		//	Debug.Log("traffic collision detected " );
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			//rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			SoundController.Static.PlayBIKECrashSound ();
			GameObject trafficCar  = incomingCollision.collider.gameObject ; 
			SoundController.Static.boostAudioControl.enabled = false;
			trafficCar.SendMessage("StopCar",SendMessageOptions.DontRequireReceiver);//to stop the car
			iTween.RotateTo(trafficCar , new Vector3(0,UnityEngine.Random.Range(-1,2)*25,0),1.0f);
			currentState=playerstates.Playerdead;
		 
			 
			GetComponent<Collider>().enabled = false ;

				}
		 

		}
	void FixedUpdate ()
	{
		switch (currentState) {
		case playerstates.playerLive:

			break;
		case playerstates.Playerdead:
			 
			break;
			
		}

	}

	void playerIFixedUpdate() {
		thisPosition = thisTrans.position;

		if (UIControl.isBrakesOn) {
			brakeSpeed = 2;

			UIControl.Static.neddleIndex = 1;
			SoundController.Static.playBrakeSound();
		}
		else {
			brakeSpeed = 1;

		}


		float moveHorizontal = 0;

		// #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

		// float smoothX = Input.acceleration.x ;
		// if(smoothX < -0.5f  && smoothX < 0) smoothX = Mathf.Lerp(smoothX,-1,Time.deltaTime/6);  //smoothX = -1;
		// else if(smoothX > 0.5f  ) smoothX = Mathf.Lerp(smoothX,1,Time.deltaTime/6); ;
		// moveHorizontal = smoothX * 2 ;

#if !UNITY_EDITOR && UNITY_ANDROID

		MoJingMove();
#else
		if (Input.GetMouseButton(0)) {
			float v = 2 * (Input.mousePosition.x / Screen.width) - 1;
			moveHorizontal = v;
		}
#endif


		// #endif

		// #if UNITY_EDITOR || UNITY_WEBPLAYER
		// moveHorizontal =  Input.GetAxis ("Horizontal");
		//  
		// #endif





		foreach (Transform t in wheelObjs) {
			t.Rotate(-wheelSpeed * Time.deltaTime, 0, 0);
		}


		Rigidbody rigidbody = GetComponent<Rigidbody>();


		Vector3 movement = new Vector3(moveHorizontal * 2, 0.0f, (carSpeed * isDoubleSpeed) / brakeSpeed);


		rigidbody.velocity = movement * turnSpeed;

		rigidbody.position = new Vector3
		(
				Mathf.Clamp(rigidbody.position.x, limits[0], limits[1]),
				0.0f, rigidbody.position.z
		);

		//carCamera.position = rigidbody.position - new Vector3(0,-10, 20 );
		rigidbody.rotation = Quaternion.Euler(0, rigidbody.velocity.x * tilt, 0);
		carBody.rotation = Quaternion.Euler(0, 0, -rigidbody.velocity.x * tilt);
	}


	private float HeavyMapping(float val, float f1, float f2, float t1, float t2) {
		val = Mathf.Clamp(val, f1, f2);
		float sd = (val - f1) / (f2 - f1);
		return t1 + (t2 - t1) * sd;
	}

	private void MoJingMove() {
		if (MirrorSdkDataModel.Instance.BodynodeData != null && MirrorSdkDataModel.Instance.BodynodeData.persons != null) {
			try {
				foreach (var target in MirrorSdkDataModel.Instance.BodynodeData.persons) {
					if (target.personId == MirrorSdkDataModel.Instance.BodynodeData.targetPersonId) {
						float mid = (target.skeletons.leftShoulder.x + target.skeletons.rightShoulder.x) / 2;
						float pos_x = HeavyMapping(mid, 0, Screen.width, limits[0], limits[1]);

						pos_x *= 2.5f;

						Rigidbody rigidbody = GetComponent<Rigidbody>();
						rigidbody.position = new Vector3(pos_x, 0.0f, rigidbody.position.z);
					}
				}
			}
			catch (Exception ex) {
				Debug.Log("Unity - 异常 - " + ex.Message);
			}
		}
		else {
			Debug.LogError("Unity - 没有 Body 数据");
		}
	}



	private float xxx() {
		Debug.Log("Unity - 标记 - 1");
		if (MirrorSdkDataModel.Instance.BodynodeData != null && MirrorSdkDataModel.Instance.BodynodeData.persons != null) {
			Debug.Log("Unity - 标记 - 2");
			try {
				foreach (var target in MirrorSdkDataModel.Instance.BodynodeData.persons) {
					Debug.Log("Unity - 标记 - 3");
					if (target.personId == MirrorSdkDataModel.Instance.BodynodeData.targetPersonId) {
						//Debug.Log("Unity - 标记 - 4");
						//Vector2 leftShoulder = new Vector2(target.skeletons.leftShoulder.x, target.skeletons.leftShoulder.y).ReversalPosY();// target.skeletons.leftShoulder.ToV2();
						//Debug.Log("Unity - 标记 - 5 - " + leftShoulder);
						//Vector2 leftAnkle = new Vector2(target.skeletons.leftAnkle.x, target.skeletons.leftAnkle.y).ReversalPosY();// target.skeletons.leftAnkle.ToV2();

						//Debug.Log("Unity - 标记 - 6 - " + leftAnkle);
						//Vector2 rightShoulder = new Vector2(target.skeletons.rightShoulder.x, target.skeletons.rightShoulder.y).ReversalPosY();// target.skeletons.rightShoulder.ToV2();
						//Debug.Log("Unity - 标记 - 7 - " + rightShoulder);
						//Vector2 rightAnkle = new Vector2(target.skeletons.rightAnkle.x, target.skeletons.rightAnkle.y).ReversalPosY();// target.skeletons.rightAnkle.ToV2();

						//Debug.Log("Unity - 标记 - 8 - " + rightAnkle);
						//Vector2 midcourt = leftShoulder + rightShoulder - leftAnkle - rightAnkle;

						//Debug.Log("Unity - 标记 - 9 - " + midcourt);
						//float Angle = Vector2.Angle(midcourt, Vector2.up);

						//Debug.Log("Unity - 标记 - 10 - " + Angle);
						//Angle = Mathf.Clamp(Angle, 0f, 45f);
						//Debug.Log("Unity - 标记 - 11 - " + Angle);
						//Angle = Angle < 10 ? 0 : Angle;
						//Debug.Log("Unity - 标记 - 12 - " + Angle);
						//Angle /= 65f;
						//Debug.Log("Unity - 标记 - 13 - " + Angle);
						//Angle = midcourt.x < 0 ? -Angle : Angle;
						//Debug.Log("Unity - 标记 - 14 - " + Angle);

						Debug.Log("Unity - 标记 - 4 - " + target.skeletons.leftShoulder.x);
						Debug.Log("Unity - 标记 - 5 - " + target.skeletons.rightShoulder.x);
						float v = 2 * ((target.skeletons.leftShoulder.x + target.skeletons.rightShoulder.x) / 2 / Screen.width) - 1;
						return v;
					}
				}
			}
			catch (Exception ex) {
				Debug.Log("Unity - 异常 - " + ex.Message);
			}
		}
		else {
			Debug.LogError("Unity - 没有 Body 数据");
		}
		return 0;
	}

	public void switchoffmagnet()
	{
		if (switchOFFMagnetPower != null)
		{
			switchOFFMagnetPower (null, null);
		}
	}

	bool isUsingNitrous = false ;
	void showNitrousParticle()
	{
		if (particleParent != null)particleParent.SetActive (true);
		
		SoundController.Static.boostAudioControl.enabled = true;
		UIControl.Static.neddleIndex=2;
		UIControl.Static.NitrousParticle.SetActive (true);

		back_currentRotation = Back_RotationTarget; 
		isUsingNitrous = true;
	}
	
	void hideNitrousParticle()
	{
		if (particleParent != null)
			particleParent.SetActive (false);


		back_currentRotation = Quaternion.identity;
		isUsingNitrous = false;
		SoundController.Static.boostAudioControl.enabled = false;
		UIControl.Static.NitrousParticle.SetActive (false);
	}
}
