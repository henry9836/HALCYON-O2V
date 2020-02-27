using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zune : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();
    AudioSource aSource;

    private void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!aSource.isPlaying)
        {
            aSource.clip = clips[Random.Range(0, clips.Count)];
            aSource.Play();
        }
    }
}
