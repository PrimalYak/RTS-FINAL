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
    public SceneBuilder scene;
    private TeamNumber thisTeamNumber;
    private PlayerGOAPAI player2AI;
    private GoapAction goapAction;
    private Spawner spawner;
    public int[] classCounts;
    

    void Start()
    {
        scene = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        thisTeamNumber = GetComponent<PlayerGOAPAI>().teamNumber;
        player2AI = GetComponent<PlayerGOAPAI>();
        spawner = scene.base2.GetComponent<Spawner>();
        classCounts = new int[scene.unitPrefabs.Length];
       // goapAction = GetComponent<GoapAction>();

    }

    // Update is called once per frame
    void Update()
    {
        updateResources();
        updateTroopsList();
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

        }
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
