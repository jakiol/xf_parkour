using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public Camera uiCamera;
	public Renderer[] menuButtonRenders;
	public Texture[] buttonTexture;
	public RaycastHit hit;
	public GameObject storeObject;
	public GameObject showcase;
	public GameObject BIKESelection;
	public GameObject totalcoins;
	public GameObject menu;
//	public GameObject bg;
	public GameObject settings;
	public GameObject bgSound;
	public string[] reviewUrls,MoreUrls;
	// Use this for initialization
	void Start () {
//		Debug.Log( "MainMenu.cs is Attached to " + gameObject.name );
		if(PlayerPrefs.GetInt("Sound") == 0){
			bgSound.SetActive (false);
		}
#if UNITY_IPHONE
		// Apple won't allow quit button in their app submission guides 
		menuButtonRenders[4].transform.parent.gameObject.SetActive(false);
#endif
	}

	void Update () {
		if( Input.GetKeyDown(KeyCode.Mouse0) )
		{
			
			MouseDown(Input.mousePosition );
		}
		if( Input.GetKeyUp(KeyCode.Mouse0) )
		{
			
			MouseUp(Input.mousePosition );
		}
	}

	bool isDrag=false;
	void OnGUI()
	{
		
		if(  Event.current.type == EventType.MouseDrag)
		{
			isDrag=true;
		}
		else isDrag=false;
		
		
		// GUI.Label(Rect(0,10,100,90),Input.mousePosition +"" );
	}

	void MouseUp(Vector3 a )
	{
		if(isDrag)return; //to avoid unwanted touches while swipine or mouse draging
		foreach(Renderer r in menuButtonRenders )
		{
			//r.material.mainTexture = buttonTexture[0];
		}
		Ray ray = uiCamera.ScreenPointToRay(a);
		
		if (Physics.Raycast(ray, out hit, 500))
		{

			switch(hit.collider.name)
			{
			case "Play":
				BIKESelection.SetActive(true);
				gameObject.SetActive(false);
				showcase.SetActive(true);
		

				break;
			case "Store":

				storeObject.SetActive(true);
				gameObject.SetActive(false);
			 
				break;
				
			case "more":
				Application.OpenURL("");
				break;
			case "RateUs":
                Application.OpenURL("");
                break;
			case "Quit":
				Application.Quit();
				break;
			}
		}
		
	}

	public void Play(){
		menu.SetActive (false);
		BIKESelection.SetActive(true);
		showcase.SetActive(true);
		totalcoins.SetActive(true);
//		bg.SetActive (false);
	}

	public void Setting(){
		settings.SetActive (true);
	}
	public void Store(){
		storeObject.SetActive (true);
	}
	public void RateUs(){
		Application.OpenURL("");
	}
	public void soundOff(){
		PlayerPrefs.SetInt ("Sound",0);
		bgSound.SetActive (false);
	}
	public void soundOn(){
		bgSound.SetActive (true);
		PlayerPrefs.SetInt ("Sound",1);
	}

	public void Back(){
		settings.SetActive (false);
	}


	void MouseDown(Vector3 a )
	{
		
		Ray ray = uiCamera.ScreenPointToRay(a);
		
		if (Physics.Raycast(ray, out hit, 500))
		{
			SoundController.Static.PlayClickSound();
			//Debug.Log("mouse hit on "+ hit.collider.name);
			switch(hit.collider.name)
			{
			case "Play":
			//	menuButtonRenders[0].material.mainTexture  = buttonTexture[1];
				break;
			case "Store":
			//	menuButtonRenders[1].material.mainTexture  = buttonTexture[1];
				break;

			case "more":
			//	menuButtonRenders[2].material.mainTexture  = buttonTexture[1];
				break;
			case "RateUs":
			//	menuButtonRenders[3].material.mainTexture  = buttonTexture[1];
				break;
			case "Quit":
			//	menuButtonRenders[4].material.mainTexture  = buttonTexture[1];
				break;
			
				
			}

			 
		}
		
	}
}
