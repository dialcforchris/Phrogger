using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class mailOpener : MonoBehaviour {

    //Need to be able to open and close emails at will
    //i.e. have them be seperate events

    public Image canvasFade;
    public Animator emailAnimator;

    int emailPos=0;

    public void moveEmail(int dir)
    {
        //if middle, play whichever animation for the correct direction

        Debug.Log(emailPos);

        if (emailPos == 0)
        {
            if (dir > 0)
                emailAnimator.Play("mail_right");
            else
                emailAnimator.Play("mail_left");

            emailPos += dir;
        }
        else if (emailPos > 0)
        {
            Debug.Log("mail_right_reverse");
            if (dir < 0)
            {
                emailAnimator.Play("mail_right_reverse");
                emailPos = 0;
            }
        }
        else
        {
            Debug.Log("mail_left_reverse");
            if (dir > 0)
            {
                emailAnimator.Play("mail_left_reverse");
                emailPos = 0;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveEmail(-1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveEmail(1);
        }
    }

    #region ye olde system
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
    }
    #endregion
}
