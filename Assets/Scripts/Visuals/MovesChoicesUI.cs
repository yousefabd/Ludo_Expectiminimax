using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MovesChoicesUI : MonoBehaviour
{
    public static MovesChoicesUI Instance {  get; private set; }
    private RectTransform rectTransform;
    private List<int> movesList;
    private Transform moveChoiceTemplate;
    private List<Transform> currentMovesTransformList;

    public event Action<int> OnChooseMove;
    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        currentMovesTransformList = new List<Transform>();
        movesList = new List<int>();
    }
    private void Start()
    {
        moveChoiceTemplate = transform.Find("MoveChoice");
        for (int i = 0; i < 6; i++) 
        {
            Transform moveChoiceTransform = Instantiate(moveChoiceTemplate, transform);
            moveChoiceTransform.Find("number").GetComponent<TextMeshPro>().text = (i+1).ToString();
            int number = i + 1;
            moveChoiceTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnChooseMove?.Invoke(number);
                Hide();
            });
            currentMovesTransformList.Add(moveChoiceTransform);
        }
        MouseInputManager.Instance.OnPuzzlePieceSelected += MouseInputManager_OnPuzzlePieceSelected;
        MouseInputManager.Instance.OnClearPuzzlePiece += MouseInputManager_OnClearPuzzlePiece;
        GameManager.Instance.OnPlayerFinishedRollingDice += GameManager_OnPlayerFinishedRollingDice;
        GameManager.Instance.OnPlayerChoseNumber += GameManager_OnPlayerChoseNumber; ;
        Hide();
        moveChoiceTemplate.gameObject.SetActive(false);
    }

    private void GameManager_OnPlayerChoseNumber(List<int> moves, int arg2)
    {
        movesList = moves;
    }

    private void MouseInputManager_OnClearPuzzlePiece()
    {
        Hide();
    }

    private void MouseInputManager_OnPuzzlePieceSelected(PuzzlePiece puzzlePiece,Vector3 puzzlePiecePosition)
    {
        float offsetY = 1.5f;
        rectTransform.position = new Vector3(puzzlePiecePosition.x,puzzlePiecePosition.y + offsetY);
        ShowMovesTransformList();
        Show();
        if (IsForcedMove(out int uniqueMove))
        {
            OnChooseMove?.Invoke(uniqueMove);
            Hide();
        }
    }
    private void GameManager_OnPlayerFinishedRollingDice(List<int> moves)
    {
        movesList = moves;
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void ShowMovesTransformList()
    {
        ResetMovesTransformList();
        foreach(int number in movesList)
        {
            currentMovesTransformList[number-1].gameObject.SetActive(true);
        }
    }
    private bool IsForcedMove(out int uniqueMove)
    {
        uniqueMove = 0;
        int uniqueMovesCount = 0;
        foreach(int number in movesList)
        {
            if (currentMovesTransformList[number - 1].gameObject.activeSelf)
            {
                uniqueMovesCount++;
                uniqueMove = number;
            }
        }
        return uniqueMovesCount == 1;
    }
    private void ResetMovesTransformList()
    {
        foreach(Transform moveChoiceTransform in currentMovesTransformList)
        {
            moveChoiceTransform.gameObject.SetActive(false);
        }
    }
}
