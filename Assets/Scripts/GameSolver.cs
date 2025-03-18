using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSolver
{
    private Team team;
    public GameSolver(Team team)
    {
        this.team = team;
    }

    private struct GameStateProbability
    {
        public GameState gameState;
        public float probability;
        public GameStateProbability(GameState gameState, float probability)
        {
            this.gameState = gameState;
            this.probability = probability;
        }
    }
    private float GetEvaluation(GameState finalState)
    {
        Player[] players = finalState.game.GetPlayers();
        float otherPlayersScore = 0;
        float playerScore = 0;
        foreach(Player p in players)
        {
            if (p.GetTeam().Equals(team))
            {
                playerScore = p.GetScore();
            }
            else
            {
                otherPlayersScore += p.GetScore();
            }
        }
        return playerScore - otherPlayersScore;
    }
    private MoveAction SolveExpectiMiniMax(GameState currentGameState, int depth)
    {
        if (depth == 0 || currentGameState.IsFinalState())
        {
            MoveAction moveAction = new MoveAction();
            moveAction.evaluation = GetEvaluation(currentGameState);
            return moveAction;
        }
        Player player = currentGameState.game.GetCurrentPlayer();
        if (player.GetPlayerState().Equals(PlayerState.RollingDice))
        {
            //chance node
            int diceFreeSlots = 3 - player.GetMovesList().Count;
            List <GameStateProbability> gameStateProbabilityList = new List<GameStateProbability>();
            for(int i=0;i<diceFreeSlots; i++)
            {
                for(int m = 1; m <= 5; m++)
                {
                    List<int> movesList = new List<int>();
                    float rollDiceProbability = 1f / 6f;
                    float nextStateProbability = rollDiceProbability;
                    for (int j = 0; j < i; j++)
                    {
                        movesList.Add(6);
                        nextStateProbability *= rollDiceProbability;
                    }
                    movesList.Add(m);
                    //GameState nextState = GameState.DeltaChooseMoves(currentGameState.game, movesList);
                    GameState nextState =  new GameState(currentGameState.game.GetCopy());
                    foreach (int move in movesList)
                    {
                        nextState.game.ChooseMove(move);
                    }
                    gameStateProbabilityList.Add(new GameStateProbability(nextState, nextStateProbability));
                }
            }
            float expectation = 0;
            foreach(GameStateProbability gp in gameStateProbabilityList)
            {
                MoveAction nextMoveAction = SolveExpectiMiniMax(gp.gameState,depth-1);
                expectation += nextMoveAction.evaluation* gp.probability;
            }
            MoveAction moveAction = new MoveAction();
            moveAction.evaluation = expectation;
            return moveAction;
        }
        List<int> availableMoves = player.GetMovesList();
        PuzzlePiece[] puzzlePieces = currentGameState.game.GetPuzzlePiecesOf(player.GetTeam());
        if (player.GetTeam().Equals(team))
        {
            //maximizing player
            MoveAction maxEvaluationMoveAction = new MoveAction();
            maxEvaluationMoveAction.evaluation = float.NegativeInfinity;
            foreach(int move in availableMoves)
            {
                for(int i = 3; i >= 0; i--)
                {
                    GameState nextState;
                    nextState = new GameState(currentGameState.game.GetCopy());
                    PuzzlePiece[] nextStatePuzzlePieces = nextState.game.GetPuzzlePiecesOf(player.GetTeam());
                    nextState.game.SelectPuzzlePiece(nextStatePuzzlePieces[i]);
                    if (move == 6 && puzzlePieces[i].InBase())
                    {
                        //nextState = GameState.DeltaSpawn(currentGameState.game,puzzlePieces[i]);
                        nextState.game.SpawnPuzzlePiece(nextStatePuzzlePieces[i]);
                    }
                    else
                    {
                        //nextState = GameState.DeltaMove(currentGameState.game, puzzlePieces[i], move);
                        nextState.game.Move(move);
                    }
                    MoveAction nextMoveAction = SolveExpectiMiniMax(nextState, depth - 1);
                    if (maxEvaluationMoveAction.evaluation <= nextMoveAction.evaluation)
                    {
                        maxEvaluationMoveAction.evaluation = nextMoveAction.evaluation;
                        maxEvaluationMoveAction.puzzlePieceIndex = i;
                        maxEvaluationMoveAction.move = move;
                    }
                }
            }
            return maxEvaluationMoveAction;
        }
        else
        {
            //minimizing player
            MoveAction minEvaluationMoveAction = new MoveAction();
            minEvaluationMoveAction.evaluation = float.PositiveInfinity;
            foreach (int move in availableMoves)
            {
                for (int i = 3; i >= 0 ; i--)
                {
                    GameState nextState;
                    nextState = new GameState(currentGameState.game.GetCopy());
                    PuzzlePiece[] nextStatePuzzlePieces = nextState.game.GetPuzzlePiecesOf(player.GetTeam());
                    nextState.game.SelectPuzzlePiece(nextStatePuzzlePieces[i]);
                    if (move == 6 && puzzlePieces[i].InBase())
                    {
                        nextState.game.SpawnPuzzlePiece(nextStatePuzzlePieces[i]);
                    }
                    else
                    {
                        nextState.game.Move(move);
                    }
                    MoveAction nextMoveAction = SolveExpectiMiniMax(nextState, depth - 1);
                    if (minEvaluationMoveAction.evaluation >= nextMoveAction.evaluation)
                    {
                        minEvaluationMoveAction.evaluation = nextMoveAction.evaluation;
                        minEvaluationMoveAction.puzzlePieceIndex = i;
                        minEvaluationMoveAction.move = move;
                    }
                }
            }
            return minEvaluationMoveAction;
        }
    }
    public MoveAction Solve(Game game)
    {
        GameState startState = new GameState(game);
        MoveAction bestMove = SolveExpectiMiniMax(startState, 2);
        PuzzlePiece[] puzzlePieces = startState.game.GetPuzzlePiecesOf(team);
        PuzzlePiece puzzlePiece = puzzlePieces[bestMove.puzzlePieceIndex];
        return bestMove;
    }
}
