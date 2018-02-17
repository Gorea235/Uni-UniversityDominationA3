using UnityEngine;

/// <summary>
/// A data storage class to allow passing of data back
/// to the main game.
/// Used for minigame mode.
/// </summary>
public class DataStore : MonoBehaviour
{
    #region Classes

    /// <summary>
    /// The storage class for the minigame data that needs to be preserved.
    /// </summary>
    public class DataStorage
    {
        /// <summary>
        /// The score the player got in the minigame.
        /// The score modifier is applied after <see cref="Finalize()"/> is called.
        /// </summary>
        public float Score { get; set; }
        /// <summary>
        /// Whether the player beat the minigame.
        /// </summary>
        public bool Succeeded { get; set; }
    }

    #endregion

    #region Private Fields

    DataStorage _store;
    bool _finalized;

    #endregion

    #region Public Properties

    /// <summary>
    /// The current score of the minigame without modifiers.
    /// </summary>
    public float CurrentScore { get { return _store.Score; } }
    /// <summary>
    /// The score modifier to apply when <see cref="Finalize()"/> is called.
    /// </summary>
    /// <returns></returns>
    public float ScoreModifier { get; set; }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _store = new DataStorage();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Adds the given amount to the current score.
    /// </summary>
    /// <param name="score">The score to add.</param>
    public void AddScore(float score) => _store.Score += score;

    /// <summary>
    /// Set whether the player beat the minigame.
    /// </summary>
    /// <param name="success">Whether the player beat the minigame.</param>
    public void SetSucceeded(bool success) => _store.Succeeded = success;

    /// <summary>
    /// Finalises the data storage. This includes applying the score modifier
    /// and destroying the <see cref="GameObject"/> in order to clean up the scene.
    /// </summary>
    /// <returns>The final minigame result.</returns>
    public DataStorage Finalize()
    {
        if (_finalized)
            throw new System.ObjectDisposedException("DataStore", "Object has already been finalized.");
        _store.Score = _store.Score * ScoreModifier; // apply modifier
        Destroy(gameObject); // clean up holding object
        _finalized = true; // say that we've now finalised
        return _store;
    }

    #endregion
}
