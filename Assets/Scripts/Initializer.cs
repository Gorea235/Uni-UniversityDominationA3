using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Initializer : MonoBehaviour
{
    #region Unity Bindings

    public Game game;
    public Game savedGame = new Game();

    #endregion

    public void Save()
    {
        //add items to savedGames

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, savedGame);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            savedGame = (Game)bf.Deserialize(file);
            file.Close();
        }
    }


    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        game.Initialize();
    }

    #endregion
}
