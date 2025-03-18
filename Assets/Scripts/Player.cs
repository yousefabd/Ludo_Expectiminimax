using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    private readonly int spawnIndex;
    private readonly int winningIndex;
    private readonly Team team;
    //private PlayerBase playerBase;
    private Board board;

    private PlayerState currentPlayerState;
    private List<int> movesList;
    private PuzzlePiece selectedPuzzlePiece;

    public event Action<int> OnDiceRolled;
    public event Action<List<int>> OnFinishedRollingDice;
    public event Action<List<int>, int> OnChoseNumber;
    public event Action OnFinishedMoving;
    public event Action OnPassTurn;
    public event Action OnKilled;

    int testIndex = 0;
    int[] test = new int[3] { 6, 6, 1 };
    public Player(Board board, Team team, int spawnIndex, int winningIndex)
    {
        movesList = new List<int>();
        currentPlayerState = PlayerState.RollingDice;
        this.board = board;
        this.spawnIndex = spawnIndex;
        this.winningIndex = winningIndex;
        this.team = team;
    }

    public void ClearPuzzlePiece()
    {
        selectedPuzzlePiece = null;
    }
    public void Ready()
    {
        currentPlayerState = PlayerState.RollingDice;
        testIndex = 0;
    }
    public void SelectPuzzlePiece(PuzzlePiece selectedPuzzlePiece)
    {
        if (selectedPuzzlePiece.GetOwnerTeam().Equals(this.team))
        {
            this.selectedPuzzlePiece = selectedPuzzlePiece;
        }
    }
    private int RollDice()
    {
        return UnityEngine.Random.Range(0, 6) + 1;
        //return test[testIndex++];
    }
    public int ChooseMove(int move = -1)
    {
        if (move < 0)
        {
            move = RollDice();
        }
        OnDiceRolled?.Invoke(move);
        movesList.Add(move);
        if (move < 6 || movesList.Count > 2)
        {
            currentPlayerState = PlayerState.Moving;
            OnFinishedRollingDice?.Invoke(movesList);
            if (WillPass())
            {
                OnPassTurn?.Invoke();
                movesList.Clear();
                currentPlayerState = PlayerState.Finished;
            }
        }
        return move;

    }
    public bool CanMove(PuzzlePiece puzzlePiece,int incrementValue,out int? newPuzzlePiecePosition)
    {
        newPuzzlePiecePosition = null;
        if (currentPlayerState != PlayerState.Moving)
            return false;
        if (!movesList.Contains(incrementValue) || puzzlePiece == null)
            return false;
        if (board.CanMove(puzzlePiece, incrementValue, out newPuzzlePiecePosition))
        {
            return true;
        }
        return false;

    }
    public bool CanSpawn(out PuzzlePiece basePuzzlePiece)
    {
        return (board.TryGetPieceFromBase(team,out basePuzzlePiece)) && movesList.Contains(6) && currentPlayerState == PlayerState.Moving;
    }
    public bool WillPass()
    {
        PuzzlePiece[] playerPuzzlePieces = board.GetPuzzlePiecesOf(team);
        //check if all moves are 6
        bool all_six = false;
        if (movesList.Count == 3)
        {
            if (movesList[0] == movesList[1] && movesList[1] == movesList[2] && movesList[2] == 6)
            {

                all_six = true;
            }
        }
        //check if we can spawn
        if (CanSpawn(out _) && !all_six)
            return false;
        //check if we can move
        foreach(var puzzlePiece in playerPuzzlePieces)
        {
            foreach(int move in movesList)
            {
                if (CanMove(puzzlePiece, move, out _) && !all_six)
                    return false;
            }
        }
        return true;
    }
    public bool Move(int incrementValue)
    {
        if (CanMove(selectedPuzzlePiece, incrementValue, out int? movePosition))
        {
            bool killed = board.MovePuzzlePiece(selectedPuzzlePiece, selectedPuzzlePiece.GetCurrentPosition(), (int)movePosition ,incrementValue);
            movesList.Remove(incrementValue);
            OnChoseNumber?.Invoke(movesList, incrementValue);
            if (killed)
            {
                OnKilled?.Invoke();
            }
            else if (!movesList.Any())
            {
                currentPlayerState = PlayerState.Finished;
                OnFinishedMoving?.Invoke();
            }
            else if (WillPass())
            {
                OnPassTurn?.Invoke();
                movesList.Clear();
                currentPlayerState = PlayerState.Finished;
            }
            return true;
        }
        return false;
    }
    public void Spawn()
    {
        if (CanSpawn(out PuzzlePiece puzzlePiece))
        {
            bool killed = board.Spawn(puzzlePiece);
            movesList.Remove(6);
            OnChoseNumber?.Invoke(movesList, 6);
            if (killed)
            {
                OnKilled?.Invoke();
            }
            else if (!movesList.Any())
            {
                OnFinishedMoving?.Invoke();
                currentPlayerState = PlayerState.Finished;
            }
            else if (WillPass())
            {
                OnPassTurn?.Invoke();
                movesList.Clear();
                currentPlayerState = PlayerState.Finished;
            }
        }
    }
    public int GetSpawnIndex()
    {
        return spawnIndex;
    }
    public int GetWinningIndex()
    {
        return winningIndex;
    }
    public Team GetTeam()
    {
        return team;
    }

    public PlayerState GetPlayerState()
    {
        return currentPlayerState;
    }

    public List<int> GetMovesList()
    {
        return movesList;
    }
    public void SetMovesList(List<int> movesList)
    {
        foreach(int move in movesList) {
            this.movesList.Add(move);
        }
    }
    public void SetPLayerState(PlayerState playerState)
    {
        this.currentPlayerState = playerState;
    }
    public int GetScore()
    {
        PuzzlePiece[] playerPuzzlePieces = board.GetPuzzlePiecesOf(team);
        int score = 0;
        foreach (var piece in playerPuzzlePieces)
        {
            score += piece.GetCurrentPathTaken();
            if (!piece.InBase())
            {
                score += 200;
            }
        }
        return score;
    }
}
