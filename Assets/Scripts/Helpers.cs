using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class for general helper functions.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// A general method for displaying popup panels.
    /// </summary>
    /// <param name="panel">The panel to make popup.</param>
    /// <param name="entryTime">The length of time it should take to enter the screen.</param>
    /// <param name="delay">The length of time to show the popup for.</param>
    /// <param name="exitTime">The length of time it should take to exit the screen.</param>
    /// <param name="text">
	/// If not null, then it will set the <see cref="Text"/> <see cref="GameObject"/> that is a
	/// child to the main panel to the given text.
	/// </param>
    /// <remarks>
    /// The panel will ONLY have its Y values changed, and it must be set up so that it fits the
	/// following constraints:
	/// - Anchors must be 'Top' and 'Stretch'
	/// - Y Pivot must be 0
	/// - Y Position must start at 0 to be offscreen
	/// - Panel must be active, as this is not changed here (since it starts offscreen, so is hidden anyway).
    /// </remarks>
    public static IEnumerator ShowPopUpMessage(GameObject panel, float entryTime, float delay, float exitTime, string text = null)
    {
        // this entire function is based off the functions that i've used for
        // the Doom clone, and has been modified to fit the general popup message
        // panel 

        // init vars
        RectTransform rectTransform = panel.GetComponent<RectTransform>(); // grab rect transform to fetch height and apply size to
        float percentShown = 0; // stores the current percentage of the panel that is shown
        float minY = 0; // the minimum Y value
        float maxY = -rectTransform.sizeDelta.y; // the maximum Y value (inverted because unity works bottom left -> up)

        // apply text if it was given
        if (text != null)
        {
            Text txt = panel.GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = text;
        }

        // do show animation
        while (percentShown < 1)
        {
            yield return new WaitForEndOfFrame();
            percentShown += Time.deltaTime / entryTime; // percent of the animation time we're thru
            percentShown = Mathf.Clamp01(percentShown);
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = Mathf.SmoothStep(minY, maxY, percentShown);
            rectTransform.anchoredPosition = pos;
        }

        // stay up for a given time
        yield return new WaitForSeconds(delay);

        // do hide animation
        while (percentShown > 0)
        {
            yield return new WaitForEndOfFrame();
            percentShown -= Time.deltaTime / exitTime; // percent of the animation time we're thru
            percentShown = Mathf.Clamp01(percentShown);
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = Mathf.SmoothStep(minY, maxY, percentShown);
            rectTransform.anchoredPosition = pos;
        }
    }
}
