using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UnitTest
{
    private Game game;
    private Map map;
    private Player[] players;
    private PlayerUI[] gui;
    private GameObject unitPrefab;

    [UnityTest]
    public IEnumerator MoveToFriendlyFromNull_UnitInCorrectSector()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving from null
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerA;

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.Sector == sectorA);
        Assert.IsTrue(sectorA.Unit == unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToNeutral_UnitInCorrectSector()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player playerA = players[0];

        // test moving from one sector to another
        unit.Sector = sectorA;
        unit.Owner = playerA;
        sectorA.Unit = unit;
        sectorB.Unit = null;
        sectorA.Owner = playerA;
        sectorB.Owner = playerA;

        unit.MoveTo(sectorB);
        Assert.IsTrue(unit.Sector == sectorB);
        Assert.IsTrue(sectorB.Unit == unit);
        Assert.IsNull(sectorA.Unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToFriendly_UnitInCorrectSector()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving into a friendly sector (no level up)
        unit.Level = 1;
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerA;

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.Level == 1);

        yield return null;
    }

    public IEnumerator MoveToHostile_UnitInCorrectSectorAndLevelUp()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];
        Player playerB = players[1];

        // test moving into a non-friendly sector (level up)
        unit.Level = 1;
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerB;

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.Level == 2);
        Assert.IsTrue(sectorA.Owner == unit.Owner);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SwapPlaces_UnitsInCorrectNewSectors()
    {

        Setup();

        Unit unitA = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Unit unitB = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player player = players[0];

        // places players unitA in sectorA
        unitA.Owner = player;
        unitA.Sector = sectorA;
        sectorA.Unit = unitA;

        // places players unitB in sectorB
        unitB.Owner = player;
        unitB.Sector = sectorB;
        sectorB.Unit = unitB;

        unitA.SwapPlacesWith(unitB);
        Assert.IsTrue(unitA.Sector == sectorB); // unitA in sectorB
        Assert.IsTrue(sectorB.Unit == unitA); // sectorB has unitA
        Assert.IsTrue(unitB.Sector == sectorA); // unitB in sectorA
        Assert.IsTrue(sectorA.Unit == unitB); // sectorA has unitB

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelIncreasesByOne()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp increments level as expected
        unit.Level = 1;
        unit.LevelUp();
        Assert.IsTrue(unit.Level == 2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelDoesNotPastFive()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp does not increment past 5
        unit.Level = 5;
        unit.LevelUp();
        Assert.IsTrue(unit.Level == 5);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectAndDeselect_SelectedTrueWhenSelectedFalseWhenDeselected()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];

        unit.Sector = sector;
        unit.IsSelected = false;

        unit.Select();
        Assert.IsTrue(unit.IsSelected);

        unit.Deselect();
        Assert.IsFalse(unit.IsSelected);

        yield return null;
    }


    [UnityTest]
    public IEnumerator DestroySelf_UnitNotInSectorAndNotInPlayersUnitsList()
    {

        Setup();

        Unit unit = Object.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];
        Player player = players[0];

        unit.Sector = sector;
        sector.Unit = unit;

        unit.Owner = player;
        player.units.Add(unit);

        unit.DestroySelf();

        Assert.IsNull(sector.Unit); // unit not on sector 
        Assert.IsFalse(player.units.Contains(unit)); // unit not in list of players units

        yield return null;
    }


    private void Setup()
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

        // establish references from each player to the game
        foreach (Player player in players)
        {
            player.Game = game;
        }

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

        // extract the unit prefab from the player class
        unitPrefab = players[0].UnitPrefab;
    }
}