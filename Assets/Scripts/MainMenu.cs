using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Unity Bindings

    //get dropdown objects
    public GameObject playerOneType;
    public GameObject playerTwoType;
    public GameObject playerThreeType;
    public GameObject playerFourType;
    public GameObject startButton;
    public GameObject errorPanel;
    public Button loadGameButton;

    #endregion

    #region Public Fields

    /// <summary>
    /// The file name of the save game file.
    /// </summary>
    public const string SaveGameFileName = "SaveGame.bin";
    static string saveGameDataPath;
    /// <summary>
    /// The full path to the save game file.
    /// </summary>
    public static string SaveGameDataPath { get { return saveGameDataPath; } }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        InitSavePath(); // init save game path
        Debug.Log("Save game path set to " + SaveGameDataPath);
        // if there is an available save game, then let the user load it,
        // otherwise don't let them try
        if (!File.Exists(SaveGameDataPath))
            loadGameButton.interactable = false;
        else
            loadGameButton.interactable = true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Initializes the full save game path.
    /// </summary>
    public static void InitSavePath()
    {
        saveGameDataPath = Application.persistentDataPath + "/" + SaveGameFileName;
    }

    /// <summary>
    /// Attempts to start up a new game with the given settings.
    /// If there are not enough human players in the game, will instead
    /// show the error dialog.
    /// </summary>
    public void StartNewGame()
    {
        List<int> types = GetPlayerTypes();
        Debug.Log("Type values: " + types[0].ToString() + "," + types[1].ToString() + ","
            + types[2].ToString() + "," + types[3].ToString());
        // if the player has selected less than two human players, display an error
        if (types.FindAll(players => players == 0).Count >= 2)
        {
            Game.GameToRestore = null;
            Game.HumanPlayersCount = types;
            ClearSave();
            SceneManager.LoadScene("MainGame");
        }
        else
        {
            StartCoroutine(ShowPopUpMessage(2));
        }

    }

    /// <summary>
    /// Removes the current save file.
    /// Since saving is currently managed like a state-preservation system,
    /// we are treating it like such by clearing the state on a new game and
    /// when the game finishes.
    /// </summary>
    public static void ClearSave()
    {
        if (File.Exists(SaveGameDataPath))
            File.Delete(SaveGameDataPath);
    }

    /// <summary>
    /// Restore the previously saved game state from a file
    /// </summary>
    public void LoadGame()
    {
        // Open the file containing the data that you want to deserialize.
        FileStream fs = new FileStream(SaveGameDataPath, FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            // Deserialize the SerializableGame memento from the file and 
            // restore state from that memento.
            Game.GameToRestore = (SerializableGame)formatter.Deserialize(fs);
            SceneManager.LoadScene("MainGame");
        }
        catch (SerializationException ex)
        {
            Debug.Log("Failed to deserialize. Reason: " + ex.Message);
            throw ex;
        }
        finally
        {
            fs.Close();
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("quitting");
        Application.Quit();
    }

    /// <summary>
    /// Shows the error dialog for the given amount of time.
    /// </summary>
    /// <param name="delay">The length of time to show the dialog for.</param>
    IEnumerator ShowPopUpMessage(float delay)
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        errorPanel.SetActive(false);
    }

    /// <summary>
    /// Returns a list of ints representing the types chosen from each dropdown.
    /// Currently: 0 = Human, 1 = AI.
    /// </summary>
    /// <returns>The list of player types.</returns>
    public List<int> GetPlayerTypes()
    {
        List<int> types;
        Dropdown One = playerOneType.GetComponent<Dropdown>();
        Dropdown Two = playerTwoType.GetComponent<Dropdown>();
        Dropdown Three = playerThreeType.GetComponent<Dropdown>();
        Dropdown Four = playerFourType.GetComponent<Dropdown>();

        types = new List<int>(new int[] { One.value, Two.value, Three.value, Four.value });
        return types;
    }

    /// <summary>
    /// Returns a list of colours for the players.
    /// </summary>
    /// <returns>The list of player colours.</returns>
    public List<Color> GetPlayerColours()
    {
        // currently, the colours are hard-coding
        // but this could be expanded in the future
        Color One = new Color(205, 0, 0);
        Color Two = new Color(177, 0, 240);
        Color Three = new Color(205, 205, 0);
        Color Four = new Color(0, 205, 0);
        List<Color> colours = new List<Color> { One, Two, Three, Four };
        return colours;
    }

    #endregion
}
