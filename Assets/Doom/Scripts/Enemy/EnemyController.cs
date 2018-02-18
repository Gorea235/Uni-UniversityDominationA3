using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EnemyController : MonoBehaviour
{
    #region Unity Bindings

    public AudioClip[] m_hurtSounds;
    public AudioClip m_deathSound;
    public float m_health;
    public bool m_active = true;
    public bool m_chase = true;
    public Vector3 m_lookAtOffset;
    public float m_maxMoveSpeed = 1f;
    public float m_moveSpeedScale = 1f;
    public float m_acceleration = 1f;
    public float m_deceleration = 1f;
    public float m_damage = 1f;
    public float m_range = 1f;
    public float m_damageRate = 1f; // per second
    public float m_killScore = 1f;

    #endregion

    #region Private Fields

    GameObject _player;
    PlayerController _playerController;
    DataStore _dataStorage;
    AudioSource _audioSource;
    Animator _animator;
    bool _dead;
    float _currentMoveSpeed;
    float _lastHit;

    #endregion

    #region Public Properties

    /// <summary>
    /// Whether the enemy is dead.
    /// </summary>
    public bool IsDead { get { return _dead; } }
    /// <summary>
    /// Whether the enemy is active currently.
    /// </summary>
    public bool IsActive
    {
        get { return m_active; }
        set { m_active = value; }
    }
    /// <summary>
    /// Whether the enemy is chasing the player.
    /// </summary>
    /// <returns></returns>
    public bool IsChasing
    {
        get { return m_chase; }
        set { m_chase = value; }
    }

    #endregion

    #region Events

    /// <summary>
    /// Fires when the enemy dies.
    /// </summary>
    public event System.EventHandler OnDeath;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _player = GameObject.Find("FPSController");
        _playerController = _player.GetComponentInChildren<PlayerController>();
        _dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (_dead) // if dead stop updates
            return;

        if (IsActive) // only process if currently active
        {
            // check if in attacking range
            if (Vector3.Distance(gameObject.transform.position, _player.transform.position) <= m_range)
            {
                SetAttacking(); // set attacking animation
                // only attack if it's been long enough since the last attack
                if (Time.realtimeSinceStartup - _lastHit >= m_damageRate)
                {
                    _playerController.Damage(m_damage);
                    _lastHit = Time.realtimeSinceStartup;
                }
            }
            else
            {
                // if chasing the player, apply animation and move
                if (IsChasing)
                {
                    SetChasing();
                    DoChasePlayer(true);
                }
                else
                    SetIdling();
            }
        }
        else
            SetIdling();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Sets the current animation state. Animator set to change the animation state immediately.
    /// 0 - Idling
    /// 1 - Running
    /// 2 - Attacking
    /// </summary>
    /// <param name="state">The animation state to move to.</param>
    void ApplyAnimationState(int state) => _animator.SetInteger("State", state);

    /// <summary>
    /// Set the enemy to the idling animation.
    /// </summary>
    void SetIdling() => ApplyAnimationState(0);

    /// <summary>
    /// Set the enemy to the running animation.
    /// </summary>
    void SetChasing() => ApplyAnimationState(1);

    /// <summary>
    /// Set the enemy to the attacking animation.
    /// </summary>
    void SetAttacking() => ApplyAnimationState(2);

    /// <summary>
    /// Set the enemy to the dead animation.
    /// Once this is set, the animator will not take into account the integer state
    /// of if this is set back to false (it will simply stay in the dead state permanently).
    /// </summary>
    void SetDead() => _animator.SetBool("IsDead", true);

    /// <summary>
    /// Looks at and moves towards the player. Allows for acceleration and deceleration.
    /// </summary>
    /// <param name="accelerate"></param>
    void DoChasePlayer(bool accelerate)
    {
        // rotate towards player
        transform.LookAt(_player.transform);
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0;
        newRot.z = 0;
        transform.eulerAngles = newRot + m_lookAtOffset;

        // apply acceleration
        if (accelerate)
            _currentMoveSpeed = Mathf.Min(_currentMoveSpeed + m_acceleration, m_maxMoveSpeed);
        else
            _currentMoveSpeed = Mathf.Max(_currentMoveSpeed - m_deceleration, 0);
        float oldPosY = transform.localPosition.y;
        transform.Translate(new Vector3(0, 0, _currentMoveSpeed * Time.deltaTime * m_moveSpeedScale));
        Vector3 newPos = transform.localPosition;
        newPos.y = oldPosY;
        transform.localPosition = newPos;
    }

    /// <summary>
    /// Causes the enemy to die.
    /// </summary>
    /// <param name="applyScore">Whether to apply the score that this enemy is worth.</param>
    public void Die(bool applyScore = true)
    {
        _dead = true;
        SetDead();
        gameObject.GetComponent<Rigidbody>().isKinematic = true; // stop all future movement
        gameObject.GetComponent<Collider>().enabled = false; // disable collisions
        _audioSource.clip = m_deathSound;
        _audioSource.Play(); // play death sound
        if (applyScore)
            _dataStorage.AddScore(m_killScore);
        OnDeath?.Invoke(this, new System.EventArgs()); // fire death event
    }

    /// <summary>
    /// Damages the enemy by the given amount.
    /// </summary>
    /// <param name="amount">The amount to damage the enemy by.</param>
    public void Damage(float amount)
    {
        m_health -= amount;
        if (m_health <= 0) // if health dropped below 0, enemy is dead
            Die();
        else
        { // otherise just play a random hurt sound
            _audioSource.clip = m_hurtSounds[Random.Range(0, m_hurtSounds.Length)];
            _audioSource.Play();
        }
    }

    #endregion
}
