
using System;

[System.Serializable]
public struct TicTacToeGameData
{
    public int Size;
    public int ColNum;
    public int RowNum;
    public int[,] Board;
    public TicTacToeGameState Gamestate;

    public TicTacToeGameData(int size)
    {
        Size = size;
        ColNum = size;
        RowNum = size;
        Board = new int[size, size];
        for (int i = 0; i < Board.GetLength(0); i++)
            for (int j = 0; j < Board.GetLength(1); j++)
                Board[i, j] = -1;

        Gamestate = new TicTacToeGameState(size);
    }
}
[System.Serializable]
public struct TicTacToeGameState
{
    public int[] FlatBoard;
    public bool IsStarted;
    public int CurrentPlayerIndex;
    public MoonActive.Scripts.PlayerType CurrentPlayer;
    public int DiagonalWin;
    public int AntiDiagonalWin;
    public int[] ColsWin;
    public int[] RowsWin;
    public int TotalClicks;

    public TicTacToeGameState(int size)
    {

        FlatBoard = new int[size * size];
        IsStarted = false;
        CurrentPlayerIndex = 0;
        CurrentPlayer = MoonActive.Scripts.PlayerType.PlayerX;
        ColsWin = new int[size];
        RowsWin = new int[size];
        Array.Clear(ColsWin, 0, size);
        Array.Clear(RowsWin, 0, size);
        DiagonalWin = 0;
        AntiDiagonalWin = 0;
        TotalClicks = 0;
        
    }

    public void Flatten2DArray(int[,] Board)
    {
        int rows = Board.GetLength(0);
        int cols = Board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                FlatBoard[i * cols + j] = Board[i, j];
            }
        }
    }

    public int[,] UnflattenTo2DArray(int ColSize, int RowSize)
    {
        int[,]  Board = new int[ColSize, RowSize];
        for (int i = 0; i < RowSize; i++)
        {
            for (int j = 0; j < ColSize; j++)
            {
                Board[i, j] = FlatBoard[i * ColSize + j];
            }
        }
        return Board;
    }
}
