using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    private Player[] players;
    private Team[] teams;
    private int[] playersSpawnIndices;
    private int[] playersWinningIndices;
    private const int playersCount = 2;

    private int currentPlayerIndex = 0;
    [SerializeField] private PlayerBase[] playerBases;

    public event Action<int> OnPlayerRolledDice;
    public event Action<List<int>> OnPlayerFinishedRollingDice;
    public event Action<List<int>,int> OnPlayerChoseNumber;
    public event Action OnPlayerFinishedMoving;
    public event Action OnPlayerPassedTurn;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        players = new Player[playersCount];
        playersSpawnIndices = new int[playersCount] { 0, 26 };
        playersWinningIndices = new int[playersCount] { 52, 57 };
        teams = new Team[playersCount] { Team.Player, Team.Computer };
        for (int i = 0; i < playersCount; i++)
        {
            players[i] =
                new Player(
                    playerBase: playerBases[i],
                    team: teams[i],
                    spawnIndex: playersSpawnIndices[i],
                    winningIndex: playersWinningIndices[i]
                );
            players[i].OnFinishedRollingDice += Player_OnFinishedRollingDice;
            players[i].OnDiceRolled += Player_OnDiceRolled;
            players[i].OnChoseNumber += Player_OnChoseNumber;
            players[i].OnFinishedMoving += Player_OnFinishedMoving;
            players[i].OnPassTurn += Player_OnPassTurn;
        }
        PlayerUI.Instance.OnRollDiceClicked += PlayerUI_OnRollDiceClicked;
        PlayerUI.Instance.OnClearPuzzlePiece += PlayerUI_OnClearPuzzlePiece;
        PlayerUI.Instance.OnSelectPuzzlePiece += PlayerUI_OnSelectPuzzlePiece;
        PlayerUI.Instance.OnSpawnPuzzlePiece += PlayerUI_OnSpawnPuzzlePiece;
        MovesChoicesUI.Instance.OnChooseMove += MovesLChoicesUI_OnChooseMove;
    }

    private void Player_OnPassTurn()
    {
        ChangeTurn();
        OnPlayerPassedTurn?.Invoke();
    }

    private void Player_OnFinishedMoving()
    {
        ChangeTurn();
        OnPlayerFinishedMoving?.Invoke();
    }

    private void PlayerUI_OnSpawnPuzzlePiece(PuzzlePiece puzzlePiece)
    {
        if (puzzlePiece.GetOwnerTeam().Equals(teams[currentPlayerIndex]))
        {
            players[currentPlayerIndex].Spawn();
        }
    }

    private void Player_OnChoseNumber(List<int> movesList,int chosenNumber)
    {
        OnPlayerChoseNumber?.Invoke(movesList,chosenNumber);
    }

    private void MovesLChoicesUI_OnChooseMove(int move)
    {
        players[currentPlayerIndex].Move(move);
    }

    private void PlayerUI_OnSelectPuzzlePiece(PuzzlePiece puzzlePiece)
    {
        players[currentPlayerIndex].SelectPuzzlePiece(puzzlePiece);
    }

    private void PlayerUI_OnClearPuzzlePiece()
    {
        players[currentPlayerIndex].ClearPuzzlePiece();
    }

    private void PlayerUI_OnRollDiceClicked()
    {
        players[currentPlayerIndex].ChooseMove();
    }

    private void Player_OnDiceRolled(int incrementValue)
    {
        OnPlayerRolledDice?.Invoke(incrementValue);
    }

    private void Player_OnFinishedRollingDice(List<int> movesList)
    {
        OnPlayerFinishedRollingDice?.Invoke(movesList);
    }
 
    public int GetPlayerSpawnIndex()
    {
        return players[currentPlayerIndex].GetSpawnIndex();
    }
    public int GetPlayerWinningIndex()
    {
        return players[currentPlayerIndex].GetWinningIndex();
    }
    public int GetPlayersCount()
    {
        return playersCount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerBases[currentPlayerIndex].SpawnPuzzlePiece();
            //ChangeTurn();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D puzzlePieceCollider = Physics2D.OverlapPoint(UtilsClass.GetMouseWorldPosition());
            if(puzzlePieceCollider != null)
            {
                PuzzlePiece puzzlePiece = puzzlePieceCollider.GetComponent<PuzzlePiece>();
                if (!puzzlePiece.InBase() && puzzlePiece.GetOwnerTeam().Equals(teams[currentPlayerIndex]))
                {
                    //Board.Instance.Move(puzzlePiece.GetCurrentPosition(), 6);
                    //ChangeTurn();
                }
            }
        }
    }
    private void ChangeTurn()
    {
        currentPlayerIndex++;
        currentPlayerIndex %= playersCount;
        players[currentPlayerIndex].Ready();
        Debug.Log(((currentPlayerIndex == 0) ? "Red's" : "Yellow's") + " Turn");
    }
}
