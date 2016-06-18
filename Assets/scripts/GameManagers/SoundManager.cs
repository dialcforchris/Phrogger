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

    public List<AudioClip> moveSounds,deskSounds;

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

    AudioClip lastMoveSound;

    public void playSound(int type)//0 for move sounds, 1 for desk sounds
    {
        int c = 0;
        while (c < audioSrcs.Count)
        {
            if (!audioSrcs[c].isPlaying)
            {
                switch (type)
                {
                    case 0:
                        if (lastMoveSound)
                            moveSounds.Remove(lastMoveSound);
                        var playMe = moveSounds[Random.Range(0, moveSounds.Count - 1)];
                        audioSrcs[c].PlayOneShot(playMe);
                        audioSrcs[c].volume = volumeMultiplayer * .4f;
                        if (lastMoveSound)
                            moveSounds.Add(lastMoveSound);

                        lastMoveSound = playMe;
                        break;
                    case 1:
                        audioSrcs[c].PlayOneShot(deskSounds[Random.Range(0, deskSounds.Count - 1)]);
                        audioSrcs[c].volume = volumeMultiplayer * 1f;
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