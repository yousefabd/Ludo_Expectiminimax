using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour
{
    public static MouseInputManager Instance { get; private set; }
    public event Action<PuzzlePiece,Vector3> OnPuzzlePieceSelected;
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
                //get puzzle piece/s in pointer clicking position
                Collider2D[] puzzlePieceColliderArray = Physics2D.OverlapPointAll(UtilsClass.GetMouseWorldPosition());
                //filter the array to make sure to select only a puzzle piece of the current player's team
                PuzzlePieceTransform puzzlePieceTransform = null;
                foreach(Collider2D collider2D in puzzlePieceColliderArray)
                {
                    if(collider2D.TryGetComponent(out PuzzlePieceTransform p))
                    {
                        if (p.GetPuzzlePiece().GetOwnerTeam().Equals(GameManager.Instance.GetCurrentPlayerTeam()))
                        {
                            puzzlePieceTransform = p;
                            break;
                        }
                    }
                }
                if (puzzlePieceTransform != null)
                {
                    PuzzlePiece puzzlePiece = puzzlePieceTransform.GetPuzzlePiece();
                    if(!puzzlePiece.InBase())
                        OnPuzzlePieceSelected?.Invoke(puzzlePiece,puzzlePieceTransform.transform.position);
                    else
                        OnPuzzlePieceSpawned?.Invoke(puzzlePiece);
                }
                else
                {
                    OnClearPuzzlePiece?.Invoke();
                }
            }
        }
    }
}
