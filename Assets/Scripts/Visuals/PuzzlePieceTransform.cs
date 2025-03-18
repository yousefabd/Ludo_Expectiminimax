using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceTransform : MonoBehaviour
{
    [SerializeField] private Team team;
    private PuzzlePiece puzzlePiece;
    public event Action<int, int, int> OnPuzzlePieceMoved;
    private void Awake()
    {
        puzzlePiece = new PuzzlePiece(team);
        puzzlePiece.OnMove += PuzzlePiece_OnMove;
    }
    private void PuzzlePiece_OnMove(int oldPosition, int newPosition, int incrementation)
    {
        OnPuzzlePieceMoved?.Invoke(oldPosition, newPosition, incrementation);
    }

    public PuzzlePiece GetPuzzlePiece()
    {
        return puzzlePiece;
    }
}
