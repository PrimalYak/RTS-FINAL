using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class CommandTroopDefendHomeBase : GoapAction
    {
        public string targetName = "Player2";
        public TaskExecutor taskExecutor;
        public WorldStateUpdater WSU;

        public CommandTroopDefendHomeBase(GoapAgent agent) : base(agent)
        {
            goal = GoapGoal.Goals.DEFEND_HOME_BASE;

            preconditions.Add(Effects.ALLIES_ALIVE, true);
            preconditions.Add(Effects.ENEMIES_NEAR_BASE, true);
           


            requiredRange = 1000f;
            targetString = targetName;

            cost = 5;


        }

        public override void Perform()
        {
            Debug.Log("CommandTroopAttackBase called");
            WSU = target.GetComponent<WorldStateUpdater>();
            foreach (GameObject ally in WSU.allyTroops)
            {
                Unit unitScript = ally.GetComponent<Unit>();
                Debug.Log("CTAB - unitScript : " + unitScript);
                unitScript.moveToGoal(unitScript.EnemySpawner.gameObject);
            }
        }

        public override GoapAction Clone()
        {
            return new CommandTroopDefendHomeBase(agent).SetClone(originalObjectGUID);
        }
    }
}