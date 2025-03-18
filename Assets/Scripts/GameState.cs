using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public Game game;
    public GameState(Game game)
    {
        this.game = game.GetCopy();
    }
    public static GameState DeltaMove(Game game,PuzzlePiece puzzlePiece,int move)
    {
        GameState nextState = new GameState(game.GetCopy());
        nextState.game.SelectPuzzlePiece(puzzlePiece);
        nextState.game.Move(move);
        return nextState;
    }
    public static GameState DeltaSpawn(Game game,PuzzlePiece puzzlePiece)
    {
        GameState nextState = new GameState(game.GetCopy());
        nextState.game.SelectPuzzlePiece(puzzlePiece);
        nextState.game.SpawnPuzzlePiece(puzzlePiece);
        return nextState;
    }
    public static GameState DeltaChooseMoves(Game game,List<int> movesList)
    {
        GameState nextState = new GameState(game.GetCopy());
        foreach(int move in movesList)
        {
            nextState.game.ChooseMove(move);
        }
        return nextState;
    }
    public bool IsFinalState()
    {
        return game.IsGameOver();
    }
}
