using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Moves;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Level level;
    [SerializeField] private GameInput input;
    [SerializeField] private float hintTime = 5f;

    private void Awake()
    {
        level.OnNoMovesLeft += OnNoMovesLeft;
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
        
    }

    public void MakeMove(Move move)
    {
        StartCoroutine(level.MakeMove(move));
    }

    private void OnClusterBreak(int tilesCount)
    {
        Debug.Log("Cluster Break, tiles count: " + tilesCount);
    }
    private void OnReadyToMakeMove()
    {
        Debug.Log("Ready to make move");
    }

    private void OnNoMovesLeft()
    {
        Debug.Log("No moves left");
    }

    private void OnDestroy()
    {
        level.OnNoMovesLeft -= OnNoMovesLeft;
        level.OnClusterBreak -= OnClusterBreak;
        input.OnMoveMade -= MakeMove;
    }

}
