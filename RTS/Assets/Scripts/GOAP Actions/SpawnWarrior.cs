using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class SpawnWarrior : GoapAction
    {
        private int targetPlayerId;
        public SceneBuilder scene;
        private PlayerGOAPAI player2ScriptAI;
        private Spawner spawner;
        public WorldStateUpdater WSU;
        public TroopClass troopClass = TroopClass.Warrior;
        public float costToApply;
        public string targetName = "Player2";
        public TaskExecutor taskExecutor;

        void Start()
        {
            
            
           
            

        }
        public SpawnWarrior(GoapAgent agent) : base(agent)
        {

            goal = GoapGoal.Goals.SPAWN_TROOPS;

            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_WARRIOR, true);

            requiredRange = 1000f;

            this.targetString = targetName;
         

            //costToApply = taskExecutor.scene.troopCosts[troopClass] - taskExecutor.WSU.classCounts[(int)troopClass];

            cost = 1;


        }
        protected override bool CheckProceduralPreconditions(DataSet data)
        {
            // Check all procedural preconditions
            // Example:
            // if(!needAmmo || !ammoInRange) return false;
            
            return base.CheckProceduralPreconditions(data);
        }
        public override void Perform()
        {
           
            taskExecutor = target.GetComponent<TaskExecutor>();
            cost = taskExecutor.scene.troopCosts[troopClass] - taskExecutor.WSU.classCounts[(int)troopClass];
            taskExecutor.tryPurchaseUnit(troopClass);
        }

        public override GoapAction Clone()
        {
            return new SpawnWarrior(agent).SetClone(originalObjectGUID);
        }
    }
}