using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SSystem<SSystemType> 
    : MonoBehaviour where SSystemType : MonoBehaviour
{
    private static SSystemType _instance;
    private bool _isPersistent = false;

    public static SSystemType Instance
    {
        // Not thread safe
        get 
        {
            if (_instance != null) return _instance;
            // --------------------------------------------------------------------
            // Attributes to find an instance
            string systemName = typeof(SSystemType).ToString();
            var instances = FindObjectsOfType<SSystemType>();
            var count = instances.Length;
            GameObject selectedInstance;

            // --------------------------------------------------------------------
            // More than one instance in Scene
            while (count > 1)
            {
                Destroy(instances[--count]); // prefix '--' to not OutOfBounds errors
            }

            // --------------------------------------------------------------------
            // Only one instance in Scene
            if (count == 1)
            {  
                selectedInstance = instances[0].gameObject;
            }

            // --------------------------------------------------------------------
            // No instance in Scene
            else 
                try // instantiate from prefab
                {
                    selectedInstance = (GameObject)
                        Instantiate(Resources.Load(systemName, typeof(GameObject)));
                } 
                catch (Exception e) // send error and create blank new
                {
                    Debug.LogError("could not instantiate prefab[ " 
                        + systemName + " ]"  + e.Message + "\n" + e.StackTrace);

                    selectedInstance = new GameObject(systemName);
                    selectedInstance.AddComponent<SSystemType>();
                }

            // Set activate, set and return instance
            _instance = selectedInstance.GetComponent<SSystemType>();
           // DontDestroyOnLoad(_instance.gameObject);
            return _instance;
        }
    }

    private void Awake() { 
        if (_isPersistent) DontDestroyOnLoad(this.gameObject); 
    }
}
