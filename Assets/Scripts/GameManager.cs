using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject canvas;
    public GameObject events;
    private GameObject resetCanvas;

    public TextMeshProUGUI title;
    public GameObject startButton;
    public TextMeshProUGUI prompt;
    public GameObject nameEntry;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject backgroundImage;
    public TextMeshProUGUI description;
    public GameObject submitButton;
    public GameObject leaderboard;
    public GameObject nextButton;

    public bool botMode = false;

    //Game Designer controlled "upgrade" values
    public double pollutionUpgrade1;
    public double pollutionUpgrade2;
    public double cityUpgrade1;
    public double cityUpgrade2;
    public double cityUpgrade3;

    private GameObject Country1;
    private GameObject Country2;
    private GameObject Country3;
    private GameObject Country4;

    private List<Country> playerList = new List<Country>();
    List<GameObject> CountryList = new List<GameObject>();

    private int currentPIndex;
    private VoteManager currentVote = new VoteManager();
    private Phase currentPhase = Phase.Menu;
    private double TotalDamage = 1250.0f;
    private bool P1Ani = false;

    private double treatyCost = -500;
    private double emissionsChangePct = -0.05;

    private int completedVotes = 0;
    public int maxTurns = 5;

    private Evaluator eval;
    private List<List<Accolades>> evaluation;


    // Start is called before the first frame update
    void Start()
    {
        resetCanvas = canvas;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(events);
            
        }
        else
        {
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
        }
    }

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
            text.color= Color.Lerp(start, end, time / 2);
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
        if (lerp) { StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 1)); }
        else StartCoroutine(ColorLerp(new Color(1, 1, 1, 1), 1)); // reverse
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

    public void start()
    {
        StartCoroutine(HideTextAfterSeconds(1, title));
        StartCoroutine(RemoveAfterSeconds(1, startButton));
        prompt.text = "Player 1\nEnter your country's name!";
        nameEntry.SetActive(true);
        submitButton.SetActive(true);
    }

    public void submit()
    {
        string name = nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        if (name.Length > 1 && name.Length <= 9)
        {
            nameEntry.GetComponent<TMP_InputField>().text = "";
            Country newPlayer = new Country(name);
            playerList.Add(newPlayer);
            if (playerList.Count == 4 || botMode)
            {
                clearMenuUI();
                initializeNames();
                startCitiesPhase();
            }
            else
            {
                int current = playerList.Count + 1;
                prompt.text = "Player " + current + "\nEnter your country's name!";
            }
        }
    }

    private void clearMenuUI()
    {
        nameEntry.SetActive(false);
        submitButton.SetActive(false);
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    private void startCitiesPhase()
    {
        if (currentPhase == Phase.Menu)
        {
            StartCoroutine(LoadYourAsyncScene(true, "Countries"));
        }
        else
        {
            TextMeshProUGUI[] nameList = leaderboard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < playerList.Count; i++)
            {
                StartCoroutine(TextLerp(playerList[i].HaveAgreed, nameList[i])); 
            }
        }
        nextButton.SetActive(true);
        currentPhase = Phase.Cities;
        nextButton.GetComponent<Animator>().Play("show_next");
        leaderboard.GetComponent<Animator>().Play("show_leader");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        CountryList.Add(GameObject.Find("Country1"));
        CountryList.Add(GameObject.Find("Country2"));
        CountryList.Add(GameObject.Find("Country3"));
        CountryList.Add(GameObject.Find("Country4"));
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

    private void setupVoteUI()
    {
        yesButton.SetActive(true);
        noButton.SetActive(true);
        StartCoroutine(ColorLerp(new Color(0.61f, 0.83f, 0.89f, 1), 1));
        yesButton.GetComponent<Animator>().Play("show_agree");
        noButton.GetComponent<Animator>().Play("show_decline");
        nextButton.GetComponent<Animator>().Play("hide_next");
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        StartCoroutine(RemoveAfterSeconds(2, nextButton));

    }

    public void Next()
    {
        if (currentPhase == Phase.Cities)
        {
            if (completedVotes == maxTurns) startResultsPhase();
            else startVotePhase();
        }
        else if (currentPhase == Phase.Results) NextResult();
    }

    private void NextResult()
    {
        if (currentPIndex == 3)
        {
            //back to main menu
            SceneManager.LoadScene("MainMenu");
            canvas = resetCanvas;
            canvas.SetActive(false);
            Debug.Log("that's all folks");
        }
        else
        {
            currentPIndex++;
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
            prompt.text = "Player " + (currentPIndex + 1) + " Results";
            string newStr = PrintAccolades(currentPIndex);
            description.text = newStr;
        }
    }

    private string PrintAccolades(int pIndex)
    {
        string accStr = "";
        foreach (Accolades a in evaluation[pIndex])
        {
            if (a == Accolades.TopGDP) accStr += "You ended with the highest GDP! Congrats!";
            if (a == Accolades.BotEmi) accStr += "Nice! You had the lowest carbon emissions!";
            if (a == Accolades.TopEmi) accStr += "You had the highest carbon emissions.";
            accStr += "\n";
        }
        return accStr;
    }
    
    public void startVotePhase()
    {
        currentPhase = Phase.Votes;
        prompt.text = GetPromptText();
        setupVoteUI();
        currentPIndex = 0;
        leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        currentVote = new VoteManager();
        currentVote.clearVotes();
    }

    private void startResultsPhase()
    {
        currentPhase = Phase.Results;
        currentPIndex = 0;
        eval = new Evaluator(playerList);
        evaluation = eval.evaluate();
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0.5f), 2));
        prompt.text = "Player " + (currentPIndex + 1) + " Results";
        description.text = PrintAccolades(0);
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
    }

    private void clearVoteUI()
    {
        leaderboard.GetComponent<Animator>().Play("hide_P4");
        yesButton.GetComponent<Animator>().Play("hide_agree");
        noButton.GetComponent<Animator>().Play("hide_decline");
        description.GetComponent<Animator>().Play("hide_desc");
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(RemoveAfterSeconds(2, yesButton));
        StartCoroutine(RemoveAfterSeconds(2, noButton));
        StartCoroutine(HideTextAfterSeconds(2, description));
        StartCoroutine(HideTextAfterSeconds(2, prompt));
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 2));
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
        var player = playerList[currentPIndex];
        player.Growth = 0.1f;
        player.ActivateGDPGrowth();
        player.adjustGDP(treatyCost);
        player.adjustEmissions(emissionsChangePct);
        currentVote.AcceptVotes += 1;
        playerList[currentPIndex].Agree();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (botMode && currentPIndex == 1) BotVote();
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void decline()
    {
        var player = playerList[currentPIndex];
        player.Growth = 0.15f;
        player.ActivateGDPGrowth();
        currentVote.DeclineVotes += 1;
        player.adjustEmissions(-emissionsChangePct);
        playerList[currentPIndex].Decline();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (botMode && currentPIndex == 1) BotVote();
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void enactVotes()
    {
        completedVotes++;
        AdjustCountries();
        updateLeaderboard();
        clearVoteUI();
        for (int i = 0; i < 4; i++)
        {
            CountryList[i].transform.Find("Land 1 (base)").gameObject.SetActive(true);
            CountryList[i].transform.Find("Land 2 (hi C)").gameObject.SetActive(false);
            CountryList[i].transform.Find("Land 3 (desert)").gameObject.SetActive(false);
            CountryList[i].transform.Find("City 1").gameObject.SetActive(true);
            CountryList[i].transform.Find("City 2").gameObject.SetActive(false);
            CountryList[i].transform.Find("City 3").gameObject.SetActive(false);
            CountryList[i].transform.Find("City 4").gameObject.SetActive(false);
            if (playerList[i].Emissions >= pollutionUpgrade1)
            {
                CountryList[i].transform.Find("Land 1 (base)").gameObject.SetActive(false);
                CountryList[i].transform.Find("Land 2 (hi C)").gameObject.SetActive(true);
            }
            if (playerList[i].Emissions >= pollutionUpgrade2)
            {
                CountryList[i].transform.Find("Land 2 (hi C)").gameObject.SetActive(false);
                CountryList[i].transform.Find("Land 3 (desert)").gameObject.SetActive(true);
            }
            if (playerList[i].GDP >= cityUpgrade1)
            {
                CountryList[i].transform.Find("City 1").gameObject.SetActive(false);
                CountryList[i].transform.Find("City 2").gameObject.SetActive(true);
            }
            if (playerList[i].GDP >= cityUpgrade2)
            {
                CountryList[i].transform.Find("City 2").gameObject.SetActive(false);
                CountryList[i].transform.Find("City 3").gameObject.SetActive(true);
            }
            if (playerList[i].GDP >= cityUpgrade2)
            {
                CountryList[i].transform.Find("City 3").gameObject.SetActive(false);
                CountryList[i].transform.Find("City 4").gameObject.SetActive(true);
            }

        }
        startCitiesPhase();
    }

    private void updateLeaderboard()
    {
        TextMeshProUGUI[] gdpList = leaderboard.transform.Find("GDPs").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] emmList = leaderboard.transform.Find("Emissions").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            string new_gdp = playerList[i].GDP.ToString("C2");
            string new_emm = playerList[i].Emissions.ToString("F2");
            gdpList[i].text = "GDP: " + new_gdp;
            emmList[i].text = "EMISSIONS: " + new_emm + "GT";
        }
    }

    private void AdjustCountries()
    {
        int numAgreed = 0;
        playerList.ForEach(p => { if (p.HaveAgreed) { numAgreed++;} });
        double damageGrowthMultiplier = 2.0f - numAgreed * 0.45f;
        double damageThisRound = TotalDamage * damageGrowthMultiplier;
        TotalDamage += damageThisRound;
        AdjustGDPEmissionDamage(damageThisRound, 0.05f, 0.4f);
    }

    private void AdjustGDPEmissionDamage(double totalMoney, double agreeMultiplier, double declineMultiplier)
    {
        playerList.ForEach(p =>
        {
            if (p.HaveAgreed)
            { p.adjustGDP(-totalMoney * agreeMultiplier); }
            else
            { p.adjustGDP(-totalMoney * declineMultiplier); }
        });
    }

    private string GetPromptText()
    {
        string toReturn = "";
        double emissionDecPerc = emissionsChangePct * -100;
        toReturn += $"Agree to a treaty with your fellow countries to reduce " +
            $"your emissions by {emissionDecPerc}% in one turn of the game?";
        return toReturn;
    }

}
