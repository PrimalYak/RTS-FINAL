using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TaskExecutor : MonoBehaviour
{
    // Start is called before the first frame update
    public SceneBuilder scene;
    public Spawner spawner;
    public TeamNumber teamNumber;
    public WorldStateUpdater WSU;

    void Start()
    {
        WSU = gameObject.GetComponent<WorldStateUpdater>();
        Debug.Log("World State Updater" + WSU);
        teamNumber = WSU.thisTeamNumber;

        scene = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        spawner = scene.base2.GetComponent<Spawner>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void tryPurchaseUnit(TroopClass troopClass)
    {
        
        //Debug.Log("Spawner : " + spawner);
        spawner.purchaseUnit(troopClass);
    }
    public void commandTroopsAttackUnit()
    {

    }

}
