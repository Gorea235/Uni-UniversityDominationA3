﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    //get dropdown objects
    public GameObject playerOneType;
	public GameObject playerOneColour;
    public GameObject playerTwoType;
	public GameObject playerTwoColour;
    public GameObject playerThreeType;
	public GameObject playerThreeColour;
    public GameObject playerFourType;
	public GameObject playerFourColour;
	public GameObject startButton;
    public GameObject errorPanel;

    public void StartNewGame()
    {
		List<int> types;
		List<int> colours;
		types = GetPlayerTypes();
		colours = GetPlayerColours();
		Debug.Log ("Type values: " +  types[0].ToString() +","+ types[1].ToString()+","
			+ types[2].ToString()+","+ types[3].ToString());
		Debug.Log ("Colour values: " +  colours[0].ToString() +","+ colours[1].ToString()+","
			+ colours[2].ToString()+","+ colours[3].ToString());
      //  StartCoroutine(ShowPopUpMessage(2));
    }

	public void ColourDropdownUpdate()
	{
	  //  Update dropdowns to disable duplicate colours	
	}

    IEnumerator ShowPopUpMessage(float delay)
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        errorPanel.SetActive(false);
    }

	public List<int> GetPlayerTypes()
	{
		List<int> types;
		Dropdown One = playerOneType.GetComponent<Dropdown>();
		Dropdown Two = playerTwoType.GetComponent<Dropdown>();
		Dropdown Three = playerThreeType.GetComponent<Dropdown>();
		Dropdown Four = playerFourType.GetComponent<Dropdown> ();
	
		types = new List<int>(new int[] {One.value, Two.value, Three.value, Four.value});
		return types;
	}

	public List<int> GetPlayerColours()
	{
		List<int> colours;
		Dropdown One = playerOneColour.GetComponent<Dropdown>();
		Dropdown Two = playerTwoColour.GetComponent<Dropdown>();
		Dropdown Three = playerThreeColour.GetComponent<Dropdown>();
		Dropdown Four = playerFourColour.GetComponent<Dropdown> ();
		colours = new List<int> (new int[] {One.value, Two.value, Three.value, Four.value});
		return colours;
	}
}
