using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    private PuzzlePiece[] puzzlePieces;
    private int currentPiecesInBase;

    public event Action OnInit;
    private void Start()
    {
        OnInit?.Invoke();
    }
    public void SetPuzzlePieces(PuzzlePiece[] puzzlePieces)
    {
        this.puzzlePieces= puzzlePieces;
        currentPiecesInBase = puzzlePieces.Length;
    }
    public PuzzlePiece[] GetPuzzlePieces() 
    { 
        return puzzlePieces;
    }
    public void SpawnPuzzlePiece()
    {
        if (currentPiecesInBase == 0)
            return;
        currentPiecesInBase--;
        Board.Instance.Spawn(puzzlePieces[currentPiecesInBase]);
    }
    public bool Empty()
    {
        return currentPiecesInBase == 0;
    }
}
