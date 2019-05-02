using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using Tayx.Graphy;
public enum TeamNumber
{
    t1 = 1, t2 = 2
}
public class Spawner : MonoBehaviour
{
   
    private int troopCost;
    public float health;
    public GameObject[] troopPrefabs = null;
    //public GameObject gathererPrefab;
    [SerializeField]  public TeamNumber thisteamNumber;
    [HideInInspector] public GameObject GM;
    [HideInInspector] public TroopScript troopScript;
    [HideInInspector] public GatherersAI gathererScript;
    [HideInInspector] public TroopClass troopClass2Spawn;
    [HideInInspector] public GameObject base1Prefab;
    [HideInInspector] public GameObject base2Prefab;
    [HideInInspector] public SceneBuilder scene;
    [HideInInspector] public ObjectFactory objectFactory;
    public List<GameObject> allyTroops;
    
    public List<GameObject> troops;

    [SerializeField] public float spawnRange = 3;

    


    void Awake()
    {
        objectFactory = GetComponent<ObjectFactory>();
    }
    // Use this for initialization
    void Start()
    {
        GM = GameObject.FindWithTag("GameManager");
        scene = GM.GetComponent<SceneBuilder>();
    }
    // Update is called once per frame
    public void autoSpawnTroops()
    {
      //  spawnRandTroop();
       
    }
    public void purchaseRandTroop()
    {
         int randTroop = Random.Range(0, troopPrefabs.Length);
        //int randTroop = 0;
        troopClass2Spawn = (TroopClass)randTroop;
       // troopClass2Spawn = TroopClass.Gatherer;
        
            purchaseUnit(troopClass2Spawn);
        
    }
    public bool purchaseUnit(TroopClass troopClass)
    {
        if (scene.canAffordTroop(thisteamNumber, troopClass))
        {
            if (trySpawnUnit(troopClass)) return true;
            else return false;
        }
        else return false;
    }
    
    public bool trySpawnUnit(TroopClass troopClass)
    {
        Unit localUnitScript = objectFactory.CreateUnit(troopClass, thisteamNumber);
        if (localUnitScript != null) return true;
        else return false;
    }

    void Update()
    {
       // autoPlay(TeamNumber.t2);

    }
    public void autoPlay(TeamNumber teamNumber)
    {
        if(thisteamNumber == teamNumber)
        {
            purchaseRandTroop();
            
            orderTroops();
        }
        
    }
    public void getAllies()
    {
        Debug.Log("getAllies called");
        troops.Clear();
        allyTroops.Clear();
        troops.AddRange(scene.getTroopsList());
        foreach(GameObject unit in troops)
        {
            if(unit.GetComponent<GatherersAI>() == null)
            {
                if (unit.GetComponent<Unit>().ThisTeamNumber == thisteamNumber)
                {
                    allyTroops.Add(unit);
                }
            }
        }
    }
    public void orderTroops()
    {
        getAllies();
        Debug.Log("OrderTroops called");
        foreach (GameObject unitGO in allyTroops)
        {
            Unit unitScript = unitGO.GetComponent<Unit>();

            if (unitScript.getClosestEnemyTroop().First().GetComponent<Unit>().CurrentTroopClass == scene.matchups[unitScript.CurrentTroopClass])
            {
                unitScript.moveToGoal(unitScript.getClosestEnemyTroop().First());
            }
            else unitScript.moveToGoal(unitScript.EnemySpawner.gameObject);
        }
    }
    public void spawnGatherer()
    {
        //GameObject localObject = instantiate(gathererPrefab, this.transform.position, Quaternin.identity);
    }
    
    public void takeDamage(float damage)
    {
        health -= damage;
        checkAlive();
    }

    public void checkAlive()
    {
        if (health <= 0)
        {

            Destroy(gameObject);

            if (thisteamNumber == TeamNumber.t1) ResultLogger.setPlayerWinnerNumber(TeamNumber.t2);
            if (thisteamNumber == TeamNumber.t2) ResultLogger.setPlayerWinnerNumber(TeamNumber.t1);

            Debug.Log("Result Loggggeeedd");

            logResultsToLogger();


            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }

    public void logResultsToLogger()
    {
        ResultLogger.logGameTime(scene.gameLength);
        ResultLogger.logMaxTroopCount(scene.maxTroopCount);
        ResultLogger.logAllDataToFile();
    }

    public GameObject MakeTroop(TroopClass _troopClass)
    {
        Spawner spawner = gameObject.GetComponent<Spawner>();
        GameObject clone = (GameObject)Instantiate(spawner.troopPrefabs[(int)_troopClass]);
        clone.GetComponent<Unit>().CurrentTroopClass = _troopClass;
        return clone;
    }
}
    
