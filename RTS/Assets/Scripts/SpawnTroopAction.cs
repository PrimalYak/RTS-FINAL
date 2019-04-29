using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class SpawnTroopAction : GoapAction
    {
        private TeamNumber teamNumber;

        public SpawnTroopAction(GoapAgent agent) : base(agent)
        {
            //preconditions.Add(Effects.HAS_SUFFICIENT_GOLD, true);
            //effects.Add(Effects.HAS_SUFFICIENT_GOLD, true);

            //distanceMultiplier = 0.5f;
           // requiredRange = 4f;
            cost = 30;
            this.teamNumber = GameObject.FindWithTag("Player2").GetComponent<PlayerGOAPAI>().teamNumber;
           // this.targetPlayerId = targetPlayerId;
        }

        protected override float DistanceToChild(GoapAction child)
        {
            return 0f;
        }

        public override void UpdateTarget()
        {
            if (childs.Count > 0 && cheapestChilds != null && cheapestChilds.Count > 0)
            {
                if (cheapestChilds[0].target != null)
                {
                    target = cheapestChilds[0].target;
                }
            }
            else
            {
                base.UpdateTarget();
            }
        }

        public override void Perform()
        {

        }

        public override GoapAction Clone()
        {
            return new SpawnTroopAction(agent).SetClone(originalObjectGUID);
        }
    }
}
