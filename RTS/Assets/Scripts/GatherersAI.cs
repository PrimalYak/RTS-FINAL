using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;


[RequireComponent(typeof(Seeker))]


public class GatherersAI : Unit
{
    // Start is called before the first frame update

    // public Transform target;
    public float startTargetNullTimer = 3f;
    private float targetNullTimer;
    public float startResourceMiningTime = 2f;
    private float resourceMiningTime;
    public bool carryingResource = false;
    public int carriedResourceValue;
    public float startIdleTimer = 2f;
    private float idleTimer;
   // private TeamNumber thisTeamNumber;
    private Unit unitScript;
    

    private TroopClass currentTroopClass;

    public List<GameObject> closestResources;


    //public LinkedList<GameObject> resourcesToSearch;
    public List<GameObject> resourcesOrdererd;
    public GathererState gState;
    //private SceneBuilder sb;
    //private AIDestinationSetter AIDSetter;



   
   // public SceneBuilder Sb { get => sb; set => sb = value; }
   // public TeamNumber ThisTeamNumber { get => thisTeamNumber; set => thisTeamNumber = value; }
    public TroopClass CurrentTroopClass { get => currentTroopClass; set => currentTroopClass = value; }
    //public AIDestinationSetter AIDSetterAccess { get => AIDSetter; set => AIDSetter = value; }
    public Unit UnitScript { get => unitScript; set => unitScript = value; }

    public enum GathererState
    {
        Idle,
        MovingToResource,
        CollectingResource,
        ReturningResourceToBase
    }


    void Awake()
    {
        base.AiPath = GetComponent<AIPath>();
        targetNullTimer = startTargetNullTimer;
        idleTimer = startIdleTimer;
       
        //AIDSetterAccess = base.AIDSetter;
        ThisTeamNumber = base.ThisTeamNumber;
       
        closestResources = new List<GameObject>();
        base.rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        //base.assignBaseHubs();
        UnitScript = GetComponent<Unit>();
        
        gState = GathererState.Idle;
       // resourcesToSearch = new LinkedList<GameObject>();
        carryingResource = false;



        //searchForTarget();
        //if(!isTargetNull())
        //{
        //    seeker.StartPath(transform.position, target.position, OnPathComplete);
        //    StartCoroutine(UpdatePath());

        //}

    }
    void Update()
    {
       // Debug.Log("Closest Resources count : " + closestResources.Count);

        if (!hasTarget())
        {
            gState = GathererState.Idle;

            
        }
        if (gState == GathererState.Idle)
        {
            //Debug.Log("Idle Timer" + idleTimer);
            if (idleTimer <= 0)
            {
                searchForTarget();
                idleTimer = startIdleTimer;
            }
            else
            {
                idleTimer -= Time.deltaTime;
            }

        }
    }
    public void addResourceToSearch(GameObject resource)
    {
        //resourcesToSearch.AddFirst(resource);
        //Debug.Log(resourcesToSearch.Peek().transform);
    }
    public GathererState getState()
    {
        return gState;
    }
    public bool searchForTarget()
    {
        if (carryingResource)
        {
            return searchForHomeBase();
        }
        else if (!carryingResource)
        {
            return searchForResource();
        }
        else return false;
    
    }
    public bool isTargetNull()
    {
        if (getAIDSetter().target == null)
        {
           
            return true;

        }
        else return false;
    }
    public AIDestinationSetter getAIDSetter()
    {
        
        return base.AIDSetter;
    }
    public SceneBuilder getSceneBuilder()
    {
        return base.Sb;
    }

    public bool getUpdatedClosestResources()
    {
        //Debug.Log(getSceneBuilder());
        Debug.Log("Resources list: " + getSceneBuilder().getResourcesList().Count);
        if (getSceneBuilder().getResourcesList() != null)
        {
            closestResources.Clear();
            closestResources.AddRange(getSceneBuilder().getResourcesList());
            closestResources.Sort(unitScript.SortByDistanceToUnit);
            return true;
        }
        else return false;
    }
    private void updateAStarBounds()
    {
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
        AstarPath.active.UpdateGraphs(bounds);
    }
   
    
    public bool searchForResource()
    {
        if (getUpdatedClosestResources())
        {
            if (closestResources.Count > 0)
            {
                GameObject targetGameObject = closestResources.First();
                Debug.Log("Gatherer's target resource: " + targetGameObject);
                moveToGoal(targetGameObject);
                gState = GathererState.MovingToResource;
                return true;
            }
            else
            {
                gState = GathererState.Idle;
                Debug.Log("No Resources in vicinity");
                return false;
            }
        }
        else return false;
    }
    public bool searchForHomeBase()
    {
        if (base.HomeSpawner != null)
        {
            moveToGoal(base.HomeSpawner.gameObject);
            gState = GathererState.ReturningResourceToBase;
            return true;
        }
        else return false;
    }
    public void moveToGoal(GameObject goalObject)
    {
        Debug.Log("Move To Goal goal object: "+ goalObject);
        UnitScript.moveToGoal(goalObject);
    }
    void FixedUpdate()
    {
        updateAStarBounds();
        //Debug.Log(state.ToString());

        //for(int i = 0; i< resourcesToSearch.Count;i++)
        //{
        //    //Debug.Log(resourcesToSearch.ToString());

        //}
    }
    // Update is called once per frame

    
    public bool hasTarget()
    {
        if (getAIDSetter().target!=null)
        {
            //searchForResource();
            return false;
        }
        else return true;
    }
    public void setGoal(GameObject go)
    {
        moveToGoal(go);
    }
    public void setTargetResource(GameObject resource)
    {
        moveToGoal(resource);
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("COLLISION!!!!!!!!!!!");
        if (other.tag == ("Resource") && !carryingResource)
        {
            //Debug.Log("Resource hit");
            Resource resource = other.gameObject.GetComponent<Resource>();
            GameObject textMeshGO = new GameObject();

            TextMesh tm = textMeshGO.AddComponent<TextMesh>() as TextMesh;
            //textMeshGO.AddComponent(tm);
            gState = GathererState.CollectingResource;
            tm.text = ("Extracting resource");
           
                resourceMiningTime = startResourceMiningTime;
                carriedResourceValue = resource.resourceValue;
                carryingResource = true;
                Destroy(tm);
                for(int i=0;i< getSceneBuilder().getResourcesList().Count;i++)
                {
                    Resource resourceInList = getSceneBuilder().getResourcesList()[i].GetComponent<Resource>();
                    if (resourceInList.resourceIndex == resource.resourceIndex)
                    {
                    getSceneBuilder().getResourcesList().RemoveAt(i);
                    }
                
                }
            getSceneBuilder().getResourcesList().Remove(other.gameObject);
                Destroy(other.gameObject);
               
            
             resourceMiningTime -= Time.deltaTime;
            //int playerGoldCount = getPlayerGoldCounts[(int)teamNumber - 1];
            searchForHomeBase();
        }
        else if(other.tag == ("Troop"))
        {
            TroopScript ts = other.GetComponent<TroopScript>();
            if(ts.ThisTeamNumber != ThisTeamNumber)
            {
                takeDamage(ts.damage);
            }
        }
        else if(other.tag == ("Base") && carryingResource)
        {
            carryingResource = false;
            getSceneBuilder().setPlayerGoldCount(getSceneBuilder().getPlayerGoldCountsArray()[(int)ThisTeamNumber - 1],carriedResourceValue);
            carriedResourceValue = 0;
            //Debug.Log("Resource deposited");]
            //Debug.Log(base.ThisTeamNumber);
            ResultLogger.addGoldNodeGatherer(base.ThisTeamNumber);
            gState = GathererState.Idle;
           // searchForResource();        
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("STAAAAYYYY");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log("EXXXIIIITTT");

    }
    int SortByDistanceToUnit(GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }
}
