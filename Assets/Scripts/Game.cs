using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    private Team[] teams;
    private int[] playersSpawnIndices;
    private int[] playersWinningIndices;
    private const int playersCount = 4;
    public Board board;
    private Player[] players;

    private int currentPlayerIndex = 0;

    public event Action<int> OnPlayerRolledDice;
    public event Action<List<int>> OnPlayerFinishedRollingDice;
    public event Action<List<int>, int> OnPlayerChoseNumber;
    public event Action OnPlayerFinishedMoving;
    public event Action OnPlayerPassedTurn;
    public event Action<Team> OnGameOver;
    public event Action<Team> OnPlayerKilled;
    public event Action<Team> OnTurnChanged;

    public Game(Board board,Team[] teams, int[] playersSpawnIndices, int[] playersWinningIndices, int currentPlayerIndex)
    {
        this.teams = teams;
        this.playersSpawnIndices = playersSpawnIndices;
        this.playersWinningIndices = playersWinningIndices;
        this.currentPlayerIndex = currentPlayerIndex;
        this.board = board;
        players = new Player[playersCount];
        for (int i = 0; i < playersCount; i++)
        {
            players[i] =
                new Player(
                    board: board,
                    team: teams[i],
                    spawnIndex: playersSpawnIndices[i],
                    winningIndex: playersWinningIndices[i]
                );
            players[i].OnFinishedRollingDice += Player_OnFinishedRollingDice; 
            players[i].OnDiceRolled += Player_OnDiceRolled; 
            players[i].OnChoseNumber += Player_OnChoseNumber; 
            players[i].OnFinishedMoving += Player_OnFinishedMoving; 
            players[i].OnKilled += Player_OnKilled; 
            players[i].OnPassTurn += Player_OnPassTurn; 
        }
    }

    //Player event Listeners, these will notify the UI whenever something happens
    private void Player_OnDiceRolled(int incrementValue)
    {
        OnPlayerRolledDice?.Invoke(incrementValue);
    }

    private void Player_OnFinishedRollingDice(List<int> movesList)
    {
        OnPlayerFinishedRollingDice?.Invoke(movesList);
    }
    private void Player_OnChoseNumber(List<int> movesList, int chosenNumber)
    {
        OnPlayerChoseNumber?.Invoke(movesList, chosenNumber);
    }
    private void Player_OnFinishedMoving()
    {
        OnPlayerFinishedMoving?.Invoke();
        if (board.IsGameOver())
        {
            OnGameOver?.Invoke(teams[currentPlayerIndex]);
            return;
        }
        ChangeTurn();
    }
    private void Player_OnPassTurn()
    {
        OnPlayerPassedTurn?.Invoke();
        if (board.IsGameOver())
        {
            OnGameOver?.Invoke(teams[currentPlayerIndex]);
            return;
        }
        ChangeTurn();
    }
    private void Player_OnKilled()
    {
        players[currentPlayerIndex].Ready();
        OnPlayerKilled?.Invoke(players[currentPlayerIndex].GetTeam());
    }
    public void SelectPuzzlePiece(PuzzlePiece puzzlePiece)
    {
        players[currentPlayerIndex].SelectPuzzlePiece(puzzlePiece);
    }
    public void ClearPuzzlePiece()
    {
        players[currentPlayerIndex].ClearPuzzlePiece();
    }
    public void ChooseMove(int move = -1)
    {
        players[currentPlayerIndex].ChooseMove(move);
    }
    public bool Move(int move)
    {
        return players[currentPlayerIndex].Move(move);
    }
    public void SpawnPuzzlePiece(PuzzlePiece puzzlePiece)
    {
        if (puzzlePiece.GetOwnerTeam().Equals(teams[currentPlayerIndex]))
        {
            players[currentPlayerIndex].Spawn();
        }
    }
    public void ChangeTurn()
    {
        currentPlayerIndex++;
        currentPlayerIndex %= playersCount;
        players[currentPlayerIndex].Ready();
        OnTurnChanged?.Invoke(players[currentPlayerIndex].GetTeam()); 
    }
    public int GetPlayerSpawnIndex()
    {
        return players[currentPlayerIndex].GetSpawnIndex();
    }
    public int GetPlayerWinningIndex()
    {
        return players[currentPlayerIndex].GetWinningIndex();
    }
    public int[] GetPlayersWinningIndices()
    {
        return playersWinningIndices;
    }
    public int GetPlayersCount()
    {
        return playersCount;
    }
    public Team GetCurrentPlayerTeam()
    {
        return players[currentPlayerIndex].GetTeam();
    }
    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }
    public Player[] GetPlayers()
    {
        return players;
    }
    public void UpdatePlayer(List<int> movesList,PlayerState playerState)
    {
        players[currentPlayerIndex].SetMovesList(movesList);
        players[currentPlayerIndex].SetPLayerState(playerState);
    }
    public List<int> GetPlayerMovesList()
    {
        return players[currentPlayerIndex].GetMovesList();
    }
    public PuzzlePiece[] GetPuzzlePiecesOf(Team team)
    {
        return board.GetPuzzlePiecesOf(team);
    }
    public Game GetCopy()
    {
        Board boardCopy = board.GetCopy();
        Game gameCopy = new Game(boardCopy,teams, playersSpawnIndices, playersWinningIndices, currentPlayerIndex);
        gameCopy.UpdatePlayer(players[currentPlayerIndex].GetMovesList(), players[currentPlayerIndex].GetPlayerState());
        return gameCopy;
    }
    public bool IsGameOver()
    {
        return board.IsGameOver();
    }
    public void PrintGameInfo()
    {
        Debug.Log("GAME: " + GetHashCode());
        Debug.Log(((currentPlayerIndex == 0) ? "Red's" : "Yellow's") + " Turn");
        for (int i = 0; i < playersCount; i++)
        {
            Debug.Log(players[i].GetTeam() + "'s score: " + players[i].GetScore());
        }
        for(int i=0;i<playersCount; i++)
        {
            Debug.Log(teams[i] + "'s puzzle pieces location:");
            PuzzlePiece[] teamPuzzlePieces = board.GetPuzzlePiecesOf(teams[i]);
            foreach(var piece in teamPuzzlePieces)
            {
                Debug.Log(piece.GetCurrentPosition());
            }
            Debug.Log("moves available:");
            string moves = "[";
            foreach(var move in players[i].GetMovesList())
            {
                moves += move + " ";
            }
            moves += "]";
            Debug.Log(moves);
        }
    }
}
