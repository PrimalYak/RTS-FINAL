using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGOAPAI : GoapAgent
{
    // Start is called before the first frame update
    public TeamNumber teamNumber;
    public WorldStateUpdater WSU;

    public override void Awake()
    {
        base.Awake();

        TeamNumber teamNumber = TeamNumber.t2;
        WSU = GetComponent<WorldStateUpdater>();
        // create goals

        goals.Add(GoapGoal.Goals.DEFEND_HOME_BASE, new DefendBaseGoal(GoapGoal.Goals.DEFEND_HOME_BASE, 1f));
        
        goals.Add(GoapGoal.Goals.KILL_ENEMY_BASE, new KillEnemyBaseGoal(GoapGoal.Goals.KILL_ENEMY_BASE, 1f));

        goals.Add(GoapGoal.Goals.SPAWN_TROOPS, new SpawnTroops(GoapGoal.Goals.SPAWN_TROOPS, 1f));

        // create Actions

            dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_WARRIOR, false);
            dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_ARCHER, false);
            dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_MAGE, false);
            dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_GATHERER, false);

            dataSet.SetData(GoapAction.Effects.ALLIES_ALIVE, false);
            dataSet.SetData(GoapAction.Effects.ENEMIES_ALIVE, false);
            dataSet.SetData(GoapAction.Effects.RESOURCES_TO_GATHER, false);
            dataSet.SetData(GoapAction.Effects.ENEMIES_NEAR_BASE, false);




        possibleActions.Add(new SpawnTroopAction(this));
            //possibleActions.Add(new WaitForGoldAction(this));
            possibleActions.Add(new SpawnWarrior(this));
            possibleActions.Add(new SpawnArcher(this));
            possibleActions.Add(new SpawnMage(this));
            possibleActions.Add(new SpawnGatherer(this));
            possibleActions.Add(new CommandTroopAttackEnemyTroops(this));
            possibleActions.Add(new CommandTroopAttackEnemyBase(this));



    }
    public void setEffectWarriorCost(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_WARRIOR, hasGold);
    }
    public void setEffectArcherCost(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_ARCHER, hasGold);

    }
    public void setEffectMageCost(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_MAGE, hasGold);
    }
    public void setEffectGathererCost(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.HAS_SUFFICIENT_GOLD_GATHERER, hasGold);
    }
    public void setEffectAlliesAlive(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.ALLIES_ALIVE, hasGold);
    }
    public void setEffectEnemiesAlive(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.ENEMIES_ALIVE, hasGold);
    }
    public void setEffectResourcesToGather(bool hasGold)
    {
        dataSet.SetData(GoapAction.Effects.RESOURCES_TO_GATHER, hasGold);
    }
    public void setEffectEnemiesNearBase(bool enemiesNear)
    {
        dataSet.SetData(GoapAction.Effects.ENEMIES_NEAR_BASE, enemiesNear);

    }

}
