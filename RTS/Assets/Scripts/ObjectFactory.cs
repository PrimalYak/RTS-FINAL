using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    protected static ObjectFactory instance; // Needed
    [HideInInspector]  public GameObject[] unitsPrefabs;
    [HideInInspector] public Spawner spawner;
    [HideInInspector] public SceneBuilder scene;
    
    void Start()
    {
        scene = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        unitsPrefabs = scene.unitPrefabs;
        spawner = scene.base1.GetComponent<Spawner>();
        //Debug.Log(spawner);

    }
    void Awake()
    {
        instance = this;

    }
    public Unit CreateUnit(TroopClass troopClass, TeamNumber teamNumber)
    {
        
        GameObject unitGO = null;
        Unit unitScript = null;
         
        float randSpawnLoc = Random.Range(-(spawner.spawnRange), spawner.spawnRange);
            //Debug.Log("TroopClass of troop about to be spawned is : " + (int)troopClass);
            unitGO = Instantiate(unitsPrefabs[(int)troopClass], (this.transform.position + new Vector3(0,randSpawnLoc,0)), Quaternion.identity) as GameObject;
            unitScript = unitGO.GetComponent<Unit>();
            unitScript.Initialize(troopClass, teamNumber);
            scene.subtractUnitCost(troopClass, teamNumber);
            scene.getTroopsList().Add(unitGO);
        //Debug.Log("Troop  Team Number : " + unitGO.GetComponent<Unit>().ThisTeamNumber);
        //Debug.Log("Spawner  Team Number : " + spawner.thisteamNumber);

        return unitScript;
    }
   
}