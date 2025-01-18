using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    private readonly int whiteCells = 52;
    private readonly int winningCells = 10;
    private int BoardSize => whiteCells + winningCells;

    private Cell[] gameBoard;
    private bool[] starCells;

    private void Awake()
    {
        gameBoard = new Cell[BoardSize];
        starCells = new bool[BoardSize];
        Instance = this;
    }
    private void Start()
    {
        List<int> starIndices = new List<int> { 8, 21, 34, 47 };
        foreach (int i in starIndices)
        {
            starCells[i] = true;
        }
        for (int i= 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new Cell(starCells[i]);
        }
    }
    public void Spawn(PuzzlePiece puzzlePiece)
    {
        int spawnIndex = GameManager.Instance.GetPlayerSpawnIndex();
        gameBoard[spawnIndex].AddPuzzlePiece(puzzlePiece);
        puzzlePiece.SetPosition(spawnIndex,0);
    }
    public void MovePuzzlePiece(PuzzlePiece puzzlePiece,int oldPosition,int newPosition,int incrementation)
    {
        Debug.Log("Moved " + puzzlePiece + " From " + oldPosition + " To " + newPosition);
        gameBoard[newPosition].AddPuzzlePiece(puzzlePiece);
        gameBoard[oldPosition].RemovePuzzlePiece();
        puzzlePiece.SetPosition(newPosition,incrementation);
    }
    public bool CanMove(PuzzlePiece puzzlePiece,int incrementValue,out int? newPosition)
    {
        newPosition = null;
        if (puzzlePiece.InBase())
            return false;
        Debug.Log("check move");
        if (puzzlePiece.IsWinningPath(incrementValue))
        {
            int fullPath = whiteCells - 2;
            int distanceToLastCell = fullPath - puzzlePiece.GetCurrentPathTaken();
            int incrementationLeft = incrementValue - distanceToLastCell;
            int playerWinningCellsCount = winningCells / GameManager.Instance.GetPlayersCount();
            if(incrementationLeft > playerWinningCellsCount)
            {
                Debug.Log("out of bounds move");
                return false;
            }
            else if(incrementationLeft > 0)
            {
                //teleportationCost
                incrementationLeft--;
                int winningCellIndex = GameManager.Instance.GetPlayerWinningIndex();
                newPosition = winningCellIndex + incrementationLeft;
                Debug.Log("To winning path");
                return true;
            }
        } 
        else if (puzzlePiece.InWinningPath())
        {
            int winningCellIndex = GameManager.Instance.GetPlayerWinningIndex();
            int distanceFromWinningIndex = puzzlePiece.GetCurrentPosition() - winningCellIndex;
            int playerWinningCellsCount = winningCells / GameManager.Instance.GetPlayersCount();
            if(distanceFromWinningIndex + incrementValue >= playerWinningCellsCount)
            {
                Debug.Log("out of bounds move");
                return false;
            }
            newPosition = puzzlePiece.GetCurrentPosition() + incrementValue;
            return true;
        }
        Debug.Log("normal move");
        newPosition = GetFixedPosition(puzzlePiece.GetCurrentPosition() + incrementValue);
        return true;
    }

    public int GetFixedPosition(int index)
    {
        return index % whiteCells;
    }
    public int GetWhiteCellsCount()
    {
        return whiteCells;
    }
}
