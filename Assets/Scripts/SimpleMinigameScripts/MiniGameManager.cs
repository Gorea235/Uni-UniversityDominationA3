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
    DataStore _dataStorage;

    #endregion

    #region Properties

    public MiniGamePlayer MiniPlayer { get { return player.GetComponent<MiniGamePlayer>(); } }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        timer = 60.0f;
        _dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
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
                EndMiniGame(false);
            }
        }

    }

    #endregion

    #region Helper Methods

    public void EndMiniGame(bool succeeded)
    {
        //handle game exit
        _dataStorage.AddScore(MiniGamePlayer.KillCount);
        _dataStorage.SetSucceeded(succeeded);
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        Debug.Log("GameOver");
    }
    #endregion
}