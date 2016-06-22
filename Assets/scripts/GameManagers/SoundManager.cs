using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public int numberOfSources;
    public float volumeMultiplayer = 1;
    public AudioSource music,computerSounds;

    public List<managedSource> managedAudioSources = new List<managedSource>();
    List<AudioSource> audioSrcs = new List<AudioSource>();

    [System.Serializable]
    public struct managedSource
    {
        public AudioSource AudioSrc;
        public float volumeLimit;
    }

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
        
        //Special handling for the computer hummming since the volume fades in/out and also needs to be managed
        if (GameStateManager.instance.previousState == GameStates.STATE_EMAIL)
        {
            computerSounds.DOKill(false);
            computerSounds.volume = newVol;
        }
        foreach (AudioSource a in audioSrcs)
        {
            a.volume = newVol;
        }
        for (int i=0; i < managedAudioSources.Count;i++)
        {
            managedAudioSources[i].AudioSrc.volume = volumeMultiplayer * managedAudioSources[i].volumeLimit;
        }
    }

    public void playSound(AudioClip sound)
    {
        int c = 0;
        while (c < audioSrcs.Count)
        {
            if (!audioSrcs[c].isPlaying)
            {
                audioSrcs[c].pitch = 1;
                audioSrcs[c].PlayOneShot(sound);
                audioSrcs[c].volume = volumeMultiplayer;
                break;
            }
            else
            {
                c++;
            }
        }
    }

    AudioClip lastMoveSound;

    public void playSound(int type,float pitch=1)//0 for move sounds, 1 for desk sounds
    {
        int c = 0;
        while (c < audioSrcs.Count)
        {
            if (!audioSrcs[c].isPlaying)
            {
                switch (type)
                {
                    case 0:
                        if (pitch == 1)
                        {
                            if (lastMoveSound)
                                moveSounds.Remove(lastMoveSound);
                            var playMe = moveSounds[Random.Range(0, moveSounds.Count)];
                            audioSrcs[c].pitch = pitch;
                            audioSrcs[c].PlayOneShot(playMe);
                            audioSrcs[c].volume = volumeMultiplayer * .4f;
                            if (lastMoveSound)
                                moveSounds.Add(lastMoveSound);

                            lastMoveSound = playMe;
                        }
                        else
                        {
                            audioSrcs[c].pitch = pitch;
                            audioSrcs[c].PlayOneShot(moveSounds[moveSounds.Count - 1]);
                            audioSrcs[c].volume = volumeMultiplayer * .4f;
                        }
                        break;
                    case 1:
                        audioSrcs[c].pitch = pitch;
                        audioSrcs[c].PlayOneShot(deskSounds[Random.Range(0, deskSounds.Count)]);
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