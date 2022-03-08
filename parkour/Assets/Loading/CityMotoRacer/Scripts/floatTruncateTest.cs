using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class floatTruncateTest : MonoBehaviour {

	// Use this for initialization
	public Text[] guiText;
	void Start () {

		guiText [1].text = "" +   0.3322244f%100 ;
	}
	
	// Update is called once per frame
	void Update () {
	   
		Start ();
	}
}
