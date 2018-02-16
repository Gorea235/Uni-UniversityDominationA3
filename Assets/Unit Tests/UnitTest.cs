using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class UnitTest
{
    Game game;
    Map map;
    Player[] players;
    PlayerUI[] gui;
    GameObject unitPrefab;
    List<Unit> units;

    #region Test Management

    [SetUp]
    public void SetUp()
    {
        UnitTestsUtil.SetupTest(ref game, ref map, ref players, ref gui);
        unitPrefab = players[0].unitPrefab;
        units = new List<Unit>();
    }

    void AddUnits(int number)
    {
        for (int i = 0; i < number; i++)
            units.Add(Object.Instantiate(unitPrefab).GetComponent<Unit>());
    }

    [TearDown]
    public void TearDown()
    {
        UnitTestsUtil.TearDownTest(ref game, ref map, ref players, ref gui);
        foreach (var unit in units)
            if (unit != null)
                Object.Destroy(unit.gameObject);
    }

    #endregion

    [UnityTest]
    public IEnumerator MoveToFriendlyFromNull_UnitInCorrectSector()
    {
        AddUnits(1);
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving from null
        units[0].Sector = null;
        sectorA.Unit = null;
        units[0].Owner = playerA;
        sectorA.Owner = playerA;

        units[0].MoveTo(sectorA);
        Assert.IsTrue(units[0].Sector == sectorA);
        Assert.IsTrue(sectorA.Unit == units[0]);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToNeutral_UnitInCorrectSector()
    {
        AddUnits(1);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player playerA = players[0];

        // test moving from one sector to another
        units[0].Sector = sectorA;
        units[0].Owner = playerA;
        sectorA.Unit = units[0];
        sectorB.Unit = null;
        sectorA.Owner = playerA;
        sectorB.Owner = playerA;

        units[0].MoveTo(sectorB);
        Assert.IsTrue(units[0].Sector == sectorB);
        Assert.IsTrue(sectorB.Unit == units[0]);
        Assert.IsNull(sectorA.Unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToFriendly_UnitInCorrectSector()
    {
        AddUnits(1);
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving into a friendly sector (no level up)
        units[0].Level = 1;
        units[0].Sector = null;
        sectorA.Unit = null;
        units[0].Owner = playerA;
        sectorA.Owner = playerA;

        units[0].MoveTo(sectorA);
        Assert.IsTrue(units[0].Level == 1);

        yield return null;
    }

    public IEnumerator MoveToHostile_UnitInCorrectSectorAndLevelUp()
    {
        AddUnits(1);
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];
        Player playerB = players[1];

        // test moving into a non-friendly sector (level up)
        units[0].Level = 1;
        units[0].Sector = null;
        sectorA.Unit = null;
        units[0].Owner = playerA;
        sectorA.Owner = playerB;

        units[0].MoveTo(sectorA);
        Assert.IsTrue(units[0].Level == 2);
        Assert.IsTrue(sectorA.Owner == units[0].Owner);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SwapPlaces_UnitsInCorrectNewSectors()
    {
        AddUnits(2);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player player = players[0];

        // places players unitA in sectorA
        units[0].Owner = player;
        units[0].Sector = sectorA;
        sectorA.Unit = units[0];

        // places players unitB in sectorB
        units[1].Owner = player;
        units[1].Sector = sectorB;
        sectorB.Unit = units[1];

        units[0].SwapPlacesWith(units[1]);
        Assert.IsTrue(units[0].Sector == sectorB); // unitA in sectorB
        Assert.IsTrue(sectorB.Unit == units[0]); // sectorB has unitA
        Assert.IsTrue(units[1].Sector == sectorA); // unitB in sectorA
        Assert.IsTrue(sectorA.Unit == units[1]); // sectorA has unitB

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelIncreasesByOne()
    {
        AddUnits(1);

        // ensure LevelUp increments level as expected
        units[0].Level = 1;
        units[0].LevelUp();
        Assert.IsTrue(units[0].Level == 2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelDoesNotPastFive()
    {
        AddUnits(1);

        // ensure LevelUp does not increment past 5
        units[0].Level = 5;
        units[0].LevelUp();
        Assert.IsTrue(units[0].Level == 5);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SelectAndDeselect_SelectedTrueWhenSelectedFalseWhenDeselected()
    {
        AddUnits(1);
        Sector sector = map.sectors[0];

        units[0].Sector = sector;
        units[0].IsSelected = false;

        units[0].Select();
        Assert.IsTrue(units[0].IsSelected);

        units[0].Deselect();
        Assert.IsFalse(units[0].IsSelected);

        yield return null;
    }


    [UnityTest]
    public IEnumerator DestroySelf_UnitNotInSectorAndNotInPlayersUnitsList()
    {
        AddUnits(1);
        Sector sector = map.sectors[0];
        Player player = players[0];

        units[0].Sector = sector;
        sector.Unit = units[0];

        units[0].Owner = player;
        player.units.Add(units[0]);

        units[0].DestroySelf();

        Assert.IsNull(sector.Unit); // unit not on sector 
        Assert.IsFalse(player.units.Contains(units[0])); // unit not in list of players units

        yield return null;
    }
}