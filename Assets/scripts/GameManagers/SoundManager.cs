using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public int numberOfSources;
    public float volumeMultiplayer = 1;
    public AudioSource music;
    List<AudioSource> audioSrcs = new List<AudioSource>();

    public AudioClip[] explosionSounds, hitSounds;

    void Awake()
    {
        for (int i = 0; i < numberOfSources; i++)
        {
            audioSrcs.Add(gameObject.AddComponent<AudioSource>());
        }
        instance = this;
    }

    public void changeVolume(float newVol)
    {
        volumeMultiplayer = newVol;
        //nothing atm
    }

    public void playSound(AudioClip sound)
    {
        int c = 0;
        while (c < audioSrcs.Count)
        {
            if (!audioSrcs[c].isPlaying)
            {
                audioSrcs[c].PlayOneShot(sound);
                audioSrcs[c].volume = volumeMultiplayer;// * .6f;
                break;
            }
            else
            {
                c++;
            }
        }
    }

    public void playSound(int type)//1 for explosions,0 for hit sounds.
    {
        int c = 0;
        while (c < audioSrcs.Count)
        {
            if (!audioSrcs[c].isPlaying)
            {
                switch (type)
                {
                    case 0:
                        audioSrcs[c].PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)]);
                        audioSrcs[c].volume = volumeMultiplayer * .4f;
                        break;
                    case 1:
                        audioSrcs[c].PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length - 1)]);
                        audioSrcs[c].volume = volumeMultiplayer * .8f;
                        break;
                }
                break;
            }
            else
            {
                c++;
            }
        }
    }
}