using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class Cell
{
    private List<PuzzlePiece> puzzlePieces;
    private bool isStar;
    public Cell(bool isStar)
    {
        puzzlePieces = new List<PuzzlePiece>();
        this.isStar = isStar;
    }
    public void AddPuzzlePiece(PuzzlePiece piece)
    {
        puzzlePieces.Add(piece);
    }
    public void RemovePuzzlePiece(PuzzlePiece puzzlePiece)
    {
        puzzlePieces.Remove(puzzlePiece);
    }
    public bool Empty()
    {
        return puzzlePieces.Count == 0;
    }
    public bool HasOppositeTeam(Team team)
    {
        if (!Empty())
        {
            if (!puzzlePieces[0].GetOwnerTeam().Equals(team))
            {
                return true;
            } 
        }
        return false;
    }
    public bool IsWall(Team team)
    {
        if (puzzlePieces.Count >= 2)
        {
            if (!puzzlePieces[0].GetOwnerTeam().Equals(team))
            {
                return true;
            }

        }
        return false;
    }
    public bool IsStar()
    {
        return isStar;
    }
    public int GetPuzzlePieceCount()
    {
        return puzzlePieces.Count;
    }
    public List<PuzzlePiece> GetPuzzlePieces()
    {
        return puzzlePieces.ToList();
    }
    public PuzzlePiece Any()
    {
        if (puzzlePieces.Count > 0)
        {
            return puzzlePieces[0]; 
        }
        return null;
    }
    public void ClearPuzzlePieces()
    {
        puzzlePieces.Clear();
    }
    public Cell GetCopy()
    {
        Cell cell = new Cell(isStar);
        return cell;
    }
}
