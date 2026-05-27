using UnityEngine;
using TMPro;

public class SharedTimer : MonoBehaviour
{
    public TextMeshProUGUI[] timerTexts;

    public float gameTime = 300f;

    private float remainingTime;

    void Start()
    {
        remainingTime = gameTime;
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime < 0)
            remainingTime = 0;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        string timerString =
            string.Format("{0:00}:{1:00}", minutes, seconds);

        foreach (TextMeshProUGUI text in timerTexts)
        {
            text.text = timerString;

            if (remainingTime <= 30)
            {
                text.color = Color.red;
            }
        }

        if (remainingTime <= 0)
        {
            Debug.Log("GAME OVER");
        }
    }
}