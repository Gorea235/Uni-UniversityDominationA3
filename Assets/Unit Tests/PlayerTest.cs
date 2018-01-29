using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PlayerTest
{
    Game game;
    Map map;
    Player[] players;
    PlayerUI[] gui;

    [UnityTest]
    public IEnumerator CaptureSector_ChangesOwner()
    {

        Setup();
        game.InitializeMap();

        Player previousOwner = map.sectors[0].Owner;
        //   bool run = false; // used to decide whether to check previous players sector list (if no previous owner, do not look in list)

        // if (map.sectors[0].GetOwner() != null)
        // {            
        //     run = true;
        // }

        game.players[0].Capture(map.sectors[0]);
        Assert.AreSame(map.sectors[0].Owner, game.players[0]); // owner stored in sector
        Assert.IsTrue(game.players[0].ownedSectors.Contains(map.sectors[0])); // sector is stored as owned by the player

        if (/*run == true*/previousOwner != null) // if sector had previous owner
        {
            Assert.IsFalse(previousOwner.ownedSectors.Contains(map.sectors[0])); // sector has been removed from previous owner list
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersBeerAmountCorrect()
    {

        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1];
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.ResourceType = ResourceType.Beer;

        // get beer amounts for each player before capture
        int attackerBeerBeforeCapture = playerA.Beer;
        int defenderBeerBeforeCapture = playerB.Beer;
        Player previousOwner = landmarkedSector.Owner;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.IsTrue(attackerBeerBeforeCapture + landmark.Amount == playerA.Beer);
        Assert.IsTrue(defenderBeerBeforeCapture - landmark.Amount == previousOwner.Beer);

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersKnowledgeAmountCorrect()
    {

        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1];
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.ResourceType = ResourceType.Knowledge;

        // get knowledge amounts for each player before capture
        int attackerKnowledgeBeforeCapture = playerA.Knowledge;
        int defenderKnowledgeBeforeCapture = playerB.Knowledge;
        Player previousOwner = landmarkedSector.Owner;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.IsTrue(attackerKnowledgeBeforeCapture + landmark.Amount == playerA.Knowledge);
        Assert.IsTrue(defenderKnowledgeBeforeCapture - landmark.Amount == previousOwner.Knowledge);

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerBeerAmountCorrect()
    {

        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1];
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.ResourceType = ResourceType.Beer;

        // get player beer amount before capture
        int oldBeer = playerA.Beer;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are gained correctly
        Assert.IsTrue(playerA.Beer - oldBeer == landmark.Amount);

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerKnowledgeAmountCorrect()
    {

        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1];
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.ResourceType = ResourceType.Knowledge;

        // get player knowledge amount before capture
        int oldKnowledge = playerA.Knowledge;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are gained correctly
        Assert.IsTrue(playerA.Knowledge - oldKnowledge == landmark.Amount);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_SpawnedWhenLandmarkOwnedAndUnoccupied()
    {

        Setup();

        Sector landmarkedSector = map.sectors[1];
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = null;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a unit has been spawned for playerA in landmarkedSector
        Assert.IsTrue(playerA.units.Contains(landmarkedSector.Unit));

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkOwnedAndOccupied()
    {

        Setup();

        Sector landmarkedSector = map.sectors[1];
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and contains a Level 5 unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = Object.Instantiate(playerA.UnitPrefab).GetComponent<Unit>();
        landmarkedSector.Unit.Level = 5;
        landmarkedSector.Unit.Owner = playerA;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a Level 1 unit has not spawned over the Level 5 unit already in landmarkedSector
        Assert.IsTrue(landmarkedSector.Unit.Level == 5);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkNotOwned()
    {

        Setup();

        Sector landmarkedSector = map.sectors[1];
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        landmarkedSector.Unit = null;

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = null;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerB.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure no unit is spawned at landmarkedSector
        Assert.IsNull(landmarkedSector.Unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsEliminated_PlayerWithNoUnitsAndNoLandmarksEliminated()
    {

        Setup();
        game.InitializeMap();

        Player playerA = game.players[0];

        Assert.IsFalse(playerA.IsEliminated); // not eliminated because they have units

        for (int i = 0; i < playerA.units.Count; i++)
        {
            playerA.units[i].DestroySelf(); // removes units
        }
        Assert.IsFalse(playerA.IsEliminated); // not eliminated because they still have a landmark

        // player[0] needs to lose their landmark
        for (int i = 0; i < playerA.ownedSectors.Count; i++)
        {
            if (playerA.ownedSectors[i].Landmark != null)
            {
                playerA.ownedSectors[i].Landmark = null; // player[0] no longer has landmarks
            }
        }
        Assert.IsTrue(playerA.IsEliminated);

        yield return null;
    }


    void Setup()
    {

        // initialize the game, map, and players with any references needed
        // the "GameManager" asset contains a copy of the GameManager object
        // in the 4x4 Test, but its script lacks references to players & the map
        game = Object.Instantiate(Resources.Load<GameObject>("GameManager")).GetComponent<Game>();

        // the "Map" asset is a copy of the 4x4 Test map, complete with
        // adjacent sectors and landmarks at (0,1), (1,3), (2,0), and (3,2),
        // but its script lacks references to the game & sectors
        map = Object.Instantiate(Resources.Load<GameObject>("Map")).GetComponent<Map>();

        // the "Players" asset contains 4 prefab Player game objects; only
        // references not in its script is each player's color
        players = Object.Instantiate(Resources.Load<GameObject>("Players")).GetComponentsInChildren<Player>();

        // the "GUI" asset contains the PlayerUI object for each Player
        gui = Object.Instantiate(Resources.Load<GameObject>("GUI")).GetComponentsInChildren<PlayerUI>();

        // the "Scenery" asset contains the camera and light source of the 4x4 Test
        // can uncomment to view scene as tests run, but significantly reduces speed
        //MonoBehaviour.Instantiate(Resources.Load<GameObject>("Scenery"));

        // establish references from game to players & map
        game.players = players;
        game.gameMap = map.gameObject;
        game.TestModeEnabled = true;

        // establish references from map to game & sectors (from children)
        map.game = game;
        map.sectors = map.gameObject.GetComponentsInChildren<Sector>();

        // establish references to SSB 64 colors for each player
        players[0].Color = Color.red;
        players[1].Color = Color.blue;
        players[2].Color = Color.yellow;
        players[3].Color = Color.green;

        // establish references to a PlayerUI and Game for each player & initialize GUI
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Gui = gui[i];
            players[i].Game = game;
            players[i].Gui.Initialize(players[i], i + 1);
        }
    }
}