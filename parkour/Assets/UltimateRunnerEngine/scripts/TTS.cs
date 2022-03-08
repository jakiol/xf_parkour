using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTS : MonoBehaviour {

    public static TTS ins;

    public Transform gameAudio;
    
    public AudioClip jump_squat;
    
    [Space(10)]
    [SerializeField]
    private List<AudioClip> tutorialAudio;
    
    private AudioSource audioSource;

    private AudioSource[] gameAudioSources;
    private float[] volumes;
    
    // Start is called before the first frame update
    void Awake() {
        ins = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        gameAudioSources = gameAudio.GetComponentsInChildren<AudioSource>();
        volumes = new float[gameAudioSources.Length];
        for (int i = 0; i < volumes.Length; i++) {
            volumes[i] = gameAudioSources[i].volume;
        }
    }

    public void playJumpSquat() {
        StopAllCoroutines();
        StartCoroutine(scaleGameAudio(jump_squat));
    }

    public void PlayTTS(AudioClip clip)
    {
        StopAllCoroutines();
        StartCoroutine(scaleGameAudio(clip));
    }


    public void playTutorial(int idx) {
        StopAllCoroutines();
        StartCoroutine(scaleGameAudio(tutorialAudio[idx]));
    }

    private IEnumerator scaleGameAudio(AudioClip clip) {
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        float length = clip.length;
        float t = 0f;
        while (length > 0) {
            length -= Time.deltaTime;
            t += Time.deltaTime;
            if (t >= 1f) t = 1f;
            for (int i = 0; i < volumes.Length; i++) {
                gameAudioSources[i].volume = Mathf.Lerp(gameAudioSources[i].volume, volumes[i] * 0.2f,  t);
            }
            yield return null;
        }
        // yield return new WaitForSeconds(1);
        t = 0f;
        while (t < 1) {
            t += Time.deltaTime;
            for (int i = 0; i < volumes.Length; i++) {
                gameAudioSources[i].volume = Mathf.Lerp(gameAudioSources[i].volume, volumes[i],  t);
            }
            yield return null;
        }
    }
    
}
