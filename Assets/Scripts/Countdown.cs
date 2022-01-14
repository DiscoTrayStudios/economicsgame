using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public int duration = 30;
    public int timeRemaining;
    public bool isCountingDown = false;
    public GameObject CountdownBox;
    public int result = 0;
    public int random = -1;
    public void Begin()
    {
        CountdownBox.GetComponentInChildren<TextMeshProUGUI>().text = "Choose: " + duration + "s";
        if (isCountingDown && duration != -1)
		{
            CancelInvoke();
            isCountingDown = false;
		}
        if (!isCountingDown && duration != -1)
        {
            CountdownBox.SetActive(true);
            Debug.Log("Time Start");
            isCountingDown = true;
            timeRemaining = duration;
            Invoke("_tick", 1f);
        }
    }

    private void _tick()
    {
        timeRemaining--;
        if (timeRemaining > 0)
        {
            CountdownBox.GetComponentInChildren<TextMeshProUGUI>().text = "Choose: " + timeRemaining + "s";
            Invoke("_tick", 1f);
        }
        else if (timeRemaining == 0)
        {
            CountdownBox.GetComponentInChildren<TextMeshProUGUI>().text = "Choose: " + duration + "s";
            CountdownBox.SetActive(false);
            isCountingDown = false;
            Debug.Log("Time's Up!");
            if (result == -1)
			{
                random = Random.Range(0, 2);
			}
            if (result == 1 || random == 1)
			{
                GameManager.Instance.agree();
			}
            else
            {
                GameManager.Instance.decline();
            }
        }
    }

	public void reset()
	{
        timeRemaining = 1;
        CountdownBox.SetActive(false);
    }

    public void changeDuration(int timer)
	{
        duration = timer;
	}

    public void changeResult(int r)
	{
        result = r;
	}
}
