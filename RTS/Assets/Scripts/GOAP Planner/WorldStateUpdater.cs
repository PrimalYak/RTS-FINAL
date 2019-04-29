using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldStateUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> Resources;
    public List<GameObject> enemyTroops;
    public List<GameObject> allyTroops;
    public List<GameObject> Units;
    private List<GameObject> enemiesCloseToBase;
    private List<GameObject> alliesCloseToBase;
    private float baseProximitySize = 5f;
    public SceneBuilder scene;
    public TeamNumber thisTeamNumber = TeamNumber.t2;
    public LayerMask enemiesLayer;
    public PlayerGOAPAI player2AI;
    public GoapAction goapAction;
    public Spawner spawner;
    public int[] classCounts;
    

    void Start()
    {
        scene = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        // thisTeamNumber = GetComponent<PlayerGOAPAI>().teamNumber;
        thisTeamNumber = TeamNumber.t2;
        Debug.Log("WSU Team NUmber: " + thisTeamNumber);
        player2AI = GetComponent<PlayerGOAPAI>();
        spawner = scene.base2.GetComponent<Spawner>();
        Debug.Log("WSU spawner assigned" + spawner);
        classCounts = new int[scene.unitPrefabs.Length];
        enemiesCloseToBase = new List<GameObject>();
        alliesCloseToBase = new List<GameObject>();

        // goapAction = GetComponent<GoapAction>();

    }

    // Update is called once per frame
    void Update()
    {
        updateResources();
        updateTroopsList();
        drawBaseProximity();
        checkConditionsSatisfied();
    }
    public void updateResources()
    {
        Resources.Clear();
        Resources.AddRange(scene.getResourcesList());
    }
    public void updateTroopsList()
    {
        Units.Clear();
        Units.AddRange(scene.getTroopsList());
        partitionTroopsList();
        orderEnemiesByDistanceToBase();
        updateClassCounts();
    }
    public bool areEnemiesNearBase()
    {
        drawBaseProximity();
        Debug.Log("Number of enemies near base" + enemiesCloseToBase.Count);
        Debug.Log("Number of allies near base" + alliesCloseToBase.Count);

        if (enemiesCloseToBase.Count > 0) return true;
        else return false;
    }
    private void drawBaseProximity()
    {
        Collider2D[] enemyColsCloseToBase = null;
        alliesCloseToBase.Clear();
        enemiesCloseToBase.Clear();
        enemyColsCloseToBase = Physics2D.OverlapCircleAll(spawner.gameObject.transform.position, baseProximitySize, enemiesLayer);
        foreach (Collider2D enemyCol in enemyColsCloseToBase)
        {
            if (enemyCol.gameObject.GetComponent<Unit>() != null)
            {
                Unit unitScript = enemyCol.gameObject.GetComponent<Unit>();
                if ((unitScript.ThisTeamNumber == thisTeamNumber))// && (!alliesCloseToBase.Contains(unitScript.gameObject)))
                    alliesCloseToBase.Add(enemyCol.gameObject);
                else if ((unitScript.ThisTeamNumber != thisTeamNumber))// && (!enemiesCloseToBase.Contains(unitScript.gameObject)))
                    enemiesCloseToBase.Add(enemyCol.gameObject);
            }
        }
        orderBaseProxList();
    }
    private void orderBaseProxList()
    {
        enemiesCloseToBase.Sort(SortByDistanceToBaseHub);
    }
   
    private void updateClassCounts()
    {
        foreach (GameObject unit in enemyTroops)
        {
            TroopClass troopClass = TroopClass.Gatherer;

            troopClass = unit.GetComponent<Unit>().CurrentTroopClass;

            classCounts[(int)troopClass]++;
        }
    }

    public void partitionTroopsList()
    {
        allyTroops.Clear();
        enemyTroops.Clear();
        foreach (GameObject unitGO in Units)
        {
            TeamNumber unitTeamNumber =  unitGO.GetComponent<Unit>().ThisTeamNumber;
            if (thisTeamNumber == unitTeamNumber) allyTroops.Add(unitGO);
            else if (thisTeamNumber != unitTeamNumber) enemyTroops.Add(unitGO);
            Debug.Log("Unit : " + unitGO + " Unit TeamNumber : " + unitTeamNumber + " WSU teamNumber " + thisTeamNumber);

        }
        Debug.Log("Ally Troops : " + allyTroops.Count);
        Debug.Log("Enemy Troops" + enemyTroops.Count);
    }
    public void checkConditionsSatisfied()
    {
        if(enemyTroops.Count>0) player2AI.setEffectEnemiesAlive(true);
        else player2AI.setEffectEnemiesAlive(false);


        if (allyTroops.Count>0) player2AI.setEffectAlliesAlive(true);
        else player2AI.setEffectAlliesAlive(false);

        if(Resources.Count>0) player2AI.setEffectResourcesToGather(true);
        else player2AI.setEffectResourcesToGather(false);

        if (scene.getPlayerGoldCountsArray()[(int)TeamNumber.t2-1] > scene.troopCosts[TroopClass.Warrior]) player2AI.setEffectWarriorCost(true);
        else player2AI.setEffectWarriorCost(false);

        if (scene.getPlayerGoldCountsArray()[(int)TeamNumber.t2-1] > scene.troopCosts[TroopClass.Archer]) player2AI.setEffectArcherCost(true);
        else player2AI.setEffectArcherCost(false);

        if (scene.getPlayerGoldCountsArray()[(int)TeamNumber.t2-1] > scene.troopCosts[TroopClass.Mage]) player2AI.setEffectMageCost(true);
        else player2AI.setEffectMageCost(false);

        if (scene.getPlayerGoldCountsArray()[(int)TeamNumber.t2-1] > scene.troopCosts[TroopClass.Gatherer]) player2AI.setEffectGathererCost(true);
        else player2AI.setEffectGathererCost(false);

        if (enemiesCloseToBase.Count > 0) player2AI.setEffectEnemiesNearBase(true);
        else player2AI.setEffectEnemiesNearBase(false);


    }
    public void orderEnemiesByDistanceToBase()
    {
        enemyTroops.Sort(SortByDistanceToBaseHub);
    }
    int SortByDistanceToBaseHub(GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - spawner.gameObject.transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - spawner.gameObject.transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }
}
