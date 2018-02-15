using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    #region Unity Bindings

    public Player[] players;
    public GameObject gameMap;
    public Player currentPlayer;
    public UnityEngine.UI.Image gameSavedPopup;
    public UnityEngine.UI.Text gameSavedPopupText;

    #endregion

    #region Private Fields

    TurnState turnState;
    bool gameFinished = false;
    bool testMode = false;

    #endregion

    #region Public Properties

    public static List<int> HumanPlayersCount { get; set; }

    public TurnState TurnState
    {
        get { return turnState; }
        set { turnState = value; }
    }
    
    public static bool PVCEncountered { get; set; }
    public static Player LastDiscovererOfPVC { get; set; }


    public bool IsFinished { get { return gameFinished; } }

    public bool TestModeEnabled
    {
        get { return testMode; }
        set { testMode = value; }
    }

    public static SerializableGame GameToRestore { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the game.
    /// </summary>
    public void Initialize()
    {
        HumanPlayersCount = HumanPlayersCount ?? new List<int> { 0, 0, 0, 0 };

        // create a specified number of human players
        CreatePlayers(HumanPlayersCount);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move1;

        // set first human player as the current player
        currentPlayer = players[0];
        currentPlayer = players[HumanPlayersCount.FindIndex(players => players == 0)];
        currentPlayer.Gui.Activate();
        players[HumanPlayersCount.FindIndex(players => players == 0)].IsActive = true;

        // update GUIs
        UpdateGUI();

        GameObject dataStore = GameObject.Find("DataStore");
        if (dataStore != null)
        {
            // if we get here, it means that a minigame just occurred
            var result = dataStore.GetComponent<DataStore>().Finalize();
            Debug.Log(string.Format("minigame score: {0}, success: {1}", result.Score, result.Succeeded));
        }
    }

    /// <summary>
    /// Initialize all sectors, allocate players to landmarks,
    /// and spawn units and PVC.
    /// </summary>
    public void InitializeMap()
    {
        // get an array of all sectors
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();

        // initialize each sector
        foreach (Sector sector in sectors)
        {
            sector.Initialize();
        }


        // get an array of all sectors containing landmarks
        Sector[] landmarkedSectors = GetLandmarkedSectors(sectors);

        // ensure there are at least as many landmarks as players
        if (landmarkedSectors.Length < players.Length)
        {
            throw new System.Exception("Must have at least as many landmarks as players; only " + landmarkedSectors.Length.ToString() + " landmarks found for " + players.Length.ToString() + " players.");
        }

        // randomly allocate sectors to players
        foreach (Player player in players)
        {
            bool playerAllocated = false;
            while (!playerAllocated)
            {

                // choose a landmarked sector at random
                int randomIndex = UnityEngine.Random.Range(0, landmarkedSectors.Length);

                // if the sector is not yet allocated, allocate the player
                if (landmarkedSectors[randomIndex].Owner == null)
                {
                    player.Capture(landmarkedSectors[randomIndex]);
                    playerAllocated = true;
                }

                // retry until player is allocated
            }
        }

        // spawn units for each player
        foreach (Player player in players)
        {
            player.SpawnUnits();
        }

        //sawn the PVC
        if (LastDiscovererOfPVC == null)
        {
            SpawnPVC();
            PVCEncountered = false;
            LastDiscovererOfPVC = null;
        }
        
        
        
    }

    /// <summary>
    /// Initialises the players using the specified number of human players.
    /// </summary>
    /// <param name="listOfPlayers">List of all players assigned at MainMenu.</param>
    public void CreatePlayers(List<int> listOfPlayers)
    {
        //// ensure that the specified number of players
        //// is at least 2 and does not exceed 4
        //numberOfPlayers = Mathf.Clamp(numberOfPlayers, 2, 4);

        // mark the specified number of players as human
        for (int i = 0; i < listOfPlayers.Count; i++)
        {
            if (listOfPlayers[i] == 0)
            {
                players[i].IsHuman = true;
            }
        }

        // give all players a reference to this game
        // and initialize their GUIs
        for (int i = 0; i < 4; i++)
        {
            players[i].Id = i;
            players[i].Game = this;
            players[i].Gui.Initialize(players[i], i + 1);
        }
    }

    #endregion

    #region Serialization

    public SerializableGame SaveToMemento()
    {
        SerializablePlayer[] sPlayers = new SerializablePlayer[players.Length];
        for (int i = 0; i < players.Length; i++)
            sPlayers[i] = players[i].SaveToMemento();
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();
        SerializableSector[] sSectors = new SerializableSector[sectors.Length];
        for (int i = 0; i < sectors.Length; i++)
            sSectors[i] = sectors[i].SaveToMemento();
        return new SerializableGame
        {
            turnState = turnState,
            players = sPlayers,
            sectors = sSectors,
            currentPlayerId = currentPlayer.Id,
            LastDiscovererOfPVCid = LastDiscovererOfPVC.Id,
            PVCEncountered = PVCEncountered

        };
    }

    public void RestoreFromMemento(SerializableGame memento)
    {
        turnState = memento.turnState;
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();
        for (int i = 0; i < memento.sectors.Length; i++)
            sectors[i].RestoreFromMemento(memento.sectors[i], players);
        for (int i = 0; i < memento.players.Length; i++)
        {
            players[i].RestoreFromMemento(memento.players[i]);
            players[i].Game = this;
            players[i].Gui.Initialize(players[i], i + 1);
        }
        currentPlayer = players[memento.currentPlayerId];
        currentPlayer.Gui.Activate();
        LastDiscovererOfPVC = players[memento.LastDiscovererOfPVCid];
        PVCEncountered = memento.PVCEncountered;
        UpdateGUI();
    }

    #endregion

    #region MonoBehaviour

    void Update()
    {
        // at the end of each turn, check for a winner and end the game if
        // necessary; otherwise, start the next player's turn

        // if the current turn has ended and test mode is not enabled
        if (turnState == TurnState.EndOfTurn && !testMode)
        {

            // if there is no winner yet
            if (GetWinner() == null)
            {

                // start the next player's turn
                NextPlayer();
                NextTurnState();

                // skip eliminated players and non-human players
                while (currentPlayer.IsEliminated || !currentPlayer.IsHuman)
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else if (!gameFinished)
                EndGame();
        }
    }

    #endregion

    #region Helper Methods

    ///<summary>
    ///Randomly spawn the PVC
    /// </summary>
    void SpawnPVC()
    {
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();
        
        while (true)
        {
            int lastPVCLocation = Array.FindIndex(sectors, sector => sector.HasPVC == true);
            Sector randomSector = sectors[UnityEngine.Random.Range(0, sectors.Length)];

            if (lastPVCLocation == -1 && randomSector.AllowPVC())
            {
                randomSector.HasPVC = true;
                PVCEncountered = false;
                Debug.Log("Allocated PVC initially at " + randomSector.ToString());
                break;
            }
            else if (LastDiscovererOfPVC != currentPlayer && randomSector.AllowPVC())
            {
                randomSector.HasPVC = true;
                sectors[lastPVCLocation].HasPVC = false;
                if(LastDiscovererOfPVC != null)
                Debug.Log("Previous Player that found it is" + LastDiscovererOfPVC.ToString());
                LastDiscovererOfPVC = currentPlayer;
                PVCEncountered = false;
                Debug.Log("Allocated PVC to a new location, which is at " + randomSector.ToString());
                Debug.Log("Player that found it is" + currentPlayer.ToString());
                Debug.Log("Last Player that found it is" + LastDiscovererOfPVC.ToString());
                break;
            }
        }

    }

    /// <summary>
    /// Return a list of all sectors that contain landmarks from the given array.
    /// </summary>
    /// <returns>The landmarked sectors.</returns>
    /// <param name="sectors">Sectors.</param>
    Sector[] GetLandmarkedSectors(Sector[] sectors)
    {
        List<Sector> landmarkedSectors = new List<Sector>();
        foreach (Sector sector in sectors)
        {
            if (sector.Landmark != null)
            {
                landmarkedSectors.Add(sector);
            }
        }

        return landmarkedSectors.ToArray();
    }

    /// <summary>
    /// Return true if no unit is selected, false otherwise.
    /// </summary>
    /// <returns>Return true if no unit is selected, false otherwise.</returns>
    public bool NoUnitSelected()
    {
        // scan through each player
        foreach (Player player in players)
        {
            // scan through each unit of each player
            foreach (Unit unit in player.units)
            {
                // if a selected unit is found, return false
                if (unit.IsSelected == true)
                    return false;
            }
        }

        // otherwise, return true
        return true;
    }

    /// <summary>
    /// Set the current player to the next player in the order.
    /// </summary>
    public void NextPlayer()
    {
        // deactivate the current player
        currentPlayer.IsActive = false;
        currentPlayer.Gui.Deactivate();

        // find the index of the current player
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentPlayer)
            {
                // set the next player's index
                int nextPlayerIndex = i + 1;

                // if end of player list is reached, loop back to the first player
                if (nextPlayerIndex == players.Length)
                {
                    currentPlayer = players[0];
                    players[0].IsActive = true;
                    players[0].Gui.Activate();
                }

                // otherwise, set the next player as the current player
                else
                {
                    currentPlayer = players[nextPlayerIndex];
                    players[nextPlayerIndex].IsActive = true;
                    players[nextPlayerIndex].Gui.Activate();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Change the turn state to the next in the order,
    /// or to initial turn state if turn is completed.
    /// </summary>
    public void NextTurnState()
    {
        switch (turnState)
        {
            case TurnState.Move1:
                turnState = TurnState.Move2;
                //Monitor if the PVC was encountered this turn
                //flag is set in Sector.TriggerMinigame()
                if (PVCEncountered)
                    SpawnPVC();
                break;

            case TurnState.Move2:
                turnState = TurnState.EndOfTurn;
                if (PVCEncountered)
                    SpawnPVC();
                break;

            case TurnState.EndOfTurn:
                turnState = TurnState.Move1;
                break;

            default:
                break;
        }

        UpdateGUI();
    }

    /// <summary>
    /// End the current turn.
    /// </summary>
    public void EndTurn()
    {
        turnState = TurnState.EndOfTurn;
    }

    /// <summary>
    /// Return the winning player, or null if no winner yet.
    /// </summary>
    /// <returns>Return the winning player, or null if no winner yet.</returns>
    public Player GetWinner()
    {
        Player winner = null;

        // scan through each player
        foreach (Player player in players)
        {
            // if the player hasn't been eliminated
            if (!player.IsEliminated)
            {
                // if this is the first player found that hasn't been eliminated,
                // assume the player is the winner
                if (winner == null)
                    winner = player;

                // if another player that was not eliminated was already,
                // found, then return null
                else
                    return null;
            }
        }

        // if only one player hasn't been eliminated, then return it as the winner
        return winner;
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    public void EndGame()
    {
        gameFinished = true;
        currentPlayer.IsActive = false;
        currentPlayer = null;
        turnState = TurnState.NULL;
        Debug.Log("GAME FINISHED");
    }

    /// <summary>
    /// Update all players' GUIs.
    /// </summary>
    public void UpdateGUI()
    {
        for (int i = 0; i < 4; i++)
        {
            players[i].Gui.UpdateDisplay();
        }
    }

    /// <summary>
    /// Copy of Update that can be called by other objects (for testing).
    /// </summary>
    public void UpdateAccessible()
    {
        if (turnState == TurnState.EndOfTurn)
        {
            // if there is no winner yet
            if (GetWinner() == null)
            {
                // start the next player's turn
                NextPlayer();
                NextTurnState();

                // skip eliminated players
                while (currentPlayer.IsEliminated || !currentPlayer.IsHuman)
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else if (!gameFinished)
                EndGame();
        }
    }

    /// <summary>
    /// Quits the current game.
    /// </summary>
    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Save the current game state to a file for later loading
    /// </summary>
    public void SaveGame()
    {
        //Save game state to our static variable
        SerializableGame memento = SaveToMemento();
        // Open the file where we would serialize.
        FileStream fs = new FileStream(MainMenu.SaveGameDataPath, FileMode.OpenOrCreate);

        // Construct a BinaryFormatter and use it to serialize the data to the stream.
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, memento);
            StartCoroutine(ShowSavedGameInfoPanel("Game saved"));
        }
        catch (SerializationException ex)
        {
            //Debug.Log("Failed to serialize. Reason: " + ex.Message);
            //throw ex;
            StartCoroutine(ShowSavedGameInfoPanel(string.Format("Unable to save game ({0})", ex.Message)));
        }
        finally
        {
            fs.Close();
        }
    }

    IEnumerator ShowSavedGameInfoPanel(string text)
    {
        // this entire function is based off the functions that i've used for
        // the Doom clone, but since this class is already full and we're only
        // using this kind of thing for this 1 panel, I don't want to do the
        // simplifications I did for that (since it requires 2 extra private
        // properties and some other things)

        // init vars
        const float entryTime = 0.3f;
        const float delayTime = 1f;
        const float exitTime = 0.3f;
        float percentShown = 0;
        float minY = 0;
        float maxY = -gameSavedPopup.rectTransform.sizeDelta.y;

        // init text
        gameSavedPopupText.text = text;

        // do show animation
        while (percentShown < 1)
        {
            yield return new WaitForEndOfFrame();
            percentShown += Time.deltaTime / entryTime; // percent of the animation time we're thru
            percentShown = Mathf.Clamp01(percentShown);
            Vector2 pos = gameSavedPopup.rectTransform.anchoredPosition;
            pos.y = Mathf.SmoothStep(minY, maxY, percentShown);
            gameSavedPopup.rectTransform.anchoredPosition = pos;
        }

        // stay up for a given time
        yield return new WaitForSeconds(delayTime);

        // do hide animation
        while (percentShown > 0)
        {
            yield return new WaitForEndOfFrame();
            percentShown -= Time.deltaTime / exitTime; // percent of the animation time we're thru
            percentShown = Mathf.Clamp01(percentShown);
            Vector2 pos = gameSavedPopup.rectTransform.anchoredPosition;
            pos.y = Mathf.SmoothStep(minY, maxY, percentShown);
            gameSavedPopup.rectTransform.anchoredPosition = pos;
        }
    }

    #endregion
}
