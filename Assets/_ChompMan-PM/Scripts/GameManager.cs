using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    
    private GameObject[] statistics;
    private TextMeshProUGUI[] stats;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI recapText;
    private GameObject[] enemyPos;
    private GameObject[] cherryPos;
    private int s;
    

    public float timer;
    public float timerCycle;
    
    public bool scaredGhosts;
    public bool leaveBase;

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
        switch (s)
        {
            case 0: //Pantalla del juego
                gameOver = false;
                points = 1;
                kills = 0;
                totalPoints = GameObject.FindGameObjectsWithTag("Point").Length;
                statistics = GameObject.FindGameObjectsWithTag("Stats");
                enemyPos = GameObject.FindGameObjectsWithTag("EnemyPos");
                cherryPos = GameObject.FindGameObjectsWithTag("CherryPos");

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
                recapText.text = "Points: " + (totalPoints - points) + "\nKills: " + kills + "\nTime: " + TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff");
            break;
        }
       
    }
   
    
    // Update is called once per frame
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
                    //Cada 10 segundos.
                    if (!scaredGhosts)
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
        int pos1  = UnityEngine.Random.Range(0, enemyPos.Length);
        int pos2 = pos1;
        while(pos1 == pos2)
        {
            pos2 = UnityEngine.Random.Range(0, enemyPos.Length);
        }

        Vector3 p1 = enemyPos[pos1].transform.position;
        Vector3 p2 = enemyPos[pos2].transform.position;
        Instantiate(smallGhostPrefab, new Vector3(p1.x,0f,p1.z), Quaternion.identity);
        Instantiate(bigGhostPrefab, new Vector3(p2.x, 0f, p2.z), Quaternion.identity);
    }
    public void SpawnCherry()
    {
        int pos = UnityEngine.Random.Range(0, cherryPos.Length);
        Instantiate(cherryPrefab, cherryPos[pos].transform.position, Quaternion.identity);
    }
    public void UpdateStats()
    {
        for (int i = 0; i < statistics.Length; i++)
        {
            stats[i] = statistics[i].GetComponent<TextMeshProUGUI>();
            switch (statistics[i].name)
            {
                case "Points":
                    stats[i].text = "Points: " + points + "/" + totalPoints;
                    break;
                case "Enemies":
                    stats[i].text = "Kills: " + kills;
                    break;
            }
        }
    }
    public IEnumerator CherryAction(GameObject cherry)
    {
        Destroy(cherry);
        scaredGhosts = true;
        yield return new WaitForSeconds(timerCycle);
        SpawnCherry();
        scaredGhosts = false;
        leaveBase = true;

    }
    public void pointDec()
    {
        points--;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
