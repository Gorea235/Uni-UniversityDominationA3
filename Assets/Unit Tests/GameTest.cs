﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameTest
{
    Game game;
    Map map;
    Player[] players;
    PlayerUI[] gui;
    List<GameObject> units;

    #region Test Management

    [SetUp]
    public void SetUp()
    {
        UnitTestsUtil.SetupGameTest(out game, out map, out players, out gui);
        units = new List<GameObject>();
    }

    Unit InitUnit(int player)
    {
        GameObject go = Object.Instantiate(players[player].UnitPrefab);
        units.Add(go);
        return go.GetComponent<Unit>();
    }

    [TearDown]
    public void TearDown()
    {
        UnitTestsUtil.TearDownGameTest(ref game, ref map, ref players, ref gui);
        foreach (var unit in units)
            if (unit != null)
                Object.Destroy(unit);
    }

    #endregion

    [Test]
    public void CreatePlayers_TwoPlayersAreHumanAndTwoNot()
    {
        // ensure creation of 2 players is accurate
        game.GetComponent<Game>().CreatePlayers(new List<int> { 0, 0, 1, 1 });
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman);
        Assert.IsFalse(game.GetComponent<Game>().players[2].IsHuman);
        Assert.IsFalse(game.GetComponent<Game>().players[3].IsHuman);
    }

    [Test]
    public void CreatePlayers_ThreePlayersAreHumanAndOneNot()
    {
        // ensure creation of 3 players is accurate
        game.GetComponent<Game>().CreatePlayers(new List<int> { 0, 0, 0, 1 });
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman);
        Assert.IsFalse(game.GetComponent<Game>().players[3].IsHuman);
    }

    [Test]
    public void CreatePlayers_FourPlayersAreHuman()
    {
        // ensure creation of 4 players is accurate
        game.GetComponent<Game>().CreatePlayers(new List<int> { 0, 0, 0, 0 });
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[3].IsHuman);
    }

    [Test]
    public void CreatePlayers_AtLeastTwoPlayersAreHuman()
    {
        // ensure that at least 2 players are created no matter what
        game.GetComponent<Game>().CreatePlayers(new List<int> { 0, 0, 1, 1 });
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman);
        Assert.IsFalse(game.GetComponent<Game>().players[2].IsHuman);
        Assert.IsFalse(game.GetComponent<Game>().players[3].IsHuman);
    }

    [Test]
    public void CreatePlayers_AtMostFourPlayersAreHuman()
    {
        // ensure that at most 4 players are created no matter what
        game.GetComponent<Game>().CreatePlayers(new List<int> { 0, 0, 0, 0 });
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman);
        Assert.IsTrue(game.GetComponent<Game>().players[3].IsHuman);
    }

    [Test]
    public void InitializeMap_OneLandmarkAllocatedWithUnitPerPlayer()
    {

        // MAY BE MADE OBSELETE BY TESTS OF THE INDIVIDUAL METHODS
        game.InitializeMap();

        // ensure that each player owns 1 sector and has 1 unit at that sector
        List<Sector> listOfAllocatedSectors = new List<Sector>();
        foreach (Player player in players)
        {
            Assert.IsTrue(player.ownedSectors.Count == 1);
            Assert.IsNotNull(player.ownedSectors[0].Landmark);
            Assert.IsTrue(player.units.Count == 1);

            Assert.AreSame(player.ownedSectors[0], player.units[0].Sector);

            listOfAllocatedSectors.Add(player.ownedSectors[0]);
        }


        foreach (Sector sector in map.sectors)
        {
            if (sector.Owner != null && !listOfAllocatedSectors.Contains(sector)) // any sector that has an owner but is not in the allocated sectors from above
            {
                Assert.Fail(); // must be an error as only sectors owned should be landmarks from above
            }
        }
    }

    [Test]
    public void NoUnitSelected_ReturnsFalseWhenUnitIsSelected()
    {
        game.Initialize();

        // clear any selected units
        foreach (Player player in game.players)
        {
            foreach (Unit unit in player.units)
            {
                unit.IsSelected = false;
            }
        }

        // assert that NoUnitSelected returns true
        Assert.IsTrue(game.NoUnitSelected());

        // select a unit
        players[0].units[0].IsSelected = true;

        // assert that NoUnitSelected returns false
        Assert.IsFalse(game.NoUnitSelected());
    }


    [Test]
    public void NextPlayer_CurrentPlayerChangesToNextPlayerEachTime()
    {
        Player playerA = players[0];
        Player playerB = players[1];
        Player playerC = players[2];
        Player playerD = players[3];

        // set the current player to the first player
        game.currentPlayer = playerA;
        playerA.IsActive = true;

        // ensure that NextPlayer changes the current player
        // from player A to player B
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerB);
        Assert.IsFalse(playerA.IsActive);
        Assert.IsTrue(playerB.IsActive);

        // ensure that NextPlayer changes the current player
        // from player B to player C
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerC);
        Assert.IsFalse(playerB.IsActive);
        Assert.IsTrue(playerC.IsActive);

        // ensure that NextPlayer changes the current player
        // from player C to player D
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerD);
        Assert.IsFalse(playerC.IsActive);
        Assert.IsTrue(playerD.IsActive);

        // ensure that NextPlayer changes the current player
        // from player D to player A
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerA);
        Assert.IsFalse(playerD.IsActive);
        Assert.IsTrue(playerA.IsActive);
    }

    [Test]
    public void NextPlayer_EliminatedPlayersAreSkipped()
    {
        Player playerA = players[0];
        playerA.IsHuman = true;
        Player playerB = players[1];
        playerB.IsHuman = true;
        Player playerC = players[2];
        playerC.IsHuman = true;
        Player playerD = players[3];
        playerD.IsHuman = true;

        game.currentPlayer = playerA;

        playerC.units.Add(InitUnit(2)); // make player C not eliminated
        playerD.units.Add(InitUnit(3)); // make player D not eliminated

        game.TurnState = TurnState.EndOfTurn;
        game.UpdateMain(); // removes players that should be eliminated (A and B)

        // ensure eliminated players are skipped
        Assert.IsTrue(game.currentPlayer == playerC);
        Assert.IsFalse(playerA.IsActive);
        Assert.IsFalse(playerB.IsActive);
        Assert.IsTrue(playerC.IsActive);
    }


    [Test]
    public void NextTurnState_TurnStateProgressesCorrectly()
    {
        // initialize turn state to Move1
        game.TurnState = TurnState.Move1;

        // ensure NextTurnState changes the turn state
        // from Move1 to Move2
        game.NextTurnState();
        Assert.IsTrue(game.TurnState == TurnState.Move2);

        // ensure NextTurnState changes the turn state
        // from Move2 to EndOfTurn
        game.NextTurnState();
        Assert.IsTrue(game.TurnState == TurnState.EndOfTurn);

        // ensure NextTurnState changes the turn state
        // from EndOfTurn to Move1
        game.NextTurnState();
        Assert.IsTrue(game.TurnState == TurnState.Move1);

        // ensure NextTurnState does not change turn state
        // if the current turn state is NULL
        game.TurnState = TurnState.NULL;
        game.NextTurnState();
        Assert.IsTrue(game.TurnState == TurnState.NULL);
    }

    [Test]
    public void GetWinner_OnePlayerWithLandmarksAndUnitsWins()
    {
        Sector landmark1 = map.sectors[1];
        Player playerA = players[0];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.IsNotNull(landmark1.Landmark);

        // ensure winner is found if only 1 player owns a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerA.units.Add(InitUnit(0));
        Assert.IsNotNull(game.GetWinner());
    }

    [Test]
    public void GetWinner_NoWinnerWhenMultiplePlayersOwningLandmarks()
    {
        Sector landmark1 = map.sectors[1];
        Sector landmark2 = map.sectors[7];
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure'landmark1' and 'landmark2' are landmarks
        landmark1.Initialize();
        landmark2.Initialize();
        Assert.IsNotNull(landmark1.Landmark);
        Assert.IsNotNull(landmark2.Landmark);

        // ensure no winner is found if >1 players own a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerB.ownedSectors.Add(landmark2);
        Assert.IsNull(game.GetWinner());
    }

    [Test]
    public void GetWinner_NoWinnerWhenMultiplePlayersWithUnits()
    {
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure no winner is found if >1 players have a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.units.Add(InitUnit(0));
        playerB.units.Add(InitUnit(1));
        Assert.IsNull(game.GetWinner());
    }

    [Test]
    public void GetWinner_NoWinnerWhenAPlayerHasLandmarkAndAnotherHasUnits()
    {
        Sector landmark1 = map.sectors[1];
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.IsNotNull(landmark1.Landmark);

        // ensure no winner is found if 1 player has a landmark
        // and another player has a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerB.units.Add(InitUnit(1));
        Assert.IsNull(game.GetWinner());
    }

    void ClearSectorsAndUnitsOfAllPlayers()
    {

        foreach (Player player in game.players)
        {
            ClearSectorsAndUnits(player);
        }
    }

    void ClearSectorsAndUnits(Player player)
    {

        player.units = new List<Unit>();
        player.ownedSectors = new List<Sector>();
    }

    [Test]
    public void EndGame_GameEndsCorrectlyWithNoCurrentPlayerAndNoActivePlayersAndNoTurnState()
    {
        GameObject resultPanel = gui[0].transform.parent.Find("GameResult").gameObject;
        game.endGamePopup = resultPanel;
        game.endGameWinnerText = resultPanel.transform.Find("GameWinnerText").GetComponent<UnityEngine.UI.Text>();
        game.currentPlayer = game.players[0];
        foreach (Player player in game.players)
            player.units.Clear();
        game.currentPlayer.units.Add(null);
        game.EndGame();

        // ensure the game is marked as finished
        Assert.IsTrue(game.IsFinished);

        // ensure the current player is null
        Assert.IsNull(game.currentPlayer);

        // ensure no players are active
        foreach (Player player in game.players)
            Assert.IsFalse(player.IsActive);

        // ensure turn state is NULL
        Assert.IsTrue(game.TurnState == TurnState.NULL);
    }

    [Test]
    public void SaveLoadTest()
    {
        // set up a game state
        Game.HumanPlayersCount = new List<int> { 0, 1, 0, 1 };
        game.Initialize();
        Game.HumanPlayersCount = null;
        map.sectors[0].Unit = InitUnit(2);
        map.sectors[0].Unit.Owner = players[2];
        map.sectors[0].Unit.LevelUp();
        map.sectors[0].Unit.LevelUp();
        int unitLevel = map.sectors[0].Unit.Level;
        map.sectors[0].Owner = players[2];
        map.sectors[1].Owner = players[1];

        // test saving
        SerializableGame memento = game.SaveToMemento();
        // clear game
        UnitTestsUtil.TearDownGameTest(ref game, ref map, ref players, ref gui);
        // re-init game to make it *fresh*
        UnitTestsUtil.SetupGameTest(out game, out map, out players, out gui);
        // test loading
        game.RestoreFromMemento(memento);

        // test if game restored properly
        Assert.That(players[0].IsHuman);
        Assert.That(players[1].IsHuman, Is.False);
        Assert.That(players[2].IsHuman);
        Assert.That(players[3].IsHuman, Is.False);
        Assert.That(map.sectors[0].Unit, Is.Not.Null);
        Assert.That(map.sectors[0].Unit.Owner, Is.EqualTo(players[2]));
        Assert.That(map.sectors[0].Unit.Level, Is.EqualTo(unitLevel));
        Assert.That(map.sectors[1].Owner, Is.EqualTo(players[1]));
    }

    [Test]
    public void SaveGameToFile_ClearSave()
    {
        // init game
        game.gameSavedPopup = gui[0].transform.parent.Find("SavedPanel").gameObject;
        game.Initialize();
        MainMenu.InitSavePath();

        // run file tests
        game.SaveGame();
        Assert.That(System.IO.File.Exists(MainMenu.SaveGameDataPath));
        MainMenu.ClearSave();
        Assert.That(System.IO.File.Exists(MainMenu.SaveGameDataPath), Is.False);
    }

    [Test]
    public void SpawnPVC_Initial()
    {
        game.Initialize();

        var search =
            from s in map.sectors
            where s.HasPVC
            select s;

        Assert.That(search.Count(), Is.EqualTo(1));
        Assert.That(search.First().AllowPVC);
    }

    [Test]
    public void SpawnPVC_Respawn()
    {
        game.Initialize();

        // grab initial PVC spawn sector
        var search =
            from s in map.sectors
            where s.HasPVC
            select s;
        Sector initialSector = search.First();

        // run the respawn 20 times
        for (int i = 0; i < 20; i++)
        {
            game.PvcDiscoveredByLast = players[i % players.Length];
            game.SpawnPVC(); // do a respawn

            // search for the new spawn sector
            search =
                from s in map.sectors
                where s.HasPVC
                select s;

            Assert.That(search.Count(), Is.EqualTo(1));
            Sector newSpawn = search.First();
            Assert.That(newSpawn.AllowPVC);
            Assert.That(newSpawn, Is.Not.EqualTo(initialSector));

            initialSector = newSpawn;
        }
    }

    [Test]
    public void PrepareForMinigame_Test()
    {
        // init test state
        game.minigameLoading = gui[0].transform.parent.Find("MinigameLoading").gameObject;
        game.Initialize();
        var search =
            from s in map.sectors
            where s.HasPVC
            select s;
        Sector initialSector = search.First();
        game.PvcDiscoveredByLast = players[0];
        game.currentPlayer = players[1];
        Game.GameToRestore = null;

        game.PrepareForMinigame();

        Assert.That(game.PvcDiscoveredByLast, Is.EqualTo(players[1]));
        search =
            from s in map.sectors
            where s.HasPVC
            select s;
        Assert.That(search.Count(), Is.EqualTo(1));
        Assert.That(search.First(), Is.Not.EqualTo(initialSector));
        Assert.That(Game.GameToRestore, Is.Not.Null);
    }

    [Test]
    public void MinigameFinishedProcess_Score070()
    {
        MinigameFinishedProcess_TestScore(70);
    }

    [Test]
    public void MinigameFinishedProcess_Score100()
    {
        MinigameFinishedProcess_TestScore(100);
    }

    [Test]
    public void MinigameFinishedProcess_Score170()
    {
        MinigameFinishedProcess_TestScore(170);
    }

    void MinigameFinishedProcess_TestScore(float score)
    {
        // set up unity bindings to stop null reference errors
        Transform minigameResult = gui[0].transform.parent.Find("MinigameResult");
        game.minigameStatusText = minigameResult.Find("MinigameStatusText").GetComponent<UnityEngine.UI.Text>();
        game.minigameScoreText = minigameResult.Find("MinigameScoreText").GetComponent<UnityEngine.UI.Text>();
        game.minigameRewardText = minigameResult.Find("MinigameRewardText").GetComponent<UnityEngine.UI.Text>();
        game.minigameResultPopup = minigameResult.gameObject;

        // init current player
        players[0].Beer = 0;
        players[0].Knowledge = 0;
        game.currentPlayer = players[0];

        // init data store
        DataStore storage = new GameObject("DataStore").AddComponent<DataStore>();
        storage.AddScore(score);
        storage.ScoreModifier = 1;
        storage.SetSucceeded(true);

        // test process finish
        game.MinigameFinishedProcess();
        float result = Mathf.Floor(score / Game.minigameScoreScaleDownModifier);
        Assert.That(players[0].Beer, Is.EqualTo(result).Within(float.Epsilon));
        Assert.That(players[0].Knowledge, Is.EqualTo(result).Within(float.Epsilon));
    }
}
#endif
