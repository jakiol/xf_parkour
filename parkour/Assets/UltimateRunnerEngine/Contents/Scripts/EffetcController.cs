using System;
using UnityEngine;
using System.Collections;

public class EffetcController : MonoBehaviour{

    public string ID;
    public float customDestroyTime;

    private Transform root;

    public void Setup(Transform effectRoot, Controller.EffetcType effetcType, Controller controller){
		transform.parent = effectRoot;
		root = effectRoot;
		StartCoroutine(CheckIfAlive(effetcType, controller));
	}
    
	
	 IEnumerator CheckIfAlive (Controller.EffetcType effetcType, Controller controller){
		 yield return new WaitForSeconds(Mathf.Max(0.5f, customDestroyTime)); 
		 controller.recoverAnEffect(effetcType, this);
	}


	 private float delTime;
	 private void Start(){
		 delTime = Mathf.Max(0.5f, customDestroyTime);
	 }

	 private void Update(){
		 if (!root){
			 delTime -= Time.deltaTime;
			 if (delTime < 0){
				 Destroy(gameObject);
			 }
		 }
	 }
}
