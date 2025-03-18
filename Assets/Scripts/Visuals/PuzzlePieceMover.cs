using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzlePieceMover : MonoBehaviour
{
    private enum PuzzlePieceState
    {
        MOVING,COOLDOWN,IDLE
    }
    private PuzzlePieceState currentPieceState;
    private readonly float cooldownTimer = 0.5f;
    private float currentCooldownTimer = 0.5f;
    private readonly float moveSpeed = 20f;
    private List<Vector3> waypointPath;
    private Vector3 targetPosition;
    private int currentWaypointIndex = 0;
    private void Start()
    {
        GetComponent<PuzzlePieceTransform>().OnPuzzlePieceMoved += PuzzlePieceMover_OnMove;
        currentPieceState = PuzzlePieceState.IDLE;

    }
    private void PuzzlePieceMover_OnMove(int oldPosition, int newPosition,int incrementation)
    {
        waypointPath = new List<Vector3>();
        currentWaypointIndex = 0;
        if (oldPosition == -1) //from base
        {
            waypointPath.Add(BoardTransformHolder.Instance.GetWorldPosition(newPosition));
        }
        else if(newPosition == -1)//to base
        {
            waypointPath.Add(PlayerBasesTransformHolder.Instance.GetBaseWorldPosition(GetComponent<PuzzlePieceTransform>().GetPuzzlePiece()));
        }
        else if(newPosition < Board.GetWhiteCellsCount()) //normal path
        {
            for(int i=oldPosition;i<oldPosition + incrementation; i++)
            {
                int nextPositionFixed = Board.GetFixedPosition(i+1);
                waypointPath.Add(BoardTransformHolder.Instance.GetWorldPosition(nextPositionFixed));
            }
        }
        else if(oldPosition > Board.GetWhiteCellsCount()) // inside winning path
        {
            for(int i=oldPosition; i<oldPosition + incrementation; i++)
            {

                waypointPath.Add(BoardTransformHolder.Instance.GetWorldPosition(i+1));
            }
        }
        else //winning path
        {
            int winningPathCount = newPosition - (GameManager.Instance.GetPlayerWinningIndex() - 1);
            int normalPathCount = incrementation - winningPathCount;
            for (int i = oldPosition, count = 0; count < normalPathCount; i++,count++) 
            {
                waypointPath.Add(BoardTransformHolder.Instance.GetWorldPosition(i + 1));
            }
            for(int i=GameManager.Instance.GetPlayerWinningIndex() - 1; i < newPosition; i++)
            {
                waypointPath.Add(BoardTransformHolder.Instance.GetWorldPosition(i + 1));
            }
        }
        if (waypointPath.Any())
        {
            targetPosition = waypointPath[0];
        }
        currentPieceState = PuzzlePieceState.MOVING;
    }
    private void Update()
    {
        switch (currentPieceState)
        {
            case PuzzlePieceState.MOVING:
                Move();
                break;
            case PuzzlePieceState.COOLDOWN:
                currentCooldownTimer -= Time.deltaTime;
                if (currentCooldownTimer < 0f)
                {
                    currentCooldownTimer = cooldownTimer;
                }
                currentPieceState = PuzzlePieceState.MOVING;
                break;
            case PuzzlePieceState.IDLE:
                break;
        }
    }
    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed*Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.005f)
        {
            currentPieceState = PuzzlePieceState.COOLDOWN;
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypointPath.Count)
            {
                currentPieceState = PuzzlePieceState.IDLE;
                return;
            }
            targetPosition = waypointPath[currentWaypointIndex];
        }
    }

}
