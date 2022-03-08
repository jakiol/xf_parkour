using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpalshScreen : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        SceneManager.LoadSceneAsync("MainMenu");
		}
	
	public GameObject logo;

}
