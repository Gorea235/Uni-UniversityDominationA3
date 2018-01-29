using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    #region Unity Bindings

    public Game game;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        game.Initialize();
    }

    #endregion
}
