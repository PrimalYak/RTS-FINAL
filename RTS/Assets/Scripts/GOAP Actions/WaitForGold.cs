using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class WaitForGoldAction : GoapAction
    {

        public WaitForGoldAction(GoapAgent agent) : base(agent)
        {
            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_WARRIOR, false);
            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_ARCHER, false);
            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_MAGE, false);
            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_GATHERER, false);

            effects.Add(Effects.HAS_SUFFICIENT_GOLD_WARRIOR, true);
            effects.Add(Effects.HAS_SUFFICIENT_GOLD_ARCHER, true);
            effects.Add(Effects.HAS_SUFFICIENT_GOLD_MAGE, true);
            effects.Add(Effects.HAS_SUFFICIENT_GOLD_GATHERER, true);



            requiredRange = 0f;
            cost = 1;

           // targetString = "Throwable";

            //removeWhenTargetless = true;
        }

        public override void Perform()
        {
            
        }

        public override GoapAction Clone()
        {
            return new WaitForGoldAction(agent).SetClone(originalObjectGUID);
        }
    }
}
