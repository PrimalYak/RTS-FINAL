using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Tayx.Graphy;

public class ResultLogger
{
    public static int[] playerXGoldNodesGathered;
    public static int[] playerXEnemiesKilled;


    public static float avgFPS;
    public static float maxFPS;
    public static float minFPS;
    public static float gameLength = 0f;
    public static SceneBuilder scene;
    public static TeamNumber playerNumberWinner;
    public static string path;
    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/gameAnalytics.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Analytics file \n\n");
        }

        scene = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        playerXGoldNodesGathered = new int[scene.numberOfPlayers];
    }

    // Update is called once per frame
    void Update()
    {
        updateGameTime();
    }

    public static void updateFPSStats()
    {
        avgFPS = GraphyManager.Instance.AverageFPS;
        minFPS = GraphyManager.Instance.MinFPS;
        maxFPS = GraphyManager.Instance.MaxFPS;
    }
    public static void updateGameTime()
    {
        gameLength += Time.deltaTime;
    }
    public static float getGameLength()
    {
        return gameLength;
    }
    public static void setPlayerWinnerNumber(TeamNumber teamNumber)
    {
        playerNumberWinner = teamNumber;
    }

    public static string logStats()
    {
        updateFPSStats();

        string serializedData =
        "PlayerNumberWinner, " + playerNumberWinner.ToString() + "\n" +
        "MinFPS, " + minFPS.ToString() + "\n" +
        "AvgFPS, " + avgFPS.ToString() + "\n" +
        "MaxFPS, " + maxFPS.ToString() + "\n" +
        "GameLength, " + gameLength.ToString() + "\n" +
        "player1GoldNodesGathered, " + playerXGoldNodesGathered[0].ToString() + "\n" +
        "player2GoldNodesGathered, " + playerXGoldNodesGathered[1].ToString() + "\n" +
        "player1EnemiesKilled, " + playerXEnemiesKilled[0].ToString() + "\n" +
        "player2EnemiesKilled, " + playerXEnemiesKilled[1].ToString() + "\n";






        return serializedData;
    }
    public static void resetAllLoggedVariables()
    {

    }
    public static void addGoldNodeGatherer(TeamNumber teamNumber)
    {
        playerXGoldNodesGathered[(int)teamNumber - 1]++;
    }
    public static void addEnemiesKilled(TeamNumber teamNumber)
    {
        playerXEnemiesKilled[(int)teamNumber - 1]++;
    }
    public static void logAllDataToFile()
    {

        //StreamReader reader = new StreamReader("gameAnalytics.txt");

        File.AppendAllText(path, "\n" + logStats());
    }
}
