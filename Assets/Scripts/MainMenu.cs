using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    //get dropdown objects
    public GameObject playerOne;
    public GameObject playerTwo;
    public GameObject playerThree;
    public GameObject playerFour;
    public GameObject errorPanel;

    public void StartNewGame()
    {

      //  StartCoroutine(ShowPopUpMessage(2));
    }


    IEnumerator ShowPopUpMessage(float delay)
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        errorPanel.SetActive(false);
    }


}
