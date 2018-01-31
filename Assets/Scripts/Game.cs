using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Unity Bindings

    public Player[] players;
    public GameObject gameMap;
    public Player currentPlayer;

    #endregion

    #region Private Fields

    [SerializeField]
    TurnState turnState;
    [SerializeField]
    bool gameFinished = false;
    [SerializeField]
    bool testMode = false;

    #endregion

    #region Public Properties

    public TurnState TurnState
    {
        get { return turnState; }
        set { turnState = value; }
    }

    public bool IsFinished { get { return gameFinished; } }

    public bool TestModeEnabled
    {
        get { return testMode; }
        set { testMode = value; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the game.
    /// </summary>
    public void Initialize()
    {
        // create a specified number of human players
        // *** currently hard-wired to 2 for testing ***
        CreatePlayers(2);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move1;

        // set Player 1 as the current player
        currentPlayer = players[0];
        currentPlayer.Gui.Activate();
        players[0].IsActive = true;

        // update GUIs
        UpdateGUI();
    }

    ///<summary>
    ///Initialize the game from a saved data set
    /// </summary>
    public void Initialize(Game savedGame)
    {
        //recreate the number of players
        CreatePlayers(savedGame.players.Length);

        //initialize the map in its previous state
        //
        // TBD
        //

        //initialize the turn state to the previous
        turnState = savedGame.turnState;

        //set last player as current player
        currentPlayer = savedGame.currentPlayer;
        currentPlayer.Gui.Activate();
        for (int i = 0; i < players.Length; i++) //find the index in current game context that corresponds to the one in the savedGame and set it to be the active player
        {
            if (players[i].Equals(savedGame.currentPlayer))
            {
                players[i].IsActive = true;
                break;
            }
        }

        //update GUIs
        UpdateGUI();

    }

    /// <summary>
    /// Initialize all sectors, allocate players to landmarks,
    /// and spawn units.
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
                int randomIndex = Random.Range(0, landmarkedSectors.Length);

                // if the sector is not yet allocated, allocate the player
                if (((Sector)landmarkedSectors[randomIndex]).Owner == null)
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
    }

    /// <summary>
    /// Initialises the players using the specified number of human players.
    /// </summary>
    /// <param name="numberOfPlayers">Number of human players.</param>
    public void CreatePlayers(int numberOfPlayers)
    {
        // ensure that the specified number of players
        // is at least 2 and does not exceed 4
        numberOfPlayers = Mathf.Clamp(numberOfPlayers, 2, 4);

        // mark the specified number of players as human
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i].IsHuman = true;
        }

        // give all players a reference to this game
        // and initialize their GUIs
        for (int i = 0; i < 4; i++)
        {
            players[i].Game = this;
            players[i].Gui.Initialize(players[i], i + 1);
        }
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

                // skip eliminated players
                while (currentPlayer.IsEliminated)
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
                break;

            case TurnState.Move2:
                turnState = TurnState.EndOfTurn;
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
                while (currentPlayer.IsEliminated)
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else if (!gameFinished)
                EndGame();
        }
    }

    #endregion
}
