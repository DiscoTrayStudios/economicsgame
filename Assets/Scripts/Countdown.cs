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
    public bool r = false;
    public void Begin()
    {
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
            if (!r)
            {
                int randomInt = Random.Range(0, 2);
            }
            r = false;
        }
    }

	public void reset()
	{
        timeRemaining = 1;
        CountdownBox.SetActive(false);
        r = true;
    }

    public void changeDuration(int timer)
	{
        duration = timer;
	}
}
