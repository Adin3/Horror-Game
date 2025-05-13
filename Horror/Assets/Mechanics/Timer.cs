using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text timerText;
    public float startTime = 40f;

    private float remainingTime;
    private bool timerRunning = true;
    private bool isFlickering = false;

    void Start()
    {
        remainingTime = startTime;
    }

    void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 30f && !isFlickering)
            {
                timerText.color = new Color(1f, 0.016f, 0.243f);
                StartCoroutine(FlickerText());
            }

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                timerRunning = false;
                StopAllCoroutines(); 
                timerText.enabled = true;
                Debug.Log("Timpul a expirat!");
            }

            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    IEnumerator FlickerText()
    {
        isFlickering = true;
        while (true)
        {
            timerText.enabled = !timerText.enabled; 
            yield return new WaitForSeconds(0.5f); 
        }
    }
}
