using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("transition", 1);
        Screen.SetResolution(720, 1280, true);
        Application.targetFrameRate = 60;
    }

    public void PlayButtonPressed()
    {
        StartCoroutine(transitionWait());
    }

    private IEnumerator transitionWait()
    {
        GameObject.Find("Transition").GetComponent<Animator>().SetBool("Exit", false);
        GameObject.Find("Transition2").GetComponent<Animator>().SetBool("Exit", false);
        GameObject.Find("Transition").GetComponent<Animator>().SetBool("Trans", true);
        GameObject.Find("Transition2").GetComponent<Animator>().SetBool("Trans", true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameScene");
    }
}
