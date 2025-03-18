using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour

{
    public static GameManager Instance {  get; private set; }
    private int currentPlayerIndex = 0;
    [SerializeField] private PlayerBase[] playerBaseTransforms;
    private Game game;

    public event Action<int> OnPlayerRolledDice;
    public event Action<List<int>> OnPlayerFinishedRollingDice;
    public event Action<List<int>,int> OnPlayerChoseNumber;
    public event Action OnPlayerFinishedMoving;
    public event Action OnPlayerPassedTurn;
    public event Action<Team> OnGameOver;
    public event Action<Team> OnPlayerKilled;
    public event Action<Team> OnTurnChanged;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        const int playersCount = 4;
        Player[] players = new Player[playersCount];
        int[] playersSpawnIndices = new int[playersCount] { 0, 13,26,39 };
        int[] playersWinningIndices = new int[playersCount] { 52, 57, 62, 67};
        Team[] teams = new Team[playersCount] { Team.Red, Team.Green, Team.Yellow, Team.Blue };
        game = new Game(BoardManager.Instance.GetGameBoard(),teams,playersSpawnIndices,playersWinningIndices,currentPlayerIndex);
        game.OnPlayerRolledDice += (x)=>OnPlayerRolledDice?.Invoke(x);
        game.OnPlayerFinishedRollingDice += (x)=>OnPlayerFinishedRollingDice?.Invoke(x);
        game.OnPlayerChoseNumber += (x,y)=>OnPlayerChoseNumber?.Invoke(x,y);
        game.OnPlayerFinishedMoving += ()=>OnPlayerFinishedMoving?.Invoke();
        game.OnGameOver += (x) => OnGameOver?.Invoke(x);
        game.OnPlayerPassedTurn += ()=>OnPlayerPassedTurn?.Invoke();
        game.OnPlayerKilled += (x) => OnPlayerKilled?.Invoke(x);
        game.OnTurnChanged += (x) => OnTurnChanged?.Invoke(x);
        PlayerUI.Instance.OnRollDiceClicked += PlayerUI_OnRollDiceClicked;
        PlayerUI.Instance.OnClearPuzzlePiece += PlayerUI_OnClearPuzzlePiece;
        PlayerUI.Instance.OnSelectPuzzlePiece += PlayerUI_OnSelectPuzzlePiece;
        PlayerUI.Instance.OnSpawnPuzzlePiece += PlayerUI_OnSpawnPuzzlePiece;
        MovesChoicesUI.Instance.OnChooseMove += MovesLChoicesUI_OnChooseMove;
    }


    //PlayerUI event listeners, whenever something happens in the UI the player will be notified
    private void PlayerUI_OnSpawnPuzzlePiece(PuzzlePiece puzzlePiece)
    {
        game.SpawnPuzzlePiece(puzzlePiece);
    }
    private void PlayerUI_OnSelectPuzzlePiece(PuzzlePiece puzzlePiece)
    {
        game.SelectPuzzlePiece(puzzlePiece);
    }
    private void PlayerUI_OnClearPuzzlePiece()
    {
        game.ClearPuzzlePiece();
    }
    private void PlayerUI_OnRollDiceClicked()
    {
        game.ChooseMove();
    }
    //whenever we choose a move the player will be notified
    private void MovesLChoicesUI_OnChooseMove(int move)
    {
        game.Move(move);
    }
    public int GetPlayerSpawnIndex()
    {
        return game.GetPlayerSpawnIndex();
    }
    public int GetPlayerWinningIndex()
    {
        return game.GetPlayerWinningIndex();
    }
    public int[] GetPlayersWinningIndices()
    {
        return game.GetPlayersWinningIndices();
    }
    public int GetPlayersCount()
    {
        return game.GetPlayersCount();
    }
    public Team GetCurrentPlayerTeam()
    {
        return game.GetCurrentPlayerTeam();
    }
    public Player GetCurrentPlayer()
    {
        return game.GetCurrentPlayer();
    }
    public Game GetGameCopy() {
        return game.GetCopy();
    }
    public Game GetGame()
    {
        return game;
    }
}
