using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrailController : MonoBehaviour
{
    #region Unity Bindings

    public float m_persistTime = 5f;

    #endregion

    #region Private Fields

    LineRenderer _line;
    float _startTime;

    #endregion

    #region Initialisation

    /// <summary>
    /// Initialises the bullet trail start and end points.
    /// </summary>
    /// <param name="start">The point to start at.</param>
    /// <param name="end">The point to end at.</param>
    /// <remarks>
    /// The start and end points are not interchangable, since the trail starts at a normal
    /// width, and then goes to a point at the end.
    /// </remarks>
    public void Init(Vector3 start, Vector3 end)
    {
        _line.SetPosition(0, start);
        _line.SetPosition(1, end);
    }

    #endregion

    #region MonoBeheviour

    void Awake()
    {
        _line = gameObject.GetComponent<LineRenderer>();
    }

    void Start()
    {
        _startTime = Time.realtimeSinceStartup; // get the time that this object was created
    }

    void Update()
    {
        // lerps the time that this has existed by the total time it should
        // the result is used to set the transparency of the bullet to simulate a 'fading' effect
        // if the result if 0, then it means that the bullet has persisted for it's allotted time,
        // and should be removed
        float lerp = Mathf.Lerp(1f, 0f, (Time.realtimeSinceStartup - _startTime) / m_persistTime);
        if (lerp == 0)
            Destroy(gameObject);
        else
            _line.startColor = _line.endColor = new Color(1f, 1f, 1f, lerp);
    }

    #endregion
}
