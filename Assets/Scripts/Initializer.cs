using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Initializer : MonoBehaviour
{
    #region Unity Bindings

    public Game game;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        if (MainMenu.startNewGame && Game.GameToRestore == null)
        {
            game.Initialize();
        }
        else
        {
            game.LoadGame("GameData.bin");
            Game.GameToRestore = null;
        }
    }

    #endregion
}
