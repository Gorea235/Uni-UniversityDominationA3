using UnityEngine;

/// <summary>
/// A data storage class to allow passing of data back
/// to the main game.
/// Used for minigame mode.
/// </summary>
public class DataStore : MonoBehaviour
{
    #region Classes

    public class DataStorage
    {
        public float Score { get; set; }
        public bool Succeeded { get; set; }
    }

    #endregion

    #region Private Fields

    DataStorage _store;

    #endregion

    #region Public Properties

    public float CurrentScore
    {
        get
        {
            return _store.Score;
        }
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _store = new DataStorage();
    }

    #endregion

    #region Helper Methods

    public void AddScore(float score) => _store.Score += score;

    public void SetSucceeded(bool success) => _store.Succeeded = success;

    public DataStorage Finalize()
    {
        Destroy(gameObject);
        return _store;
    }

    #endregion
}
