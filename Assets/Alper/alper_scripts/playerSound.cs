using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSound : MonoBehaviour
{


    [Header("FootSteps")]
    public List<AudioClip> groundSteps;

    private AudioSource footStepSource;

    private void Start()
    {
        footStepSource = GetComponent<AudioSource>();
        Debug.Log(footStepSource.name);
    }
    public void PlayFootSteps()
    {
        AudioClip audioClip = null;
        audioClip = groundSteps[Random.Range(0, groundSteps.Count)];
        footStepSource.clip = audioClip;
        footStepSource.volume = Random.Range(0.02f, 0.05f);
        footStepSource.pitch = Random.Range(0.8f, 1.2f);
        footStepSource.Play();
    }

}
