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
        // the minigame locks the cursor, so we need to undu that
        // if we just came from there
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // if GameToRestore has a value, than it means we want to restore
        // the game state from the memento, otherwise assume that we're
        // running a new game
        if (Game.GameToRestore == null)
            game.Initialize(); // start new game
        else
        {
            game.RestoreFromMemento(Game.GameToRestore); // restore game from memento
            Game.GameToRestore = null; // clear memento so we don't reload it by accident
        }
    }

    #endregion
}
