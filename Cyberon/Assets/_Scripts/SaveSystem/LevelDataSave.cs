using System.Collections.Generic;

[System.Serializable]
public class LevelDataSave
{
    public int phase;
    public int turn;
    public int playerActionPoints;
    public int playerEnergy;
    public int playerScore;
    public int turnActorIndex;
    public int objectiveIndex;

    public List<SerializableData.SerializableCyberonActor> AllActors = new List<SerializableData.SerializableCyberonActor>();
    public List<SerializableData.SerializableIHackable> AllIHackables = new List<SerializableData.SerializableIHackable>();
    public List<SerializableData.SerializableResourceSilo> AllResourceSilos = new List<SerializableData.SerializableResourceSilo>();
    public List<SerializableData.SerializablePatroller> AllPatrollers = new List<SerializableData.SerializablePatroller>();
}
