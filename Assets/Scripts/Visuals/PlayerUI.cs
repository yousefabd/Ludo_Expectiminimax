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
        GameManager.Instance.OnPlayerFinishedRollingDice += GameManager_OnPlayerFinishedRollingDice;
        GameManager.Instance.OnPlayerFinishedMoving += GameManager_OnPlayerFinishedMoving;
        GameManager.Instance.OnPlayerPassedTurn += GameManager_OnPlayerPassedTurn;
        rollDiceButton.onClick.AddListener(() => {
            OnRollDiceClicked?.Invoke();
        });
    }

    private void GameManager_OnPlayerPassedTurn()
    {
        StartCoroutine(PassAfterCooldown());
    }

    private void GameManager_OnPlayerFinishedMoving()
    {
        rollDiceButton.interactable = true;
    }

    private void GameManager_OnPlayerFinishedRollingDice(List<int> obj)
    {
        rollDiceButton.interactable = false;
    }

    private void MouseInputManager_OnPuzzlePieceSelected(PuzzlePiece puzzlePiece)
    {
        OnSelectPuzzlePiece?.Invoke(puzzlePiece.GetComponent<PuzzlePiece>());
    }
    private void MouseInputManager_OnPuzzlePieceSpawned(PuzzlePiece puzzlePiece)
    {
        OnSpawnPuzzlePiece?.Invoke(puzzlePiece);
    }

    private void MouseInputManager_OnClearPuzzlePiece()
    {
        OnClearPuzzlePiece?.Invoke();
    }

    private IEnumerator PassAfterCooldown()
    {
        yield return new WaitForSeconds(1f);
        rollDiceButton.interactable = true;
    }
}
