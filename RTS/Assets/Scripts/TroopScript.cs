using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;




public class TroopScript : Unit
{

    // public Spawner spawner;
    protected bool hasCollided = false;
    private SceneBuilder sb;
    //private AIDestinationSetter aIDSetterAccess;
   // private TeamNumber thisTeamNumber;
    private Unit unitScript;
    public enum TroopState
    {
        Idle,
        MovingToEnemyBase,
        SpottedEnemyTroop,
    }
    public int damageMultiplier = 2;



    private TroopState currentTroopState;

    private TroopClass currentTroopClass;
    
    
   // public TeamNumber ThisTeamNumber { get => thisTeamNumber; set => thisTeamNumber = value; }
    
    //public SceneBuilder Sb { get => sb; set => sb = value; }
    //public AIDestinationSetter AIDSetterAccess { get => aIDSetterAccess; set => aIDSetterAccess = value; }
   // public TroopClass CurrentTroopClass { get => currentTroopClass; set => currentTroopClass = value; }
    public TroopState CurrentTroopState { get => currentTroopState; set => currentTroopState = value; }
    public Unit UnitScript { get => unitScript; set => unitScript = value; }


    // Use this for initialization

    public TroopScript()
    {


    }
    public TroopScript(TroopClass _troopClass)
    {
        CurrentTroopClass = _troopClass;

    }
    void Start()
    {
        //base.assignBaseHubs();
        base.AiPath = GetComponent<AIPath>();

        base.rb = GetComponent<Rigidbody2D>();
        UnitScript = GetComponent<Unit>();
       // Debug.Log("Unit Script ::: "+ UnitScript);
        CurrentTroopState = TroopState.Idle;
        CurrentTroopClass = UnitScript.CurrentTroopClass;
       
        UnitScript.HealthTextMesh = transform.Find("HealthTextMesh").GetComponent<TextMesh>();
        ThisTeamNumber = UnitScript.ThisTeamNumber;
        //Debug.Log("This unit : " + CurrentTroopClass + "is on team :: " + ThisTeamNumber);
       
      
        // troopstate = Tstate(0);
        //if()
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (ThisTeamNumber == TeamNumber.t2)
        //{
        //    Spawner spawner = UnitScript.EnemySpawner;
        //    //Debug.Log("Troop Team ::::: " + base.ThisTeamNumber + "Enemy team Number" + "Enemyspawner ::::::: + " + spawner.thisteamNumber);
        //    moveToGoal(spawner.gameObject);
        //}
       
    }

    public void moveToGoal(GameObject goalObject)
    {
        UnitScript.moveToGoal(goalObject);
    }

   

    private void updateClosestEnemyTroopsList()
    {
        
        foreach (GameObject unit in Sb.getTroopsList())
        {
            if ((unit.GetComponent<Unit>() != null) && !unitScript.closestEnemyTroops.Contains(unit))
            {
                Unit unitScript = unit.GetComponent<Unit>();
                if (ThisTeamNumber != unitScript.ThisTeamNumber) unitScript.closestEnemyTroops.Add(unit);
            }
           
        }
        //closestEnemyTroops.AddRange();
        closestEnemyTroops.Sort(SortByDistanceToUnit);
    }
    private void updateBounds()
    {
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
        Bounds bounds2 = GetComponent<CircleCollider2D>().bounds;
        AstarPath.active.UpdateGraphs(bounds);
        AstarPath.active.UpdateGraphs(bounds2);
    }
    private void setHealth()
    {
        if (health > 0)
        {
            HealthTextMesh.text = health.ToString();
        }
    }
    //public void moveToGoal(GameObject goalObject)
    //{
    //    base.AIDSetter.target = goalObject.transform;
    //}
    //public void moveToEnemyBase()
    //{
    //    moveToGoal(base.enemySpawner.gameObject);
    //}
    public Transform checkForEnemiesInRange()
    {
        base.enemiesInSightArray = Physics2D.OverlapCircleAll(transform.position, sightRadius, EnemiesLayer);
        Transform closestEnemy = null;
        if (enemiesInSightArray.Length > 0)
        {

           closestEnemy =  findClosestEnemyInRange().transform;
        }
        else
        {
            //moveToEnemyBase(); 
        }
        return closestEnemy;
    }
    public GameObject findClosestEnemyInRange()
    {
        float closestEnemyDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        for (int i = 0; i < enemiesInSightArray.Length; i++)
        {
            Unit unitScript = enemiesInSightArray[i].gameObject.GetComponent<Unit>();
            float tempDist = Vector3.Distance(transform.position, enemiesInSightArray[i].transform.position);

            if (tempDist < closestEnemyDistance && (enemiesInSightArray[i].gameObject != gameObject))
            {
                closestEnemyDistance = tempDist;
                closestEnemy = enemiesInSightArray[i].gameObject;
            }
        }
        if (closestEnemy != null)
        {
            CurrentTroopState = TroopState.SpottedEnemyTroop;
            //moveToGoal(closestEnemy);
        }
        else
        {
           // moveToEnemyBase();
        }
        return closestEnemy;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            if (areBothTroops(this.gameObject,other.gameObject)) handleTroopCollision(other);
            else handleBaseHit(other);
        }
        else hasCollided = false;
    }

   


    private bool areBothTroops(GameObject g1, GameObject g2)
    {
        if (((g1.tag == "Troop" && g2.tag == "Troop") || (g1.tag =="Gatherer" && g2.tag == "Troop") || (g1.tag == "Troop" && g2.tag == "Gatherer"))) return true;
        else return false;
    }
   

    public void handleTroopCollision(Collider2D other)
    {
            Unit unitScript = other.GetComponent<Unit>();
           // Debug.Log("Collision Detected");
           
            if (ThisTeamNumber != unitScript.ThisTeamNumber)  //  troops different teams
            {
                troopCollision(unitScript);
            }
    }
   
    public void troopCollision(Unit unitScript)
    {
        float damageToDeal = 0;
        TroopClass otherTroopClass = unitScript.CurrentTroopClass;

       // Debug.Log("Collision team true");
        if (!isSameClass(unitScript))
        {
           // Debug.Log("Different class");
            switch (CurrentTroopClass)
            {
                case TroopClass.Warrior:
                    damageToDeal = warriorInteractions(otherTroopClass);
                    break;
                case TroopClass.Archer:
                    damageToDeal = archerInteractions(otherTroopClass);
                    break;
                case TroopClass.Mage:
                    damageToDeal = mageInteractions(otherTroopClass);
                    break;
                case TroopClass.Gatherer:
                    break;
                default:
                    Debug.Log("Error - This class switch hit default");
                    break;
            }
        }
        else damageToDeal = (damage * 2) + 1;

        unitScript.takeDamage(damageToDeal);

         //Debug.Log("Enemy " + unitScript.CurrentTroopClass.ToString() + " taken : " + damageToDeal + " damage");
    }

    
    private bool isSameClass(Unit other)
    {
        if (CurrentTroopClass == other.CurrentTroopClass) return true;
        else return false;
    }
    public float archerInteractions(TroopClass otherTroopClass)
    {
        float damageToDeal = 0;
        if (otherTroopClass == TroopClass.Warrior)
        {
            damageToDeal = damage / damageMultiplier;
        }
        else if (otherTroopClass == TroopClass.Mage)
        {
            damageToDeal = damage * damageMultiplier;
        }
        return damageToDeal;
    }
    
    public float warriorInteractions(TroopClass otherTroopClass)
    {
        float damageToDeal = 0;

        if (otherTroopClass == TroopClass.Archer)
        {
            damageToDeal = damage * damageMultiplier;
        }
        if (otherTroopClass == TroopClass.Mage)
        {
            damageToDeal = damage / damageMultiplier;
        }
        
        return damageToDeal;
    }
    public float mageInteractions(TroopClass otherTroopClass)
    {
        float damageToDeal = 0;

            if (otherTroopClass == TroopClass.Archer)
            {
                damageToDeal = damage / damageMultiplier;
            }
            else if (otherTroopClass == TroopClass.Warrior)
            {
                damageToDeal = damage * damageMultiplier;

            }
            return damageToDeal;
    }   
    public bool handleBaseHit(Collider2D other)
    {
        if (other.tag == "Base")
        { 
            Spawner spawner = other.GetComponent<Collider2D>().GetComponent<Spawner>();
            if(spawner.thisteamNumber != ThisTeamNumber)
            {
                spawner.takeDamage(damage);
                takeDamage(1000);
                return true;
            }
            else return false;
         
        }
        else return false;
    }

    public void Move()
    {

        //transform.Translate(direction * speed * Time.deltaTime);
    }
    public void setGoal(GameObject go)
    {
        moveToGoal(go);
    }
    public void GetInput()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;

        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;

        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;

        }
    }
    
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    int SortByDistanceToUnit(GameObject a, GameObject b)
    {
        float squaredRangeA = (a.transform.position - transform.position).sqrMagnitude;
        float squaredRangeB = (b.transform.position - transform.position).sqrMagnitude;
        return squaredRangeA.CompareTo(squaredRangeB);
    }
}

