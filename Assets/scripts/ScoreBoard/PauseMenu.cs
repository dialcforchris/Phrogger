using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MenuSelect 
{
    Slider slide;
	// Use this for initialization
	void Start ()
    {
        slide = box[1].GetComponentInChildren<Slider>();
	}
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
        SetFunctionToButton();
    }

    protected override void SetFunctionToButton()
    {
        if (selectBox == 0)
        {
            if (Input.GetButtonDown("Fire1"))
                GameStateManager.instance.ChangeState(GameStateManager.instance.previousState);
        }
        else if (selectBox == 1)
        {
            DoAction(ChangeVolumeSlide);
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (selectBox == 2)
            {
                DoAction(QuitToMain);
            }
        }
    }

    void QuitToMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    void ChangeVolumeSlide()
    {
        if (slide.value > 0 || slide.value < 1)
        {
            if (Input.GetJoystickNames().Length > 0)
            {
                if (Input.GetJoystickNames()[0] == "")
                    box[1].GetComponentInChildren<Slider>().value += Input.GetAxis("Horizontal") * 10;
                else
                    box[1].GetComponentInChildren<Slider>().value += Input.GetAxis("Horizontal") / 100;
            }
            else
                box[1].GetComponentInChildren<Slider>().value += Input.GetAxis("Horizontal") * 10;

            SoundManager.instance.changeVolume(slide.value);
        }
    }
}
