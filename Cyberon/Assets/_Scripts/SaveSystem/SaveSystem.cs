using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveSystem : SSystem<SaveSystem>
{
    public string path = "";

    void Awake()
    {
        path = Application.dataPath + "/SaveFileCyberon.save";
    }

    public LevelDataSave CreateSaveObject()
    {
        LevelDataSave save = new LevelDataSave();

        save.phase = (int)GameStateManager.Instance.Phase;
        save.turn = TurnManager.Instance.TurnCount;
        save.playerActionPoints = GameStateManager.Instance.Player.ActionPoints;
        save.playerEnergy = GameStateManager.Instance.PlayerStats.aEnergy;
        save.playerScore = GameStateManager.Instance.score;
        save.turnActorIndex = TurnManager.Instance.actorIndex;
        save.objectiveIndex = GameStateManager.Instance.objectivesQueue.currentObjectivIndex;
        foreach (CyberonActor actor in GameStateManager.Instance.AllActors)
        {
            save.AllActors.Add(new SerializableData.SerializableCyberonActor(actor));
        }

        foreach (IHackable hackable in GameStateManager.Instance.AllHackables)
        {
            save.AllIHackables.Add(new SerializableData.SerializableIHackable(hackable));
        }

        foreach (ResourceSilo silo in GameStateManager.Instance.AllResourceSilos)
        {
            save.AllResourceSilos.Add(new SerializableData.SerializableResourceSilo(silo));
        }

        foreach (Patroller patroller in GameStateManager.Instance.AllPatrollers)
        {
            save.AllPatrollers.Add(new SerializableData.SerializablePatroller(patroller));
        }

        return save;
    }


    public void LoadSavedObject(LevelDataSave save)
    {

        GameStateManager.Instance.Player.ActionPoints = save.playerActionPoints;
        GameStateManager.Instance.PlayerStats.aEnergy = save.playerEnergy;
        GameStateManager.Instance.score = save.playerScore;
        TurnManager.Instance.TurnCount = save.turn;
        TurnManager.Instance.actorIndex = save.turnActorIndex;
        GameStateManager.Instance.objectivesQueue.ForwardToObjective(save.objectiveIndex);
        GameStateManager.Instance.SetScore(GameStateManager.Instance.score);

        // Load Data
        int index = 0;
        foreach (CyberonActor actor in GameStateManager.Instance.AllActors)
        {
            save.AllActors[index].LoadTo(actor);
            index++;
        }

        index = 0;
        foreach (IHackable hackable in GameStateManager.Instance.AllHackables)
        {
            save.AllIHackables[index].LoadTo(hackable);
            index++;
        }

        index = 0;
        foreach (ResourceSilo silo in GameStateManager.Instance.AllResourceSilos)
        {
            save.AllResourceSilos[index].LoadTo(silo);
            index++;
        }

        index = 0;
        foreach (Patroller patroller in GameStateManager.Instance.AllPatrollers)
        {
            save.AllPatrollers[index].LoadTo(patroller);
            index++;
        }

        if (save.phase == 1)
        {
            GameStateManager.Instance.ChangePhase();
        }
    }

    public void SaveGame(bool exportToJason)
    {
        UIManager.Instance.ActivateLoads();
        PlayerPrefs.SetInt("SceneIndex", SceneManager.GetActiveScene().buildIndex);
        //PlayerPrefs.SetInt("Score", GameStateManager.Instance.score);

        LevelDataSave save = CreateSaveObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, save);
        file.Close();

        if (exportToJason)
        {
            string jason = JsonUtility.ToJson(save);
            Debug.Log("Saving as JSON: " + jason);
            // Add filewriting as jason
        }

        UIManager.Instance.DisplaySaveTxt();
    }


    public bool LoadGame()
    {
        if (!File.Exists(path))
        {
            Debug.Log("Save file not found in " + path);
            return false;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        LevelDataSave save = (LevelDataSave)bf.Deserialize(file);
        file.Close();               //Always make sure to close the file stream

        LoadSavedObject(save);
        return true;
    }

    public void ClearSaveGame()
    {
        if (File.Exists(path))
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            directory.Delete(true);
            Directory.CreateDirectory(path);
        }
        else
        {
            Debug.Log("Save file not found in " + path);
        }
    }

    public bool HasSavedGame()
    {
        return File.Exists(path);
    }
}
