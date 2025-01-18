using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardTransformHolder : MonoBehaviour
{
    public static BoardTransformHolder Instance { get; private set; }
    private List<Transform> cellWorldPositions;
    private List<Transform> playerCellWorldPositions;
    private void Awake()
    {
        Instance = this;
        cellWorldPositions = new List<Transform>();
        playerCellWorldPositions = new List<Transform>();
    }
    private void Start()
    {
        Transform cellsList = transform.Find("cells");
        foreach (Transform cellTransform in cellsList)
        {
            cellWorldPositions.Add(cellTransform);
        }
    }
    public Vector3 GetWorldPosition(int boardPosition)
    {
        if (boardPosition >= cellWorldPositions.Count)
        {
            return Vector3.zero;
        }
        return cellWorldPositions[boardPosition].position;
    }
}
