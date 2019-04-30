using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Panda;

public class BehaviourTree : MonoBehaviour
{
    public enum gameState
    {

    }
    [SerializeField] TeamNumber teamNumber = TeamNumber.t1;
    [SerializeField] public float baseProximitySize = 4;

    private SceneBuilder sb;
    private GameObject gatherer;
    private GatherersAI gathererScript;
    TroopClass troopClassToSpawn;
    //private List<List<GameObject>> listOfGathererResourceList;
    public float startQueueRate = 1f;
    private float queueRate;
    private List<Resource> resourceCopy;
    public  List<GameObject> Resources;
    public  List<GameObject> enemyTroops;
    public  List<GameObject> allyTroops;
    private List<GameObject> enemiesCloseToBase;
    private List <GameObject> alliesCloseToBase;
  
    public LayerMask enemiesLayer;
    private int[] classCounts;
    public Spawner spawner;
    public Spawner enemySpawner;
    public GameObject[] troopsLeftToManage;
    public int troopManaged;
    public int troopsToManageCount;
    // Start is called before the first frame update
    void Start()
    {
        sb = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
        foreach(GameObject basehub in bases)
        {

            if (basehub.GetComponent<Spawner>().thisteamNumber == teamNumber)
            {
                Debug.Log("basehub team number : "+basehub.GetComponent<Spawner>().thisteamNumber);
                spawner = basehub.GetComponent<Spawner>();
                 Debug.Log("Home BaseHub found");
            }
            else if ((basehub.GetComponent<Spawner>().thisteamNumber != teamNumber))
            {
                Debug.Log("basehub team number : " + basehub.GetComponent<Spawner>().thisteamNumber);

                enemySpawner = basehub.GetComponent<Spawner>();//Debug.Log("Spawner is other team");
                Debug.Log("Enemy BaseHub found");

            }
            else Debug.Log("Error");
        }
        enemyTroops = new List<GameObject>();
        allyTroops = new List<GameObject>();
        enemiesCloseToBase = new List<GameObject>();
        alliesCloseToBase = new List<GameObject>();

        classCounts = new int[4];
        updateEnemyTroopsList();
        // for (int i = 0; i < gatherers.Length; i++) gatherer.Add(initialGatherers[i]);

        Resources = new List<GameObject>();

        queueResources();
        // queueResources();
    }
    private void drawBaseProximity()
    {
        Collider2D[] enemyColsCloseToBase = null;
        alliesCloseToBase.Clear();
        enemiesCloseToBase.Clear();
        enemyColsCloseToBase = Physics2D.OverlapCircleAll(spawner.gameObject.transform.position, baseProximitySize, enemiesLayer);
        foreach(Collider2D enemyCol in enemyColsCloseToBase)
        {
            if(enemyCol.gameObject.GetComponent<Unit>()!=null)
            {
                Unit unitScript = enemyCol.gameObject.GetComponent<Unit>();
                if ((unitScript.ThisTeamNumber == teamNumber))// && (!alliesCloseToBase.Contains(unitScript.gameObject)))
                    alliesCloseToBase.Add(enemyCol.gameObject);
                else if ((unitScript.ThisTeamNumber != teamNumber))// && (!enemiesCloseToBase.Contains(unitScript.gameObject)))
                    enemiesCloseToBase.Add(enemyCol.gameObject);
            }
        }
        orderBaseProxList();
    }
    private void orderBaseProxList()
    {
        enemiesCloseToBase.Sort(SortByDistanceToBaseHub);
    }
    [Task]
    public void initiateTroopsLeftToManage()
    {
        troopsLeftToManage = null;
        troopsLeftToManage = new GameObject[allyTroops.Count];
    }
    [Task]
    public bool stillTroopsToManage()
    {
        if (hasAllyTroops())
        {
            if (getTroopsToManageCount() > 0) return true;
            else return false;
        }
        else return false;

        
    }
    [Task]
    public void initializeMAT()
    {
        updateEnemyTroopsList();
        troopsToManageCount = allyTroops.Count;
        Task.current.Succeed();
    }
    public void decreaseTroopsLeftToManage()
    {
        troopsToManageCount--;
    }
    public int getTroopsToManageCount()
    {
        return troopsToManageCount;
    }

    [Task]
    public void manageTroop()
    {
        GameObject unitGO = allyTroops[getTroopsToManageCount()-1];
        if (isUnit(unitGO))
        {
            Unit unitScript = unitGO.GetComponent<Unit>();

            if (unitScript.isTroop()) // Is a Troop
            {
                //Debug.Log("IS TROOP :: : : ");
                TroopScript troop = unitGO.GetComponent<TroopScript>();
                bool targetFound = false;
                if(enemyTroopsAlive())
                {
                    foreach (GameObject enemyGO in unitScript.getClosestEnemyTroopWithoutUpdate())
                    {
                        Debug.Log("Behaviour Tree closestEnemy Troop Count " + unitScript.getClosestEnemyTroopWithoutUpdate().Count);

                        Unit enemyUnitScript = enemyGO.GetComponent<Unit>();

                        if ((enemyUnitScript.CurrentTroopClass == sb.getMatchups()[unitScript.CurrentTroopClass]) || (enemyUnitScript.CurrentTroopClass == TroopClass.Gatherer))
                        {
                            troop.moveToGoal(enemyGO);
                            targetFound = true;
                            Task.current.Succeed();
                            break;

                        }

                    }
                    if (targetFound == false)
                    {
                        troop.moveToGoal(unitScript.EnemySpawner.gameObject);
                        Task.current.Succeed();

                    }
                }
                else
                {
                    troop.setGoal(unitScript.EnemySpawner.gameObject);
                    Task.current.Succeed();

                }


            }
            else if (unitScript.isGatherer()) // is a Gatherer
            {
                //GatherersAI gatherer = unitGO.GetComponent<GatherersAI>();
                //if (gatherer.closestResources.First().gameObject != null)
                //{
                //    gatherer.setGoal(gatherer.closestResources.First().gameObject);
                    Task.current.Succeed();

                //}
                //else Task.current.Fail();
            }
            else
            {
                Debug.Log("Error in managing troops");
                Task.current.Fail();
            }
            decreaseTroopsLeftToManage();

        }
        else
        {
            Debug.Log("Non unit in allyTroops list");
            Task.current.Fail();
        }
    }
    [Task]
    public bool areEnemiesNearBase()
    {
        drawBaseProximity();
        Debug.Log("Number of enemies near base" + enemiesCloseToBase.Count);
        Debug.Log("Number of allies near base" + alliesCloseToBase.Count);

        if (enemiesCloseToBase.Count > 0) return true;
        else return false;
    }

    [Task]
    public void callDefendBase()
    {
        foreach (GameObject allyTroop in allyTroops)
        {
            drawBaseProximity();
            allyTroop.GetComponent<Unit>().moveToGoal(enemiesCloseToBase.First());
        }
        Task.current.Succeed();
    }

    [Task]
    public void spawnGatherer()
    {
        if(spawner.purchaseUnit(TroopClass.Gatherer))
        Task.current.Succeed();
        else Task.current.Fail();

    }
    [Task]
    public void spawnWarrior()
    {
        if (spawner.purchaseUnit(TroopClass.Warrior))
        Task.current.Succeed();
        else Task.current.Fail();
    }
    [Task]
    public void spawnArcher()
    {
        if (spawner.purchaseUnit(TroopClass.Archer))
            Task.current.Succeed();
        else Task.current.Fail();

    }
    [Task]
    public void spawnMage()
    {
        if (spawner.purchaseUnit(TroopClass.Mage))
            Task.current.Succeed();
        else Task.current.Fail();

    }

    [Task]
    public void commandTroopAttack(Unit unitScript, GameObject enemyTroop)
    {
        unitScript.moveToGoal(enemyTroop);
        Task.current.Succeed();

    }
    [Task]
    public bool enemyTroopsAlive()
    {
        updateEnemyTroopsList();
        if (enemyTroops.Count > 0) return true;
        else return false;
    }
    [Task]
    public bool resourcesActive()
    {
        if (Resources.Count > 0) return true;
        else return false;
    }

    
    [Task]
    void SpawnCounterTroop()
    {
        TroopClass counterclasstospawn = findCounterClass();
        switch (counterclasstospawn)
        {
            case TroopClass.Warrior:
                spawnWarrior();
                Task.current.Succeed();
                break;
            case TroopClass.Archer:
                spawnArcher();
                Task.current.Succeed();
                break;

            case TroopClass.Mage:
                spawnMage();
                Task.current.Succeed();
                break;

            default:

                Task.current.Fail();
                break;
        }
       
    }
    //void SpawnCounterTroop()
    //{
    //    //spawnWarrior();
    //    Task.current.Succeed();
    //}
    public TroopClass findCounterClass()
    {
        TroopClass counterClass = sb.matchups[sb.matchups[getMostCommonEnemyClass()]];
        return counterClass;
    }
    
    public TroopClass getMostCommonEnemyClass()
    {
        updateEnemyTroopsList();
        TroopClass mostCommonClass = TroopClass.Warrior;

        for (int i=0;i<classCounts.Length-1;i++)
        {
            if(classCounts[i] > classCounts[i+1])
            {
                mostCommonClass = (TroopClass)i;
            }
        }
        return mostCommonClass;
    }
    
    public void manageActiveTroops()
    {
        Debug.Log("allyTroops count = " + allyTroops.Count);
        foreach(GameObject unitGO in allyTroops)
        {
            
        }
    }
    [Task]
    public void setUnitGoal(Unit unit, GameObject goal)
    {
        unit.moveToGoal(goal);
        Task.current.Succeed();
    }
    [Task]
    public void findGoal(Unit unit)
    {

    }
    [Task]
    public bool hasAllyTroops()
    {
        if (allyTroops.Count > 0) return true;
        else return false;
    }
    private bool isUnit(GameObject go)
    {
        if (go.GetComponent<Unit>() != null) return true;
        else return false;
    }
    

// Update is called once per frame
    void FixedUpdate()
    {
        updateEnemyTroopsList();
        drawBaseProximity();
    }
    private void updateEnemyTroopsList()
    {
        enemyTroops.Clear();
        allyTroops.Clear();
        foreach (GameObject unit in sb.getTroopsList())
        {
            if (unit.GetComponent<Unit>() != null)
            {
                Debug.Log("TroopScript not null");

                Unit unitScript = unit.GetComponent<Unit>();

                if ((unitScript.ThisTeamNumber == teamNumber)) allyTroops.Add(unit);
                else if ((unitScript.ThisTeamNumber != teamNumber)) enemyTroops.Add(unit);
                else Debug.Log("Error adding troop to ally/enemy lists");


            }
            else Debug.Log("Unit Script cannot be found");
        }
        updateClassCounts();

        //Debug.Log("AllyTroops length  = " + allyTroops.Count);
        //Debug.Log("EnemyTroops length = " + enemyTroops.Count);
    }

    private void updateClassCounts()
    {
        foreach(GameObject unit in enemyTroops)
        {
            TroopClass troopClass = TroopClass.Gatherer;
           
            troopClass = unit.GetComponent<Unit>().CurrentTroopClass;

            classCounts[(int)troopClass]++;
        }
    }

    public void callQueueAtRate()
    {
        if (queueRate <= 0)
        {
            if(sb.getResourcesList().Count > 0)
            {
                queueResources();
                queueRate = startQueueRate;
            }
        }
        else queueRate -= Time.deltaTime;
    }
    public void queueResources()
    {
       //for(int i=0; i<sb.Resources.Count;i++)
       //{
       //     Resources.Add(sb.Resources[i]);
       //}
       if(Resources.Count>1)
        {
            Resources.Sort(SortByDistanceToGatherer);
        }

    }
    int SortByDistanceToGatherer(GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - gatherer.transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - gatherer.transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }
    int SortByDistanceToUnit(GameObject distanceTo,GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - distanceTo.transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - distanceTo.transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }
    int SortByDistanceToBaseHub(GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - spawner.gameObject.transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - spawner.gameObject.transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawner.gameObject.transform.position, baseProximitySize);
    }


}
