using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class CommandTroopAttackEnemyTroops : GoapAction
    {
        public string targetName = "Player2";
        public TaskExecutor taskExecutor;
        public WorldStateUpdater WSU;

        public CommandTroopAttackEnemyTroops(GoapAgent agent) : base(agent)
        {
            goal = GoapGoal.Goals.KILL_ENEMY_BASE;

            preconditions.Add(Effects.ALLIES_ALIVE, true);
            preconditions.Add(Effects.ENEMIES_ALIVE, true);
            

            requiredRange = 1000f;
            cost = 5;
            targetString = targetName; 

        }

        public override void Perform()
        {
            WSU = target.GetComponent<WorldStateUpdater>();
            foreach(GameObject ally in WSU.allyTroops)
            {
                Unit unitScript = ally.GetComponent<Unit>();
                Debug.Log("Closest enemy troop : " + unitScript.getClosestEnemyTroop().First());
                unitScript.moveToGoal(unitScript.getClosestEnemyTroop().First());
            }
        }

        public override GoapAction Clone()
        {
            return new CommandTroopAttackEnemyTroops(agent).SetClone(originalObjectGUID);
        }
    }
}