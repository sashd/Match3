using System;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Moving,
        WaitingForMove,
        GameOver
    }

    [SerializeField] private Level level;
    [SerializeField] private GameInput input;

    [Header("Game settings")]
    [Min(1f)] [SerializeField] private float hintTime = 3f;
    [Min(0f)] [SerializeField] private float timeLimit = 60f;
    [Min(3)]  [SerializeField] private int targetScore = 40;
    [Min(1)]  [SerializeField] private int movesLimit = 10;

    private GameState gameState;
    private int   currentScore = 0;
    private int   currentMoves = 0;
    private float currentGameTime = 0f;
    private float currentHintTime = 0f;
    private bool  hintGiven;
    private bool  gameWon = false;

    public static event Action<int> OnScoreChange;

    private void Awake()
    {
        level.OnReadyToMakeMove += OnReadyToMakeMove;
        level.OnMatchBreak += OnMatchBreak;
        input.OnMoveMade += MakeMove;
    }

    private void Start()
    {
        level.Generate();
    }

    private void Update()
    {
        if (gameState == GameState.GameOver)
            return;

        currentGameTime += Time.deltaTime;

        if (gameState == GameState.WaitingForMove)
        {
            currentHintTime -= Time.deltaTime;
        }

        if (currentHintTime < 0)
        {
            GiveHint();
            currentHintTime = hintTime;
        }

        if (currentGameTime > timeLimit)
        {
            GameLost();
        }
    }

    public void MakeMove(Move move)
    {
        if (gameState != GameState.WaitingForMove)
            return;

        StartCoroutine(level.MakeMove(move));
        gameState = GameState.Moving;
        currentMoves++;
        hintGiven = false;
    }

    private void OnMatchBreak(int tilesCount)
    {
        if (gameState == GameState.GameOver)
            return;

        currentScore += tilesCount;
        OnScoreChange(currentScore);

        if (currentScore >= targetScore)
        {
            GameWon();
        }
    }

    private void OnReadyToMakeMove()
    {
        if (currentMoves >= movesLimit)
        {
            GameLost();
            return;
        }

        gameState = GameState.WaitingForMove;
        currentHintTime = hintTime;
    }

    private void GiveHint()
    {
        if (hintGiven)
            return;

        level.ShowHint();

        hintGiven = true;
    }

    private void GameWon()
    {
        gameState = GameState.GameOver;
        gameWon = true;
    }

    private void GameLost()
    {
        gameState = GameState.GameOver;
    }

    private void OnDestroy()
    {
        level.OnReadyToMakeMove -= OnReadyToMakeMove;
        level.OnMatchBreak -= OnMatchBreak;
        input.OnMoveMade -= MakeMove;
    }

    // FOR TESTING
    GUIStyle style = new GUIStyle();
    StringBuilder debugInfo = new StringBuilder(20);
    private void OnGUI()
    {
        style.fontSize = 48;
        if (gameWon)
        {
            debugInfo.AppendLine($"You are win!");
        }
        else
        {
            debugInfo.AppendLine($"Game state: {gameState}");
            debugInfo.AppendLine($"Target score: {targetScore}");
            debugInfo.AppendLine($"Time left: {Mathf.Ceil(timeLimit - currentGameTime)}");
            debugInfo.AppendLine($"Moves left: {movesLimit - currentMoves}");
        }
        GUI.Label(new Rect(10, 10, 100, 20), debugInfo.ToString(), style);
        debugInfo.Clear();
    }
}
