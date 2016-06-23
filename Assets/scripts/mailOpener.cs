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
    public Slider angerMeter;
    public bool activeCountdown;
    Vector2 angerMeterOrigin;
    public ParticleSystem angryParticles;
    public List<mailColection> messages; //All the possible emails the player might have to deal with

    public float multi;

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
        angerMeterOrigin = angerMeter.GetComponent<RectTransform>().anchoredPosition;
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
        if (InOut)
        {
            pop = false;
            angerMeter.value = 0;
        }
        computerSounds.DOFade((InOut) ? SoundManager.instance.volumeMultiplayer : 0, 2);
        Random.seed = System.DateTime.Now.Millisecond;
        mainCamTransition.material.SetTexture("_SliceGuide", gradients[Random.Range(0, gradients.Length)]);
        monCamTransition.material.SetTexture("_SliceGuide", gradients[Random.Range(0, gradients.Length)]);

        float lerpy = 1;
        while (lerpy > 0)
        {
            lerpy -= Time.deltaTime * 2;

            if (!InOut)
                monCamTransition.material.SetFloat("_SliceAmount", lerpy);
            else
                mainCamTransition.material.SetFloat("_SliceAmount", lerpy);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.1f);
        mainCam.enabled = !InOut;
        monitorCamera.enabled = InOut;

        lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime*2;

            if (InOut)
                monCamTransition.material.SetFloat("_SliceAmount", lerpy);
            else
                mainCamTransition.material.SetFloat("_SliceAmount", lerpy);

            yield return new WaitForEndOfFrame();
        }

        if (InOut)
        {
            activeCountdown = true;
            //Intro animation
            monitorAnimator.Play("mail_intro");
            pickEmail();

            StopCoroutine("zoomInOut");
            StartCoroutine(zoomInOut(7.5f));
        }
        else
        {
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);
        }
        angryParticles.Stop();
    }

    int nonFrogMail=0;
    bool frogStory = true;
    void pickEmail()
    {
        if (nonFrogMail > 5 && frogStory)
        {
            nonFrogMail = 0;
            selectedList = messages[0];
        }
        else
        {
            //Pick a random list of emails to display messages from
            selectedList = messages[Random.Range(1, messages.Count)];
            nonFrogMail++;
        }

        if (selectedList.randomSelection)
        {
            //Pick a random message form the selected list
            currentMail = selectedList.messages[Random.Range(0, selectedList.messages.Count)];
            //Remove this email from the list so we can't get it twice
            selectedList.messages.Remove(currentMail);
            if (selectedList.messages.Count == 0)
            {
                messages.Remove(selectedList);
            }
        }
        else
        {
            currentMail = selectedList.messages[selectedList.index];
            selectedList.index++;
            if (selectedList.index > selectedList.messages.Count)
                messages.Remove(selectedList);
        }

        emailContent.sprite = currentMail.image;
    }

    IEnumerator zoomInOut(float newSize)
    {
        yield return new WaitForSeconds(1);
        while (Mathf.Abs(monitorCamera.orthographicSize - newSize) > 0.1f)
        {
            monitorCamera.orthographicSize = Mathf.Lerp(monitorCamera.orthographicSize, newSize, Time.deltaTime*2);
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
                SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length)]);
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
                    SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length)]);
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
                    SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length)]);
                    soundPlaying = true;
                    Invoke("allowSounds", .5f);
                }
            }
        }
    }

    bool pop;
    void Update()
    {
        float shift = ((angerMeter.value* angerMeter.value* angerMeter.value) / angerMeter.maxValue)*multi;
        angerMeter.GetComponent<RectTransform>().anchoredPosition = angerMeterOrigin + new Vector2(Mathf.Sin(Random.value) * shift, Mathf.Sin(Random.value) * shift);
        if (angerMeter.value ==angerMeter.maxValue && !pop)
        {
            StartCoroutine(AngerMeterPop());
        }
        InputRelated();
    }

    IEnumerator AngerMeterPop()
    {
        pop = true;
        while (multi < 0.1f)
        {
            multi += Time.deltaTime * 0.1f;
            yield return new WaitForEndOfFrame();
        }
        angryParticles.Emit(150);
        angryParticles.Play();
        while (multi >0 )
        {
            multi -= Time.deltaTime*0.2f;
            yield return new WaitForEndOfFrame();
        }
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
                    SoundManager.instance.playSound(keypressSounds[Random.Range(0, keypressSounds.Length)]);
                    soundPlaying = true;
                    Invoke("allowSounds", .5f);
                }
                if (emailPos > 0) //If email is in the JUNK zone
                {
                    activeCountdown = false;
                    BossFace.instance.addEmailAngerXP();
                    if (selectedList == messages[0])
                        frogStory = false;

                    //Do animation for email being destroyed
                    monitorAnimator.Play("mail_junk");

                    if (currentMail.isJunk)
                    {
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

                        Invoke("exitView",3.5f);
                    }
                    else
                    {
                        //You put a safe email in the junk pile
                        //oooooo
                        StatTracker.instance.scoreToAdd -= (int)(.8f*selectedList.score);
                        StatTracker.instance.safeEmailsWrong++;
                        BossFace.instance.CheckEmails(false);
                        dayTimer.completedEmail newMail;
                        newMail.junk = false;
                        newMail.correctAnswer = false;
                        dayTimer.instance.todaysEmails.Add(newMail);

                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));

                        Invoke("exitView", 3.5f);
                    }
                }
                else if (emailPos < 0) //If email is in the SAFE zone
                {
                    activeCountdown = false;
                    BossFace.instance.addEmailAngerXP();
                    //Do animation for email being marked as safe
                    monitorAnimator.Play("mail_safe");
                    miniEmailAnimator.Play("email_leave 0");

                    if (currentMail.isJunk)
                    {
                        //You put junk in the safe pile
                        //- points
                        StatTracker.instance.scoreToAdd -= (int)(.8f * selectedList.score);
                        StatTracker.instance.junkEmailsWrong++;
                        BossFace.instance.CheckEmails(false);
                        dayTimer.completedEmail newMail;
                        newMail.junk = true;
                        newMail.correctAnswer = false;
                        dayTimer.instance.todaysEmails.Add(newMail);

                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));

                        Invoke("exitView", 2.5f);
                    }
                    else
                    {
                        //Safe mail was marked as safe, woopee
                        //+ points
                        StatTracker.instance.scoreToAdd += selectedList.score;
                        StatTracker.instance.safeEmailsCorrect++;
                        BossFace.instance.CheckEmails(true);
                        dayTimer.completedEmail newMail;
                        newMail.junk = false;
                        newMail.correctAnswer = true;
                        dayTimer.instance.todaysEmails.Add(newMail);

                        StopCoroutine("zoomInOut");
                        StartCoroutine(zoomInOut(11));

                        Invoke("exitView", 2.5f);
                    }
                }
            }
        }
    }
}
