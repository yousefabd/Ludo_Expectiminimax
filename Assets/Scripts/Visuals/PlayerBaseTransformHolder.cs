using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseTransformHolder : MonoBehaviour
{
    [SerializeField] private Transform puzzlePiecePrefab;
    private PlayerBase playerBase;
    private void Awake()
    {
        playerBase = GetComponent<PlayerBase>();
        playerBase.OnInit += PlayerBase_OnInit;
    }

    private void PlayerBase_OnInit()
    {
        playerBase.SetPuzzlePieces(CreateBase());
    }

    private PuzzlePiece[] CreateBase()
    {
        PuzzlePiece[] pieces = new PuzzlePiece[4];
        int i = 0;
        foreach(Transform pos in transform) 
        { 
            pieces[i] = Instantiate(puzzlePiecePrefab,pos.position,Quaternion.identity).GetComponent<PuzzlePiece>();
            i++;
        }
        return pieces;
    }
}
