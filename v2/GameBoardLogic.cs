using MoonActive.Scripts;
using System;
using UnityEngine;



public class GameBoardLogic
{
    private GameView _gameView;
    private UserActionEvents _userActionEvents;
    private TicTacToeGameData _gameData;
    string _inMemorySavedGameStateJson;
    BoardTilePosition _tile_position;
    FileManager _fileManager;

    public GameBoardLogic(GameView gameView, UserActionEvents userActionEvents)
    {

        _gameView = gameView;
        _userActionEvents = userActionEvents;
        _gameData = new TicTacToeGameData(3);
        _inMemorySavedGameStateJson = "";

        _tile_position = new BoardTilePosition(0, 0);

        _fileManager = new FileManager();

        _userActionEvents.StartGameClicked += _StartGameClicked;
        _userActionEvents.TileClicked += _TileClicked;
        _userActionEvents.SaveStateClicked += _SaveStateClicked;
        _userActionEvents.LoadStateClicked += _LoadStateClicked;

    }

    private void _StartGameClicked()
    {
        Initialize(_gameData.ColNum, _gameData.RowNum);
        _gameData.Gamestate.IsStarted = true;
        _gameView.StartGame(_gameData.Gamestate.CurrentPlayer);
        

        Debug.Log("Start game clicked!");

    }

    private void _TileClicked(BoardTilePosition position)
    {

        if (_gameData.Board[position.Column, position.Row] != -1)
        {
            return;
        }

        if (!_gameData.Gamestate.IsStarted)
        {
            return;
        }


        _gameData.Gamestate.TotalClicks++;

        _tile_position = position;
        _gameData.Board[position.Column, position.Row] = _gameData.Gamestate.CurrentPlayerIndex;
        _gameView.SetTileSign(_gameData.Gamestate.CurrentPlayer, _tile_position);

        IsGameWon(position);
        IsGameTie();

        _gameData.Gamestate.CurrentPlayerIndex = 1 - _gameData.Gamestate.CurrentPlayerIndex;
        _gameData.Gamestate.CurrentPlayer = PlayerTypeByIndex(_gameData.Gamestate.CurrentPlayerIndex);
        _gameView.ChangeTurn(_gameData.Gamestate.CurrentPlayer);






        Debug.Log($"Tile {position.Column},{position.Row} clicked!");
    }

    private void IsGameWon(BoardTilePosition position)
    {
        if (CheckWin(position.Row, position.Column, _gameData.Gamestate.CurrentPlayerIndex))
        {
            _gameView.GameWon(_gameData.Gamestate.CurrentPlayer);
            Initialize(_gameData.ColNum, _gameData.RowNum);

        }
    }


    private void IsGameTie()
    {
        if (_gameData.Gamestate.TotalClicks == 9)
        {
            _gameView.GameTie();
            Initialize(_gameData.ColNum, _gameData.RowNum);

        }
    }

    public void Initialize(int columns, int rows)
    {
        _gameData = new TicTacToeGameData(3);


    }

    private void _SaveStateClicked(GameStateSource StateSource)
    {

        _gameData.Gamestate.Flatten2DArray(_gameData.Board);

        if (StateSource == GameStateSource.PlayerPrefs)
        {
            
            //string jsonToFile = JsonUtility.ToJson(_gameData.Gamestate);

            _fileManager.SaveData(_gameData.Gamestate, "/gamestate.json");
            
            //System.IO.File.WriteAllText("/gamestate.json", jsonToFile);

            Debug.Log($"Save game {GameStateSource.PlayerPrefs} clicked!");
        }

        if (StateSource == GameStateSource.InMemory)
        {

            _inMemorySavedGameStateJson = JsonUtility.ToJson(_gameData.Gamestate);
            Debug.Log($"Save game {GameStateSource.InMemory} clicked!");
        }
    }

    private void _LoadStateClicked(GameStateSource StateSource)
    {
        Initialize(_gameData.ColNum, _gameData.RowNum);


        if (StateSource == GameStateSource.PlayerPrefs) 
        {

            TicTacToeGameState? temp_gameState = _fileManager.LoadData("/gamestate.json");
            if (temp_gameState.HasValue) 
            
            {
                _gameData.Gamestate = (TicTacToeGameState)temp_gameState;
                _gameData.Board = _gameData.Gamestate.UnflattenTo2DArray(_gameData.ColNum, _gameData.RowNum);
                _gameView.StartGame(_gameData.Gamestate.CurrentPlayer);
                DrawSignsFromBoard();
            }

            else
            {
                Debug.Log($"cant read file");
            }

            /*
            if (System.IO.File.Exists("/gamestate.json"))
            {
                string jsonFromFile = System.IO.File.ReadAllText("/gamestate.json");
                _gameData.Gamestate = JsonUtility.FromJson<TicTacToeGameState>(jsonFromFile);
                _gameData.Board = _gameData.Gamestate.UnflattenTo2DArray(_gameData.ColNum, _gameData.RowNum);

                _gameView.StartGame(_gameData.Gamestate.CurrentPlayer);
                DrawSignsFromBoard();
            }

            else
            {
                Debug.Log($"cant read file , path not exist");
            }
        */

        }


        if (StateSource == GameStateSource.InMemory)
        {
            if (_inMemorySavedGameStateJson == "") 
            {
                Debug.Log($"sorry,No InMemory saved game");
                Initialize(_gameData.ColNum, _gameData.RowNum);
                _gameView.StartGame(_gameData.Gamestate.CurrentPlayer);
                DrawSignsFromBoard();
                return;
            }
                    
            Initialize(_gameData.ColNum, _gameData.RowNum);

            _gameData.Gamestate = JsonUtility.FromJson<TicTacToeGameState>(_inMemorySavedGameStateJson);
            _gameData.Board = _gameData.Gamestate.UnflattenTo2DArray(_gameData.ColNum, _gameData.RowNum);

            _gameView.StartGame(_gameData.Gamestate.CurrentPlayer);
            DrawSignsFromBoard();
            Debug.Log($"Save game {GameStateSource.InMemory} clicked!");
        }
    }   

    public void DeInitialize()
    {
        _userActionEvents.StartGameClicked -= _StartGameClicked;
        _userActionEvents.TileClicked -= _TileClicked;
        _userActionEvents.SaveStateClicked -= _SaveStateClicked;
        _userActionEvents.LoadStateClicked -= _LoadStateClicked;
    }

    public PlayerType PlayerTypeByIndex(int currentPlayer)
    {
        if (currentPlayer == 0)
        {
            return PlayerType.PlayerX;
        }
        else 
        {
            return PlayerType.PlayerO;
        }
    }

    private bool CheckWin(int row, int col, int player)
    {
        int playerValue = (player == 1) ? 1 : -1;

        _gameData.Gamestate.RowsWin[row] += playerValue;
        _gameData.Gamestate.ColsWin[col] += playerValue;
   
        if (row == col)
        {
            _gameData.Gamestate.DiagonalWin += playerValue;
        }

        if (row + col == _gameData.Size - 1)
        {
            _gameData.Gamestate.AntiDiagonalWin += playerValue;
        }

        if (Math.Abs(_gameData.Gamestate.RowsWin[row]) == _gameData.Size ||
            Math.Abs(_gameData.Gamestate.ColsWin[col]) == _gameData.Size ||
            Math.Abs(_gameData.Gamestate.DiagonalWin) == _gameData.Size ||
            Math.Abs(_gameData.Gamestate.AntiDiagonalWin) == _gameData.Size)
        {

            return true; // Current player wins
        }

        return false; // No winner yet
    }

    private void DrawSignsFromBoard()
    {
        for (int i = 0; i < _gameData.Board.GetLength(0); i++)
        {
            for (int j = 0; j < _gameData.Board.GetLength(1); j++)
            {

                

                if (_gameData.Board[i, j] == -1)
                {
                    continue;
                }

                PlayerType player = PlayerTypeByIndex(_gameData.Board[i, j]);
                BoardTilePosition position = new BoardTilePosition(j, i);
                Debug.Log($"position : {position.Column},{position.Row} ");
                _gameView.SetTileSign(player, position);

            }


        }
          

        
    }
}
