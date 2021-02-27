using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Moves;

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
    [SerializeField] private float hintTime = 5f;

    [Space]
    [SerializeField] private int targetScore;
    [SerializeField] private float timeLimit;
    [SerializeField] private int movesLimit;

    private GameState gameState;
    private int   currentScore = 0;
    private int   currentMoves = 0;
    private float currentGameTime = 0f;
    private float currentHintTime = 0f;
    private bool givingHint;

    public static event Action<int> OnScoreChange;

    private void Awake()
    {
        level.OnReadyToMakeMove += OnReadyToMakeMove;
        level.OnClusterBreak += OnClusterBreak;
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

        if (currentGameTime > timeLimit)
        {
            Debug.Log("Game over");
            gameState = GameState.GameOver;
            return;
        }
        
        if (currentHintTime < 0)
        {
            GiveHint();
            currentHintTime = hintTime;
        }


    }

    public void MakeMove(Move move)
    {
        if (gameState != GameState.WaitingForMove)
            return;

        StartCoroutine(level.MakeMove(move));
        gameState = GameState.Moving;
        currentMoves++;
        givingHint = false;
    }

    private void OnClusterBreak(int tilesCount)
    {
        if (gameState == GameState.GameOver)
            return;

        currentScore += tilesCount;
        OnScoreChange(currentScore);
    }

    private void OnReadyToMakeMove()
    {
        Debug.Log("OnReadyToMakeMove");

        gameState = GameState.WaitingForMove;

        currentHintTime = hintTime;
    }

    private void GiveHint()
    {
        if (givingHint)
            return;

        givingHint = true;
        Debug.Log("Give a hint");
    }

    private void OnDestroy()
    {
        level.OnReadyToMakeMove -= OnReadyToMakeMove;
        level.OnClusterBreak -= OnClusterBreak;
        input.OnMoveMade -= MakeMove;
    }

}
