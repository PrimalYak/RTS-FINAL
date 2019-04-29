using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class CommandTroopAttackEnemyTroops : GoapAction
    {
        private int targetPlayerId;

        public CommandTroopAttackEnemyTroops(GoapAgent agent) : base(agent)
        {
            

            preconditions.Add(Effects.ALLIES_ALIVE, true);
            preconditions.Add(Effects.ENEMIES_ALIVE, true);
            

            requiredRange = 0f;
            cost = 20;


        }

        public override void Perform()
        {
            
        }

        public override GoapAction Clone()
        {
            return new CommandTroopAttackEnemyTroops(agent).SetClone(originalObjectGUID);
        }
    }
}