using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzlePiece : MonoBehaviour
{
    private readonly int basePosition = -1;
    private int currentPathTaken = 0;
    private int currentPosition;
    [SerializeField] private Team owner;
    public event Action<int,int,int> OnMove;
    public event Action<int> OnSetCount;
    private void Awake()
    {
        currentPosition = basePosition;
    }
    public int GetPosition()
    {
        return Board.Instance.GetFixedPosition(currentPosition);
    }
    public void SetPosition(int newPosition,int incrementation)
    {
        OnMove?.Invoke(currentPosition, newPosition, incrementation);
        currentPosition = newPosition;
        currentPathTaken += incrementation;
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
        return currentPathTaken >= (Board.Instance.GetWhiteCellsCount() - 2);
    }
    public bool IsWinningPath(int incrementation)
    {
        return (currentPathTaken + incrementation - (Board.Instance.GetWhiteCellsCount() - 2) > 0) && !InWinningPath();
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
}
