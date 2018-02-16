using UnityEngine;
using System.Collections;

public static class UnitTestsUtil
{
	public static void SetupTest(ref Game game, ref Map map, ref Player[] players, ref PlayerUI[] gui)
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

        // enable game's test mode
        game.TestModeEnabled = true;

        // establish references from map to game & sectors (from children)
        map.game = game;
        map.sectors = map.gameObject.GetComponentsInChildren<Sector>();

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
    }

    public static void TearDownTest(ref Game game, ref Map map, ref Player[] players, ref PlayerUI[] gui)
    {
        Object.Destroy(game.gameObject);
        Object.Destroy(map.gameObject);
        Object.Destroy(players[0].transform.parent.gameObject);
        Object.Destroy(gui[0].transform.parent.gameObject);
        //foreach (var player in players)
        //    Object.Destroy(player.gameObject);
        //foreach (var ui in gui)
            //Object.Destroy(ui.gameObject);
    }
}
