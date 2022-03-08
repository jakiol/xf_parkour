using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BIKESelection : MonoBehaviour {

	// Use this for initialization

	public Camera uiCamera;
	public Renderer[] buttonRenders;
	public Texture[] buttonTexture;
	public RaycastHit hit;
	public GameObject buyButton,playButton;
	public GameObject buyPopUp,InAPPMenu;
	public GameObject bikeselection;
	public GameObject showcase;
//	public GameObject bg;
	public GameObject loadingLevelObj;
	public GameObject totalcoins;
	public GameObject bike1;
	public GameObject bike2;
	public GameObject bike3;
	public GameObject bike4;
	public GameObject bike5;

	public static int BIKEIndex=0;
	public GameObject[] BIKEMeshObjs;
	//public TextMesh BIKESpeedDisplayText,BIKEPriceDisplayText,headingText;
	public Text BIKESpeedDisplayText,BIKEPriceDisplayText,headingText;
	void Start () 
	{
		
		//Debug.Log( "BIKESelection.cs is Attached to " + gameObject.name );
		BIKEPriceDisplayText.text = " FREE ";
#if UNITY_EDITOR || UNITY_WEBPLAYER
		//TotalCoins.staticInstance.ClearCoins(); //allot some coins to test it 
		//totalcoins.SetActive(f);
#endif

	}
	public GameObject menuObj;
	void Update () {
		if( Input.GetKeyDown(KeyCode.Mouse0) )
		{
			
			MouseDown(Input.mousePosition );
		}
		if( Input.GetKeyUp(KeyCode.Mouse0) )
		{
			
			MouseUp(Input.mousePosition );
		}
		

		if( Input.GetKeyUp(KeyCode.P) )
		{
			TotalCoins.staticInstance.AddCoins(999999);
		}
		if( Input.GetKeyUp(KeyCode.Q) )
		{
			TotalCoins.staticInstance.ClearCoins();
		}
	}
	
	
	
	void MouseUp(Vector3 a )
	{
		foreach(Renderer r in buttonRenders )
		{
			//r.material.mainTexture = buttonTexture[0];
		}
		Ray ray = uiCamera.ScreenPointToRay(a);
		
		if (Physics.Raycast(ray, out hit, 500))
		{

			switch(hit.collider.name)
			{
			case "next":
				showNextBIKE();
				break;
			case "previous":
				showPreviousBIKE();
				break;
				
			case "play":
				 
				loadingLevelObj.SetActive (true);
				gameObject.SetActive (false);
				totalcoins.SetActive (false);
				 
				break;
			case "buyBIKE" :

				purchaseBIKEs();
				break;
			case "back" :

				menuObj.SetActive(true);
				gameObject.SetActive(false);
				totalcoins.SetActive(false);

				break;
				
			}

		}
		
	}
	void MouseDown(Vector3 a )
	{
		
		Ray ray = uiCamera.ScreenPointToRay(a);
		
		if (Physics.Raycast(ray, out hit, 500))
		{
			SoundController.Static.PlayClickSound();
			Debug.Log("mouse hit on "+ hit.collider.name);
			switch(hit.collider.name)
			{
			case "next":
				//buttonRenders[0].material.mainTexture  = buttonTexture[1];
				break;
			case "previous":
			//	buttonRenders[1].material.mainTexture  = buttonTexture[1];
				break;
				
			case "play":
				//buttonRenders[2].material.mainTexture  = buttonTexture[1];
				break;
			case "buyBIKE" :
				//buttonRenders[3].material.mainTexture  = buttonTexture[1];
				break;
			case "wwq":
				//buttonRenders[4].material.mainTexture  = buttonTexture[1];
				break;
				
			}
			
			
		}
		
	}
	public void play()
	{
		loadingLevelObj.SetActive (true);
		bikeselection.SetActive (false);
		totalcoins.SetActive (false);
		showcase.SetActive (false);
	}
	public void Back(){
		menuObj.SetActive(true);
		bikeselection.SetActive(false);
		totalcoins.SetActive(false);
//		bg.SetActive (true);
		showcase.SetActive (false);
	}



	public void showNextBIKE()
	{
		BIKEIndex++;
		Debug.Log (BIKEIndex);
		if( BIKEIndex > BIKEMeshObjs.Length-1 ) BIKEIndex=0;
		for( int BIKECount=0 ; BIKECount<= BIKEMeshObjs.Length-1; BIKECount ++ )
		{
			BIKEMeshObjs[BIKECount].SetActive(false);
			
		}
		BIKEMeshObjs[BIKEIndex].SetActive(true);
		showBIKEINFO();
	}
	public void showPreviousBIKE()
	{
		BIKEIndex--;
		if( BIKEIndex < 0 ) BIKEIndex=BIKEMeshObjs.Length-1;
		for( int BIKECount=0 ; BIKECount<= BIKEMeshObjs.Length-1; BIKECount ++ )
		{
			BIKEMeshObjs[BIKECount].SetActive(false);
			
		}
		BIKEMeshObjs[BIKEIndex].SetActive(true);
		showBIKEINFO();
	}
	void OnEnable()
	{
		if(BIKEIndex ==0 ) return;
		if( PlayerPrefs.GetInt("isBIKE"+BIKEIndex+"Purchased",0) == 1 )
		{
			playButton.SetActive(true);
			buyButton.SetActive(false);
		}
		else{
			buyButton.SetActive(true);
			playButton.SetActive(false);
		}
		 
		 
	}
	void showBIKEINFO()
	{

		switch(BIKEIndex)
		{
		case 0:
			BIKEPriceDisplayText.text = "    FREE ";
			bike1.SetActive(true);
			bike2.SetActive(false);
			bike3.SetActive(false);
			bike4.SetActive(false);
			bike5.SetActive(false);
			playButton.SetActive(true);
			buyButton.SetActive(false);
			break;
		case 1:
			BIKEPriceDisplayText.text = "   5000 ";
			bike1.SetActive(false);
			bike2.SetActive(true);
			bike3.SetActive(false);
			bike4.SetActive(false);
			bike5.SetActive(false);
			if(PlayerPrefs.GetInt("isBIKE1Purchased",0) == 1 )
			{
				playButton.SetActive(true);
				buyButton.SetActive(false);
			}
			else{
				buyButton.SetActive(true);
				playButton.SetActive(false);
			}
			break;
		case 2:
			BIKEPriceDisplayText.text = "   7000 ";
			bike1.SetActive(false);
			bike2.SetActive(false);
			bike3.SetActive(true);
			bike4.SetActive(false);
			bike5.SetActive(false);
			
			if(PlayerPrefs.GetInt("isBIKE2Purchased",0) == 1 )
			{
				playButton.SetActive(true);
				buyButton.SetActive(false);
			}
			else{
				buyButton.SetActive(true);
				playButton.SetActive(false);
			}
			break;
		case 3:
			BIKEPriceDisplayText.text = "   9000 ";
			bike1.SetActive(false);
			bike2.SetActive(false);
			bike3.SetActive(false);
			bike4.SetActive(true);
			bike5.SetActive(false);
			if(PlayerPrefs.GetInt("isBIKE3Purchased",0) == 1 )
			{
				playButton.SetActive(true);
				buyButton.SetActive(false);
			}
			else{
				buyButton.SetActive(true);
				playButton.SetActive(false);
			}
			break;
		case 4:
			BIKEPriceDisplayText.text = "   10,000 ";
			bike1.SetActive(false);
			bike2.SetActive(false);
			bike3.SetActive(false);
			bike4.SetActive(false);
			bike5.SetActive(true);
			if(PlayerPrefs.GetInt("isBIKE4Purchased",0) == 1 )
			{
				playButton.SetActive(true);
				buyButton.SetActive(false);
			}
			else{
				buyButton.SetActive(true);
				playButton.SetActive(false);
			}
			break;
		case 5:
			BIKEPriceDisplayText.text = "  12,000 ";
	
			if(PlayerPrefs.GetInt("isBIKE5Purchased",0) == 1 )
			{
				playButton.SetActive(true);
				buyButton.SetActive(false);
			}
			else{
				buyButton.SetActive(true);
				playButton.SetActive(false);
			}
			break;
		}

	}

	public void purchaseBIKEs()
	{

		switch(BIKEIndex)
		{
		case 1:

			if( TotalCoins.staticInstance.totalCoins >= 5000 )
			{
				buyPopUP.BIKECost=1000;//to set the cost in buyPopUpScript
				buyPopUp.SetActive (true);
				//gameObject.SetActive(false);
			}
			else {
				InAPPMenu.SetActive(true);
					//gameObject.SetActive(false);
			}
			 
			break;
		case 2:
			if( TotalCoins.staticInstance.totalCoins >= 7000 )
			{
				buyPopUP.BIKECost=3000;
				buyPopUp.SetActive(true);
				//gameObject.SetActive(false);
			}
			else {
				InAPPMenu.SetActive(true);
				//gameObject.SetActive(false);
			}
			
			break;
		case 3:
			if( TotalCoins.staticInstance.totalCoins >= 9000 )
			{
				buyPopUP.BIKECost=4000;
				buyPopUp.SetActive(true);
				//gameObject.SetActive(false);
			}
			else {
				InAPPMenu.SetActive(true);
				//gameObject.SetActive(false);
			}
			
			break;
		case 4:
			if( TotalCoins.staticInstance.totalCoins >= 10000 )
			{
				buyPopUP.BIKECost=5000;
				buyPopUp.SetActive(true);
				//gameObject.SetActive(false);
			}
			else {
				InAPPMenu.SetActive(true);
				//gameObject.SetActive(false);
			}
			
			break;
		case 5:
			if( TotalCoins.staticInstance.totalCoins >= 12000 )
			{
				buyPopUP.BIKECost=6000;
				buyPopUp.SetActive(true);
				//gameObject.SetActive(false);
			}
			else {
				InAPPMenu.SetActive(true);
				//gameObject.SetActive(false);
			}
			
			break;
		}

	}
}
