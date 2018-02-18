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

    /// <summary>
    /// The inital state of the objective UI.
    /// </summary>
    public enum ShowState
    {
        Show,
        Hide
    }

    /// <summary>
    /// The current state of the objective UI.
    /// </summary>
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

    // ========== full objective display ==========

    /// <summary>
    /// The current objective UI <see cref="Image"/> width.
    /// </summary>
    float ObjectiveImageWidth { get { return _objectiveImage.rectTransform.sizeDelta.x; } }
    /// <summary>
    /// Get or set the current X position of the objective UI.
    /// </summary>
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
    /// <summary>
    /// Get or set the amount of the objective UI image that is shown.
    /// </summary>
    float ObjectiveImageShowPercent
    {
        get { return ObjectiveImageX / ObjectiveImageWidth; }
        set { ObjectiveImageX = ObjectiveImageWidth * (1 - value); }
    }
    /// <summary>
    /// Get or set the amount of the objective UI that is shown.
    /// Will also apply a smooth step to <see cref="ObjectiveImageShowPercent"/>
    /// </summary>
    float PercentShown
    {
        get { return _percentShown; }
        set
        {
            _percentShown = Mathf.Clamp01(value);
            ObjectiveImageShowPercent = Mathf.SmoothStep(0, 1, _percentShown);
        }
    }

    // ========== objective text & flash ==========
    /// <summary>
    /// The text currently displayed by the objective UI.
    /// </summary>
    public string ObjectiveText
    {
        get { return m_objectiveText.text; }
        set { m_objectiveText.text = value; }
    }
    /// <summary>
    /// The current alpha value of the flash image that sits over the objective UI.
    /// </summary>
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
    /// <summary>
    /// The percentage through the flash we are currently at. Uses a sin function to
    /// set the alpha value of the flash.
    /// </summary>
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
        // init default state
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

    void DoActivateTrigger() => Toggle(); // allows toggling of the UI via activate triggers

    #endregion

    #region Helper Methods

    /// <summary>
    /// Calculates the percentage of time that passed during the last frame according to the
    /// given animation time. Used since animations are processed every frame while occurring,
    /// meaning that the time the last frame took is how much of the animation should have passed.
    /// </summary>
    /// <param name="animTime">The animation time to get the percent of.</param>
    float PercentDelta(float animTime) => Time.deltaTime / animTime;

    #region Show/Hide

    // internal coroutines

    /// <summary>
    /// Controls the show animation of the objective UI.
    /// </summary>
    IEnumerator InternalShow()
    {
        while (PercentShown < 1)
        {
            yield return new WaitForEndOfFrame();
            // if Hide was called during the show animation, cancel this animation
            if (_currentState != InternalShowState.Showing)
                yield break;
            PercentShown += PercentDelta(m_showTime);
        }
        _currentState = InternalShowState.Shown;
    }

    /// <summary>
    /// Controls the hide animation of the objective UI.
    /// </summary>
    IEnumerator InternalHide()
    {
        while (PercentShown > 0)
        {
            yield return new WaitForEndOfFrame();
            // if Show was called during the show animation, cancel this animation
            if (_currentState != InternalShowState.Hiding)
                yield break;
            PercentShown -= PercentDelta(m_hideTime);
        }
        _currentState = InternalShowState.Hidden;
    }

    // public methods

    /// <summary>
    /// Shows the objective UI. Will handle re-firings without issue.
    /// </summary>
    public void Show()
    {
        // prevent re-firings
        if (_currentState == InternalShowState.Shown ||
            _currentState == InternalShowState.Showing)
            return;
        _currentState = InternalShowState.Showing;
        StartCoroutine("InternalShow");
    }

    /// <summary>
    /// Hides the objective UI. Will handle re-firings without issue.
    /// </summary>
    public void Hide()
    {
        if (_currentState == InternalShowState.Hidden ||
            _currentState == InternalShowState.Hiding)
            return;
        _currentState = InternalShowState.Hiding;
        StartCoroutine("InternalHide");
    }

    /// <summary>
    /// Toggles the show state of the objective UI.
    /// </summary>
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

    /// <summary>
    /// Controls the flash animation text update of the objective UI. The text is only
    /// updated when the animation is at its peak so the user cannot see the text change,
    /// making the animation cleaner.
    /// </summary>
    /// <param name="text">The text to update to.</param>
    /// <param name="delay">
    /// How long to wait before starting the animation. Allows for it to be started immediately
    /// but have the animation actually occur later on.
    /// </param>
    IEnumerator InternalUpdateObjective(string text, float delay)
    {
        // if we want to delay, do so
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        PercentFlashed = 0;
        while (PercentFlashed < 1)
        {
            yield return new WaitForEndOfFrame();
            PercentFlashed += PercentDelta(m_objectiveUpdateFlashTime);
            if (PercentFlashed >= _objectiveTextApplyPoint &&
                ObjectiveText != text) // if >= 50% done and we haven't update the text, do so
                ObjectiveText = text;
        }
        _flashing = false;
    }

    // public methods

    /// <summary>
    /// Updates the objective UI to the given text.
    /// </summary>
    /// <param name="text">The text to update to.</param>
    /// <param name="delay">
    /// How long to wait before starting the animation. Allows for it to be started immediately
    /// but have the animation actually occur later on.
    /// </param>
    public void UpdateObjective(string text, float delay = 0)
    {
        if (_flashing) // if we are already updating the UI, stop it and restart
            StopCoroutine("InternalUpdateObjective");
        _flashing = true;
        StartCoroutine(InternalUpdateObjective(text, delay));
    }

    #endregion

    #endregion
}
