using System.Collections;
using UnityEngine;

public class DoomMinigameManager : MonoBehaviour
{
    #region Unity Bindings

    public float m_startSequenceLength;
    public GameObject m_startSequenceSphere;
    public Vector3 m_startSequenceSphereMaxScale;
    public MonoBehaviour[] m_startSequenceDisabledControllers;
    public GameObject m_startSequenceEndTrigger;

    public float m_endSequenceLength;
    public GameObject m_endSequenceRamp;
    public Vector3 m_endSequenceRampFinalPosition;

    public float m_winBonusScore;
    public ObjectiveUiController m_objectiveController;
    public string m_objectiveTextInitial;
    public string m_objectiveTextFirstRoomActive;
    public string m_objectiveTextFirstRoomCleared;
    public string m_objectiveTextSecondRoomActive;
    public string m_objectiveTextSecondRoomCleared;

    #endregion

    #region Private Fields

    Vector3 _startSequenceSphereStartScale;
    float _startSequenceScalePercent;
    Vector3 _endSequenceRampStartPosition;
    float _endSequencePositionPercent;
    int _triggerCount;

    #endregion

    #region Private Properties

    /// <summary>
    /// The scale of the sphere that surrounds the player to start with.
    /// </summary>
    Vector3 StartSequenceSphereScale
    {
        get { return m_startSequenceSphere.transform.localScale; }
        set { m_startSequenceSphere.transform.localScale = value; }
    }
    /// <summary>
    /// The percentage of the scaling we are through for the starting sphere.
    /// </summary>
    float StartSequenceSphereScalePercent
    {
        get { return _startSequenceScalePercent; }
        set
        {
            _startSequenceScalePercent = Mathf.Clamp01(value);
            StartSequenceSphereScale = Vector3.Lerp(_startSequenceSphereStartScale,
                                                    m_startSequenceSphereMaxScale,
                                                    _startSequenceScalePercent);
        }
    }

    /// <summary>
    /// The position of the ending ramp.
    /// </summary>
    Vector3 EndSequenceRampPosition
    {
        get { return m_endSequenceRamp.transform.position; }
        set { m_endSequenceRamp.transform.position = value; }
    }
    /// <summary>
    /// The percentage of the movement we are through for the ending ramp.
    /// </summary>
    float EndSequenceRampPositionPercent
    {
        get { return _endSequencePositionPercent; }
        set
        {
            _endSequencePositionPercent = Mathf.Clamp01(value);
            EndSequenceRampPosition = Vector3.Lerp(_endSequenceRampStartPosition,
                                                   m_endSequenceRampFinalPosition,
                                                   _endSequencePositionPercent);
        }
    }

    #endregion

    #region MonoBehaviour

    void Start()
    {
        StartCoroutine("StartSequence"); // do start sequence
    }

    void DoActivateTrigger()
    {
        // on activate trigger, do the next step in the sequence
        switch (_triggerCount)
        {
            case 0:
                // when all the enemies in the first room are killed
                m_objectiveController.UpdateObjective(m_objectiveTextFirstRoomCleared);
                break;
            case 1:
                // when the player enters one of the triggers that are just before the second room
                m_objectiveController.UpdateObjective(m_objectiveTextSecondRoomActive);
                break;
            case 2:
                // when all the enemies in the second room are killed
                m_objectiveController.UpdateObjective(m_objectiveTextSecondRoomCleared);
                StartCoroutine("EndSequence"); // do end sequence
                break;
            case 3:
                // when the player enters the trigger in front of the PVC model
                Debug.Log("won game");
                DataStore dataStorage = GameObject.Find("DataStore").GetComponent<DataStore>();
                dataStorage.AddScore(m_winBonusScore);
                dataStorage.SetSucceeded(true);
                // change scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
                break;
        }
        _triggerCount++;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// See <see cref="ObjectiveUiController.PercentDelta(float)"/>.
    /// </summary>
    float PercentDelta(float animTime) => Time.deltaTime / animTime;

    /// <summary>
    /// Sets the enabled start on all the controllers given.
    /// </summary>
    /// <param name="controllers">The controllers to set the enabled state of.</param>
    /// <param name="state">The state to apply.</param>
    void ApplyEnabled(MonoBehaviour[] controllers, bool state)
    {
        foreach (MonoBehaviour controller in controllers)
            controller.enabled = state;
    }

    /// <summary>
    /// Does the starting sequence.
    /// </summary>
    IEnumerator StartSequence()
    {
        // disable player looking and moving
        ApplyEnabled(m_startSequenceDisabledControllers, false);
        // lock cursor anyway
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // set the first objective
        m_objectiveController.ObjectiveText = "";
        m_objectiveController.UpdateObjective(m_objectiveTextInitial, 0.4f);
        m_objectiveController.Show();
        // scale the sphere out to make it look like the scene is loading in
        // and to give the player time to get ready
        _startSequenceSphereStartScale = StartSequenceSphereScale;
        _startSequenceScalePercent = 0;
        while (StartSequenceSphereScalePercent < 1)
        {
            yield return new WaitForEndOfFrame();
            StartSequenceSphereScalePercent += PercentDelta(m_startSequenceLength);
        }
        // re-enable player moving and looking
        ApplyEnabled(m_startSequenceDisabledControllers, true);
        // remove the sphere
        Destroy(m_startSequenceSphere);
        // trigger the start sequence end GameObject
        m_startSequenceEndTrigger?.BroadcastMessage("DoActivateTrigger");
        // update the objective to the next one
        m_objectiveController.UpdateObjective(m_objectiveTextFirstRoomActive);
    }

    /// <summary>
    /// Does the ending sequence.
    /// </summary>
    IEnumerator EndSequence()
    {
        // sets the ramp to active to enable it on the scene and moves it into
        // position via an animation
        m_endSequenceRamp.SetActive(true);
        _endSequenceRampStartPosition = EndSequenceRampPosition;
        _endSequencePositionPercent = 0;
        while (EndSequenceRampPositionPercent < 1)
        {
            yield return new WaitForEndOfFrame();
            EndSequenceRampPositionPercent += PercentDelta(m_endSequenceLength);
        }
    }

    #endregion
}
