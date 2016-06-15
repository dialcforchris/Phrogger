using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class mailOpener : MonoBehaviour {

    //Need to be able to open and close emails at will
    //i.e. have them be seperate events

    public SpriteRenderer emailContent;
    public Animator monitorAnimator,miniEmailAnimator;

    public mail[] messages;
    mail currentMail;
    
    [System.Serializable]
    public struct mail
    {
        public Sprite image;
        public bool isJunk;
    }

    void Awake()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        enterView();
    }

    public void enterView()
    {
        int chosen = Random.Range(0, messages.Length);
        currentMail = messages[chosen];
        emailContent.sprite = currentMail.image;        
    }

    int emailPos=0;
    public void moveEmail(int dir)
    {
        if (!monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_right_reverse")
            && !monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_left_reverse")
            && !monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_left")
            && !monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("mail_right"))
        {
            if (emailPos == 0)
            {
                if (dir > 0)
                    monitorAnimator.Play("mail_right");
                else
                    monitorAnimator.Play("mail_left");

                emailPos += dir;
            }
            else if (emailPos > 0)
            {
                if (dir < 0)
                {
                    monitorAnimator.Play("mail_right_reverse");
                    emailPos = 0;
                }
            }
            else
            {
                if (dir > 0)
                {
                    monitorAnimator.Play("mail_left_reverse");
                    emailPos = 0;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveEmail(-1);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveEmail(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (emailPos>0)
            {
                //Do animation for email being destroyed
                if (currentMail.isJunk)
                {
                    //Junk email put in junk pile, good job
                    //+ points
                }
                else
                {
                    //You put a safe email in the junk pile
                    //oooooo
                }
            }
            else if (emailPos < 0)
            {
                monitorAnimator.Play("mail_safe");
                miniEmailAnimator.Play("email_leave 0");
                //Do animation for email being marked as safe
                if (currentMail.isJunk)
                {
                    //You put junk in the safe pile
                    //- points
                }
                else
                {
                    //Safe mail was marked as safe, woopee
                    //+ points
                }
            }
        }
    }

    #region ye olde system
    /*
    public void openMail()
    {
        //Turn game manager to paused, don't let the player move or anything like that
        StartCoroutine(mail());
    }
    public void closeMail()
    {
        emailAnimator.SetBool("finished", true);
        emailAnimator.SetBool("openMail", false);
    }

    IEnumerator mail()
    {
        while (canvasFade.color.a < .5f)
        {
            canvasFade.color = Color.Lerp(canvasFade.color, new Color(0, 0, 0, .6f), 0.1f);
            yield return new WaitForSeconds(.05f);
        }
        emailAnimator.SetBool("finished", false);
        emailAnimator.SetBool("openMail", true);

        while (!emailAnimator.GetBool("finished"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        while (canvasFade.color.a > 0f)
        {
            canvasFade.color = Color.Lerp(canvasFade.color, new Color(0, 0, 0, 0f), 0.1f);
            yield return new WaitForSeconds(.05f);
        }
    }

    public void closeEmail()
    {
        canvasFade.color = new Color(0, 0, 0, 0f);
    }*/
    #endregion
}
