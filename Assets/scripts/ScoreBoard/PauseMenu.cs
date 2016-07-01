using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MenuSelect 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
        SetFunctionToButton();
	}

    protected override void SetFunctionToButton()
    {
          if (selectBox==0)
            {
                DoAction(ChangeVolumeSlide);
            }
        //base.SetFunctionToButton();
        if (Input.GetButtonDown("Fire1"))
        {
            if (selectBox==1)
            {
                DoAction(QuitToMain);
            }
        }
    }

    void QuitToMain()
    {
        GameStateManager.instance.ChangeState(GameStates.STATE_DAYOVER);
        SceneManager.LoadScene(0);
    }

    void ChangeVolumeSlide()
    {
        if (box[0].GetComponentInChildren<Slider>().value > 0 || box[0].GetComponentInChildren<Slider>().value < 1)
        {
          box[0].GetComponentInChildren<Slider>().value += Input.GetAxis("Horizontal") / 100;
        }
    }
}
