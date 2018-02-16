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

    public const string SaveGameFileName = "SaveGame.bin";
    static string saveGameDataPath;
    public static string SaveGameDataPath { get { return saveGameDataPath; } }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        saveGameDataPath = Application.persistentDataPath + "/" + SaveGameFileName;
        Debug.Log("Save game path set to " + SaveGameDataPath);
        if (!File.Exists(SaveGameDataPath))
            loadGameButton.interactable = false;
        else
            loadGameButton.interactable = true;
    }

    #endregion

    #region Helper Methods

    public void StartNewGame()
    {
        List<int> types = GetPlayerTypes(); ;
        Debug.Log("Type values: " + types[0].ToString() + "," + types[1].ToString() + ","
            + types[2].ToString() + "," + types[3].ToString());
        //if the player has selected less than two human players, display an error
        if (types.FindAll(players => players == 0).Count >= 2)
        {
            Game.GameToRestore = null;
            Game.HumanPlayersCount = types;
            if (File.Exists(SaveGameDataPath))
                File.Delete(SaveGameDataPath);
            SceneManager.LoadScene("MainGame");
        }
        else
        {
            StartCoroutine(ShowPopUpMessage(2));
        }

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

    IEnumerator ShowPopUpMessage(float delay)
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        errorPanel.SetActive(false);
    }

    //  Returns a list of ints representing the types chosen from each dropdown
    //  currently - 0 = Human, 1 = AI
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

    //  Returns a list of colours for the players
    public List<Color> GetPlayerColours()
    {
        Color One = new Color(205, 0, 0);
        Color Two = new Color(177, 0, 240);
        Color Three = new Color(205, 205, 0);
        Color Four = new Color(0, 205, 0);
        List<Color> colours = new List<Color> { One, Two, Three, Four };
        return colours;
    }

    #endregion
}
