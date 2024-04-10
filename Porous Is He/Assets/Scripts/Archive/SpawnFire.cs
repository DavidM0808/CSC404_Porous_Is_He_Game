using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFire : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject[] spawnObject;
    public GameObject[] objectClone;

    // Start is called before the first frame update
    void Start()
    {
        spawnThing();
    }

    void Update()
    {
        
    }

    // Update is called once per frame

    void spawnThing()
    {
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            objectClone[0] = Instantiate(spawnObject[0], spawnLocations[i].transform.position, Quaternion.Euler(0, 0, 0));
        }

    }
}
