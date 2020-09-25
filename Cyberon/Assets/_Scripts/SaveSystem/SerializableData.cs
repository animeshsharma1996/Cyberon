using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializableData
{

    [System.Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        public static implicit operator Vector3(SerializableVector3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        public static implicit operator SerializableVector3(Vector3 rValue)
        {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }
    }

    [System.Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(float rX, float rY, float rZ, float rW)
        {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        public static implicit operator Quaternion(SerializableQuaternion rValue)
        {
            return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        public static implicit operator SerializableQuaternion(Quaternion rValue)
        {
            return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }

    }

    [System.Serializable]
    public struct SerializableCyberonActor
    {
        public string name;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public bool isDead;

        public SerializableCyberonActor(CyberonActor actor)
        {
            name = actor.gameObject.name.ToString();
            position = actor.gameObject.transform.position;
            rotation = actor.gameObject.transform.rotation;
            isDead = actor.isDead;
        }

        public void LoadTo(CyberonActor actor)
        {
            if (actor.gameObject.name.ToString() != name)
            {
                Debug.LogError("No such actor found to load.");
                return;
            }
            actor.gameObject.transform.position = position;
            actor.gameObject.transform.rotation = rotation;
            actor.isDead = isDead;
        }
    }

    [System.Serializable]
    public struct SerializableIHackable 
    {
        public string name;
        public bool isHacked;

        public SerializableIHackable(IHackable hackable)
        {
            name = hackable.GetGameObject().name.ToString();
            isHacked = hackable.IsHacked();
        }

        public void LoadTo(IHackable hackable)
        {
            if (hackable.GetGameObject().name.ToString() != name)
            {
                Debug.LogError("No such IHackable found to load.");
                return;
            }

            if (isHacked)
            {
                hackable.Hack();
            }
        }
    }

    [System.Serializable]
    public struct SerializableResourceSilo
    {
        public string name;
        public bool isDepleted;
        public int energyCount;

        public SerializableResourceSilo(ResourceSilo silo)
        {
            name = silo.gameObject.name.ToString();
            isDepleted = silo.isDepleted;
            energyCount = silo.energyCount;
        }

        public void LoadTo(ResourceSilo silo)
        {
            if (silo.gameObject.name.ToString() != name)
            {
                Debug.LogError("No such Silo found to load.");
                return;
            }

            if (isDepleted)
            {
                silo.Deplete();
            }
        }
    }


    [System.Serializable]
    public struct SerializablePatroller
    {
        public string name;
        public int patrolPoint;
        public SerializablePatroller(Patroller patroller)
        {
            name = patroller.gameObject.name.ToString();
            patrolPoint = patroller.currentPatrolPoint;
        }

        public void LoadTo(Patroller patroller)
        {
            if (patroller.gameObject.name.ToString() != name)
            {
                Debug.LogError("No such Silo found to load.");
                return;
            }

            patroller.currentPatrolPoint = patrolPoint;
        }
    }
}
