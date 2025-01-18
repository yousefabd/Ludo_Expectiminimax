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
    private PlayerBase playerBase;

    private PlayerState currentPlayerState;
    private List<int> movesList;
    private PuzzlePiece selectedPuzzlePiece;

    public event Action<int> OnDiceRolled;
    public event Action<List<int>> OnFinishedRollingDice;
    public event Action<List<int>,int> OnChoseNumber;
    public event Action OnFinishedMoving;
    public event Action OnPassTurn;
    public Player(PlayerBase playerBase, Team team, int spawnIndex, int winningIndex)
    {
        movesList = new List<int>();
        currentPlayerState = PlayerState.RollingDice;
        this.spawnIndex = spawnIndex;
        this.winningIndex = winningIndex;
        this.team = team;
        this.playerBase = playerBase;
    }

    public void ClearPuzzlePiece()
    {
        selectedPuzzlePiece = null;
    }
    public void Ready()
    {
        currentPlayerState = PlayerState.RollingDice;
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
        if (Board.Instance.CanMove(puzzlePiece, incrementValue, out newPuzzlePiecePosition))
        {
            return true;
        }
        return false;

    }
    public bool CanSpawn()
    {
        return (!playerBase.Empty()) && movesList.Contains(6) && currentPlayerState == PlayerState.Moving;
    }
    public bool WillPass()
    {
        PuzzlePiece[] playerPuzzlePieces = playerBase.GetPuzzlePieces();
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
        if (CanSpawn() && !all_six)
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
    public void Move(int incrementValue)
    {
        if (CanMove(selectedPuzzlePiece, incrementValue, out int? movePosition))
        {
            Board.Instance.MovePuzzlePiece(selectedPuzzlePiece, selectedPuzzlePiece.GetCurrentPosition(), (int)movePosition ,incrementValue);
            movesList.Remove(incrementValue);
            OnChoseNumber?.Invoke(movesList, incrementValue);
            if (!movesList.Any())
            {
                OnFinishedMoving?.Invoke();
            }
            else if (WillPass())
            {
                OnPassTurn?.Invoke();
                movesList.Clear();
                currentPlayerState = PlayerState.Finished;
            }
        }
    }
    public void Spawn()
    {
        if (CanSpawn())
        {
            playerBase.SpawnPuzzlePiece();
            movesList.Remove(6);
            OnChoseNumber?.Invoke(movesList, 6);
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
}
