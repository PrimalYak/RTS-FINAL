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
        TroopClass troopClass = TroopClass.Warrior;
        public float costToApply;

        void Start()
        {
            scene = GameObject.FindWithTag("GameManager").GetComponent<SceneBuilder>();
            GameObject player2 = GameObject.FindWithTag("Player2");
            player2ScriptAI = player2.GetComponent<PlayerGOAPAI>();
            spawner = scene.base2.GetComponent<Spawner>();
            WSU = player2.GetComponent<WorldStateUpdater>();
            costToApply = (scene.troopCosts[troopClass]) - (WSU.classCounts[(int)troopClass]);
            Debug.Log("Cost to apply ::: " + costToApply);

        }
        public SpawnWarrior(GoapAgent agent) : base(agent)
        {

            goal = GoapGoal.Goals.SPAWN_TROOPS;

            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_WARRIOR, true);

            requiredRange = 1000f;
            
            cost = costToApply;


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
            Debug.Log("SPAWN WARRIOR ACTION RUNNING");
            spawner.purchaseUnit(TroopClass.Warrior);
        }

        public override GoapAction Clone()
        {
            return new SpawnWarrior(agent).SetClone(originalObjectGUID);
        }
    }
}