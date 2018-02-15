using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class DoorController : MonoBehaviour
{
    #region Unity Bindings

    public bool m_startOpen;
    public AudioClip m_openSound;
    public AudioClip m_closeSound;

    #endregion

    #region Private Fields

    Animator _animator;
    AudioSource _audioSource;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        if (m_startOpen)
        {
            _animator.Play("Open");
            _animator.SetBool("IsOpen", true);
        }
    }

    void DoActivateTrigger()
    {
        _animator.SetBool("IsOpen", !_animator.GetBool("IsOpen"));
    }

    #endregion

    #region Helper Methods

    internal void PlayOpenSound()
    {
        _audioSource.clip = m_openSound;
        _audioSource.Play();
    }

    internal void PlayCloseSound()
    {
        _audioSource.clip = m_closeSound;
        _audioSource.Play();
    }

    #endregion
}
