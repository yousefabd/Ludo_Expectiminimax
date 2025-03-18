using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovesListUI : MonoBehaviour
{
    Transform moveTemplate;
    List<Transform> movesList;

    private void Awake()
    {
        movesList = new List<Transform>();
    }
    private void Start()
    {
        moveTemplate = transform.Find("MoveTemplate");
        moveTemplate.gameObject.SetActive(false);
        GameManager.Instance.OnPlayerRolledDice += GameManager_OnPlayerRolledDice;
        GameManager.Instance.OnPlayerChoseNumber += GameManager_OnPlayerChoseNumber;
        GameManager.Instance.OnPlayerFinishedMoving += GameManager_OnPlayerFinishedMoving;
        GameManager.Instance.OnPlayerPassedTurn += GameManager_OnPlayerPassedTurn;
    }

    private void GameManager_OnPlayerPassedTurn()
    {
        StartCoroutine(ClearMovesListAfterCooldown());
    }

    private void GameManager_OnPlayerFinishedMoving()
    {
        ClearMovesList();
    }

    private void GameManager_OnPlayerChoseNumber(List<int> arg1,int chosenNumber)
    {
        foreach(Transform moveTransform in movesList)
        {
            int number = int.Parse(moveTransform.Find("moveValue").GetComponent<TextMeshProUGUI>().text);
            if(number == chosenNumber && moveTransform.gameObject.activeSelf)
            {
                moveTransform.gameObject.SetActive(false);
                break;
            }
        }
    }

    private void GameManager_OnPlayerRolledDice(int move)
    {
        Transform moveTransform = Instantiate(moveTemplate,this.transform);
        moveTransform.gameObject.SetActive(true);
        moveTransform.Find("moveValue").GetComponent<TextMeshProUGUI>().text = move.ToString();
        movesList.Add(moveTransform);
    }
    private IEnumerator ClearMovesListAfterCooldown()
    {
        yield return new WaitForSeconds(1f);
        ClearMovesList();
    }
    private void ClearMovesList()
    {
        foreach(Transform moveTransform in movesList)
        {
            Destroy(moveTransform.gameObject);
        }
        movesList.Clear();
    }
}
