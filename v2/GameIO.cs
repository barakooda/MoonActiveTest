using UnityEngine;

public interface GameIO
{
    void Save(TicTacToeGameState data, string filePath);
    TicTacToeGameState? Load(string filePath);
}



public class JsonFile: GameIO
{

    public void Save(TicTacToeGameState data, string filePath)
    {
  
        // Implementation for saving data in JSON format
        string jsonToFile = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(filePath, jsonToFile);
    }

    public TicTacToeGameState? Load(string filePath)
    {

        if (System.IO.File.Exists(filePath))
        {
            string jsonFromFile = System.IO.File.ReadAllText("/gamestate.json");
            TicTacToeGameState Gamestate = JsonUtility.FromJson<TicTacToeGameState>(jsonFromFile);
            return Gamestate;
        }
        else 
        { 
            return null; 
        }

        
        
    }
}



public class FileManager
{
    private GameIO _GameIO;

    public FileManager(GameIO fileSavingType)
    {
        _GameIO = fileSavingType;
    }

    // Default constructor sets JSON strategy as the default
    public FileManager() : this(new JsonFile())
    {
    }

    public void SaveData(TicTacToeGameState data, string filePath)
    {
        _GameIO.Save(data, filePath);
    }

    public TicTacToeGameState? LoadData(string filePath)
    {
        return _GameIO.Load(filePath);
    }
}