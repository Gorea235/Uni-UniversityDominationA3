using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpaceInvadersMinigameManager : MonoBehaviour
{
    #region Unity bindings

    public GameObject player;
    public Text timeLeftText;
    public Text scoreText;

    #endregion

    #region Private fields

    const float maxGameLength = 60f;
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
        timer = maxGameLength; // init the time left
        dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
        dataStorage.ScoreModifier = 1; // set default score modifier
    }

    void Update()
    {
        scoreText.text = dataStorage.CurrentScore.ToString("N0"); // update score text
        timer -= Time.deltaTime; // remove the time left according to how long it's been
        timeLeftText.text = timer.ToString("N0"); // update time left text

        // if the timer ran out or the player was removed, end the game
        if (timer <= 0 || player == null)
        {
            EndMiniGame(false);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Adds a standard kill to the score.
    /// </summary>
    public void AddKill() => dataStorage.AddScore(killScore);

    /// <summary>
    /// Adds a bonus kill to the score.
    /// </summary>
    public void AddBonusKill() => dataStorage.AddScore(bonusKillScore);

    /// <summary>
    /// Ends the minigame. Will apply win score if the player succeeded in beating the
    /// minigame, and switches back to the main game.
    /// </summary>
    /// <param name="succeeded">Whether the player beat the minigame.</param>
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