using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceTesting : MonoBehaviour
{
    public static PuzzlePieceTesting Instance { get; private set; }
    public PuzzlePieceTransform pTransform;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
    }
}
