using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoQuitApp : MonoBehaviour {

    public static AutoQuitApp ins;

    public int minutes;

    private int s = 0;
    private float _t = 0;
    // Start is called before the first frame update
    void Start() {
        s = minutes * 60;
        ins = this;
    }

    // Update is called once per frame
    void Update() {
        _t += Time.unscaledDeltaTime;
        if (_t >= s || Input.GetKeyUp(KeyCode.Escape)) {
            _t = 0;
            Application.Quit();
        }
    }

    public void Living() {
        _t = 0;
    }
    
    
}
