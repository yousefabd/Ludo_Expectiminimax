using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMover : MonoBehaviour
{
    private Dictionary<Team, GameSolver> teamGameSolver;
    private void Start()
    {
        GameManager.Instance.OnTurnChanged += GameManager_OnTurnChanged;
        teamGameSolver = new Dictionary<Team, GameSolver>();
        teamGameSolver.Add(Team.Yellow, new GameSolver(Team.Yellow));
        teamGameSolver.Add(Team.Green, new GameSolver(Team.Green));
        teamGameSolver.Add(Team.Blue, new GameSolver(Team.Blue));
    }

    private void GameManager_OnTurnChanged(Team currentPlayer)
    {
        if (!currentPlayer.Equals(Team.Red))
        {
            StartCoroutine(AutoMove(GameManager.Instance.GetCurrentPlayer(),1.8f));

        }
    }
    private IEnumerator AutoMove(Player player,float cooldownTimer)
    {
        bool finished = false;
        while (!finished)
        {
            yield return new WaitForSeconds(cooldownTimer);
            cooldownTimer = 0.5f;
            PlayerState playerState = player.GetPlayerState();
            Game game = GameManager.Instance.GetGame();
            switch (playerState)
            {
                case PlayerState.RollingDice:
                    game.ChooseMove();
                    break;
                case PlayerState.Moving:
                    Move(player);
                    break;
                case PlayerState.Finished:
                    finished = true;
                    break;
            }
        }
    }
    private void Move(Player player)
    {
        Game game = GameManager.Instance.GetGame();
        List<int> moves = player.GetMovesList();
        PuzzlePiece[] puzzlePieces = BoardManager.Instance.GetGameBoard().GetPuzzlePiecesOf(player.GetTeam());
        MoveAction bestMove = teamGameSolver[player.GetTeam()].Solve(game);
        PuzzlePiece puzzlePiece = puzzlePieces[bestMove.puzzlePieceIndex];
        if (puzzlePiece.InBase() && bestMove.move == 6)
        {
            game.SpawnPuzzlePiece(puzzlePiece);
            Debug.Log("spawn" +  puzzlePiece);
        }
        else
        {
            game.SelectPuzzlePiece(puzzlePiece);
            Debug.Log("move" + bestMove.move +" " +bestMove.puzzlePieceIndex + " " + puzzlePiece.GetCurrentPosition());
            game.Move(bestMove.move);
        }
        /*
        foreach(int move in moves)
        {
            bool moved = false;
            if (move == 6)
            {
                for(int i = 0; i < puzzlePieces.Length; i++)
                {
                    if (puzzlePieces[i].InBase())
                    {
                        GameManager.Instance.GetGame().SpawnPuzzlePiece(puzzlePieces[i]);
                        moved = true;
                        break;
                    }
                }
            }
            if (moved)
                break;
            //either move wasn't 6 or there aren't any pieces in base
            for (int i = 0; i < puzzlePieces.Length; i++)
            {
                GameManager.Instance.GetGame().SelectPuzzlePiece(puzzlePieces[i]);
                if (GameManager.Instance.GetGame().Move(move))
                {
                    moved = true;
                    break;
                }
            }
            if (moved)
                break;
        }*/

    }
}
