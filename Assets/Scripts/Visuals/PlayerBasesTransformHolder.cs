using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasesTransformHolder : MonoBehaviour
{
    public static PlayerBasesTransformHolder Instance {  get; private set; }    
    [SerializeField] private List<PlayerBase> playerBasesTransformHolderList;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetBaseWorldPosition(PuzzlePiece puzzlePiece)
    {
        foreach(PlayerBase h in playerBasesTransformHolderList)
        {
            if (h.GetPlayerBaseTeam().Equals(puzzlePiece.GetOwnerTeam()))
            {
                return h.GetBasePosition(puzzlePiece);
            }
        }
        return Vector3.zero;
    }
}
