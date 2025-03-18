using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private static int whiteCells;
    private int winningCells;
    private int BoardSize => whiteCells + winningCells;
    private bool[] starCells;

    private Cell[] gameBoard;
    Dictionary<Team, PuzzlePiece[]> teamPuzzlePiecesDictionary;

    public Board(int winningCells)
    {
        whiteCells = 52;
        this.winningCells = winningCells;
        starCells = new bool[BoardSize];
        List<int> starIndices = new List<int> { 8, 21, 34, 47 };
        teamPuzzlePiecesDictionary = new Dictionary<Team, PuzzlePiece[]>();
        foreach (int i in starIndices)
        {
            starCells[i] = true;
        }

        gameBoard = new Cell[BoardSize];
        for (int i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new Cell(starCells[i]);
        }
    }
    public void SetGameBoard(Cell[] gameBoard)
    {
        this.gameBoard = gameBoard;
    }
    public void SetTeamPuzzlePiecesDictionary(Dictionary<Team, PuzzlePiece[]> teamPuzzlePiecesDictionary)
    {
        this.teamPuzzlePiecesDictionary= teamPuzzlePiecesDictionary;
    }
    public void AddTeamPuzzlePieces(Team team,PuzzlePiece[] puzzlePieces)
    {
        teamPuzzlePiecesDictionary.Add(team, puzzlePieces);
    }
    public PuzzlePiece[] GetPuzzlePiecesOf(Team team)
    {
        return teamPuzzlePiecesDictionary[team];
    }
    public bool IsGameOver()
    {
        int[] playerWinningIndices = GameManager.Instance.GetPlayersWinningIndices();
        int playersCount = GameManager.Instance.GetPlayersCount();
        foreach (int playerWinningIndex in playerWinningIndices)
        {
            int incToLastCellValue = (winningCells / playersCount) - 1;
            int playerLastCellIndex = playerWinningIndex + incToLastCellValue;
            if (gameBoard[playerLastCellIndex].GetPuzzlePieceCount() == 4)
                return true;
        }
        return false;
    }
    public int GetTeamPuzzlePieceBaseIndex(PuzzlePiece puzzlePiece)
    {
        Team puzzlePieceOwner = puzzlePiece.GetOwnerTeam();
        PuzzlePiece[] teamPuzzlePiece = teamPuzzlePiecesDictionary[puzzlePieceOwner];
        return Array.IndexOf(teamPuzzlePiece, puzzlePiece);
    }
    public bool TryGetPieceFromBase(Team team,out PuzzlePiece basePuzzlePiece)
    {
        PuzzlePiece[] teamPuzzlePieces = teamPuzzlePiecesDictionary[team];
        for(int i=teamPuzzlePieces.Length - 1;i>=0; i--)
        {
            if (teamPuzzlePieces[i].InBase())
            {
                basePuzzlePiece = teamPuzzlePieces[i];
                return true;
            }
        }
        basePuzzlePiece = null;
        return false;
    }
    public bool Spawn(PuzzlePiece puzzlePiece)
    {
        bool killed = false;
        int spawnIndex = GameManager.Instance.GetPlayerSpawnIndex();
        if (gameBoard[spawnIndex].HasOppositeTeam(puzzlePiece.GetOwnerTeam()))
        {
            killed = true;
            Return(gameBoard[spawnIndex].GetPuzzlePieces());
            gameBoard[spawnIndex].ClearPuzzlePieces();
        }
        gameBoard[spawnIndex].AddPuzzlePiece(puzzlePiece);
        puzzlePiece.SetPosition(spawnIndex, 0);
        return killed;
    }
    public void Return(List<PuzzlePiece> puzzlePieces)
    {
        foreach (PuzzlePiece puzzlePiece in puzzlePieces)
        {
            gameBoard[puzzlePiece.GetCurrentPosition()].RemovePuzzlePiece(puzzlePiece);
            puzzlePiece.SetPosition(-1, 0);
        }
    }
    public bool MovePuzzlePiece(PuzzlePiece puzzlePiece, int oldPosition, int newPosition, int incrementation)
    {
        bool killed = false;
        if (gameBoard[newPosition].HasOppositeTeam(puzzlePiece.GetOwnerTeam()) && !gameBoard[newPosition].IsStar())
        {
            killed = true;
            Return(gameBoard[newPosition].GetPuzzlePieces());
            gameBoard[newPosition].ClearPuzzlePieces();
        }
        gameBoard[newPosition].AddPuzzlePiece(puzzlePiece);
        gameBoard[oldPosition].RemovePuzzlePiece(puzzlePiece);
        puzzlePiece.SetPosition(newPosition, incrementation);
        return killed;
    }
    public bool CanMove(PuzzlePiece puzzlePiece, int incrementValue, out int? newPosition)
    {
        newPosition = null;
        if (puzzlePiece.InBase())
            return false;
        if (puzzlePiece.IsWinningPath(incrementValue))
        {
            int fullPath = whiteCells - 2;
            int distanceToLastCell = fullPath - puzzlePiece.GetCurrentPathTaken();
            int incrementationLeft = incrementValue - distanceToLastCell;
            int playerWinningCellsCount = winningCells / GameManager.Instance.GetPlayersCount();
            //checking if there's a wall ahead, we passed the puzzle piece team because the wall is 1< pieces of opposite team
            /*the incrementation in this case is distanceToLastCell - 1 because 
             * the last cell to move at isn't considered blocked even if it's a wall*/
            if (IsBlocked(puzzlePiece.GetOwnerTeam(), oldPosition: puzzlePiece.GetCurrentPosition(), incrementation: distanceToLastCell - 1))
            {
                return false;
            }
            if (incrementationLeft > playerWinningCellsCount)
            {
                return false;
            }
            if (incrementationLeft > 0)
            {
                //teleportationCost
                incrementationLeft--;
                int winningCellIndex = GameManager.Instance.GetPlayerWinningIndex();
                newPosition = winningCellIndex + incrementationLeft;
                return true;
            }
        }
        else if (puzzlePiece.InWinningPath())
        {
            int winningCellIndex = GameManager.Instance.GetPlayerWinningIndex();
            int distanceFromWinningIndex = puzzlePiece.GetCurrentPosition() - winningCellIndex;
            int playerWinningCellsCount = winningCells / GameManager.Instance.GetPlayersCount();
            if (distanceFromWinningIndex + incrementValue >= playerWinningCellsCount)
            {
                return false;
            }
            newPosition = puzzlePiece.GetCurrentPosition() + incrementValue;
            return true;
        }
        //normal path
        if (IsBlocked(puzzlePiece.GetOwnerTeam(), puzzlePiece.GetCurrentPosition(), incrementValue - 1))
        {
            return false;
        }
        newPosition = GetFixedPosition(puzzlePiece.GetCurrentPosition() + incrementValue);
        return true;
    }
    public bool IsBlocked(Team puzzleBlockTeam, int oldPosition, int incrementation)
    {
        for (int i = oldPosition, j = incrementation; j > 0; j--)
        {
            i++;
            i = GetFixedPosition(i);
            if (gameBoard[i].IsWall(puzzleBlockTeam))
                return true;
        }
        return false;
    }
    public static int GetFixedPosition(int index)
    {
        return index % whiteCells;
    }
    public static int GetWhiteCellsCount()
    {
        return whiteCells;
    }
    public Board GetCopy()
    {
        Board board = new Board(winningCells);
        Cell[] gameBoardCopy = new Cell[gameBoard.Length];
        Dictionary<Team, PuzzlePiece[]> teamPuzzlePiecesDictionaryCopy = new Dictionary<Team, PuzzlePiece[]>();
        for(int i = 0; i < gameBoard.Length; i++)
        {
            //copy empty cells
            gameBoardCopy[i] = gameBoard[i].GetCopy();
        }
        //copy puzzle piece references
        foreach (var entry in teamPuzzlePiecesDictionary)
        {
            //copy puzzle piece array
            PuzzlePiece[] puzzlePiecesCopy = new PuzzlePiece[4];
            for(int i = 0;i < entry.Value.Length; i++)
            {
                //entry.value is PuzzlePiece[] array
                //we make a copy of each puzzle piece object
                puzzlePiecesCopy[i] = entry.Value[i].GetCopy();
                //re-initialize the game board with the puzzle piece copy
                if (!puzzlePiecesCopy[i].InBase())
                    gameBoardCopy[puzzlePiecesCopy[i].GetCurrentPosition()].AddPuzzlePiece(puzzlePiecesCopy[i]);
            }
            //copy a team and its puzzle pieces to the dictionary
            teamPuzzlePiecesDictionaryCopy.Add(entry.Key, puzzlePiecesCopy);
        }
        board.SetGameBoard(gameBoardCopy);
        board.SetTeamPuzzlePiecesDictionary(teamPuzzlePiecesDictionaryCopy);
        return board;
    }
}
