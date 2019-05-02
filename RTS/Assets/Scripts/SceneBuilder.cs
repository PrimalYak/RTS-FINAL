using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneBuilder : MonoBehaviour {

    // Use this for initialization
    public Text p1GoldDisp;
    public Text p2GoldDisp;
    public Text p1AllyTroopsCount;
    public Text p2AllyTroopsCount;
    public Text p1EnemyTroopsCount;
    public Text p2EnemyTroopsCount;
    public Text troopCount;
    public int maxTroopCount = 0;

    public float gameLength = 0f;
    public float maxSpeedTowardsBase = 2f;
    public float normalMaxSpeed = 1f; 

    public GameObject base1;
    public GameObject base2;
    public GameObject[] bases;
    public GameObject resourceGameObject;
    [HideInInspector] public Resource resource;

    private int p1GoldCount;
    private int p2GoldCount;
    public int numberOfPlayers = 2;

    private int[] playerGoldCounts;


    [HideInInspector] public BehaviourTree bt;

    public int goldTimer;
    [SerializeField] public GameObject[] unitPrefabs;
    [SerializeField] public int goldIncrement;

    [SerializeField] public int[] playerBonusGold;
    


    [SerializeField] public float startResourceSpawnTimer = 5f;
    [SerializeField] public int warriorCost;
    [SerializeField] public int archerCost;
    [SerializeField] public int mageCost;
    [SerializeField] public int gathererCost;
    [SerializeField] public int maxResourceNumber = 4;
    private float resourceSpawnTimer;

    public Dictionary<TroopClass, int> troopCosts;
    public Dictionary<TroopClass, TroopClass> matchups;

    private List<GameObject> Resources;

    private List<GameObject> Troops;

    private int resourceSpawnIndex = 0;
    

    [Header("X Spawn Range")]
    public float xMin = -10;
    public float xMax = 10;

    // the range of y
    [Header("Y Spawn Range")]
    public float yMin = -5;
    public float yMax = 5;

    void Awake()
    {
        troopCosts = new Dictionary<TroopClass, int>();
        matchups = new Dictionary<TroopClass, TroopClass>();
        Resources = new List<GameObject>();
        Troops = new List<GameObject>();
        


        bases = GameObject.FindGameObjectsWithTag("Base");
        foreach(GameObject baseHub in bases)
        {
            if (baseHub.GetComponent<Spawner>().thisteamNumber == TeamNumber.t1) base1 = baseHub;
            else if(baseHub.GetComponent <Spawner>().thisteamNumber == TeamNumber.t2) base1 = baseHub;
        }

        troopCosts.Add(TroopClass.Warrior, warriorCost);
        troopCosts.Add(TroopClass.Archer, archerCost);
        troopCosts.Add(TroopClass.Mage, mageCost);
        troopCosts.Add(TroopClass.Gatherer, gathererCost);

        matchups.Add(TroopClass.Warrior, TroopClass.Archer);
        matchups.Add(TroopClass.Archer, TroopClass.Mage);
        matchups.Add(TroopClass.Mage, TroopClass.Warrior);
    }
    void Start()
    {
        bt = GameObject.FindWithTag("Player1").GetComponent<BehaviourTree>();
        resource = resourceGameObject.GetComponent<Resource>();
        ResultLogger.assignLoggerVariables();
        ResultLogger.tryCreateFile();


        p1GoldCount = 0;
        p2GoldCount = 0;
        playerGoldCounts = new int[numberOfPlayers];

        for(int i = 0; i<playerGoldCounts.Length;i++)
        {
            playerGoldCounts[i] = 0;
        }

        displayGoldCount();


        InvokeRepeating("addGold", 0.001f, goldTimer);


        if(Resources.Count<=0)
        {
            spawnResource();
            //Debug.Log("Resources count: " + Resources.Count);
        }
        if(Troops == null)
        {
            //Debug.Log("Troops list is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameLength += Time.deltaTime;
        // GameObject[] TroopsArray = GameObject.FindGameObjectsWithTag("Troop");
        //  Troops.AddRange(TroopsArray);
        isTroopCountMax();
        if (resourceReadyToSpawn())
        {
            spawnResource();
        }
      

        displayGoldCount();
        displayTroopCounts();
    }
    public void isTroopCountMax()
    {
        //Debug.Log("maxTroopCount" + maxTroopCount);
        if (Troops.Count > maxTroopCount) maxTroopCount = Troops.Count;
    }
    void displayGoldCount()
    {
        p1GoldDisp.text = "Player 1 Gold : " + playerGoldCounts[0].ToString();
        p2GoldDisp.text = "Player 2 Gold : " + playerGoldCounts[1].ToString();
    }
    void displayTroopCounts()
    {
        p1AllyTroopsCount.text = "Player 1 Ally Troops: " + bt.allyTroops.Count.ToString();
        p2AllyTroopsCount.text = "Player 2 Ally Troops: " + bt.allyTroops.Count.ToString();
        p1EnemyTroopsCount.text = "Player 1 Enemy Troops " + bt.enemyTroops.Count.ToString();
        p2EnemyTroopsCount.text = "Player 1 Enemy Troops " + bt.enemyTroops.Count.ToString();
        troopCount.text = "Total Troop Count : " + Troops.Count.ToString();
    }
    public bool canAffordTroop(TeamNumber teamNumber,TroopClass troopClass)
    {
        //Debug.Log("Team Number : " + ((int)teamNumber));
       // Debug.Log("TroopClass  : " + ((int)troopClass));
        int playerGold = playerGoldCounts[(int)teamNumber - 1];
        int troopCost = troopCosts[troopClass];

        if (playerGold >= troopCost) return true;
        else return false;
    }

    public void subtractUnitCost(TroopClass troopClass, TeamNumber teamNumber)
    {
        playerGoldCounts[(int)teamNumber - 1] -= troopCosts[troopClass];
    }
    public void addResourceGold(int resourceValue,TeamNumber teamNumber)
    {
        playerGoldCounts[(int)teamNumber - 1] +=resourceValue;
    }
    public bool resourceReadyToSpawn()
    {
        if (resourceSpawnTimer <= 0 && Resources.Count < maxResourceNumber)
        {
            resourceSpawnTimer = startResourceSpawnTimer;
            return true;
        }
        else
        {
            resourceSpawnTimer -= Time.deltaTime;
            return false;
        }
    }
    public void spawnResource() // Spawns 2 Resources mirroring each side of the map
    {
        
        Vector3 pos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax),1);
        Vector3 mirrorPos = new Vector3(-pos.x, -pos.y, 1);

        GameObject localResourceObject       = Instantiate(resourceGameObject,pos, Quaternion.identity);
        GameObject localResourceObjectMirror = Instantiate(resourceGameObject,mirrorPos, Quaternion.identity);

        Resource localResource               = localResourceObject.GetComponent<Resource>();
        Resource localResourceMirror         = localResourceObjectMirror.GetComponent<Resource>();

        Resources.Add(localResourceObject);
        Resources.Add(localResourceObjectMirror);
        bt.Resources.Add(localResourceObject);
        bt.Resources.Add(localResourceObjectMirror);

        resourceSpawnIndex++;
        localResource.resourceIndex = resourceSpawnIndex;

        //Debug.Log("Added Resource at : " + localResourceObject.transform.position);
    }

    void addGold()
    {
        for (int i = 0; i < playerGoldCounts.Length; i++)
        {
            playerGoldCounts[i] += goldIncrement + playerBonusGold[i];
        }
    }
    
    public void addResource(GameObject resource)
    {
        Resources.Add(resource);
    }
    public void removeResource(GameObject resource)
    {
        Resources.Remove(resource);
    }
    public void addTroop(GameObject troop)
    {
        Troops.Add(troop);
    }
    public void removeTroop(GameObject troop)
    {
        Troops.Remove(troop);
    }
    public List<GameObject> getTroopsList()
    {
        return Troops;
    }
    public List<GameObject> getResourcesList()
    {
        return Resources;
    }
    public int getPlayerGoldCount(int playerNumber)
    {
        return playerGoldCounts[playerNumber - 1];
    }
   public void setPlayerGoldCount(int playerNumber,int goldAmount)
    {
        playerGoldCounts[playerNumber] = goldAmount;
    }
    public int[] getPlayerGoldCountsArray()
    {
        return playerGoldCounts;
    }
    public Vector3 getBasePosition()
    {
        return transform.position;
    }
    public Dictionary<TroopClass,TroopClass> getMatchups()
    {
        return matchups;
    }
    public void destroyTroop(Unit unit)
    {

        Troops.Remove(unit.gameObject);
        Destroy(unit.gameObject);
    }
    

}
