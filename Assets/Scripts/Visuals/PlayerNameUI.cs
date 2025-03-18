using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameUI : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    Color oulLineColor;
    [SerializeField] Color red;
    [SerializeField] Color green;
    [SerializeField] Color yellow;
    [SerializeField] Color blue;
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        Team team = GameManager.Instance.GetCurrentPlayerTeam();
        string currentPlayer = team.ToString();
        textMesh.text = currentPlayer;
        switch(team)
        {
            case Team.Red:
                textMesh.outlineColor = red;
            break;
            case Team.Green:
                textMesh.outlineColor = green;
            break;
            case Team.Yellow:
                textMesh.outlineColor = yellow;
            break;
            case Team.Blue:
                textMesh.outlineColor = blue;
            break;
        }
    }
}
