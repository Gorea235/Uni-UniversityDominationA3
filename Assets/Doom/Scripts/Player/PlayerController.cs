using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public float m_maxHealth;
    [Range(0, 1)]
    public float m_minHealthScoreModifier;
    public Mask m_healthBarMask;
    public Text m_HealthPercentText;

    #endregion

    #region Private Fields

    const string healthTextFormat = "{0:P0}";

    DataStore _dataStorage;
    AudioSource _audioSource;
    float _health;
    Vector2 _defaultHealthBarMaskSize;

    #endregion

    #region Private Properties

    /// <summary>
    /// The percentage of the players health that is currently remaining.
    /// </summary>
    float PercentHealth { get { return _health / m_maxHealth; } }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _defaultHealthBarMaskSize = m_healthBarMask.rectTransform.sizeDelta;
        _health = m_maxHealth;
    }

    void Start()
    {
        UpdateHealthBar();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Updates the mask and text of the health bar displayed on screen.
    /// </summary>
    void UpdateHealthBar()
    {
        m_healthBarMask.rectTransform.sizeDelta = new Vector2(_defaultHealthBarMaskSize.x * PercentHealth,
                                                              _defaultHealthBarMaskSize.y);
        m_HealthPercentText.text = string.Format(healthTextFormat, PercentHealth);
    }

    /// <summary>
    /// Applies damage to the player.
    /// </summary>
    /// <param name="amount">The amount of damage to apply.</param>
    public void Damage(float amount)
    {
        // update items
        _health -= amount;
        UpdateHealthBar();
        // update the modifier. will scale from m_minHealthScoreModifier to 1 depending on the percent health left
        _dataStorage.ScoreModifier = Mathf.Lerp(m_minHealthScoreModifier, 1, PercentHealth);
        // check if failed, otherwise play hurt sound
        if (_health <= 0)
        {
            Debug.Log("failed game");
            _dataStorage.SetSucceeded(false);
            // change scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
        }
        else
        {
            _audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            _audioSource.Play();
        }
    }

    #endregion
}
