using System.Collections;
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
    public Image arrowOfCurrentPlayer;

    #endregion

    #region Private Fields

    int numberOfSectors;
    readonly Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the player UI.
    /// </summary>
    /// <param name="player">The player that this UI refers to.</param>
    /// <param name="playerId">The id of the given player.</param>
    public void Initialize(Player player, int playerId)
    {
        this.player = player;

        // fetch in all the UI elements
        header = transform.Find("Header").GetComponent<Text>();
        headerHighlight = transform.Find("HeaderHighlight").GetComponent<Text>();
        percentOwned = transform.Find("PercentOwned_Value").GetComponent<Text>();
        beer = transform.Find("Beer_Value").GetComponent<Text>();
        knowledge = transform.Find("Knowledge_Value").GetComponent<Text>();
        numberOfSectors = player.Game.gameMap.GetComponent<Map>().sectors.Length;
        arrowOfCurrentPlayer = transform.Find("Arrow").GetComponent<Image>();

        header.text = (player.IsHuman) ? "Player " + playerId.ToString() : "AI";
        headerHighlight.text = header.text;
        headerHighlight.color = player.Color;
        arrowOfCurrentPlayer.CrossFadeAlpha(0f, 0f, true);
    }


    #endregion

    #region Helper Methods

    /// <summary>
    /// Updates the UI with the current values.
    /// </summary>
    public void UpdateDisplay()
    {
        percentOwned.text = Mathf.Round(100 * player.ownedSectors.Count / numberOfSectors).ToString() + "%";
        beer.text = player.Beer.ToString();
        knowledge.text = player.Knowledge.ToString();
    }

    /// <summary>
    /// Sets the UI to active and shows the necessary UI elements.
    /// </summary>
    public void Activate()
    {
        arrowOfCurrentPlayer.CrossFadeAlpha(1f, 0.5f, true);
        header.color = player.Color;
    }

    /// <summary>
    /// Sets the UI to deactive and hides the necessary UI elements.
    /// </summary>
    public void Deactivate()
    {
        arrowOfCurrentPlayer.CrossFadeAlpha(0f, 0f, true);
        header.color = defaultHeaderColor;
    }

    #endregion
}
