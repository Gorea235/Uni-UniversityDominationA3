using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    #region Unity bindings

    public GameObject player;
    public Text timeLeftText;
    public Text scoreText;

    #endregion

    #region Private fields

    const float maxGameLength = 70f;
    const float killScore = 1f;
    const float bonusKillScore = 10f;
    const float winBonusScore = 50f;

    float timer;
    DataStore dataStorage;
    bool gameEnded;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        timer = maxGameLength;
        dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
        dataStorage.ScoreModifier = 1;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "SimpleMinigame")
        {
            scoreText.text = dataStorage.CurrentScore.ToString("N0");
            //decrement timer;
            timeLeftText.text = timer.ToString("##");
            timer = 60f - Time.time;

            if (Time.time >= 60f || player == null)
            {
                EndMiniGame(false);
            }
        }
    }

    #endregion

    #region Helper Methods

    public void AddKill() => dataStorage.AddScore(killScore);

    public void AddBonusKill() => dataStorage.AddScore(bonusKillScore);

    public void EndMiniGame(bool succeeded)
    {
        if (!gameEnded)
        {
            //handle game exit
            if (succeeded)
                dataStorage.AddScore(winBonusScore); // winning gives bonus score
            dataStorage.SetSucceeded(succeeded);
            SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
            Debug.Log("GameOver");
            gameEnded = true;
        }
    }
    #endregion
}