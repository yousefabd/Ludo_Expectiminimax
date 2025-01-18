using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour
{
    public static MouseInputManager Instance { get; private set; }
    public event Action<PuzzlePiece> OnPuzzlePieceSelected;
    public event Action<PuzzlePiece> OnPuzzlePieceSpawned;
    public event Action OnClearPuzzlePiece;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { }
            else
            {
                Collider2D puzzlePieceCollider = Physics2D.OverlapPoint(UtilsClass.GetMouseWorldPosition());
                if (puzzlePieceCollider != null)
                {
                    PuzzlePiece puzzlePiece = puzzlePieceCollider.GetComponent<PuzzlePiece>(); 
                    if(!puzzlePiece.InBase())
                        OnPuzzlePieceSelected?.Invoke(puzzlePiece);
                    else
                    {
                        OnPuzzlePieceSpawned?.Invoke(puzzlePiece);
                    }
                }
                else
                {
                    OnClearPuzzlePiece?.Invoke();
                }
            }
        }
    }
}
