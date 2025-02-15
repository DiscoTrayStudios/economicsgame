using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Countdown countdown;
    // Various Variables
    public GameObject canvas;
    //private GameObject resetCanvas;
    public GameObject events;
    public AudioSource BackgroundAmbience;
    public TextMeshProUGUI title;
    public GameObject startButton;
    public TextMeshProUGUI prompt;
    public GameObject backgroundImage;
    public TextMeshProUGUI description;
    public GameObject submitButton;
    public GameObject leaderboard;
    public GameObject nextButton;
    public GameObject backButton;
    public GameObject feedbackBoard;
    // Credits Stuff
    public GameObject creditsText;
    public GameObject creditsButton;
    // Tutorial stuff
    public GameObject tutorial;
    public GameObject howToPlayButton;
    public GameObject tutorialNextButton;
    public GameObject tutorialTextBox;
    public TextMeshProUGUI tutorialText;
    public GameObject tutorialBackground;
    private int tutorialCount;
    public GameObject tutorialImage1;
    public GameObject tutorialImage2;
    public GameObject tutorialImage3;
    public GameObject tutorialImage4;
    // Game Designer controlled "upgrade" values
    public double pollutionUpgrade1;
    public double pollutionUpgrade2;
    public double cityUpgrade1;
    public double cityUpgrade2;
    public double cityUpgrade3;
    public double eMulti;
    // Country Related | Camera Panning
    public GameObject nameEntry;
    private List<Country> playerList = new List<Country>();
    List<GameObject> CountryList = new List<GameObject>();
    private GameObject mainCamera;
    public bool botMode = false;
    string[] names = { "", "", "" };
    private Phase currentPhase = Phase.Menu;
    public GameObject nextCountryButton;
    public GameObject lastCountryButton;
    public GameObject currentCountry;
    // Treaty | Votes | Rounds | End
    private double treatyCost = 1000f;
    private double emissionsChangePct = -0.20;
    private VoteManager currentVote = new VoteManager();
    public GameObject yesButton;
    public GameObject noButton;
    private int currentPIndex = 0;
    private int completedVotes = 0;
    public int maxTurns = 5;
    public int punishmentCost;
    private Evaluator eval;
    private List<List<Accolades>> evaluation;
    List<Country> declineList = new List<Country>();
    List<Country> acceptList = new List<Country>();
    private bool havePunished = false;
    public TextMeshProUGUI finalGDP;
    public TextMeshProUGUI finalCO2;
    public TextMeshProUGUI finalScore;
    public GameObject roundCount;
    // Backgrounds
    public GameObject des1;
    public GameObject des2;

    public GameObject Settings;
    // Volume
    public AudioMixer mixer;
    public GameObject volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        //resetCanvas = canvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (currentPhase == Phase.Names) { submit(); }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(events);
            DontDestroyOnLoad(BackgroundAmbience);
            DontDestroyOnLoad(countdown);

        }
        else
        {
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
        }
    }
    public void start()
    {
        //StartCoroutine(HideTextAfterSeconds(1, title));
        nameEntry.GetComponent<TMP_InputField>().text = "";
        playerList = new List<Country>();
        StartCoroutine(RemoveAfterSeconds(1, startButton));
        prompt.text = "Player 1\nEnter your country's name!";
        nameEntry.SetActive(true);
        submitButton.SetActive(true);
        backButton.SetActive(true);
        currentPhase = Phase.Names;
        roundCount.GetComponentInChildren<TextMeshProUGUI>().text = "ROUND 1 OF " + maxTurns;
        
    }

    // Tutorial
    public void TutorialStart()
    {
        tutorialTextBox.SetActive(true);
        tutorialText.text = "In this game, you represent your country at international climate accords. \n \n" +
            "There are two main numbers you need to consider in this game: GDP and Emissions.";
        tutorialNextButton.SetActive(true);
        tutorialCount = 0;
        tutorialBackground.SetActive(true);
        leaderboard.GetComponent<Animator>().Play("show_P1");
        //backButton.SetActive(true);
        backButton.GetComponent<Animator>().Play("back_show");
        tutorialImage1.SetActive(true);
    }

    public void TutorialNext()
    {
        tutorialCount++;

        if (tutorialCount == 1)
        {
            tutorialText.text = "Your GDP is a measure of the total economic productivity in your country in USD.\n \n" +
                "As your GDP grows, your country will expand";
            //point to GDP on panel
            tutorialImage1.SetActive(false);
            tutorialImage2.SetActive(true);
        }
        else if (tutorialCount == 2)
        {
            tutorialText.text = "CO<sub><size=150%>2</size></sub> tracks how much your country's emissions in megatons of carbon. \n \n" +
                "Each turn, you will be presented with the opportunity to join a climate treaty.\n" +
                "These treaties have some up-front costs, but a healthy environment will help your growth in the long run.\n\n" +
                "In dire situations, you may get ahead by skimping out on your treaties. But beware the damage that this can do.";
            //point to emissions on panel
            tutorialImage2.SetActive(false);
            tutorialImage3.SetActive(true);
        }
        else if (tutorialCount == 3)
        {
            leaderboard.GetComponent<Animator>().Play("hide_P1");
            tutorialText.text = "Consider the treaty each turn carefully to balance your GDP growth and environmental impact \n \n " +
                "Remember, your decisions will have an impact on others. \n \n" +
                "Later in the game, you may find that some players are ignoring their impact on the environment, so you can choose to impose tariffs on the offending country.";
            tutorialImage3.SetActive(false);
            tutorialImage4.SetActive(true);
            backButton.SetActive(true);
            tutorial.GetComponent<Animator>().Play("next_hide");
            StartCoroutine(RemoveAfterSeconds(1, tutorialNextButton));
            //display treaty screen and start button
        }
    }

    // Credits
    public void credits()
    {
        clearMenuUI();
        backButton.SetActive(true);
        creditsText.SetActive(true);

    }

    // Settings
    public void settings()
	{
        clearMenuUI();
        backButton.SetActive(true);
        Settings.SetActive(true);

	}
    // Go back to Main Menu
    public void Back()
    {
        //this can probably go through the 'clear menu UI function'
        StartCoroutine(RemoveAfterSeconds(1, backButton));

        //game start off
        prompt.GetComponent<Animator>().Play("hide_prompt");
        submitButton.SetActive(false);
        nameEntry.SetActive(false);

        //tutorial off
        //tutorialBackground.SetActive(false);
        //tutorialNextButton.SetActive(false);
        //tutorialText.text = "";
        StartCoroutine(RemoveAfterSeconds(1, tutorialBackground));
        StartCoroutine(HideTextAfterSeconds(1, tutorialText));
        StartCoroutine(RemoveAfterSeconds(1, tutorialTextBox));
        tutorialImage1.SetActive(false);
        tutorialImage2.SetActive(false);
        tutorialImage3.SetActive(false);
        tutorialImage4.SetActive(false);
        //tutorialTextBox.SetActive(false);

        //credits off
        StartCoroutine(RemoveAfterSeconds(1, creditsText));
        StartCoroutine(RemoveAfterSeconds(1, Settings));
        //creditsText.SetActive(false);
        
        //reset main menu
        startButton.SetActive(true);
        title.text = "Climate Goes Political";
    }

    public void submit()
    {
        int current = playerList.Count + 1;
        if (current == 1)
        {
            string[] names = { "", "", "" };
        }
        string name = nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        if (names[0] == name || names[1] == name || names[2] == name)
        {
            prompt.text = "Player " + current + "\nEnter your country's name!\nPlease enter an available name!";
        }
        else if (name.Length > 10)
        {
            prompt.text = "Player " + current + "\nEnter your country's name!\nPlease enter a name fewer than 10 characters!";
        }
        else if (name.Length > 1)
        {
            nameEntry.GetComponent<TMP_InputField>().text = "";
            Country newPlayer = new Country(name);
            playerList.Add(newPlayer);
            if (playerList.Count == 4 || botMode)
            {
                treatyCost = Random.Range(1000, 1500);
                clearMenuUI();
                initializeNames();
                updateLeaderboard();
                startCitiesPhase();
            }
            else
            {
                names[current - 1] = name;
                current = playerList.Count + 1;
                prompt.text = "Player " + current + "\nEnter your country's name!";
                nameEntry.GetComponent<TMP_InputField>().Select();
                nameEntry.GetComponent<TMP_InputField>().ActivateInputField();
            }
        }


    }

    private void clearMenuUI()
    {
        nameEntry.SetActive(false);
        submitButton.SetActive(false);
        backButton.SetActive(false);
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    private void startCitiesPhase()
    {
        if (currentPhase == Phase.Names)
        {
            StartCoroutine(LoadYourAsyncScene(true, "Countries"));
            playerList.ForEach(p => p.adjustScore(eMulti));
        }
        else
        {
            //mainCamera.transform.position = new Vector3(-9.7f, 4.9f, -1f);
            //mainCamera.GetComponent<CameraPanning>().CurrentCity = 0;
            TextMeshProUGUI[] nameList = leaderboard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < playerList.Count; i++)
            {
                StartCoroutine(TextLerp(playerList[i].HaveAgreed, nameList[i]));
            }
            currentPIndex = 0;
        }
        nextButton.SetActive(true);
        nextCountryButton.SetActive(true);
        lastCountryButton.SetActive(true);
        currentPhase = Phase.Cities;
        nextButton.GetComponent<Animator>().Play("show_next");
        leaderboard.GetComponent<Animator>().Play("show_leader");
        if(completedVotes > 0)
		{
            feedbackBoard.GetComponent<Animator>().Play("feedback_show");
		}
        if (completedVotes < maxTurns)
        {
            roundCount.GetComponentInChildren<TextMeshProUGUI>().text = "ROUND " + (completedVotes + 1) + " OF " + maxTurns;
            roundCount.GetComponent<Animator>().Play("show_round");
        }
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name + "\nScore: " + playerList[currentPIndex].Score;
        currentCountry.GetComponent<Animator>().Play("show_current");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Connects the Cities Phase objects to the GameManager
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CountryList.Add(GameObject.Find("Country1"));
        CountryList.Add(GameObject.Find("Country2"));
        CountryList.Add(GameObject.Find("Country3"));
        CountryList.Add(GameObject.Find("Country4"));
        mainCamera = GameObject.Find("Main Camera");
        Debug.Log("Main Camera Set");
    }

    private void initializeNames()
    {
        if (botMode)
        {
            for (int i = 1; i < 4; i++)
            {
                string name = "Player " + (i + 1);
                Country newPlayer = new Country(name);
                playerList.Add(newPlayer);
            }
        }

        TextMeshProUGUI[] nameList = leaderboard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            nameList[i].text = playerList[i].Name;
        }
    }

    // Voting Related
    private void setupVoteUI()
    {
        yesButton.SetActive(true);
        noButton.SetActive(true);
        StartCoroutine(ColorLerp(new Color(0.61f, 0.83f, 0.89f, 1), 0.75f));
        yesButton.GetComponent<Animator>().Play("show_agree");
        noButton.GetComponent<Animator>().Play("show_decline");
        nextButton.GetComponent<Animator>().Play("hide_next");
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        currentCountry.GetComponent<Animator>().Play("hide_current");
        roundCount.GetComponent<Animator>().Play("hide_round");
        countdown.Begin();
        if (completedVotes > 0)
        {
            feedbackBoard.GetComponent<Animator>().Play("feedback_hide");
        }
        //StartCoroutine(RemoveAfterSeconds(2, nextButton));
        nextCountryButton.SetActive(false);
        lastCountryButton.SetActive(false);

    }

    public void Next()
    {
        if (currentPhase == Phase.Cities)
        {
            if (completedVotes == maxTurns) startResultsPhase();
            else if
            (completedVotes > 1
            && !havePunished
            && acceptList.Count > 0
            && declineList.Count > 0
            && completedVotes < maxTurns - 1)
            {
                startPunishmentPhase();
            }
            else
            {
                startVotePhase();
            }
        }
        else if (currentPhase == Phase.Results) NextResult();
    }

    public void startVotePhase()
    {
        currentPhase = Phase.Votes;
        prompt.text = GetPromptText();
        setupVoteUI();
        currentPIndex = 0;
        havePunished = false;
        leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        currentVote = new VoteManager();
        currentVote.clearVotes();
        acceptList.Clear();
        declineList.Clear();
    }

    private string GetPromptText()
    {
        string toReturn = "";
        toReturn += 
            $"Treaty: Spend ${treatyCost} to reduce " +
            $"your emissions by 5% this year?\n\n" +
            $"Accept: GDP - {treatyCost}\n    " +
            $"        CO<sub><size=150%>2</size></sub> - 5%\n" +
            "Decline: CO<sub><size=150%>2</size></sub> + 10%";
        return toReturn;
    }

    private void BotVote()
    {
        for (int i = 0; i < 3; i++)
        {
            int choice = Random.Range(0, 2);
            if (choice == 0) agree();
            else decline();
        }
    }

    public void agree()
    {
        des1.GetComponent<Animator>().Play("agree");
        if (currentPhase == Phase.Punishment)
        {
            if (playerList[currentPIndex].GDP > declineList.Count * 1000)
            {
                enactPunishments();
                currentPIndex += 1;
                if (currentPIndex == acceptList.Count)
                {
                    countdown.reset();
                    AdjustCountries();
                    updateLeaderboard();
                    clearVoteUI();
                    mapUpdate();
                    havePunished = true;
                    startCitiesPhase();
                }
                else
                {
                    countdown.Begin();
                    prompt.text = "<b>" + acceptList[currentPIndex].Name + "</b>"
                    + " would you like to impose tariffs on everyone who declined?\n\n\nCost = $"
                    + (declineList.Count * 1000)
                    + " \nEffect: Each decliner loses 0.05% growth rate";
                }
            }
        }
        else {
            if (playerList[currentPIndex].GDP > treatyCost)
            {
                currentVote.AcceptVotes += 1;
                acceptList.Add(playerList[currentPIndex]);
                playerList[currentPIndex].Agree();
                currentPIndex = currentVote.sumVotes();
                if (currentVote.sumVotes() < 4)
                {
                    leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
                    countdown.Begin();
                }
                if (botMode && currentPIndex == 1) BotVote();
                if (currentVote.sumVotes() == 4) enactVotes();
            }
        }
    }

    public void decline()
    {
        des2.GetComponent<Animator>().Play("deny");
        if (currentPhase == Phase.Punishment)
        {
            currentPIndex += 1;
            if (currentPIndex == acceptList.Count)
            {
                countdown.reset();
                AdjustCountries();
                updateLeaderboard();
                clearVoteUI();
                mapUpdate();
                havePunished = true;
                startCitiesPhase();
            }
            else
            {
                countdown.Begin();
                prompt.text = "<b>" + acceptList[currentPIndex].Name + "</b>"
                + " would you like to impose tariffs on everyone who declined?\n\n\nCost = $"
                + (declineList.Count * 1000)
                + " \nEffect: Each decliner loses 0.05% growth rate";
            }
        }
        else {
            currentVote.DeclineVotes += 1;
            declineList.Add(playerList[currentPIndex]);
            playerList[currentPIndex].Decline();
            currentPIndex = currentVote.sumVotes();
            if (currentVote.sumVotes() < 4)
            {
                leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
                countdown.Begin();
            }
            if (botMode && currentPIndex == 1) BotVote();
            if (currentVote.sumVotes() == 4) enactVotes();
        }
    }

    private void enactPunishments()
    {
        acceptList[currentPIndex].GDP -= declineList.Count * 1000;

        declineList.ForEach(p => {
            p.GrowthMod -= 0.005;
        });
    }

    private void startResultsPhase()
    {
        feedbackBoard.GetComponent<Animator>().Play("feedback_hide");
        mainCamera.GetComponent<CameraPanning>().CurrentCity = 3;
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
        currentCountry.GetComponent<Animator>().Play("hide_current");
        nextCountryButton.SetActive(false);
        lastCountryButton.SetActive(false);
        currentPhase = Phase.Results;
        currentPIndex = 0;
        eval = new Evaluator(playerList);
        evaluation = eval.evaluate();
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0.5f), 0.75f));
        prompt.text = playerList[currentPIndex].Name + "'s Achievements:";
        finalGDP.text = "GDP: " + playerList[currentPIndex].GDP.ToString("C2");
        finalCO2.text = "CO<sub><size=150%>2</size></sub>: " + playerList[currentPIndex].Emissions.ToString("F2") + "MT";
        finalScore.text = "Score: " + playerList[currentPIndex].Score.ToString();
        description.text = PrintAccolades(0);
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        roundCount.GetComponent<Animator>().Play("hide_round");
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
    }

    public void enactVotes()
    {
        countdown.reset();
        completedVotes++;
        AdjustCountries();
        updateLeaderboard();
        clearVoteUI();
        mapUpdate();
        startCitiesPhase();
    }

    private void mapUpdate()
    {
        TextMeshProUGUI[] feedbackList = feedbackBoard.transform.Find("Feedbacks").GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < 4; i++)
        {
            int prevL = playerList[i].previousE;
            int currL = playerList[i].environment;
            int prevC = playerList[i].previousC;
            int currC = playerList[i].city;

            //Environment Related
            if (currL > prevL)
            {
                //Debug.Log("Oh no! " + playerList[i].Name + " became more polluted!");
                if (currL == 2)
                {
                    feedbackList[i].text = "Environment is being harmed";
                } else if (currL == 3)
                {
                    feedbackList[i].text = "Environment is being ravaged";
                }
            } 
            else if (currL < prevL)
            {
                //Debug.Log("Yay! " + playerList[i].Name + " became less polluted!");
                if (currL == 1)
                {
                    feedbackList[i].text = "Restored the environment";
                } else if (currL == 2)
                {
                    feedbackList[i].text = "Helped the environment somwhat";
                }
            } else
            {
                feedbackList[i].text = "Pollution remains roughly the same";
            }

            CountryList[i].transform.Find("Land " + prevL).gameObject.SetActive(false);
            CountryList[i].transform.Find("Land " + currL).gameObject.SetActive(true);

            //City Related
            if (currC > prevC)
            {
                //Debug.Log("Yay! " + playerList[i].Name + " grew!");
                
                if (currC == 2)
                {
                    feedbackList[i].text += "\nThis country is getting richer";
                } else if (currC == 3)
                {
                    feedbackList[i].text += "\nThis country is prosperous";
                } else if (currC == 4)
                {
                    feedbackList[i].text += "\nThe economy is one of the biggest!";
                }
            }
            else if (currC < prevC)
            {
                //Debug.Log("Oh No! " + playerList[i].Name + " shrunk!");
                if (currC == 1)
                {
                    feedbackList[i].text += "\nThis country is destitute.";
                } else if (currC == 2)
                {
                    feedbackList[i].text += "\nThis country is getting poorer...";
                } else if (currC == 3)
                {
                    feedbackList[i].text += "\nThe economy has seen better days...";
                }
            }else
            {
                feedbackList[i].text += "\nThe economy is about the same size";
            }
            CountryList[i].transform.Find("City " + prevC).gameObject.SetActive(false);
            CountryList[i].transform.Find("City " + currC).gameObject.SetActive(true);

            renderSmokes(i);
        }
    }

    private void renderSmokes(int index)
    {
        int envLevel = playerList[index].environment;
        if (envLevel == 1)
        {
            CountryList[index].transform.Find("Smoke1").gameObject.SetActive(false);
            CountryList[index].transform.Find("Smoke2").gameObject.SetActive(false);
            CountryList[index].transform.Find("Smoke3").gameObject.SetActive(false);
            CountryList[index].transform.Find("Smog").gameObject.SetActive(false);
        }
        else if (envLevel == 2)
        {
            CountryList[index].transform.Find("Smoke1").gameObject.SetActive(true);
            CountryList[index].transform.Find("Smoke2").gameObject.SetActive(true);
            CountryList[index].transform.Find("Smoke3").gameObject.SetActive(false);
            CountryList[index].transform.Find("Smog").gameObject.SetActive(false);

        }
        else
        {
            CountryList[index].transform.Find("Smoke1").gameObject.SetActive(true);
            CountryList[index].transform.Find("Smoke2").gameObject.SetActive(true);
            CountryList[index].transform.Find("Smoke3").gameObject.SetActive(true);
            CountryList[index].transform.Find("Smog").gameObject.SetActive(true);
        }
    }

    private void updateLeaderboard()
    {
        //leaderboard
        TextMeshProUGUI[] gdpList = leaderboard.transform.Find("GDPs").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] emmList = leaderboard.transform.Find("Emissions").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] groList = leaderboard.transform.Find("Growths").GetComponentsInChildren<TextMeshProUGUI>();

        //feedback board
        TextMeshProUGUI[] dgdpList = feedbackBoard.transform.Find("Delta GDPs").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] demmList = feedbackBoard.transform.Find("Delta Emissions").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] scoreList = feedbackBoard.transform.Find("Scores").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] namesList = feedbackBoard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();


        for (int i = 0; i < 4; i++)
        {
            //leaderboard
            string new_gdp = playerList[i].GDP.ToString("C2");
            string new_emm = playerList[i].Emissions.ToString("F2");
            string new_gro = playerList[i].Growth.ToString("P01");
            gdpList[i].text = "GDP: " + new_gdp;
            emmList[i].text = "CO<sub><size=150%>2</size></sub>: " + new_emm + "MT";
            groList[i].text = "GROWTH: " + new_gro;

            //feedback board
            string new_score = playerList[i].Score.ToString();
            string new_demm = playerList[i].DeltaEmissions.ToString("F2");
            string new_name = playerList[i].Name;
            scoreList[i].text = new_score;
            demmList[i].text = "Emission Change: " + new_demm + "MT";
            namesList[i].text = new_name;

            //trouble displaying if delta gdp is negative
            string new_dgdp = "";
            if (playerList[i].DeltaGDP < 0)
            {
                new_dgdp = "-$" + ((-1) * playerList[i].DeltaGDP).ToString("F2");
            }
            else
            {
                new_dgdp = playerList[i].DeltaGDP.ToString("C2");
            }
            dgdpList[i].text = "GDP Change: " + new_dgdp;
        }
    }


    private void AdjustCountries()
    {
        int numAgreed = 0;
        double totalEmissions = 0f;
        playerList.ForEach(p => 
        { 
            if (currentPhase == Phase.Votes)
            {
                if (p.HaveAgreed)
                {
                    numAgreed++;
                    p.LastGDP = p.GDP;
                    p.GDP -= treatyCost;
                    p.LastEmissions = p.Emissions;
                    p.Emissions -= p.Emissions * (1.0f / 20.0f);
                }
                else
                {
                    p.LastGDP = p.GDP;
                    p.LastEmissions = p.Emissions;
                    p.Emissions *= (11.0f / 10.0f);
                }
            }
            totalEmissions += p.Emissions;
        });
        double growthIncrease = 9 - (totalEmissions * 0.002f);
        playerList.ForEach(p => p.ActivateGDPGrowth());
        playerList.ForEach(p => p.UpdateDelta());
        //playerList.ForEach(p => p.Growth += (growthIncrease / 100f));
        playerList.ForEach(p => p.Growth = (growthIncrease / 100f) + p.GrowthMod);
        playerList.ForEach(player =>
        {
            player.adjustCity(cityUpgrade1, cityUpgrade2, cityUpgrade3);
            player.adjustEnvironment(pollutionUpgrade1, pollutionUpgrade2);
            player.adjustScore(eMulti);
            //Debug.Log(player.Name + "'s score is: " + (int)player.Score);
        } );
    }

    private void clearVoteUI()
    {
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 0.75f));
        leaderboard.GetComponent<Animator>().Play("hide_P4");
        yesButton.GetComponent<Animator>().Play("hide_agree");
        noButton.GetComponent<Animator>().Play("hide_decline");
        description.GetComponent<Animator>().Play("hide_desc");
        prompt.GetComponent<Animator>().Play("hide_prompt");
        //StartCoroutine(RemoveAfterSeconds(1, yesButton));
        //StartCoroutine(RemoveAfterSeconds(1, noButton));
        //StartCoroutine(HideTextAfterSeconds(1, description));
        //StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    // Punishments
    public void startPunishmentPhase()
	{
        currentPhase = Phase.Punishment;
        currentPIndex = 0;
        setupVoteUI();
        prompt.text = "<b>" + acceptList[0].Name + "</b>" 
                + " would you like to impose tariffs on everyone who declined?\n\n\nCost = $"
                + (declineList.Count * 1000)
                + " \nEffect: Each decliner loses 0.05% growth rate";
        //Debug.Log("Cost = $" + (declineList.Count*3000) + "GDP | Punishment: -"+ (0.05) +" Growth per country that declined");
        //declineList.ForEach(q =>
        //{
        //    q.Growth -= .05;
        //});
    }
    
    public void showWinner()
    {
        description.GetComponent<Animator>().Play("hide_desc");
        finalGDP.text = "";
        finalCO2.text = "";
        finalScore.text = "";
        prompt.GetComponent<Animator>().Play("center_prompt");
        List<Country> winners = GetWinner();
        if (winners.Count == 1) { prompt.text = "The winner is...\n" + winners[0].Name + "!"; }
        else if (winners.Count == 2) { 
            prompt.text = "The winners are...\n" + winners[0].Name + " and "+winners[1].Name+"!"; 
        }
        else
        {
            string winString = "The winners are...\n";
            for (int i = 0; i < winners.Count - 1; i++)
            {
                winString = winString + winners[i].Name + ", ";
            }
            winString = winString + "and " + winners[winners.Count - 1].Name + "!";
            prompt.text = winString;
        }
    }

    // Camera Panning Related
    public void OnRightButton()
	{
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
        if (currentPIndex < 3) currentPIndex++;
        else currentPIndex = 0;
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name + "\nScore: " + playerList[currentPIndex].Score;
	}
    public void OnLeftButton()
    {
        mainCamera.GetComponent<CameraPanning>().OnLeftButtonPress();
        if (currentPIndex > 0) currentPIndex--;
        else currentPIndex = 3;
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name + "\nScore: " + playerList[currentPIndex].Score;
    }
    // End
    private void NextResult()
    {
        if (currentPIndex == 3)
        {
            showWinner();
            currentPIndex++;
            
        }
        else if (currentPIndex == 4)
        {
            //back to main menu
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
            SceneManager.LoadScene("MainMenu");
            //canvas = resetCanvas;
            Debug.Log("that's all folks");
        }
        else
        {
            currentPIndex++;
            prompt.text = playerList[currentPIndex].Name + "'s Achievements:";
            finalGDP.text = "GDP: " + playerList[currentPIndex].GDP.ToString("C2");
            finalCO2.text = "CO<sub><size=150%>2</size></sub>: " + playerList[currentPIndex].Emissions.ToString("F2") + "MT";
            finalScore.text = "Score: " + playerList[currentPIndex].Score.ToString();
            string newStr = PrintAccolades(currentPIndex);
            description.text = newStr;
        }
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
    }

    private string PrintAccolades(int pIndex)
    {
        string accStr = "";
        foreach (Accolades a in evaluation[pIndex])
        {
            if (a == Accolades.TopGDP) accStr += "You ended with the highest GDP! Congrats!";
            if (a == Accolades.BotEmi) accStr += "Nice! You had the lowest carbon emissions!";
            if (a == Accolades.TopEmi) accStr += "You had the highest carbon emissions.";
            if (a == Accolades.AllAgree) accStr += "You demonstrated remarkable concern for the environment by accepting every treaty.";
            if (a == Accolades.AllDecline) accStr += "You didn't agree to a single climate treaty.";
            accStr += "\n";
        }
        return accStr;
    }

    private List<Country> GetWinner()
    {
        List<Country> bestPlayers = new List<Country> {playerList[0]};
        int bestScore = 0;
        int ci = 0;
        int bi = 0;
        playerList.ForEach(p =>
        {
            if (p.Score > bestScore)
            {
                bestPlayers = new List<Country> {p};
                bestScore = p.Score;
                Debug.Log(bestPlayers[0].Name);
                bi = ci;
            }
            else if (p.Score == bestScore)
            {
                bestPlayers.Add(p);
            }
            ci++;
        });
        for (int i = 0; i < bi; i++)
		{
            mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
        }
        return bestPlayers;
    }

    // Coroutine Related
    IEnumerator ColorLerp(Color endValue, float duration)
    {
        float time = 0;
        Image sprite = backgroundImage.GetComponent<Image>();
        Color startValue = sprite.color;

        while (time < duration)
        {
            sprite.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        sprite.color = endValue;
    }

    IEnumerator TextLerp(bool agreed, TextMeshProUGUI text)
    {
        float time = 0;

        Color start;
        Color end = Color.white;

        if (agreed) start = Color.green;
        else start = Color.red;
        text.color = start;
        while (time < 2)
        {
            text.color = Color.Lerp(start, end, time / 2);
            time += Time.deltaTime;
            yield return null;
        }
        text.color = end;
    }

    IEnumerator LoadYourAsyncScene(bool lerp, string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (lerp) { StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 0.75f)); }
        else StartCoroutine(ColorLerp(new Color(1, 1, 1, 1), 0.75f)); // reverse
    }

    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    IEnumerator HideTextAfterSeconds(int seconds, TextMeshProUGUI t)
    {
        yield return new WaitForSeconds(seconds);
        t.text = "";
    }

    IEnumerator Wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // Sets the volume using the slider
    public void setVolume(float sliderValue)
    {
        mixer.SetFloat("masterVol", (Mathf.Log10(sliderValue) * 20));
    }
    public void setMusic(float sliderValue)
    {
        mixer.SetFloat("musicVol", (Mathf.Log10(sliderValue) * 20));
    }
    public void setOther(float sliderValue)
    {
        mixer.SetFloat("otherVol", (Mathf.Log10(sliderValue) * 20));
    }
}
