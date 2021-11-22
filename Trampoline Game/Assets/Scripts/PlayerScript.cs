using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using EZCameraShake;
//using UnityEngine.Monetization;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Text scoreText;
    public Text flipText;
    public Text coinText;
    public Text inGameCoinText;
    public Text highscoreText;
    public Text highscoreInGameText;

    public GameObject newHighScoreText;
    public GameObject playerCamera;
    public GameObject boundary;
    public GameObject menu;
    public GameObject deathMenu;
    public GameObject watchAd;
    public GameObject prizeSprite;
    public GameObject trampoline;
    public GameObject ratePrompt;
    public GameObject purchaseButton;
    public GameObject soundButton;
    public GameObject coinCollectable;
    public GameObject heighestHeightBar;
    public GameObject settingsWindow;
    public GameObject tutorialButtons;

    public Sprite[] prizeButton;
    public Sprite[] musicSprite;

    public Transform tester1;
    public Transform tester2;
    public Transform tester3;
    public Transform tester4;

    public fbManager faceBookManager;

    AudioSource audioSource;

    int score = 0;
    int gainedScore;
    int ranPick;
    int headMax = 10;
    int torsoMax = 7;
    int ranDeg;
    int torsoInt;
    int flipAmount = 0;
    int adType;
    int timer = 0;
    int height = 0;

    float rotSpeed = 0;
    [SerializeField] float maxRotSpeed;
    [SerializeField] float rotIncrease;
    [SerializeField] float rotForce;
    [SerializeField] float bounceForce;
    [SerializeField] float bounceForceIncrease;
    [SerializeField] float bounceForceIncreasePlus;
    [SerializeField] float bounceDifficulty;
    [SerializeField] float bounceDiffIncrease;
    [SerializeField] float birdSpawnWaitMin;
    [SerializeField] float birdSpawnWaitMax;

    bool flip;
    bool dead;
    bool soundOn;
    bool onceL;
    bool onceR;
    bool onceE;
    bool frozen;

    Sprite[] heads;
    Sprite[] legs;
    Sprite[] arms;
    Sprite[] legsbent;
    Sprite[] armsbent;
    Sprite[] torso;
    Sprite[] bodies;

    int[] collectedHeadArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    int[] collectedTorsoArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    string[] nameHeadArray = new string[] { "Default", "Hat", "Zombie", "Santa", "Plague Doctor", "Trump", "Pirate", "Ninja", "Astronaut", "Dog"};
    string[] nameTorsoArray = new string[] { "Default", "Bee", "Robot", "Dog", "Suit", "Caveman", "Tutu", "Trampo Man" };

    public AudioClip[] audioClips;

    private void Start()
    {
        Advertisement.Initialize("3372701", true);

        gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        //PlayerPrefs.SetInt("first", 0);

        if (PlayerPrefs.GetInt("first", 0) == 0)
        {
            tutorialButtons.SetActive(true);
            PlayerPrefs.SetInt("first", 1);
            PlayerPrefs.SetInt("highscore", 0);
            PlayerPrefs.SetInt("highestheight", -5);
            PlayerPrefs.SetInt("coins", 0);
            PlayerPrefs.SetInt("selectedHead", 0);
            PlayerPrefs.SetInt("selectedTorso", 0);
            PlayerPrefs.SetInt("headMax", 0);
            PlayerPrefs.SetInt("torsoMax", 0);
            PlayerPrefs.SetInt("tries", 0);
            PlayerPrefs.SetInt("astronaut", 0);
            PlayerPrefs.SetInt("ranHead", 1);
            PlayerPrefs.SetInt("ranTorso", 1);
            PlayerPrefs.SetInt("rated", 0);
            PlayerPrefs.SetInt("transition", 0);
            PlayerPrefs.SetInt("twitterPressed", 0);
            PlayerPrefs.SetInt("faceBookPressed", 0);
            PlayerPrefs.SetInt("freeLootBox", 0);

            PlayerPrefs.SetInt("head" + 0, 1);
            PlayerPrefs.SetInt("torso" + 0, 1);

            for (int i = 1; i < headMax; i++)
            {
                PlayerPrefs.SetInt("head" + i, 0);
            }
            for (int i = 1; i < torsoMax; i++)
            {
                PlayerPrefs.SetInt("torso" + i, 0);
            }
        }
        else
        {
            tutorialButtons.SetActive(false);
        }

        int rand;

        if (PlayerPrefs.GetInt("ranHead") == 1) //if random for head
        {
            do
            {
                rand = Random.Range(0, headMax);
            } while (PlayerPrefs.GetInt("head" + rand) == 0);

            PlayerPrefs.SetInt("selectedHead", rand);
        }
        if (PlayerPrefs.GetInt("ranTorso") == 1) //if random for torso
        {
            do
            {
                rand = Random.Range(0, torsoMax);
            } while (PlayerPrefs.GetInt("torso" + rand) == 0);

            PlayerPrefs.SetInt("selectedTorso", rand);
        }

        for (int i = 0; i < headMax; i++)
        {
            collectedHeadArray[i] = PlayerPrefs.GetInt("head" + i);
        }
        for (int i = 0; i < torsoMax; i++)
        {
            collectedTorsoArray[i] = PlayerPrefs.GetInt("torso" + i);
        }

        menu.SetActive(true);
        settingsWindow.SetActive(false);
        deathMenu.SetActive(false);
        soundOn = false;
        frozen = true;

        audioSource = gameObject.GetComponent<AudioSource>();

        heads = Resources.LoadAll<Sprite>("Heads");
        legs = Resources.LoadAll<Sprite>("Legs");
        arms = Resources.LoadAll<Sprite>("Arms");
        legsbent = Resources.LoadAll<Sprite>("LegsBent");
        armsbent = Resources.LoadAll<Sprite>("ArmsBent");
        torso = Resources.LoadAll<Sprite>("Torsos");
        bodies = Resources.LoadAll<Sprite>("Bodies");

        torsoInt = PlayerPrefs.GetInt("selectedTorso");

        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = legs[torsoInt];
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = legs[torsoInt];
        gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = heads[PlayerPrefs.GetInt("selectedHead")];
        gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = arms[torsoInt];
        gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = arms[torsoInt];
        gameObject.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = torso[torsoInt];

        scoreText.text = "" + score;
        flipText.text = "";
        coinText.text = "" + PlayerPrefs.GetInt("coins");
        highscoreInGameText.text = "HighScore: " + PlayerPrefs.GetInt("highscore", 0);
        inGameCoinText.gameObject.SetActive(false);

        for (int i = 0; i < 30; i++)
        {
            float height = (i * 4) + 2.2f;
            Instantiate(coinCollectable, new Vector3(Random.Range(-72.9f, -67.1f), Random.Range(height, height + 3.2f), 0), Quaternion.identity);
        }

        UpdatePrizeButton();

        if (PlayerPrefs.GetInt("freeLootBox") == 0)
        {
            purchaseButton.transform.GetChild(0).GetComponent<Text>().text = "FREE";
        }

        PlayerPrefs.SetInt("tries", PlayerPrefs.GetInt("tries") + 1);

        if (PlayerPrefs.GetInt("transition") == 1)
        {
            StartCoroutine(transitionBackwardsWait());
            PlayerPrefs.SetInt("transition", 0);
        }
    }

    void FixedUpdate()
    {
        if (!frozen)
        {
            boundary.transform.position = new Vector3(-70, transform.position.y, 0);

            if((int)transform.position.y > height)
            {
                height = (int)transform.position.y;
            }

            //out of range of camera
            if (transform.position.y > 3.3f)
            {
                playerCamera.transform.position = new Vector3(-70, transform.position.y - 2.3f, -10);
            }
            if (transform.position.x > -66.8f)
            {
                OutOfBounds();
                playerCamera.transform.position = new Vector3(-66, playerCamera.transform.position.y, -10);
            }
            if (transform.position.x < -73.2f)
            {
                OutOfBounds();
                playerCamera.transform.position = new Vector3(-74, playerCamera.transform.position.y, -10);
            }

            //rotation leveling
            transform.Rotate(0, 0, rotSpeed * 0.1f);

            if (!dead)
            {
                //print("rotSpeed " + rotSpeed);

                //flip detection
                if (transform.localEulerAngles.z < 15 || transform.localEulerAngles.z > 345)
                {
                    if (flip)
                    {
                        flipAmount++;
                        gainedScore += 5;

                        if (transform.localEulerAngles.z < 15)
                        {
                            flipText.text = "Front Flip\n+" + gainedScore;
                        }
                        else if (transform.localEulerAngles.z > 345)
                        {
                            flipText.text = "Back Flip\n+" + gainedScore;
                        }
                        PlayAudio(5, 0.3f);

                        flip = false;
                    }
                }
                if (transform.localEulerAngles.z > 165 && transform.localEulerAngles.z < 195)
                {
                    flip = true;
                }

                //player inputs
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.touchCount > 0 && Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x > -70)
                {
                    if (!onceR)
                    {
                        onceR = true;
                        onceL = false;
                        onceE = false;
                        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = legsbent[torsoInt];
                        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = legsbent[torsoInt];
                        gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = armsbent[torsoInt];
                        gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = armsbent[torsoInt];
                    }

                    transform.GetChild(0).transform.rotation = Quaternion.Slerp(transform.GetChild(0).transform.rotation, tester1.rotation, 0.1f);
                    transform.GetChild(1).transform.rotation = Quaternion.Slerp(transform.GetChild(1).transform.rotation, tester1.rotation, 0.1f);

                    rotSpeed -= rotForce * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.touchCount > 0 && Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x < -70)
                {
                    if (!onceL)
                    {
                        onceR = false;
                        onceL = true;
                        onceE = false;
                        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = legsbent[torsoInt];
                        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = legsbent[torsoInt];
                        gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = armsbent[torsoInt];
                        gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = armsbent[torsoInt];
                    }

                    transform.GetChild(3).transform.rotation = Quaternion.Slerp(transform.GetChild(3).transform.rotation, tester3.rotation, 0.1f);
                    transform.GetChild(4).transform.rotation = Quaternion.Slerp(transform.GetChild(4).transform.rotation, tester3.rotation, 0.1f);

                    rotSpeed += rotForce * Time.deltaTime;
                }
                else
                {
                    transform.GetChild(0).transform.rotation = Quaternion.Slerp(transform.GetChild(0).transform.rotation, tester2.rotation, 0.1f);
                    transform.GetChild(1).transform.rotation = Quaternion.Slerp(transform.GetChild(1).transform.rotation, tester2.rotation, 0.1f);
                    transform.GetChild(3).transform.rotation = Quaternion.Slerp(transform.GetChild(3).transform.rotation, tester4.rotation, 0.1f);
                    transform.GetChild(4).transform.rotation = Quaternion.Slerp(transform.GetChild(4).transform.rotation, tester4.rotation, 0.1f);

                    if (!onceE)
                    {
                        onceR = false;
                        onceL = false;
                        onceE = true;
                        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = legs[torsoInt];
                        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = legs[torsoInt];
                        gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = arms[torsoInt];
                        gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = arms[torsoInt];
                    }
                }

                if (rotSpeed > 0)
                {
                    rotSpeed -= rotIncrease * Time.deltaTime;
                }
                else if (rotSpeed < 0)
                {
                    rotSpeed += rotIncrease * Time.deltaTime;
                }

                rotSpeed = Mathf.Clamp(rotSpeed, -maxRotSpeed, maxRotSpeed);
            }
        }
        else
        {
            transform.position = new Vector3(-70, 1.6f, 0);
        }

        //restart
        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }

        //give free money
        if (Input.GetKey(KeyCode.M))
        {
            PlayerPrefs.SetInt("coins", 3000);
            coinText.text = "" + PlayerPrefs.GetInt("coins");
        }
    }

    public void Play()
    {
        do
        {
            ranDeg = Random.Range(-8, 8);
            transform.localEulerAngles = new Vector3(0, 0, ranDeg);
        }
        while (ranDeg == 0);

        StartCoroutine(HideTutorial());

        //gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        frozen = false;

        height = 0;
        heighestHeightBar.transform.position = new Vector3(-70, PlayerPrefs.GetInt("highestheight"), 0);
        heighestHeightBar.transform.GetChild(0).GetComponent<TextMesh>().text = "Highest height - " + PlayerPrefs.GetInt("highestheight") + "m";

        menu.SetActive(false);

        inGameCoinText.gameObject.SetActive(true);
        inGameCoinText.text = "" + PlayerPrefs.GetInt("coins");

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show("bannerPlacement");

        StartCoroutine(SpawnBird());
    }

    public void Restart()
    {
        PlayAudio(0, 0);
        SceneManager.LoadScene("GameScene");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!dead)
        {
            if (other.gameObject.CompareTag("Trampoline"))
            {
                PlayAudio(1, 0.3f);

                CameraShaker.Instance.ShakeOnce(4f, 0.4f, 0.2f, 0.5f);

                if (transform.localEulerAngles.z > 180)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 360 - ((360 - transform.localEulerAngles.z) / bounceDifficulty));
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z / bounceDifficulty);
                }

                gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                gameObject.transform.GetComponent<Rigidbody2D>().AddForce(transform.up * bounceForce);

                bounceDifficulty += bounceDiffIncrease * Mathf.Round((flipAmount * 0.4f) + 0.1f);
                bounceForce += bounceForceIncrease + (flipAmount * bounceForceIncreasePlus);
                //score += 1;
                score += gainedScore;
                scoreText.text = "" + score;

                gainedScore = 0;
                flipAmount = 0;
                flipText.GetComponent<Animator>().SetBool("Flipped", true);
                StartCoroutine(wait());
            }

            if(other.gameObject.CompareTag("Coin"))
            {
                PlayAudio(2, 0.3f);
                other.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Play");
                Destroy(other.GetComponent<CircleCollider2D>());

                PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 5);
                inGameCoinText.text = "" + PlayerPrefs.GetInt("coins");

                Destroy(other.gameObject, 1);
                //score += 2;
                //scoreText.text = "" + score;
            }

            if(other.gameObject.CompareTag("Bird"))
            {
                PlayAudio(6, 0);
                Destroy(other.gameObject);
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = gameObject.transform.GetComponent<Rigidbody2D>().velocity / 2;
                gameObject.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + Random.Range(-30, 30));

                gainedScore = 0;
                flipAmount = 0;
                flipText.text = "";
                bounceForce -= bounceForceIncrease * 2;
            }
        }
    }

    void UpdatePrizeButton()
    {
        if (PlayerPrefs.GetInt("coins") >= 500 || PlayerPrefs.GetInt("freeLootBox") == 0)
        {
            purchaseButton.GetComponent<Image>().sprite = prizeButton[0];
        }
        else
        {
            purchaseButton.GetComponent<Image>().sprite = prizeButton[1];
        }
    }

    private IEnumerator wait()
    {
        trampoline.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TrampolineBounce");
        yield return new WaitForSeconds(0.2f);
        trampoline.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Trampoline");
        yield return new WaitForSeconds(0.6f);
        flipText.GetComponent<Animator>().SetBool("Flipped", false);
        flipText.text = "";
    }

    public void CustomPressed()
    {
        PlayAudio(0, 0);
        StartCoroutine(transitionWait());
    }

    private IEnumerator transitionWait()
    {
        GameObject.Find("Transition").GetComponent<Animator>().SetBool("Trans", true);
        GameObject.Find("Transition2").GetComponent<Animator>().SetBool("Trans", true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SelectionScene");
    }

    private IEnumerator transitionBackwardsWait()
    {
        GameObject.Find("Transition").GetComponent<Animator>().SetBool("Exit", true);
        GameObject.Find("Transition2").GetComponent<Animator>().SetBool("Exit", true);
        yield return new WaitForSeconds(1f);
        GameObject.Find("Transition").GetComponent<Animator>().SetBool("Exit", false);
        GameObject.Find("Transition2").GetComponent<Animator>().SetBool("Exit", false);
    }

    public void PurchasePressed()
    {
        PlayAudio(0, 0);
        PlayAudio(3, 0);

        if (PlayerPrefs.GetInt("coins") >= 500 || PlayerPrefs.GetInt("freeLootBox") == 0)
        {
            if (PlayerPrefs.GetInt("freeLootBox") == 1)
            {
                PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - 500);
            }

            PlayerPrefs.SetInt("freeLootBox", 1);

            coinText.text = "" + PlayerPrefs.GetInt("coins");

            purchaseButton.transform.GetChild(0).GetComponent<Text>().text = "500";

            int ran = Random.Range(0, 2);

            if (PlayerPrefs.GetInt("headMax") < headMax - 1 && PlayerPrefs.GetInt("torsoMax") < torsoMax - 1 && ran == 0) //head pickeds
            {
                do
                {
                    ranPick = Random.Range(1, headMax);
                }
                while (PlayerPrefs.GetInt("head" + ranPick) == 1);

                PlayerPrefs.SetInt("headMax", PlayerPrefs.GetInt("headMax") + 1);
                PlayerPrefs.SetInt("head" + ranPick, 1);

                prizeSprite.SetActive(true);
                prizeSprite.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
                prizeSprite.transform.GetChild(2).GetComponent<Image>().sprite = heads[ranPick];
                prizeSprite.transform.GetChild(3).GetComponent<Text>().text = nameHeadArray[ranPick] + " Head";
            }
            else if(PlayerPrefs.GetInt("torsoMax") < torsoMax - 1) //torso picked
            {
                do
                {
                    ranPick = Random.Range(1, torsoMax);
                }
                while (PlayerPrefs.GetInt("torso" + ranPick) == 1);

                PlayerPrefs.SetInt("torsoMax", PlayerPrefs.GetInt("torsoMax") + 1);
                PlayerPrefs.SetInt("torso" + ranPick, 1);

                prizeSprite.SetActive(true);
                prizeSprite.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(100, 180);
                prizeSprite.transform.GetChild(2).GetComponent<Image>().sprite = bodies[ranPick];
                prizeSprite.transform.GetChild(3).GetComponent<Text>().text = nameTorsoArray[ranPick] + " Body";
            }
            else
            {
                print("none left");
            }

            UpdatePrizeButton();
        }
        else
        {
            //prompt not enough money
        }
    }

    //gameover
    public void OutOfBounds()
    {
        if (!dead)
        {
            if (Random.Range(0, 3) == 0)
            {
                ShowAdVideo(0);
            }

            watchAd.SetActive(true);
            timer = 5;
            StartCoroutine(WatchTimer());
            newHighScoreText.SetActive(false);

            rotSpeed = 0;

            Advertisement.Banner.Hide();

            if (score > PlayerPrefs.GetInt("highscore"))
            {
                PlayerPrefs.SetInt("highscore", score);

                newHighScoreText.SetActive(true);
                newHighScoreText.GetComponent<Text>().text = "New HighScore! " + score;

                int ran = Random.Range(0, 8);

                if (ran == 0 && PlayerPrefs.GetInt("rated") == 0)
                {
                    ratePrompt.SetActive(true);
                    watchAd.SetActive(false);
                }
                else if(ran == 1 || ran == 2 || ran == 3)
                {
                    ratePrompt.SetActive(false);
                    watchAd.SetActive(true);
                }
                else
                {
                    ratePrompt.SetActive(false);
                    watchAd.SetActive(false);
                }
            }

            if(Random.Range(0, 20) == 0) //show rate game
            {
                ratePrompt.SetActive(true);
                watchAd.SetActive(false);
            }

            UpdatePrizeButton();

            if (height >= PlayerPrefs.GetInt("highestheight"))
            {
                PlayerPrefs.SetInt("highestheight", height);
            }

            /*if(score > 25 && PlayerPrefs.GetInt("astronaut") == 0)
            {
                PlayerPrefs.SetInt("astronaut", 1);

                PlayerPrefs.SetInt("headMax", PlayerPrefs.GetInt("headMax") + 1);
                PlayerPrefs.SetInt("head" + 8, 1);

                prizeSprite.SetActive(true);
                prizeSprite.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
                prizeSprite.transform.GetChild(2).GetComponent<Image>().sprite = heads[8];
                prizeSprite.transform.GetChild(3).GetComponent<Text>().text = nameHeadArray[8] + " Head";
            }*/

            PlayAudio(4, 0);
            dead = true;
            deathMenu.SetActive(true);

            //PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + score);
            PlayerPrefs.Save();

            coinText.text = "" + PlayerPrefs.GetInt("coins");

            flipText.text = "";

            highscoreText.text = "HighScore: " + PlayerPrefs.GetInt("highscore");
        }
    }

    public void SoundPressed()
    {
        PlayAudio(0, 0);
        soundOn = !soundOn;

        if (soundOn)
        {
            AudioListener.volume = 0;
            soundButton.transform.GetComponent<Image>().sprite = musicSprite[0];
        }
        else
        {
            AudioListener.volume = 1;
            soundButton.transform.GetComponent<Image>().sprite = musicSprite[1];
        }
    }

    public void AdClicked(int _adType)
    {
        PlayAudio(0, 0);

        ShowAdReward(_adType);
    }

    public void ShowAdReward(int _adType)
    {
        adType = _adType;

        //play ad

        const string placementId = "rewardedVideo";

        if (Advertisement.IsReady())
        {
            print("is ready!");
            Advertisement.Show(placementId, new ShowOptions() { resultCallback = adViewResult });
        }
        else
        {
            print("is not ready!");
        }
    }

    public void ShowAdVideo(int _adType)
    {
        adType = _adType;

        //play ad

        const string placementId = "video";

        if (Advertisement.IsReady())
        {
            Advertisement.Show(placementId, new ShowOptions() { resultCallback = adViewResult });
        }
    }

    private void adViewResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                if(adType == 1)
                {
                    PlayerPrefs.SetInt("coins", 50);
                }
                else if(adType == 2)
                {
                    //continue game
                    frozen = false;
                    gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                    gameObject.transform.position = new Vector3(-70, 2.0f, 0);

                    GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                    GetComponent<Rigidbody2D>().angularVelocity = 0;

                    dead = false;
                    deathMenu.SetActive(false);
                    playerCamera.transform.position = new Vector3(-70, 1, -10);
                    rotSpeed = 0;
                }
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");

                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");

                break;
        }
    }

    private IEnumerator WatchTimer()
    {
        yield return new WaitForSeconds(1);
        timer--;
        watchAd.transform.GetChild(1).GetComponent<Text>().text = "" + timer;

        if(timer <= 0)
        {
            watchAd.SetActive(false);
        }
        else
        {
            StartCoroutine(WatchTimer());
        }
    }

    public void ExitButton()
    {
        PlayAudio(0, 0);
        prizeSprite.SetActive(false);
        ratePrompt.SetActive(false);
    }

    public void RateUsButton()
    {
        PlayAudio(0, 0);
        //open app store for rating

        //#if UNITY_ANDROID
        //Application.OpenURL("market://details?id=");

        if (PlayerPrefs.GetInt("rated") == 0)
        {
            PlayerPrefs.SetInt("rated", 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 100);
        }

        ratePrompt.SetActive(false);
    }

    public void SettingsPressed()
    {
        PlayAudio(0, 0);
        settingsWindow.SetActive(!settingsWindow.activeSelf);
    }

    public void TwitterButtonPressed()
    {
        PlayAudio(0, 0);

        if (PlayerPrefs.GetInt("twitterPressed") == 0)
        {
            PlayerPrefs.SetInt("twitterPressed", 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 100);
        }

        Application.OpenURL("https://twitter.com/AppleSeedGames");
    }

    public void FacebookButtonPressed()
    {
        PlayAudio(0, 0);

        if (PlayerPrefs.GetInt("faceBookPressed") == 0)
        {
            PlayerPrefs.SetInt("faceBookPressed", 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 100);
        }

        //do the facebook jazz here
        faceBookManager.Share();
    }

    void PlayAudio(int _i, float _pitchIntensity)
    {
        audioSource.clip = audioClips[_i];
        audioSource.pitch = Random.Range(1 - _pitchIntensity, 1 + _pitchIntensity);
        audioSource.Play();
    }

    private IEnumerator SpawnBird()
    {
        yield return new WaitForSeconds(Random.Range(birdSpawnWaitMin, birdSpawnWaitMax));
        Instantiate(Resources.Load("Bird"), new Vector3(0, 0, 0), Quaternion.identity);
        StartCoroutine(SpawnBird());
    }

    private IEnumerator HideTutorial()
    {
        yield return new WaitForSeconds(2);
        tutorialButtons.SetActive(false);
    }

    public void MoneyCheat()
    {
        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 250);
        coinText.text = "" + PlayerPrefs.GetInt("coins");
        UpdatePrizeButton();
    }
}