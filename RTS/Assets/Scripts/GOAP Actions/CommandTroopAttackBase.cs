using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class CommandTroopAttackEnemyBase : GoapAction
    {
        private int targetPlayerId;

        public CommandTroopAttackEnemyBase(GoapAgent agent) : base(agent)
        {
            goal = GoapGoal.Goals.KILL_ENEMY_BASE;

            preconditions.Add(Effects.ALLIES_ALIVE, true);
            preconditions.Add(Effects.ENEMIES_ALIVE , false);
            

            requiredRange = 0f;
            cost = 20;

            
        }

        public override void Perform()
        {

        }

        public override GoapAction Clone()
        {
            return new CommandTroopAttackEnemyBase(agent).SetClone(originalObjectGUID);
        }
    }
}