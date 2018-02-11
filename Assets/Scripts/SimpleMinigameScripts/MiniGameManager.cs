using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    #region Unity bindings

    public GameObject player;
    public Text timeLeft;
    public Text bonusesAquired;

    #endregion

    #region Private fields

    float timer;

    #endregion

    #region Properties

    public static int BonusKnowledge { get; set; }
    public static int BonusBeer { get; set; }
    public MiniGamePlayer MiniPlayer { get { return player.GetComponent<MiniGamePlayer>(); } }

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        timer = 60.0f;
        BonusKnowledge = 0;
        BonusBeer = 0;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        
        if (SceneManager.GetActiveScene().name == "SimpleMinigame")
        {
            bonusesAquired.text = ((int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 20))).ToString();
            //decrement timer;
            timeLeft.text = timer.ToString("##");
            timer = 60f - Time.time;

            if (Time.time >= 60f || player == null)
            {
                EndMiniGame();
            }
        }

    }
    #endregion

    #region Helper Methods

    void EndMiniGame()
    {
        //handle game exit
        BonusBeer = (int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 20));
        BonusKnowledge = (int)System.Math.Floor((double)(MiniGamePlayer.KillCount / 20));
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        Debug.Log("GameOver");
    }
    #endregion
}