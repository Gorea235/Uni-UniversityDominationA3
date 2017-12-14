﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public List <Sector> ownedSectors;
    public List <Unit> units;

    [SerializeField] private Game game;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private int beer = 0;
    [SerializeField] private int knowledge = 0;
    [SerializeField] private Color color;
    [SerializeField] private bool human;
    [SerializeField] private bool active = false;


    public Game GetGame() {
        return game;
    }

    public void SetGame(Game game) {
        this.game = game;
    }

    public GameObject GetUnitPrefab() {
        return unitPrefab;
    }

    public int GetBeer() {
        return beer;
    }

    public void SetBeer(int beer) {
        this.beer = beer;
    }

    public int GetKnowledge() {
        return knowledge;
    }

    public void SetKnowledge(int knowledge) {
        this.knowledge = knowledge;
    }

    public Color GetColor() {
        return color;
    }

    public void SetColor(Color color) {
        this.color = color;
    }

    public bool IsHuman() {
        return human;
    }

    public void SetHuman(bool human) {
        this.human = human;
    }

    public bool IsActive() {
        return active;
    }

    public void SetActive(bool active) {
        this.active = active;
    }



    public void Capture(Sector sector) {

        // capture the given sector


        // store a copy of the sector's previous owner
        Player previousOwner = sector.GetOwner();

        // add the sector to the list of owned sectors
        ownedSectors.Add(sector);

        // remove the sector from the previous owner's
        // list of sectors
        if (previousOwner != null)
            previousOwner.ownedSectors.Remove(sector);

        // set the sector's owner to this player
        sector.SetOwner(this);

        // if the sector contains a landmark
        if (sector.IsLandmark())
        {
            Landmark landmark = sector.GetComponentInChildren<Landmark>();

            // remove the landmark's resource bonus from the previous
            // owner and add it to this player
            if (landmark.GetResourceType() == Landmark.ResourceType.Beer)
            {
                this.beer += landmark.GetAmount();
                if (previousOwner != null)
                    previousOwner.beer -= landmark.GetAmount();
            }
            else if (landmark.GetResourceType() == Landmark.ResourceType.Knowledge)
            {
                this.knowledge += landmark.GetAmount();
                if (previousOwner != null)
                    previousOwner.knowledge -= landmark.GetAmount();
            }
        }
    }

    public void SpawnUnits() {

        // spawn a unit at each unoccupied landmark


        // scan through each owned sector
		foreach (Sector sector in ownedSectors) 
		{
            // if the sector contains a landmark and is unoccupied
            if (sector.IsLandmark() && sector.GetUnit() == null)
            {
                // instantiate a new unit at the sector
                Unit newUnit = Instantiate(unitPrefab).GetComponent<Unit>();

                // initialize the new unit
                newUnit.Initialize(this, sector);

                // add the new unit to the player's list of units and 
                // the sector's unit parameters
                units.Add(newUnit);
                sector.SetUnit(newUnit);
            }
		}
	}

    public bool IsEliminated() {

        // returns true if the player is eliminated, false otherwise;
        // a player is considered eliminated if it has no units left
        // and does not own a landmark

        if (units.Count == 0 && !OwnsLandmark())
            return true;
        else
            return false;
    }

    private bool OwnsLandmark() {

        // returns true if the player owns at least one landmark,
        // false otherwise

        
        // scan through each owned sector
        foreach (Sector sector in ownedSectors)
        {
            // if a landmarked sector is found, return true
            if (sector.IsLandmark())
                return true;
        }

        // otherwise, return false
        return false;
    }

}