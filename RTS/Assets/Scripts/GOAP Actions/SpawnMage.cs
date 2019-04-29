using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class SpawnMage : GoapAction
    {
        private int targetPlayerId;
        private SceneBuilder scene;
        private PlayerGOAPAI player2ScriptAI;

        private Spawner spawner;
        public WorldStateUpdater WSU;
        public TroopClass troopClass = TroopClass.Mage;
        public float costToApply;
        public string targetName = "Player2";
        public TaskExecutor taskExecutor;


        void Start()
        {
           

        }
        public SpawnMage(GoapAgent agent) : base(agent)
        {

            goal = GoapGoal.Goals.SPAWN_TROOPS;

            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_MAGE, true);

            requiredRange = 1000f;
            cost = 1;

        }

        public override void Perform()
        {
            taskExecutor = target.GetComponent<TaskExecutor>();
            cost = taskExecutor.scene.troopCosts[troopClass] - taskExecutor.WSU.classCounts[(int)troopClass];
            taskExecutor.tryPurchaseUnit(troopClass);

        }

        public override GoapAction Clone()
        {
            return new SpawnMage(agent).SetClone(originalObjectGUID);
        }
    }
}