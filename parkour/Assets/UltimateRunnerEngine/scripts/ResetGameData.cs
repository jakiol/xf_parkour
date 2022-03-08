using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameData : MonoBehaviour
{
    public void Reste(){
        PlayerPrefs.SetInt("tutorial", 0);
        Debug.Log("DeleteAll");
    }
}
