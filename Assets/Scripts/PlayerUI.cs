using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    #region Private Fields

    [SerializeField]
    Player player;
    [SerializeField]
    UnityEngine.UI.Text header;
    [SerializeField]
    UnityEngine.UI.Text headerHighlight;
    [SerializeField]
    UnityEngine.UI.Text percentOwned;
    [SerializeField]
    UnityEngine.UI.Text beer;
    [SerializeField]
    UnityEngine.UI.Text knowledge;
    [SerializeField]
    int numberOfSectors;
    readonly Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    #endregion

    #region Initialization

    public void Initialize(Player player, int player_id)
    {
        this.player = player;

        header = transform.Find("Header").GetComponent<UnityEngine.UI.Text>();
        headerHighlight = transform.Find("HeaderHighlight").GetComponent<UnityEngine.UI.Text>();
        percentOwned = transform.Find("PercentOwned_Value").GetComponent<UnityEngine.UI.Text>();
        beer = transform.Find("Beer_Value").GetComponent<UnityEngine.UI.Text>();
        knowledge = transform.Find("Knowledge_Value").GetComponent<UnityEngine.UI.Text>();
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
