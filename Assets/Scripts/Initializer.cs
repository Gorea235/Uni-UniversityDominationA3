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

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (Game.GameToRestore == null)
            game.Initialize();
        else
        {
            game.RestoreFromMemento(Game.GameToRestore);
            Game.GameToRestore = null;
        }
    }

    #endregion
}
