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

    #region Test Management

    [SetUp]
    public void SetUp()
    {
        UnitTestsUtil.SetupGameTest(out game, out map, out players, out gui);
    }

    [TearDown]
    public void TearDown()
    {
        UnitTestsUtil.TearDownGameTest(ref game, ref map, ref players, ref gui);
    }

    #endregion

    [UnityTest]
    public IEnumerator CaptureSector_ChangesOwner()
    {
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
        Sector landmarkedSector = map.sectors[1];
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and contains a Level 5 unit
        landmarkedSector.Initialize();
        GameObject unit = Object.Instantiate(playerA.UnitPrefab);
        landmarkedSector.Unit = unit.GetComponent<Unit>();
        landmarkedSector.Unit.Level = 5;
        landmarkedSector.Unit.Owner = playerA;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a Level 1 unit has not spawned over the Level 5 unit already in landmarkedSector
        Assert.IsTrue(landmarkedSector.Unit.Level == 5);

        Object.Destroy(unit);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkNotOwned()
    {
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
}