using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ObjectiveUiController : MonoBehaviour
{
    #region Unity Bindings

    public float m_showTime;
    public float m_hideTime;
    public ShowState m_defaultState;
    public float m_objectiveUpdateFlashTime;
    public Text m_objectiveText;
    public Image m_objectiveFlash;

    #endregion

    #region Data Structures

    public enum ShowState
    {
        Show,
        Hide
    }

    enum InternalShowState
    {
        Shown,
        Showing,
        Hidden,
        Hiding
    }

    #endregion

    #region Private Fields

    const float _objectiveTextApplyPoint = 0.5f;

    Image _objectiveImage;
    InternalShowState _currentState;
    float _percentShown;
    bool _flashing;
    float _percentFlashed;

    #endregion

    #region Private Properties

    // full objective display

    float ObjectiveImageWidth { get { return _objectiveImage.rectTransform.sizeDelta.x; } }
    float ObjectiveImageX
    {
        get { return _objectiveImage.rectTransform.localPosition.x; }
        set
        {
            Vector3 pos = _objectiveImage.rectTransform.anchoredPosition;
            pos.x = value;
            _objectiveImage.rectTransform.anchoredPosition = pos;
        }
    }
    float ObjectiveImageShowPercent
    {
        get { return ObjectiveImageX / ObjectiveImageWidth; }
        set { ObjectiveImageX = ObjectiveImageWidth * (1 - value); }
    }
    float PercentShown
    {
        get { return _percentShown; }
        set
        {
            _percentShown = Mathf.Clamp01(value);
            ObjectiveImageShowPercent = Mathf.SmoothStep(0, 1, _percentShown);
        }
    }

    // objective text & flash
    public string ObjectiveText
    {
        get { return m_objectiveText.text; }
        set { m_objectiveText.text = value; }
    }
    float ObjectiveFlashAlpha
    {
        get { return m_objectiveFlash.color.a; }
        set
        {
            Color col = m_objectiveFlash.color;
            col.a = value;
            m_objectiveFlash.color = col;
        }
    }
    float PercentFlashed
    {
        get { return _percentFlashed; }
        set
        {
            _percentFlashed = Mathf.Clamp01(value);
            // flash sin function
            float a = 2;
            float b = -(Mathf.PI / 2f);
            ObjectiveFlashAlpha = (Mathf.Sin(a * _percentFlashed * Mathf.PI + b) / 2) + 0.5f;
        }
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _objectiveImage = gameObject.GetComponent<Image>();
        _objectiveImage.material.SetFloat("_FlashAmount", 0.5f);
        switch (m_defaultState)
        {
            case ShowState.Show:
                PercentShown = 1;
                _currentState = InternalShowState.Shown;
                break;
            case ShowState.Hide:
                PercentShown = 0;
                _currentState = InternalShowState.Hidden;
                break;
        }
    }

    void DoActivateTrigger() => Toggle();

    int test = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
            Toggle();
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            ObjectiveText = "";
            UpdateObjective(test.ToString(), test == 0 ? 0.2f : 0);
            test++;
        }
    }

    #endregion

    #region Helper Methods

    float PercentDelta(float animTime) => Time.deltaTime / animTime;

    #region Show/Hide

    // internal coroutines

    IEnumerator InternalShow()
    {
        while (PercentShown < 1)
        {
            yield return new WaitForEndOfFrame();
            if (_currentState != InternalShowState.Showing)
                yield break;
            PercentShown += PercentDelta(m_showTime);
        }
        _currentState = InternalShowState.Shown;
    }

    IEnumerator InternalHide()
    {
        while (PercentShown > 0)
        {
            yield return new WaitForEndOfFrame();
            if (_currentState != InternalShowState.Hiding)
                yield break;
            PercentShown -= PercentDelta(m_hideTime);
        }
        _currentState = InternalShowState.Hidden;
    }

    // public methods

    public void Show()
    {
        if (_currentState == InternalShowState.Shown ||
            _currentState == InternalShowState.Showing)
            return;
        _currentState = InternalShowState.Showing;
        StartCoroutine("InternalShow");
    }

    public void Hide()
    {
        if (_currentState == InternalShowState.Hidden ||
            _currentState == InternalShowState.Hiding)
            return;
        _currentState = InternalShowState.Hiding;
        StartCoroutine("InternalHide");
    }

    public void Toggle()
    {
        switch (_currentState)
        {
            case InternalShowState.Shown:
            case InternalShowState.Showing:
                Hide();
                break;
            case InternalShowState.Hidden:
            case InternalShowState.Hiding:
                Show();
                break;
        }
    }

    #endregion

    #region Text

    // internal coroutine

    IEnumerator InternalUpdateObjective(string text, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        PercentFlashed = 0;
        while (PercentFlashed < 1)
        {
            yield return new WaitForEndOfFrame();
            PercentFlashed += PercentDelta(m_objectiveUpdateFlashTime);
            if (PercentFlashed >= _objectiveTextApplyPoint &&
                ObjectiveText != text)
                ObjectiveText = text;
        }
        _flashing = false;
    }

    // public methods

    public void UpdateObjective(string text, float delay = 0)
    {
        if (_flashing)
        {
            StopCoroutine("InternalUpdateObjective");
        }
        _flashing = true;
        StartCoroutine(InternalUpdateObjective(text, delay));
    }

    #endregion

    #endregion
}
