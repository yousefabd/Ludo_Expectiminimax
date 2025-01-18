using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void RemovePuzzlePiece()
    {
        if(puzzlePieces.Count > 0)
            puzzlePieces.RemoveAt(0);
    }
    public bool Empty()
    {
        return puzzlePieces.Count == 0;
    }
    public int GetPuzzlePieceCount()
    {
        return puzzlePieces.Count;
    }
    public PuzzlePiece Any()
    {
        if (puzzlePieces.Count > 0)
        {
            return puzzlePieces[0]; 
        }
        return null;
    }
}
