using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sector : MonoBehaviour
{
    #region Unity Bindings

    public Map map;
    public Sector[] adjacentSectors;
    public Landmark landmark;

    #endregion

    #region Private Fields

    Unit unit;
    Player owner;
    bool PVC;

    #endregion

    #region Public Properties

    /// <summary>
    /// The reference to the current <see cref="Map"/> object.
    /// </summary>
    /// <returns></returns>
    public Map Map
    {
        get { return map; }
        set { map = value; }
    }
    /// <summary>
    /// The unit current occupying this sector.
    /// </summary>
    public Unit Unit
    {
        get { return unit; }
        set { unit = value; }
    }
    /// <summary>
    /// The player that currently owns the sector.
    /// </summary>
    /// <returns></returns>
    public Player Owner
    {
        get { return owner; }
        set
        {
            owner = value;

            // set sector color to the color of the given player
            // or gray if null
            if (owner == null)
                gameObject.GetComponent<Renderer>().material.color = Color.gray;
            else
                gameObject.GetComponent<Renderer>().material.color = owner.Color;
        }
    }
    /// <summary>
    /// The list of adjacent sectors.
    /// </summary>
    public Sector[] AdjacentSectors
    {
        get { return adjacentSectors; }
    }
    /// <summary>
    /// The landmark accociated with this sector.
    /// </summary>
    public Landmark Landmark
    {
        get { return landmark; }
        set { landmark = value; }
    }
    /// <summary>
    /// Whether this sector has the PVC hidden on it.
    /// </summary>
    /// <returns></returns>
    public bool HasPVC
    {
        get { return PVC; }
        set { PVC = value; }
    }
    /// <summary>
    /// Whether the sector allows the PVC to be hidden on it.
    /// </summary>
    public bool AllowPVC
    {
        get
        {
            //instantiate the pvc only if the sector has nothing in it
            return landmark == null && unit == null;
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the sector object.
    /// </summary>
    public void Initialize()
    {
        // initialize the sector by setting its owner and unit to null
        // and determining if it contains a landmark or not

        // reset owner
        Owner = null;

        // clear unit
        unit = null;

        // get landmark (if any)
        landmark = gameObject.GetComponentInChildren<Landmark>();

        //initialize the PVC spawn to none
        HasPVC = false;
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Saves the sector state to a memento.
    /// </summary>
    /// <returns>The memento of the current sector state.</returns>
    public SerializableSector SaveToMemento()
    {
        return new SerializableSector
        {
            unit = unit?.SaveToMemento(),
            landmark = landmark?.SaveToMemento(),
            ownerId = owner?.Id ?? -1,
            PVC = PVC
        };
    }

    /// <summary>
    /// Restores the sector state from a memento. This also needs the list of
    /// available players in order to set up the Owner reference.
    /// </summary>
    /// <param name="memento">The memento to restore from.</param>
    /// <param name="players">The list of current players.</param>
    public void RestoreFromMemento(SerializableSector memento, Player[] players)
    {
        // restore the occupying unit
        if (memento.unit != null)
        {
            unit = Instantiate(players[memento.ownerId].unitPrefab).GetComponent<Unit>();
            unit.RestoreFromMemento(memento.unit, players, this);
        }
        landmark = gameObject.GetComponentInChildren<Landmark>();
        if (landmark != null)
            landmark.RestoreFromMemento(memento.landmark);
        if (memento.ownerId >= 0)
            players[memento.ownerId].Capture(this);
        PVC = memento.PVC;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Triggers the minigame. This function will prepare the game for
    /// the switch to the other scene, and then will conmmence it.
    /// </summary>
    public void TriggerMinigame()
    {
        Debug.Log("Oof! You've just stepped on the PVC! GET READY FOR SOME *industrial* ACTION");
        //Set the flag so Game would know to reallocate the PVC at the end of this player's turn
        Game.PVCEncountered = true;
        Game.LastDiscovererOfPVC = unit.Owner;
        GameObject.Find("GameManager").GetComponent<Game>().PrepareForMinigame();
        SceneManager.LoadScene("DoomMinigame");
    }

    /// <summary>
    /// Highlights the sector by increasing its RGB values by a specified amount.
    /// </summary>
    /// <param name="amount">The amount to increase the RGB values by.</param>
    public void ApplyHighlight(float amount)
    {
        Renderer currentRenderer = GetComponent<Renderer>();
        Color currentColor = currentRenderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor + offset;

        currentRenderer.material.color = newColor;
    }

    /// <summary>
    /// Unhighlights the sector by decreasing its RGB values by a specified amount.
    /// </summary>
    /// <param name="amount">The amount to decrease the RGB values by.</param>
    public void RevertHighlight(float amount)
    {
        Renderer currentRenderer = GetComponent<Renderer>();
        Color currentColor = currentRenderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor - offset;

        currentRenderer.material.color = newColor;
    }

    /// <summary>
    /// Highlights each sector adjacent to this one.
    /// </summary>
    public void ApplyHighlightAdjacent()
    {
        foreach (Sector adjacentSector in adjacentSectors)
        {
            adjacentSector.ApplyHighlight(0.2f);
        }
    }

    /// <summary>
    /// Unhighlight each sector adjacent to this one.
    /// </summary>
    public void RevertHighlightAdjacent()
    {
        foreach (Sector adjacentSector in adjacentSectors)
        {
            adjacentSector.RevertHighlight(0.2f);
        }
    }

    /// <summary>
    /// Clear this sector of any unit.
    /// </summary>
    public void ClearUnit()
    {
        unit = null;
    }

    /// <summary>
    /// When this sector is clicked, determine the context and act accordingly.
    /// </summary>
    void OnMouseUpAsButton()
    {
        OnMouseUpAsButtonAccessible();
    }

    /// <summary>
    /// A method of OnMouseUpAsButton that is accessible to other objects for testing.
    /// </summary>
    public void OnMouseUpAsButtonAccessible()
    {
        // if this sector contains a unit and belongs to the
        // current active player, and if no unit is selected
        if (unit != null && owner.IsActive && map.game.NoUnitSelected())
        {
            // select this sector's unit
            unit.Select();
        }

        // if this sector's unit is already selected
        else if (unit != null && unit.IsSelected)
        {
            // deselect this sector's unit           
            unit.Deselect();
        }

        // if this sector is adjacent to the sector containing
        // the selected unit
        else if (AdjacentSelectedUnit() != null)
        {
            // get the selected unit
            Unit selectedUnit = AdjacentSelectedUnit();

            // deselect the selected unit
            selectedUnit.Deselect();

            // if this sector is unoccupied
            if (unit == null)
                MoveIntoUnoccupiedSector(selectedUnit);

            // if the sector is occupied by a friendly unit
            else if (unit.Owner == selectedUnit.Owner)
                MoveIntoFriendlyUnit(selectedUnit);

            // if the sector is occupied by a hostile unit
            else if (unit.Owner != selectedUnit.Owner)
                MoveIntoHostileUnit(selectedUnit, unit);
        }
    }

    /// <summary>
    /// Moves the given unit into the current, unoccupied, sector.
    /// </summary>
    /// <param name="unit">The unit to move.</param>
    public void MoveIntoUnoccupiedSector(Unit unit)
    {
        //flag monitoring if the sector has a PVC spawned and the player moving in is neither
        //owning the sector, nor is he the last person to discover the PVC
        //Note: this is separated in a flag so that we can trigger the minigame after the MoveTo to avoid scene change problems
        bool foundPVC = HasPVC && unit.Owner != owner && Game.LastDiscovererOfPVC != unit.Owner;

        // move the selected unit into this sector
        unit.MoveTo(this);

        //if stepped on the PVC, trigger minigame
        if (foundPVC)
            TriggerMinigame();

        // advance turn state
        map.game.NextTurnState();
    }

    /// <summary>
    /// Swaps 2 friendly units between their sectors.
    /// </summary>
    /// <param name="otherUnit">The other unit to swap with.</param>
    public void MoveIntoFriendlyUnit(Unit otherUnit)
    {
        // swap the two units
        unit.SwapPlacesWith(otherUnit);

        // advance turn state
        map.game.NextTurnState();
    }

    /// <summary>
    /// Start and resolve a conflict.
    /// </summary>
    /// <param name="attackingUnit">Attacking unit.</param>
    /// <param name="defendingUnit">Defending unit.</param>
    public void MoveIntoHostileUnit(Unit attackingUnit, Unit defendingUnit)
    {
        // if the attacking unit wins
        if (Conflict(attackingUnit, defendingUnit))
        {
            // destroy defending unit
            defendingUnit.DestroySelf();

            // move the attacking unit into this sector
            attackingUnit.MoveTo(this);

            //if the sector just conquered has a PVC, trigger the minigame
            if (HasPVC)
                TriggerMinigame();
        }

        // if the defending unit wins
        else
        {
            // destroy attacking unit
            attackingUnit.DestroySelf();
        }

        // end the turn
        map.game.EndTurn();
    }


    /// <summary>
    /// Return the selected unit if it is adjacent to this sector,
    /// return null otherwise.
    /// </summary>
    /// <returns>The adjacent selected unit.</returns>
    public Unit AdjacentSelectedUnit()
    {
        // scan through each adjacent sector
        foreach (Sector adjacentSector in adjacentSectors)
            // if the adjacent sector contains the selected unit,
            // return the selected unit
            if (adjacentSector.unit != null && adjacentSector.unit.IsSelected)
                return adjacentSector.unit;

        // otherwise, return null
        return null;
    }

    /// <summary>
    /// Return 'true' if attacking unit wins,
    /// return 'false' if defending unit wins.
    /// </summary>
    /// <returns>The conflict.</returns>
    /// <param name="attackingUnit">Attacking unit.</param>
    /// <param name="defendingUnit">Defending unit.</param>
    bool Conflict(Unit attackingUnit, Unit defendingUnit)
    {
        /*
         * Conflict resolution is done by comparing a random roll 
         * from each unit involved. The roll is weighted based on
         * the unit's level and the amount of the associated 
         * resource the unit's owner has. Beer is associated with
         * attacking, and Knowledge is associated with defending.
         * 
         * The formula is:
         * 
         *     roll = [ a random integer with a lowerbound of 1
         *              and an upperbound of 5 + the unit's level ] 
         *           + [ the amount of the associated resource the
         *               unit's owner has ]
         * 
         * In the event of a tie, the defending unit wins the conflict
         */

        // calculate the rolls of each unit
        int attackingUnitRoll = Random.Range(1, (5 + attackingUnit.Level)) + attackingUnit.Owner.Beer;
        int defendingUnitRoll = Random.Range(1, (5 + defendingUnit.Level)) + defendingUnit.Owner.Knowledge;

        return (attackingUnitRoll > defendingUnitRoll);
    }

    #endregion
}