﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Unity Bindings

    public Player player;
    public Text header;
    public Text headerHighlight;
    public Text percentOwned;
    public Text beer;
    public Text knowledge;

    #endregion

    #region Private Fields

    int numberOfSectors;
    readonly Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    #endregion

    #region Initialization

    public void Initialize(Player player, int player_id)
    {
        this.player = player;

        header = transform.Find("Header").GetComponent<Text>();
        headerHighlight = transform.Find("HeaderHighlight").GetComponent<Text>();
        percentOwned = transform.Find("PercentOwned_Value").GetComponent<Text>();
        beer = transform.Find("Beer_Value").GetComponent<Text>();
        knowledge = transform.Find("Knowledge_Value").GetComponent<Text>();
        numberOfSectors = player.Game.gameMap.GetComponent<Map>().sectors.Length;

        header.text = "Player " + player_id.ToString();
        headerHighlight.text = header.text;
        headerHighlight.color = player.Color;
    }


    #endregion

    #region Helper Methods

    public void UpdateDisplay()
    {
        percentOwned.text = Mathf.Round(100 * player.ownedSectors.Count / numberOfSectors).ToString() + "%";
        beer.text = player.Beer.ToString();
        knowledge.text = player.Knowledge.ToString();
    }

    public void Activate()
    {
        header.color = player.Color;
    }

    public void Deactivate()
    {
        header.color = defaultHeaderColor;
    }

    #endregion
}
