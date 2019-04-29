using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class SpawnGatherer : GoapAction
    {
        private int targetPlayerId;
        private SceneBuilder scene;
        private PlayerGOAPAI player2ScriptAI;

        private Spawner spawner;
        public WorldStateUpdater WSU;
        TroopClass troopClass = TroopClass.Gatherer;
        public float costToApply;


        void Start()
        {
            scene = GameObject.FindWithTag("GameManger").GetComponent<SceneBuilder>();
            GameObject player2 = GameObject.FindWithTag("Player2");

            player2ScriptAI = player2.GetComponent<PlayerGOAPAI>();
            spawner = scene.base2.GetComponent<Spawner>();
            WSU = player2.GetComponent<WorldStateUpdater>();
            costToApply = scene.troopCosts[troopClass];

        }
        public SpawnGatherer(GoapAgent agent) : base(agent)
        {

            goal = GoapGoal.Goals.SPAWN_TROOPS;

            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_GATHERER, true);
            preconditions.Add(Effects.RESOURCES_TO_GATHER, true);

            requiredRange = 0f;
            cost = costToApply;


        }

        public override void Perform()
        {
            spawner.purchaseUnit(TroopClass.Gatherer);

        }

        public override GoapAction Clone()
        {
            return new SpawnGatherer(agent).SetClone(originalObjectGUID);
        }
    }
}