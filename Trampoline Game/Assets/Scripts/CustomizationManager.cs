using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class CustomizationManager : MonoBehaviour
{
    public GameObject lock1;
    public GameObject lock2;
    public GameObject headScroll;
    public GameObject torsoScroll;
    public GameObject headContent;
    public GameObject torsoContent;

    public Text coinsText;

    Sprite[] heads;
    Sprite[] torsos;

    int currentPos;

    int headMax = 10;
    int torsoMax = 8;

    List<int> headArrangement = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    List<int> torsoArrangement = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

    bool head;

    private void Start()
    {
        PlayerPrefs.SetInt("transition", 1);
        coinsText.text = "" + PlayerPrefs.GetInt("coins");
        GameObject.Find("Transition").GetComponent<Animator>().SetBool("Exit", true);
        GameObject.Find("Transition2").GetComponent<Animator>().SetBool("Exit", true);

        heads = Resources.LoadAll<Sprite>("Heads");
        torsos = Resources.LoadAll<Sprite>("Bodies");

        for (int i = 1; i < headMax; i++)
        {
            if (PlayerPrefs.GetInt("head" + i) == 1)
            {
                headArrangement.Remove(i);
                headArrangement.Insert(1, i);
            }
        }
        for (int i = 1; i < torsoMax; i++)
        {
            if (PlayerPrefs.GetInt("torso" + i) == 1)
            {
                torsoArrangement.Remove(i);
                torsoArrangement.Insert(1, i);
            }
        }

        for (int i = 0; i < headMax; i++)
        {
            headContent.transform.GetChild(i + 1).GetComponent<Image>().sprite = heads[headArrangement[i]];
        }
        for (int i = 0; i < torsoMax; i++)
        {
            torsoContent.transform.GetChild(i + 1).GetComponent<Image>().sprite = torsos[torsoArrangement[i]];
        }

        int headPos = headArrangement.IndexOf(PlayerPrefs.GetInt("selectedHead")) + 1;
        int torsoPos = torsoArrangement.IndexOf(PlayerPrefs.GetInt("selectedTorso")) + 1;

        print("headPos " + headPos);
        print("torsoPos " + torsoPos);

        headScroll.GetComponent<Scrollbar>().value = headPos * 0.1f;
        torsoScroll.GetComponent<Scrollbar>().value = torsoPos * 0.13f; //check if 0.13 is correct

        headChange();
        torsoChange();
    }

    public void BackPressed()
    {
        print("back pressed");

        buttonPressedSound();

        /*int selectedHead = Mathf.RoundToInt(headScroll.GetComponent<Scrollbar>().value * 10) - 1;
        //print("selectedHead " + (selectedHead + 1));
        //print("PlayerPrefs.GetInt(head + selectedHead) " + PlayerPrefs.GetInt("head" + headArrangement[selectedHead + 1]));
        if (selectedHead == PlayerPrefs.GetInt("head" + headArrangement[selectedHead + 1]))
        {
            print("item exists");
            PlayerPrefs.SetInt("selectedHead", selectedHead);
        }
        else
        {
            print("item doesn't exist");
        }

        int selectedTorso = Mathf.RoundToInt(torsoScroll.GetComponent<Scrollbar>().value * 8) - 1;
        //print("selectedTorso " + (selectedTorso + 1));
        //print("PlayerPrefs.GetInt(torso + selectedTorso) " + PlayerPrefs.GetInt("torso" + torsoArrangement[selectedTorso + 1]));
        if (selectedTorso == PlayerPrefs.GetInt("torso" + torsoArrangement[selectedTorso + 1]))
        {
            print("item exists");
            PlayerPrefs.SetInt("selectedTorso", selectedTorso);
        }
        else
        {
            print("item doesn't exist");
        }*/

        int selectedHead = Mathf.RoundToInt(headScroll.GetComponent<Scrollbar>().value * 10) - 1;
        int selectedTorso = Mathf.RoundToInt(torsoScroll.GetComponent<Scrollbar>().value * 8) - 1;

        print("selectedHead " + selectedHead);
        print("headArrangement[selectedHead] " + headArrangement[selectedHead]);

        print("PlayerPrefs.GetInt(head + selectedHead) " + PlayerPrefs.GetInt("head" + headArrangement[selectedHead]));
        print("PlayerPrefs.GetInt(torso + selectedTorso) " + PlayerPrefs.GetInt("torso" + torsoArrangement[selectedTorso]));

        PlayerPrefs.SetInt("selectedHead", headArrangement[selectedHead]); //+1
        PlayerPrefs.SetInt("selectedTorso", torsoArrangement[selectedTorso]); //+1

        if (PlayerPrefs.GetInt("selectedHead") == -1)
        {
            PlayerPrefs.SetInt("ranHead", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ranHead", 0);
        }

        if (PlayerPrefs.GetInt("selectedTorso") == -1)
        {
            PlayerPrefs.SetInt("ranTorso", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ranTorso", 0);
        }

        if (PlayerPrefs.GetInt("head" + PlayerPrefs.GetInt("selectedHead")) == 0)
        {
            print("reset");
            PlayerPrefs.SetInt("selectedHead", 0);
        }

        if (PlayerPrefs.GetInt("torso" + PlayerPrefs.GetInt("selectedTorso")) == 0)
        {
            print("reset");
            PlayerPrefs.SetInt("selectedTorso", 0);
        }

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

    public void headChange()
    {
        currentPos = Mathf.RoundToInt(headScroll.GetComponent<Scrollbar>().value * 10) - 1;

        if(currentPos == -1)
        {
            lock1.SetActive(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("head" + headArrangement[currentPos]) == 1)
            {
                lock1.SetActive(false);
            }
            else
            {
                lock1.SetActive(true);
            }
        }
    }

    public void torsoChange()
    {
        currentPos = Mathf.RoundToInt(torsoScroll.GetComponent<Scrollbar>().value * 8) - 1;

        if (currentPos == -1)
        {
            lock2.SetActive(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("torso" + torsoArrangement[currentPos]) == 1)
            {
                lock2.SetActive(false);
            }
            else
            {
                lock2.SetActive(true);
            }
        }
    }

    public void SetHead(bool _head)
    {
        head = _head;
    }

    public void ArrowPressed(bool right)
    {
        buttonPressedSound();
        if (head)
        {
            if(right)
            {
                if (headScroll.GetComponent<Scrollbar>().value + 0.1f <= 1)
                {
                    headScroll.GetComponent<Scrollbar>().value += 0.1f;
                }
            }
            else
            {
                if (headScroll.GetComponent<Scrollbar>().value - 0.1f >= 0)
                {
                    headScroll.GetComponent<Scrollbar>().value -= 0.1f;
                }
            }
        }
        else
        {
            if (right)
            {
                if(torsoScroll.GetComponent<Scrollbar>().value + 0.15f <= 1.1f)
                {
                    torsoScroll.GetComponent<Scrollbar>().value += 0.15f;
                }
            }
            else
            {
                if (torsoScroll.GetComponent<Scrollbar>().value - 0.15f >= -0.1f)
                {
                    torsoScroll.GetComponent<Scrollbar>().value -= 0.15f;
                }
            }
        }
    }

    public void buttonPressedSound()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }
}
