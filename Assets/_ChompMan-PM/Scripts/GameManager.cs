using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int totalPoints;
    public int points;
    private int enemies;
    private GameObject[] statistics;
    private TextMeshProUGUI[] stats;
    public bool gameOver;

    public GameObject gameScreen;
    public GameObject endScreen;
    void Start()
    {
        totalPoints = GameObject.FindGameObjectsWithTag("Point").Length;
        enemies = GameObject.FindGameObjectsWithTag("Player").Length;
        points = totalPoints;
        statistics = GameObject.FindGameObjectsWithTag("Stats");
        stats = new TextMeshProUGUI[statistics.Length];
        UpdateStats();
    }
    public void pointDec()
    {
        points--;
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
                    stats[i].text = "Enemies: " + enemies;
                    break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameOver || points > 0)
        {
            UpdateStats();
            if(points == 0)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
    public void EndScreen()
    {
        if (gameOver)
        {

        }
        else
        {

        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
