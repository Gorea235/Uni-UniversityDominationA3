using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{

    #region Unity bindings

    public GameObject player;

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
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            //handle game exit
            BonusBeer = (int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 10)/2);
            BonusKnowledge = (int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 10)/2);
        }


    }

    #endregion
}