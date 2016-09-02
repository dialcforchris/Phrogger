using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public int numberOfSources;
    public float volumeMultiplayer = 1,musicVolume=1;
    public AudioSource music,bossMusic,officeAmbience;

    public List<managedSource> managedAudioSources = new List<managedSource>();
    List<AudioSource> audioSrcs = new List<AudioSource>();

    [System.Serializable]
    public class managedSource
    {
        public AudioSource AudioSrc;
        public float volumeLimit;
    }

    public List<AudioClip> moveSounds,deskSounds,carHorns;

    void Awake()
    {
        for (int i = 0; i < numberOfSources; i++)
        {
            audioSrcs.Add(gameObject.AddComponent<AudioSource>());
        }
        instance = this;
    }
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY||GameStateManager.instance.GetState()==GameStates.STATE_PAUSE)
        {
            managedAudioSources[0].AudioSrc.Stop();
            managedAudioSources[1].AudioSrc.Stop();
        }

        /*if (officeAmbience.isPlaying)
            officeAmbience.volume = 0.5f;*/

    }
    public void StopSound(AudioClip sound)
    {
        foreach (AudioSource a in audioSrcs)
        {
            if (a.clip == sound)
            {
                a.Stop();
            }
        }
    }
    public void changeVolume(float newVol)
    {
        volumeMultiplayer = newVol;

        //Special handling for the computer hummming since the volume fades in/out and also needs to be managed
        if (GameStateManager.instance.previousState == GameStates.STATE_EMAIL)
        {
            managedAudioSources[0].AudioSrc.DOKill(false);
            officeAmbience.DOKill(false);
        }
        else if (GameStateManager.instance.previousState == GameStates.STATE_DAYOVER)
            officeAmbience.volume = 0;

        if (GameStateManager.instance.previousState == GameStates.STATE_GAMEPLAY)
            officeAmbience.volume = newVol;
        else if (GameStateManager.instance.previousState == GameStates.STATE_EMAIL)
            officeAmbience.volume = newVol * 0.3f;

        foreach (AudioSource a in audioSrcs)
        {
            a.volume = newVol;
        }
        for (int i = 0; i < managedAudioSources.Count;i++)
        {
            managedAudioSources[i].AudioSrc.volume = volumeMultiplayer * managedAudioSources[i].volumeLimit;
        }      
    }

    public void playSound(AudioClip sound,float volume = 1)
    {
        int c = 0;
        //changeVolume(volume);
        while (c < audioSrcs.Count)
        {
            if (!audioSrcs[c].isPlaying)
            {
                audioSrcs[c].clip = sound;
                audioSrcs[c].pitch = 1;
                audioSrcs[c].PlayOneShot(sound);
                audioSrcs[c].volume = volume * volumeMultiplayer;
                break;
            }
            if (audioSrcs[c].isPlaying && c == (audioSrcs.Count - 1))
            {
                audioSrcs.Add(gameObject.AddComponent<AudioSource>());
            }
            else
            {
                c++;
            }
        }
    }
    public bool IsSoundPlaying(AudioClip clip)
    {
        foreach (AudioSource a in audioSrcs)
        {
            if (a.clip == clip)
            {
                return true;
            }
        }
        return false;
    }

    public void PauseSound(AudioClip clip,bool pause)
    {
        foreach (AudioSource a in audioSrcs)
        {
            if (a.clip == clip)
            {
               if (pause)
               {
                   a.Pause();
               }
               else
               {
                   a.UnPause();
               }
            }
        }
    }

    AudioClip lastMoveSound;

    public void playSound(int type,float pitch=1)//0 for move sounds, 1 for desk sounds, 2 for car horns
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
                            audioSrcs[c].volume = volumeMultiplayer * .4f;
                            audioSrcs[c].PlayOneShot(moveSounds[moveSounds.Count - 1]);
                        }
                        break;
                    case 1:
                        audioSrcs[c].pitch = pitch;
                        audioSrcs[c].PlayOneShot(deskSounds[Random.Range(0, deskSounds.Count)]);
                        audioSrcs[c].volume = volumeMultiplayer * 1f;
                        break;
                    case 2:
                        audioSrcs[c].pitch = pitch;
                        audioSrcs[c].PlayOneShot(carHorns[Random.Range(0, carHorns.Count)]);
                        audioSrcs[c].volume = volumeMultiplayer * 1f;
                        break;
                }
                break;
            }
            if (audioSrcs[c].isPlaying && c == (audioSrcs.Count - 1))
            {
                audioSrcs.Add(gameObject.AddComponent<AudioSource>());
            }
            else
            {
                c++;
            }
        }
    }
}