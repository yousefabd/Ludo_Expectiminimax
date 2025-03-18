using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button rollDiceButton;
    public static PlayerUI Instance { get; private set; }
    public event Action<PuzzlePiece> OnSelectPuzzlePiece;
    public event Action OnClearPuzzlePiece;
    public event Action OnRollDiceClicked;
    public event Action<PuzzlePiece> OnSpawnPuzzlePiece;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        MouseInputManager.Instance.OnPuzzlePieceSelected += MouseInputManager_OnPuzzlePieceSelected;
        MouseInputManager.Instance.OnPuzzlePieceSpawned += MouseInputManager_OnPuzzlePieceSpawned;
        MouseInputManager.Instance.OnClearPuzzlePiece += MouseInputManager_OnClearPuzzlePiece;
        rollDiceButton.onClick.AddListener(() => {
            OnRollDiceClicked?.Invoke();
        });
    }
    private void MouseInputManager_OnPuzzlePieceSelected(PuzzlePiece puzzlePiece,Vector3 arg2)
    {
        OnSelectPuzzlePiece?.Invoke(puzzlePiece);
    }
    private void MouseInputManager_OnPuzzlePieceSpawned(PuzzlePiece puzzlePiece)
    {
        OnSpawnPuzzlePiece?.Invoke(puzzlePiece);
    }

    private void MouseInputManager_OnClearPuzzlePiece()
    {
        OnClearPuzzlePiece?.Invoke();
    }
    private void Update()
    {
        bool IsPlayerTurn = GameManager.Instance.GetGame().GetCurrentPlayerTeam().Equals(Team.Red);
        bool IsRollingDice = GameManager.Instance.GetGame().GetCurrentPlayer().GetPlayerState().Equals(PlayerState.RollingDice);
        rollDiceButton.interactable = IsRollingDice && IsPlayerTurn;
        
    }
}
