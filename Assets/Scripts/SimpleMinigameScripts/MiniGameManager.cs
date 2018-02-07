using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    #region Unity bindings

    public GameObject player;
    public Text timeLeft;

    #endregion

    #region Private fields

    float timer = 60.0f;

    #endregion

    #region Properties

    public static int BonusKnowledge { get; set; }
    public static int BonusBeer { get; set; }
    public MiniGamePlayer MiniPlayer { get { return player.GetComponent<MiniGamePlayer>(); } }

    #endregion

    #region MonoBehaviour

    void Update()
    {
        //decrement timer;
        timeLeft.text = timer.ToString("##");
        timer = 60f - Time.time;

        if (Time.time >= 60f)
        {
            //handle game exit
            BonusBeer = (int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 10) / 2);
            BonusKnowledge = (int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 10) / 2);
            Debug.Log("GameOver");
        }
    }

    #endregion
}