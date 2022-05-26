using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{

    public Image Tiao;
    public Image Dian;
    public float lf = 0.05f;
    public float rf = 0.05f;
    //public Image ImgWidth;

    public bool delPlayerPrefs;

    private float progress = 0;

    private AsyncOperation mAsyncOperation; //异步加载信息
    void Start()
    {
        if(delPlayerPrefs) PlayerPrefs.DeleteAll();
        StartCoroutine(LoadSceneFunction());
        Sdktest.Instance.notifyDownload();
        // 去查找屏幕尺寸
        Sdktest.Instance.checkOneOrMini();

    }

    void Update()
    {
        if (mAsyncOperation != null)
        {
            progress += Time.deltaTime / 3.6f;
            if (progress > 0.97f && Sdktest.Instance.IsNotifyDownloadOk) {
                if(!mAsyncOperation.allowSceneActivation) mAsyncOperation.allowSceneActivation = true;
            }
        }
        
        Tiao.fillAmount = Mathf.Clamp(progress, lf, rf);
        Dian.rectTransform.anchoredPosition = new Vector3(Tiao.rectTransform.sizeDelta.x * Tiao.fillAmount, Dian.rectTransform.localPosition.y);
    }

    IEnumerator LoadSceneFunction() {
        yield return null;
        mAsyncOperation = SceneManager.LoadSceneAsync("MainMenu");       //ChooseScene为场景C的名称
        mAsyncOperation.allowSceneActivation = false;
    }
    
    
}
