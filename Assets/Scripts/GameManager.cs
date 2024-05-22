using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Initial,
        Playing,
        End
    }

    public GameState state;

    public static GameManager Instance;
    [SerializeField] private GameObject[] spawnerPoints;
    [SerializeField] private GameObject[] spawnObjects;

    [SerializeField] private TextMeshProUGUI txtRightScore;
    [SerializeField] private TextMeshProUGUI txtWrongScore;
    [SerializeField] private TextMeshProUGUI txtGameTimer; // Add this for displaying the game timer
    [SerializeField] private TextMeshProUGUI txtCountdown; // Add this for displaying the countdown

    [SerializeField] private int rightScore;
    [SerializeField] private int wrongScore;
    [SerializeField] private int maxScore;
    [SerializeField] private int gameTimer; // Initial game timer value in seconds

    [SerializeField] private float spawnInterval = 1.0f; // Time interval between spawns

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;

    private Coroutine spawnCoroutine;
    private Coroutine timerCoroutine;

    private void Start()
    {
        Instance = this;
        InitializeGame();
    }

    private void InitializeGame()
    {
        rightScore = 0;
        wrongScore = 0;
        txtRightScore.text = rightScore.ToString();
        txtWrongScore.text = wrongScore.ToString();
        txtGameTimer.text = gameTimer.ToString();
        txtCountdown.text = "";
        endPanel.SetActive(false);
        startPanel.SetActive(true);
        state = GameState.Initial;
    }

    public void StartGame()
    {
#if UNITY_EDITOR
        Debug.Log("Game Started");
#endif
        StartCoroutine(CountdownRoutine());
    }

    public void RestartGame()
    {
#if UNITY_EDITOR
        Debug.Log("Restarting Game");
#endif
        StopAllCoroutines();
        InitializeGame();
        StartGame();
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        Debug.Log("Game Ended");
#endif
        state = GameState.End;
        StopAllCoroutines();
        endPanel.SetActive(true);
    }

    private IEnumerator SpawnObjectsRoutine()
    {
        while (state == GameState.Playing)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Select a random spawner point
            int randomSpawnerIndex = Random.Range(0, spawnerPoints.Length);
            GameObject randomSpawner = spawnerPoints[randomSpawnerIndex];

            // Select a random spawn object
            int randomObjectIndex = Random.Range(0, spawnObjects.Length);
            GameObject randomObject = spawnObjects[randomObjectIndex];

            // Instantiate the spawn object at the spawner point's position
            Instantiate(randomObject, randomSpawner.transform.position, Quaternion.identity);
        }
    }

    private IEnumerator GameTimerRoutine()
    {
        while (gameTimer > 0 && state == GameState.Playing)
        {
            yield return new WaitForSeconds(1.0f);
            gameTimer--;
            txtGameTimer.text = gameTimer.ToString();
        }

        if (gameTimer == 0)
        {
            EndGame();
        }
    }

    private IEnumerator CountdownRoutine()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            txtCountdown.text = countdown.ToString();
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }
        txtCountdown.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        txtCountdown.text = "";

        state = GameState.Playing;
        spawnCoroutine = StartCoroutine(SpawnObjectsRoutine());
        timerCoroutine = StartCoroutine(GameTimerRoutine());
        startPanel.SetActive(false);
    }

    public void UpdateScore(bool isTrue)
    {
        if (state != GameState.Playing) return;

        if (isTrue)
        {
            rightScore++;
            txtRightScore.text = rightScore.ToString();
        }
        else
        {
            wrongScore++;
            txtWrongScore.text = wrongScore.ToString();
        }
    }
}
