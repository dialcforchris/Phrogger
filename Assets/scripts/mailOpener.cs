
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class mailOpener : MonoBehaviour 
{

    public static mailOpener instance;

    //Need to be able to open and close emails at will
    //i.e. have them be seperate events

    public SpriteRenderer emailContent;
    public Animator monitorAnimator,miniEmailAnimator;
    public Camera monitorCamera,mainCam;
    public SpriteRenderer mainCamTransition, monCamTransition;
    public Texture[] gradients;
    public AudioClip[] keypressSounds;
    public AudioSource computerSounds;

    public List<mailColection> messages; //All the possible emails the player might have to deal with

    private mailColection selectedList;
    private mail currentMail;
    
    [System.Serializable]
    public class mailColection
    {
        public string name;
        public List<mail> messages;
        public bool randomSelection;
        public int index;
        public int score;
    }

    //An email object, might need variables for score and such in the future
    [System.Serializable]
    public struct mail
    {
        public Sprite image;
        public bool isJunk;
    }
    void Awake()
    {
        instance = this;
        //Always remember to seed your random :)
        Random.seed = System.DateTime.Now.Millisecond;
    }

    public void enterView()
    {
        //should probably change game manager state to email viewing
        GameStateManager.instance.ChangeState(GameStates.STATE_EMAIL);
        emailPos = 0;

        //Change cameras over
        StartCoroutine(camTransition(true));
    }
    IEnumerator camTransition(bool InOut) //True for entering email mode, false for exiting it
    {
        computerSounds.DOFade((InOut) ? SoundManager.instance.volumeMultiplayer: 0, 2);
        Random.seed = System.DateTime.Now.Millisecond;
        mainCamTransition.material.SetTexture("_SliceGuide", gradients[Random.Range(0, gradients.Length - 1)]);
        monCamTransition.material.SetTexture("_SliceGuide", gradients[Random.Range(0, gradients.Length - 1)]);

        float lerpy = 1;
        while (lerpy > 0)
        {
            lerpy -= Time.deltaTime;

            if (!InOut)
                monCamTransition.material.SetFloat("_SliceAmount", lerpy);
            else
                mainCamTransition.material.SetFloat("_SliceAmount", lerpy);
            
            yield return new WaitForEndOfFrame();
        }

        mainCam.enabled = !InOut;
        monitorCamera.enabled = InOut;

        lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime;

            if (InOut)
                monCamTransition.material.SetFloat("_SliceAmount", lerpy);
            else
                mainCamTransition.material.SetFloat("_SliceAmount", lerpy);

            yield return new WaitForEndOfFrame();
        }

        if (InOut)
        {
            //Intro animation
            monitorAnimator.Play("mail_intro");

            //pick a random list
            selectedList =  messages[Random.Range(0, messages.Count - 1)];
            
            //Remove this email from the list so we can't get it twice
        //    messages.Remove(currentMail);
            messages.TrimExcess();
            if (selectedList.randomSelection)
            {
                //Pick a random message form the selected list
                currentMail = selectedList.messages[Random.Range(0, selectedList.messages.Count - 1)];
                //Remove this email from the list so we can't get it twice
                selectedList.messages.Remove(currentMail);
            }
            else
            {
                currentMail = selectedList.messages[selectedList.index];
                selectedList.index++;
                if (selectedList.index > selectedList.messages.Count - 1)
                    messages.Remove(selectedList);
            }
            emailContent.sprite = currentMail.image;
            
            StopCoroutine("zoomInOut");
            StartCoroutine(zoomInOut(7.5f));
        }
        else
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);
        
    }

    IEnumerator zoomInOut(float newSize)
    {
        yield return new WaitForSeconds(1);
        while (Mathf.Abs(monitorCamera.orthographicSize - newSize) > 0.1f)
        {
            monitorCamera.orthographicSize = Mathf.Lerp(monitorCamera.orthographicSize, newSize, Time.deltaTime);
            if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
            {
                monitorCamera.orthographicSize = 11;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void exitView()
    {
        //Change cameras over
        StartCoroutine(camTransition(false));
    }

    int emailPos = 0;
    public void moveEmail(int dir)
    {
        if (emailPos == 0) //Email is in the middle of the screen
        {
            if (dir > 0)
                monitorAnimator.Play("mail_right");
            else
                monitorAnimator.Play("mail_left");

            emailPos += dir;

            if (!soundPlaying)
            {
                SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length - 1)]);
                soundPlaying = true;
                Invoke("allowSounds", .5f);
            }
        }
        else if (emailPos > 0) //Email currently sits at the right of the screen
        {
            if (dir < 0)//Move left
            {
                monitorAnimator.Play("mail_right_reverse");
                emailPos = 0;//Email is back in the middle now
                if (!soundPlaying)
                {
                    SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length - 1)]);
                    soundPlaying = true;
                    Invoke("allowSounds", .5f);
                }
            }
        }
        else //Email must be at the left hand side of the monitor
        {
            if (dir > 0)//Move right
            {
                monitorAnimator.Play("mail_left_reverse");
                emailPos = 0;//Email is back in the middle now
                if (!soundPlaying)
                {
                    SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length - 1)]);
                    soundPlaying = true;
                    Invoke("allowSounds", .5f);
                }
            }
        }
    }

    void Update()
    {
        InputRelated();
    }

    void allowSounds()
    {
        soundPlaying = false;
    }

    bool soundPlaying;

    void InputRelated()
    {
        //Only allow the player to move the email if it's not moving
        if (monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_idle_middle")
            || monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_idle_left")
            || monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_idle_right"))
        {

            if (Input.GetAxis("Horizontal") != 0)
            {
                moveEmail(Input.GetAxis("Horizontal") > 0 ? 1 : -1);
            }

            if (Input.GetButtonDown("Fire1") && emailPos !=0)
            {
                if (!soundPlaying)
                {
                    SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length - 1)]);
                    soundPlaying = true;
                    Invoke("allowSounds", .5f);
                }
                if (emailPos > 0) //If email is in the JUNK zone
                {
                    //Do animation for email being destroyed
                    monitorAnimator.Play("mail_junk");

                    if (currentMail.isJunk)
                    {
                        Debug.Log("Junk email put in junk pile, good job");
                        //Junk email put in junk pile, good job
                        //+ points
                        StatTracker.instance.scoreToAdd += selectedList.score;
                        StatTracker.instance.junkEmailsCorrect++;
                        BossFace.instance.CheckEmails(true);
                        dayTimer.completedEmail newMail;
                        newMail.junk = true;
                        newMail.correctAnswer = true;
                        dayTimer.instance.todaysEmails.Add(newMail);
                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));

                        Invoke("exitView", 4);
                    }
                    else
                    {
                        Debug.Log("You put a safe email in the junk pile, YOU WALLY");
                        //You put a safe email in the junk pile
                        //oooooo
                        BossFace.instance.CheckEmails(false);
                        StatTracker.instance.scoreToAdd -= (int)(.8f*selectedList.score);
                        StatTracker.instance.safeEmailsWrong++;
                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));

                        Invoke("exitView", 4);
                    }
                }
                else if (emailPos < 0) //If email is in the SAFE zone
                {
                    //Do animation for email being marked as safe
                    monitorAnimator.Play("mail_safe");
                    miniEmailAnimator.Play("email_leave 0");

                    if (currentMail.isJunk)
                    {
                        Debug.Log("You put junk in the safe pile");
                        //You put junk in the safe pile
                        //- points
                        BossFace.instance.CheckEmails(false);
                        StatTracker.instance.scoreToAdd -= 100;
                        StatTracker.instance.junkEmailsWrong++;
                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));

                        Invoke("exitView", 2.5f);
                    }
                    else
                    {
                        Debug.Log("Safe mail was marked as safe, woopee");
                        //Safe mail was marked as safe, woopee
                        //+ points
                        StatTracker.instance.scoreToAdd += 100;
                        StatTracker.instance.safeEmailsCorrect++;
                        BossFace.instance.CheckEmails(true);
                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));
                        Invoke("exitView", 2.5f);
                    }
                }
            }
        }
    }

}

[CustomEditor(typeof(mailOpener))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (EditorApplication.isPlaying)
        {
            //mailOpener myScript = (mailOpener)target;
            if (GUILayout.Button("Enter Monitor View"))
            {
                mailOpener.instance.enterView();
            }
        }
    }
}
