using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;



public enum TroopClass
{
    Warrior  = 0,
    Archer   = 1,
    Mage     = 2,
    Gatherer = 3
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour
{
    
    // public Spawner spawner;


    //[SerializeField] public float speed;
    [SerializeField] public float sightRadius;
    [SerializeField] public float damage;
    [SerializeField] public float health;
    [SerializeField] public TroopClass CurrentTroopClass;
    [SerializeField] private TeamNumber thisTeamNumber;
    [SerializeField] private AIDestinationSetter aIDSetter;
    [SerializeField] private SceneBuilder sb;
    protected GameObject[] baseHubs = null;
    private AIPath aiPath;
    public Rigidbody2D rb;
    //public List<GameObject> enemiesInSight = new List<GameObject>();



    protected Vector2 direction;
    private TextMesh healthTextMesh;
    private LayerMask enemiesLayer;
    public Collider2D[] enemiesInSightArray;
    public List<GameObject> closestEnemyTroops;
    private Spawner homeSpawner;
    private Spawner enemySpawner;
   


    public TeamNumber ThisTeamNumber { get => thisTeamNumber; set => thisTeamNumber = value; }
    public AIDestinationSetter AIDSetter { get => aIDSetter; set => aIDSetter = value; }
    public LayerMask EnemiesLayer { get => enemiesLayer; set => enemiesLayer = value; }
    public SceneBuilder Sb { get => sb; set => sb = value; }
    public Spawner HomeSpawner { get => homeSpawner; set => homeSpawner = value; }
    public Spawner EnemySpawner { get => enemySpawner; set => enemySpawner = value; }
    public TextMesh HealthTextMesh { get => healthTextMesh; set => healthTextMesh = value; }
    public AIPath AiPath { get => aiPath; set => aiPath = value; }








    // Use this for initialization
    void Awake()
    {
       
        //Debug.Log("Scenebuilder = " + Sb);
        if (CurrentTroopClass != TroopClass.Gatherer) HealthTextMesh = transform.Find("HealthTextMesh").GetComponent<TextMesh>();
        closestEnemyTroops = new List<GameObject>();
        AIDSetter = GetComponent<AIDestinationSetter>();
        AiPath = GetComponent<AIPath>();
        assignBaseHubs();
        
    }
    void Start()
    {
        

        // troopstate = Tstate(0);


    }
    public void Initialize(TroopClass troopClass, TeamNumber teamNumber)
    {
        CurrentTroopClass = troopClass;
        ThisTeamNumber = teamNumber;
        assignBaseHubs();
        Sb = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
        AIDSetter = GetComponent<AIDestinationSetter>();

    }
    public void assignBaseHubs()
    {
        GameObject[] baseHubs = GameObject.FindGameObjectsWithTag("Base");
        foreach (GameObject basehub in baseHubs)
        {
           // Debug.Log("Basehub: " + basehub + "Base Team Number : " + basehub.GetComponent<Spawner>().thisteamNumber);
            //Debug.Log("Unit: " + this + "Unit Team Number : " + ThisTeamNumber);

            if (basehub.GetComponent<Spawner>().thisteamNumber == ThisTeamNumber)
            {
                //Debug.Log("basehub team number : " + basehub.GetComponent<Spawner>().thisteamNumber);
                HomeSpawner = basehub.GetComponent<Spawner>();
               // Debug.Log("Home BaseHub found");
            }
            else if ((basehub.GetComponent<Spawner>().thisteamNumber != ThisTeamNumber))
            {
               // Debug.Log("basehub team number : " + basehub.GetComponent<Spawner>().thisteamNumber);

                EnemySpawner = basehub.GetComponent<Spawner>();//Debug.Log("Spawner is other team");
               // Debug.Log("Enemy BaseHub found");

            }
            else Debug.Log("Error");
        }
    }
    // Update is called once per frame
    void Update()
    {
        

    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

    }
    public void updateClosestEnemyTroopsList()
    {
        closestEnemyTroops.Clear();
        //Debug.Log("Sb Troops List Count :::" + Sb.getTroopsList().Count);
        foreach (GameObject unit in Sb.getTroopsList())
        {
            if (unit != this.gameObject)
            {
                if (unit.GetComponent<Unit>() != null)
                {
                    Unit unitScript = unit.GetComponent<Unit>();
                    if (unitScript.ThisTeamNumber != ThisTeamNumber) closestEnemyTroops.Add(unit);
                }
            }
        }
        //closestEnemyTroops.AddRange();
        closestEnemyTroops.Sort(SortByDistanceToUnit);
    }
    public virtual void updateBounds()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;
            AstarPath.active.UpdateGraphs(bounds);

        }
        if (GetComponent<CircleCollider2D>() != null)
        {
            Bounds bounds2 = GetComponent<CircleCollider2D>().bounds;
            AstarPath.active.UpdateGraphs(bounds2);
        }
    }
    public virtual void setHealth()
    {
        if (health > 0)
        {
            HealthTextMesh.text = health.ToString();
        }
    }
    public virtual void moveToGoal(GameObject goalObject)
    {
        //if(goalObject!=null) 

        determineSpeed( goalObject);


       // Debug.Log("Unit Max Speed: " + AiPath.maxSpeed);
        
        AIDSetter.target = goalObject.transform;
    }

    public void determineSpeed(GameObject goalObject)
    {
        if (ThisTeamNumber == TeamNumber.t1)
        { 
            if(goalObject.transform.position.x <= transform.position.x) AiPath.maxSpeed = Sb.maxSpeedTowardsBase;
            else AiPath.maxSpeed = Sb.normalMaxSpeed;
        }
       
        if (ThisTeamNumber == TeamNumber.t2)
        {
            if (goalObject.transform.position.x >= transform.position.x) AiPath.maxSpeed = Sb.maxSpeedTowardsBase;
            else AiPath.maxSpeed = Sb.normalMaxSpeed;
        }
            

    }

    public virtual void moveToEnemyBase()
    {
        moveToGoal(EnemySpawner.gameObject);
    }
    public virtual bool isTargetNull()
    {
        if (AIDSetter.target == null) return true;
        else return false;
    }
    public virtual Transform checkForEnemiesInRange()
    {
        enemiesInSightArray = Physics2D.OverlapCircleAll(transform.position, sightRadius, EnemiesLayer);
        Transform closestEnemy = null;
        if (enemiesInSightArray.Length > 0)
        {

            closestEnemy = findClosestEnemyInRange().transform;
        }
        else
        {
            //moveToEnemyBase(); 
        }
        return closestEnemy;
    }
    public virtual GameObject findClosestEnemyInRange()
    {

        float closestEnemyDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        for (int i = 0; i < enemiesInSightArray.Length; i++)
        {
            TroopScript troopScript = enemiesInSightArray[i].gameObject.GetComponent<TroopScript>();
            float tempDist = Vector3.Distance(transform.position, enemiesInSightArray[i].transform.position);


            if (tempDist < closestEnemyDistance && (enemiesInSightArray[i].gameObject != gameObject))
            {
                closestEnemyDistance = tempDist;
                closestEnemy = enemiesInSightArray[i].gameObject;
            }
        }
        if (closestEnemy != null)
        {

            //moveToGoal(closestEnemy);
        }
        else
        {
            // moveToEnemyBase();
        }

        return closestEnemy;
    }
    public virtual bool isSameClass(TroopScript other)
    {
        if (CurrentTroopClass == other.CurrentTroopClass) return true;
        else return false;
    }
    public virtual void takeDamage(float damage)
    {
        health -= damage;
        //Debug.Log(gameObject + "Health : " + health);
        if (health <= 0)
        {
            //Debug.Log("Troops length : " + Sb.getTroopsList().Count);
            
            Sb.destroyTroop(this);
            if (ThisTeamNumber == TeamNumber.t1) ResultLogger.addEnemiesKilled(TeamNumber.t2);
            if (ThisTeamNumber == TeamNumber.t2) ResultLogger.addEnemiesKilled(TeamNumber.t1);
            //Debug.Log(CurrentTroopClass + " died");
        }
    }

    public int SortByDistanceToUnit(GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }


    public virtual bool isTroop()
    {
        if (gameObject.GetComponent<TroopScript>() != null) return true;
        else return false;
    }
    public virtual bool isGatherer()
    {
        if (gameObject.GetComponent<GatherersAI>() != null) return true;
        else return false;
    }
    public List<GameObject> getClosestEnemyTroop()
    {
        updateClosestEnemyTroopsList();
        return closestEnemyTroops;
    }
    public  List<GameObject> getClosestEnemyTroopWithoutUpdate()
    {
        return closestEnemyTroops;
    }
    public virtual TeamNumber getThisTeamNumber()
    {
        return ThisTeamNumber;
    }
    public virtual AIDestinationSetter getAIDSetter()
    {
        return AIDSetter;
    }
   
    public virtual Spawner getEnemySpawner()
    {
        return EnemySpawner;
    }
    public virtual Spawner getHomeSpawner()
    {
        return HomeSpawner;
    }

}



