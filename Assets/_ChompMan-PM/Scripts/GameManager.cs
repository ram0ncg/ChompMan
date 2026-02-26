using System;
using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{
    private int s; //scene index
    private GameObject[] statistics;
    private TextMeshProUGUI[] stats;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI recapText;
    private GameObject[] enemyPos;
    private GameObject[] cherryPos;
    private GameObject menuPanel;
    private GameObject creditsPanel;
    private GameObject configPanel;
    private GameObject chomp;
    private AudioSource audioSource;

    public float timer;
    public float timerCycle;
    public AudioClip hardModeMusic;
    public AudioClip music;
    public AudioClip scaredGhostsClip;
    public AudioClip ghostChomp;

    public static bool hardMode;
    public static int points;
    public static int kills;
    private static int totalPoints;
    public static bool gameOver;
    public static float time;

    public GameObject smallGhostPrefab;
    public GameObject bigGhostPrefab;
    public GameObject cherryPrefab;

    void Start()
    {   
        s = SceneManager.GetActiveScene().buildIndex;
        audioSource = GetComponent<AudioSource>();
        switch (s)
        {
            case 0: //Pantalla del juego
                if (hardMode)
                {
                    audioSource.clip = hardModeMusic;
                    timerCycle = 11;
                }
                else
                {
                    audioSource.clip = music;
                    timerCycle = 13;
                }
                audioSource.Play();
                gameOver = false;
                points = 1;
                kills = 0;
                totalPoints = GameObject.FindGameObjectsWithTag("Point").Length;
                statistics = GameObject.FindGameObjectsWithTag("Stats");
                enemyPos = GameObject.FindGameObjectsWithTag("EnemyPos");
                cherryPos = GameObject.FindGameObjectsWithTag("CherryPos");
                chomp = GameObject.Find("Chomp");

                points = totalPoints;
                timer = 0f;
                stats = new TextMeshProUGUI[statistics.Length];
                
                SpawnEnemy();
                SpawnCherry();
                break;
            case 1://Pantalla de GameOver
                gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();
                recapText = GameObject.Find("Recap").GetComponent<TextMeshProUGUI>();
                if (gameOver) 
                { 
                    gameOverText.text = "GAME OVER";
                }
                else
                {
                    gameOverText.text = "YOU WIN!";
                }
                string diff = hardMode ? "Hard" : "Easy";
                float score = ((points / totalPoints) * 1000) + (kills * 100) - (time * 5);
                score = Mathf.Max(0, score);
                score = Mathf.RoundToInt(score);
                recapText.text ="Total Score: " + score + "\nPoints: " + (totalPoints - points) + "\nKills: " + kills + "\nTime: " + TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff")  + "\nDifficulty: " + diff;
                break;
            case 2: //Pantalla MainMenu
                menuPanel = GameObject.Find("MenuPanel");
                creditsPanel = GameObject.Find("CreditsPanel");
                configPanel = GameObject.Find("ConfigPanel");
                menuPanel.SetActive(true);
                creditsPanel.SetActive(false);
                configPanel.SetActive(false);
                break;
        }
       
    }
    //TODO scaredGhosts, should it be individual or share timer.
    void Update()
    {
        if(s == 0)
        { 
            if (!gameOver)
            {
                time += Time.deltaTime;
                timer += Time.deltaTime;
                UpdateStats();
                if (timer >= timerCycle)
                {
                    if (ScareGhosts(false, true)) //Si no hay ningun fantasma asustado podran aparecer otros.
                    {
                        SpawnEnemy();
                    }
                    timer -= timerCycle;
                }
            }
            if(gameOver || points == 0)
            {
                SceneManager.LoadScene(1);
            }
        }  
    }
    public void SpawnEnemy()
    {
        int pos1 = hardMode ? UnityEngine.Random.Range(0, enemyPos.Length): closeFarPosition(enemyPos, false);
        int pos2 = (pos1 + UnityEngine.Random.Range(1, enemyPos.Length)) % enemyPos.Length;
        Vector3 p1 = enemyPos[pos1].transform.position;
        Vector3 p2 = enemyPos[pos2].transform.position;
        Instantiate(smallGhostPrefab, new Vector3(p1.x, 0f, p1.z), Quaternion.identity);
        Instantiate(bigGhostPrefab, new Vector3(p2.x, 0f, p2.z), Quaternion.identity);
    }
    public void SpawnCherry()
    {
        Vector3 pos = Vector3.zero;
        if (!hardMode) //Si no esta en modo dificil, la cereza aparecera lo mas cerca posible del jugador.
        {
            pos = cherryPos[closeFarPosition(cherryPos, true)].transform.position;
        }
        else //En modo dificil es aleatorio.
        {
           pos = cherryPos[UnityEngine.Random.Range(0, cherryPos.Length)].transform.position;
        }    
        Instantiate(cherryPrefab, pos, Quaternion.identity);
    }
    public IEnumerator CherryAction(GameObject cherry)
    {
        Destroy(cherry);
        ScareGhosts(true); //Al consumir las cerezas asusta a todos los fantasmas.
        float timer = hardMode ? timerCycle + 2f : timerCycle;
        yield return new WaitForSeconds(timer);
        SpawnCherry();
        ScareGhosts(false); //Al aparecer una cereza nueva resetea los fantasmas que queden asustados.
    }
    public bool ScareGhosts(bool scare = true, bool isScared = false) //Asusta o resetea ne funcion de scare, o devuelve si existe algun fantasma ya asustado.
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool spawn = false;
        if (enemies.Length > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                if (!isScared)
                {
                    enemy.GetComponent<Enemy>().scaredGhosts = scare;
                }
                else
                {
                    if (enemy.GetComponent<Enemy>().scaredGhosts == true) //Si todavia hay algun fantasma asustado no puede spawnear otro
                    {
                        return false;
                    }
                    else
                    {
                        spawn = true;
                    }
                }
            }
        }
        else //Si no hay ningun fantasma con vida
        {
            return true;
        }
        return spawn;
        
    }
    public int closeFarPosition(GameObject[] spawnPoints, bool closer)
    {
        int index = 0;
        Vector3 pos = spawnPoints[0].transform.position;
        Vector3 chompPos = chomp.transform.position;
        float diffDistance = (pos - chompPos).sqrMagnitude;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float distance = (spawnPoints[i].transform.position - chompPos).sqrMagnitude;
            bool cf = closer ? (distance < diffDistance) : (distance > diffDistance);

            if (cf)
            {
                diffDistance = distance;
                index = i;
                pos = spawnPoints[i].transform.position;
            }

        }
        return index;
    }
    public void UpdateStats()
    {
        for (int i = 0; i < statistics.Length; i++)
        {
            stats[i] = statistics[i].GetComponent<TextMeshProUGUI>();
            switch (statistics[i].name)
            {
                case "Points":
                    string diff = hardMode ? "Hard" : "Easy";
                    stats[i].text = "Mode: " + diff + "\nPoints: " + points + "/" + totalPoints;
                    break;
                case "Enemies":
                    stats[i].text = "Kills: " + kills;
                    break;
            }
        }
    }
   
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(2);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ShowConfig()
    {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        configPanel.SetActive(true);
    }
    public void ShowCredits()
    {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
    public void Difficulty(bool easy)
    {
        hardMode = !easy;
        MainMenu();
    }
}
