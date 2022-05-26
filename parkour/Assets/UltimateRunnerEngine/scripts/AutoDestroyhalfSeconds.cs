using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyhalfSeconds : MonoBehaviour{

    private float t = 0.5f;
    

    // Update is called once per frame
    void Update(){
        t -= Time.deltaTime;
        if (t < 0){
            Destroy(gameObject);
        }
    }
}
