using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeView : MonoBehaviour {
    
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs.DeleteAll();
        button.onClick.AddListener(StartGame);
        Sdktest.Instance.GameState = E_GameState.Home;
    }

    private void StartGame() {
        // SceneManager.LoadScene("highWayGameplay");
        SceneManager.LoadScene("dubaicity");
    }

    private void StartGame(string msg) {
        // SceneManager.LoadScene("highWayGameplay");
        if (msg == "StartGame") {
            SceneManager.LoadScene("dubaicity");
        }
    }



    private void OnDestroy() {
        button.onClick.RemoveAllListeners();
    }
}
