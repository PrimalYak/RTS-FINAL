using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class SpawnArcher : GoapAction
    {
        private int targetPlayerId;
        private SceneBuilder scene;
        private PlayerGOAPAI player2ScriptAI;

        private Spawner spawner;
        public WorldStateUpdater WSU;
        TroopClass troopClass = TroopClass.Archer;
        public float costToApply;
        public string targetName = "Player2";
        public TaskExecutor taskExecutor;

        void Start()
        {
            scene = GameObject.FindWithTag("GameManger").GetComponent<SceneBuilder>();
            GameObject player2 = GameObject.FindWithTag("Player2");

            player2ScriptAI = player2.GetComponent<PlayerGOAPAI>();
            spawner = scene.base2.GetComponent<Spawner>();
            WSU = player2.GetComponent<WorldStateUpdater>();
            costToApply = scene.troopCosts[troopClass] - WSU.classCounts[(int)troopClass];
        }
        public SpawnArcher(GoapAgent agent) : base(agent)
        {

            goal = GoapGoal.Goals.SPAWN_TROOPS;

            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_ARCHER, true);


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
            return new SpawnArcher(agent).SetClone(originalObjectGUID);
        }
    }
}