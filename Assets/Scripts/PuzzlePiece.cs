using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzlePiece
{
    private readonly int basePosition = -1;
    private int currentPathTaken = 0;
    private int currentPosition;
    private Team owner;
    public event Action<int,int,int> OnMove;
    public event Action<int> OnSetCount;
    public PuzzlePiece(Team owner)
    {
        currentPosition = basePosition;
        this.owner = owner;
    }
    public int GetPosition()
    {
        return Board.GetFixedPosition(currentPosition);
    }
    public void SetPosition(int newPosition,int incrementation)
    {
        OnMove?.Invoke(currentPosition, newPosition, incrementation);
        currentPosition = newPosition;
        currentPathTaken += incrementation;
        if(newPosition == basePosition) 
        {
            currentPathTaken = 0;
        }
    }
    public void SetPathTaken(int currentPathTaken)
    {
        this.currentPathTaken = currentPathTaken;
    }
    public void SetCount(int count)
    {
        OnSetCount?.Invoke(count);
    }
    public bool InBase()
    {
        return currentPosition == basePosition;
    }
    public bool InWinningPath()
    {
        return currentPathTaken > (Board.GetWhiteCellsCount() - 2);
    }
    public bool IsWinningPath(int incrementation)
    {
        return (currentPathTaken + incrementation - (Board.GetWhiteCellsCount() - 2) > 0) && !InWinningPath();
    }
    public int GetCurrentPosition()
    {
        return currentPosition; 
    }
    public int GetCurrentPathTaken()
    {
        return currentPathTaken;
    }
    public Team GetOwnerTeam()
    {
        return owner;
    }
    public PuzzlePiece GetCopy()
    {
        PuzzlePiece puzzlePiece = new PuzzlePiece(owner);
        puzzlePiece.SetPosition(currentPosition,0);
        puzzlePiece.SetPathTaken(currentPathTaken);
        return puzzlePiece;
    }
}
