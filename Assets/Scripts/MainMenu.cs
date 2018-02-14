using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    #region Public properties
    //a boolean passed to the Initializer to know if it should start a new game
    //or deserialize a saved one from our GameData.bin
    public static bool startNewGame;
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!File.Exists("GameData.bin"))
            loadGameButton.interactable = false;
        else
            loadGameButton.interactable = true;
    }

    #region Helper Methods

    public void StartNewGame()
    {
        List<int> types;
        types = GetPlayerTypes();
        Debug.Log("Type values: " + types[0].ToString() + "," + types[1].ToString() + ","
            + types[2].ToString() + "," + types[3].ToString());
        //  StartCoroutine(ShowPopUpMessage(2));
        startNewGame = true;
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        startNewGame = false;
        SceneManager.LoadScene(1);
    }


    IEnumerator ShowPopUpMessage(float delay)
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        errorPanel.SetActive(false);
    }

    //  Returns a list of ints representing the types chosen from each dropdown
    //  currently - 0 = Human, 1 = AI, 2 = None
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
