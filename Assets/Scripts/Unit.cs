using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region Unity Bindings

    public Material level1Material;
    public Material level2Material;
    public Material level3Material;
    public Material level4Material;
    public Material level5Material;

    #endregion

    #region Private Fields

    Player owner;
    Sector sector;
    int level;
    Color color;
    bool selected = false;

    #endregion

    #region Public Properties

    public Player Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public Sector Sector
    {
        get { return sector; }
        set { sector = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    public bool IsSelected
    {
        get { return selected; }
        set { selected = value; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the unit to be owned by the specified
    /// player and in the specified sector.
    /// </summary>
    /// <returns>The initialize.</returns>
    /// <param name="player">Current player.</param>
    /// <param name="sector">Current sector.</param>
    public void Initialize(Player player, Sector sector, Color? _color = null)
    {
        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        color = _color ?? Color.white;

        // set the material color to the player color
        GetComponent<Renderer>().material.color = color;

        // place the unit in the sector
        MoveTo(sector);
    }

    #endregion

    #region Serialization

    public SerializableUnit SaveToMemento()
    {
        return new SerializableUnit()
        {
            ownerId = owner.Id,
            level = level,
            color = color
        };
    }

    public void RestoreFromMemento(SerializableUnit memento, Player[] players, Sector containingSector)
    {
        Initialize(players[memento.ownerId], containingSector, memento.color);
        owner.units.Add(this);
        level = 0;
        for (int i = 0; i < memento.level; i++)
            LevelUp();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Move the unit into the target sector, capturing it,
    /// and levelling up if necessary.
    /// </summary>
    /// <param name="targetSector">The target sector.</param>
    public void MoveTo(Sector targetSector)
    {
        // clear the unit's current sector
        if (sector != null)
        {
            sector.ClearUnit();
        }

        // set the unit's sector to the target sector
        // and the target sector's unit to the unit
        sector = targetSector;
        targetSector.Unit = this;
        Transform targetTransform = targetSector.transform.Find("Units").transform;

        // set the unit's transform to be a child of
        // the target sector's transform
        transform.SetParent(targetTransform);

        // align the transform to the sector
        transform.position = targetTransform.position;

        // if the target sector belonged to a different 
        // player than the unit, capture it and level up
        if (targetSector.Owner != owner)
        {
            // level up
            LevelUp();

            // capture the target sector for the owner of this unit
            owner.Capture(targetSector);
        }
    }

    /// <summary>
    /// Switch the sectors of this unit and another unit.
    /// </summary>
    /// <param name="otherUnit">The unit to swap placecs with.</param>
    public void SwapPlacesWith(Unit otherUnit)
    {
        // swap the sectors' references to the units
        sector.Unit = otherUnit;
        otherUnit.sector.Unit = this;

        // get the index of this unit's sector in the map's list of sectors
        int tempSectorIndex = -1;
        for (int i = 0; i < owner.Game.gameMap.GetComponent<Map>().sectors.Length; i++)
        {
            if (sector == owner.Game.gameMap.GetComponent<Map>().sectors[i])
                tempSectorIndex = i;
        }

        // swap the units' references to their sectors
        sector = otherUnit.sector;
        otherUnit.sector = owner.Game.gameMap.GetComponent<Map>().sectors[tempSectorIndex];

        // realign transforms for each unit
        transform.SetParent(sector.transform.Find("Units").transform);
        transform.position = sector.transform.Find("Units").position;

        otherUnit.transform.SetParent(otherUnit.sector.transform.Find("Units").transform);
        otherUnit.transform.position = otherUnit.sector.transform.Find("Units").position;
    }

    /// <summary>
    /// Level up the unit, capping at Level 5.
    /// </summary>
    public void LevelUp()
    {
        if (level < 5)
        {
            // increase level
            level++;

            // change texture to reflect new level
            switch (level)
            {
                case 2:
                    gameObject.GetComponent<MeshRenderer>().material = level2Material;
                    break;
                case 3:
                    gameObject.GetComponent<MeshRenderer>().material = level3Material;
                    break;
                case 4:
                    gameObject.GetComponent<MeshRenderer>().material = level4Material;
                    break;
                case 5:
                    gameObject.GetComponent<MeshRenderer>().material = level5Material;
                    break;
                default:
                    gameObject.GetComponent<MeshRenderer>().material = level1Material;
                    break;
            }

            // set material color to match owner color
            GetComponent<Renderer>().material.color = color;
        }

    }

    /// <summary>
    /// Select the unit and highlight the sectors adjacent to it.
    /// </summary>
    public void Select()
    {
        selected = true;
        sector.ApplyHighlightAdjacent();
    }

    /// <summary>
    /// Deselect the unit and unhighlight the sectors adjacent to it.
    /// </summary>
    public void Deselect()
    {
        selected = false;
        sector.RevertHighlightAdjacent();
    }

    /// <summary>
    /// Safely destroy the unit by removing it from its owner's
    /// list of units before destroying.
    /// </summary>
    public void DestroySelf()
    {
        sector.ClearUnit();
        owner.units.Remove(this);
        Destroy(gameObject);
    }

    #endregion
}
