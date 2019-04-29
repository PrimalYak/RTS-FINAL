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
            SceneBuilder scene = WSU.scene;
            Debug.Log("SceneBuilder" + scene);

            foreach(GameObject ally in WSU.allyTroops)
            {
                if (ally.GetComponent<TroopScript>() != null)
                {
                    bool targetSet = false;
                    Unit unitScript = ally.GetComponent<TroopScript>();
                    Debug.Log("Closest Enemies : " + unitScript.getClosestEnemyTroop().Count);
                    foreach (GameObject enemyGO in unitScript.getClosestEnemyTroop())
                    {
                        Unit enemyUnitScript = enemyGO.GetComponent<Unit>();
                        Debug.Log("Enemy Script: " + enemyUnitScript);
                        if ((enemyUnitScript.CurrentTroopClass == scene.getMatchups()[unitScript.CurrentTroopClass]) || (enemyUnitScript.CurrentTroopClass == TroopClass.Gatherer))
                        {
                            //unitScript.updateClosestEnemyTroopsList();
                            Debug.Log("Unit about to move!");
                            unitScript.moveToGoal(enemyGO);
                            targetSet = true;
                            break;

                        }

                    }
                    if (targetSet == false)
                    {
                        
                       unitScript.moveToGoal(unitScript.getClosestEnemyTroop().First());
                       targetSet = true;
                        
                        
                    }
                }
            }
        }

        public override GoapAction Clone()
        {
            return new CommandTroopAttackEnemyTroops(agent).SetClone(originalObjectGUID);
        }
    }
}