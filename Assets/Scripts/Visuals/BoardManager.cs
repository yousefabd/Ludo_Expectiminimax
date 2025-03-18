using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    private readonly int whiteCells = 52;
    private readonly int playerWinningCellsCount = 20;
    private int BoardSize => whiteCells + playerWinningCellsCount;
    private Board gameBoard;
    private void Awake()
    {
        Instance = this;
        gameBoard = new Board(playerWinningCellsCount * 4);
    }
    public Board GetGameBoard()
    {
        return gameBoard;
    }
    public void AddTeamPuzzlePieces(Team team, PuzzlePiece[] puzzlePieces)
    {
        gameBoard.AddTeamPuzzlePieces(team, puzzlePieces);
    }
    public int GetPuzzlePieceIndex(PuzzlePiece puzzlePiece)
    {
        return gameBoard.GetTeamPuzzlePieceBaseIndex(puzzlePiece);
    }
}
