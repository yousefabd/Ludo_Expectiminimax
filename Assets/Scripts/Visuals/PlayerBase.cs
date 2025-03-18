using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private Transform puzzlePiecePrefab;
    [SerializeField] private Team team;
    private void Start()
    {
        CreateBase();
    }

    private void CreateBase()
    {
        PuzzlePiece[] pieces = new PuzzlePiece[4];
        int i = 0;
        foreach(Transform pos in transform) 
        { 
            pieces[i] = Instantiate(puzzlePiecePrefab,pos.position,Quaternion.identity).GetComponent<PuzzlePieceTransform>().GetPuzzlePiece();
            i++;
        }
        BoardManager.Instance.AddTeamPuzzlePieces(team, pieces);
    }
    public Team GetPlayerBaseTeam()
    {
        return team;  
    }
    public Vector3 GetBasePosition(PuzzlePiece puzzlePiece)
    {
        int puzzlePieceIndex = BoardManager.Instance.GetPuzzlePieceIndex(puzzlePiece) + 1;
        return transform.Find("Position" + puzzlePieceIndex).position;
    }

}
