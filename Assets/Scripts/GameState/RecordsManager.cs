using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ranks
{
    D,C,B,A,S,SS
}

public class RecordsManager : MonoBehaviour
{
    public float gameTimer = 0f;
    public int timesBitten = 0;
    public int timesHealed = 0;
    public int zombiesKilled = 0;
    public Ranks rank;
    private bool isCounting = true;

    // Start is called before the first frame update
    void Start()
    {
        gameTimer = 0f;
        timesBitten = 0;
        timesHealed = 0;
        zombiesKilled = 0;
        isCounting = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCounting)
            gameTimer += Time.deltaTime;
    }

    public void Win()
    {
        isCounting = false;
        if (gameTimer < 120 &&
            timesBitten < 1 &&
            timesHealed < 1)
        {
            rank = Ranks.SS;
        }
        else if (gameTimer < 135 &&
            timesBitten < 1 &&
            timesHealed < 1)
        {
            rank = Ranks.S;
        }
        else if ((gameTimer < 150 &&
            timesBitten < 3) || timesBitten < 1)
        {
            rank = Ranks.A;
        }
        else if ((gameTimer < 240 &&
            timesBitten < 4) || timesBitten < 2)
        {
            rank = Ranks.B;
        }
        else if (gameTimer < 360)
        {
            rank = Ranks.C;
        }
        else
        {
            rank = Ranks.D;
        }
    }
}
