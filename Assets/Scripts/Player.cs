using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Unity Bindings

    public Game game;
    public GameObject unitPrefab;
    public PlayerUI gui;
    public Color color;

    public List<Sector> ownedSectors;
    public List<Unit> units;

    #endregion

    #region Private Fields

    int id;
    int beer = 0;
    int knowledge = 0;
    bool human;
    bool active = false;

    #endregion

    #region Public Properties

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public Game Game
    {
        get { return game; }
        set { game = value; }
    }

    public GameObject UnitPrefab
    {
        get { return unitPrefab; }
    }

    public PlayerUI Gui
    {
        get { return gui; }
        set { gui = value; }
    }

    public int Beer
    {
        get { return beer; }
        set { beer = value; }
    }

    /// <summary>
    /// In here with my Lamborghini.
    /// </summary>
    /// <value>Up here in the hollywood hills.</value>
    /// <remarks>Just installed these 7 new bookshelves.</remarks>
    public int Knowledge
    {
        get { return knowledge; }
        set { knowledge = value; }
    }

    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    public bool IsHuman
    {
        get { return human; }
        set { human = value; }
    }

    public bool IsActive
    {
        get { return active; }
        set { active = value; }
    }

    /// <summary>
    /// Returns true if the player is eliminated, false otherwise.
    /// A player is considered eliminated if it has no units left
    /// and does not own a landmark.
    /// </summary>
    /// <returns><c>true</c>, if eliminated was ised, <c>false</c> otherwise.</returns>
    public bool IsEliminated
    {
        get
        {
            if (units.Count == 0 && !OwnsLandmark)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Returns true if the player owns at least one landmark,
    /// false otherwise.
    /// </summary>
    /// <returns><c>true</c>, if landmark was ownsed, <c>false</c> otherwise.</returns>
    bool OwnsLandmark
    {
        get
        {
            // scan through each owned sector
            foreach (Sector sector in ownedSectors)
            {
                // if a landmarked sector is found, return true
                if (sector.Landmark != null)
                    return true;
            }

            // otherwise, return false
            return false;
        }
    }

    #endregion

    #region Serialization

    public SerializablePlayer SaveToMemento()
    {
        return new SerializablePlayer
        {
            id = id,
            beer = beer,
            knowledge = knowledge,
            human = human,
            active = active
        };
    }

    public void RestoreFromMemento(SerializablePlayer memento)
    {
        id = memento.id;
        beer = memento.beer;
        knowledge = memento.knowledge;
        human = memento.human;
        active = memento.active;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Capture the given sector.
    /// </summary>
    /// <returns>The capture.</returns>
    /// <param name="sector">Sector.</param>
    public void Capture(Sector sector)
    {
        // store a copy of the sector's previous owner
        Player previousOwner = sector.Owner;

        // add the sector to the list of owned sectors
        ownedSectors.Add(sector);

        // remove the sector from the previous owner's
        // list of sectors
        if (previousOwner != null)
            previousOwner.ownedSectors.Remove(sector);

        // set the sector's owner to this player
        sector.Owner = this;

        // if the sector contains a landmark
        if (sector.Landmark != null)
        {
            Landmark landmark = sector.Landmark;

            // remove the landmark's resource bonus from the previous
            // owner and add it to this player
            switch (landmark.ResourceType)
            {
                case ResourceType.Beer:
                    beer += landmark.Amount;
                    if (previousOwner != null)
                        previousOwner.beer -= landmark.Amount;
                    break;
                case ResourceType.Knowledge:
                    knowledge += landmark.Amount;
                    if (previousOwner != null)
                        previousOwner.knowledge -= landmark.Amount;
                    break;
            }
        }
    }

    /// <summary>
    /// Spawn a unit at each unoccupied landmark.
    /// </summary>
    public void SpawnUnits()
    {
        // scan through each owned sector
        foreach (Sector sector in ownedSectors)
        {
            // if the sector contains a landmark and is unoccupied
            if (sector.Landmark != null && sector.Unit == null)
            {
                // instantiate a new unit at the sector
                Unit newUnit = Instantiate(unitPrefab).GetComponent<Unit>();

                // initialize the new unit
                newUnit.Initialize(this, sector);

                // add the new unit to the player's list of units and 
                // the sector's unit parameters
                units.Add(newUnit);
                sector.Unit = newUnit;
            }
        }
    }

    #endregion
}