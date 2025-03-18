using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCopyTesting : MonoBehaviour
{
    Game game;
    Game test;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Copied original game");
            game = GameManager.Instance.GetGameCopy();
            Test();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Applied move 1 to game");
            test.Move(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Applied move 2 to game");
            test.Move(2);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            game.PrintGameInfo();
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            test.PrintGameInfo();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PuzzlePiece selected =test.board.GetPuzzlePiecesOf(game.GetCurrentPlayerTeam())[3]; 
            test.SpawnPuzzlePiece(selected);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PuzzlePiece selected = test.board.GetPuzzlePiecesOf(game.GetCurrentPlayerTeam())[3];
            test.SelectPuzzlePiece(selected);
            test.Move(6);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            test.ChooseMove();
        }
    }
    private void Test()
    {
        Game test1 = game.GetCopy();
        PuzzlePiece[] puzzlePieces1 = test1.board.GetPuzzlePiecesOf(game.GetCurrentPlayerTeam());
        test1.ChooseMove(6);
        test1.ChooseMove(5);
        PuzzlePiece selectedTest1 = puzzlePieces1[3];
        test1.SpawnPuzzlePiece(selectedTest1);
        Game test2 = test1.GetCopy();
        PuzzlePiece[] puzzlePieces2 = test2.board.GetPuzzlePiecesOf(game.GetCurrentPlayerTeam());
        PuzzlePiece selectedTest2 = puzzlePieces2[3];
        test2.SelectPuzzlePiece(selectedTest2);
        test2.Move(5);
        test1.PrintGameInfo();
        test2.PrintGameInfo();
    }
}
