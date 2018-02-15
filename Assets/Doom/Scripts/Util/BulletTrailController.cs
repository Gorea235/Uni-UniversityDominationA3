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
        _startTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        float lerp = Mathf.Lerp(1f, 0f, (Time.realtimeSinceStartup - _startTime) / m_persistTime);
        if (lerp == 0)
            Destroy(gameObject);
        else
            _line.startColor = _line.endColor = new Color(1f, 1f, 1f, lerp);
    }

    #endregion
}
