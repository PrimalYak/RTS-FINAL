﻿using SwordGC.AI.Goap;
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
        public string targetName = "Player2";
        public TaskExecutor taskExecutor;
        public float enemyCountsModifier = 2f;


        void Start()
        {
            

        }
        public SpawnGatherer(GoapAgent agent) : base(agent)
        {

            goal = GoapGoal.Goals.SPAWN_TROOPS;

            preconditions.Add(Effects.HAS_SUFFICIENT_GOLD_GATHERER, true);
            preconditions.Add(Effects.RESOURCES_TO_GATHER, true);

            requiredRange = 1000f;
            
            cost = 1;


        }

        public override void Perform()
        {
            taskExecutor = target.GetComponent<TaskExecutor>();
            cost = taskExecutor.scene.troopCosts[troopClass] - WSU.enemyTroops.Count*2 - WSU.enemiesCloseToBase.Count*5;
            taskExecutor.tryPurchaseUnit(troopClass);
        }

        public override GoapAction Clone()
        {
            return new SpawnGatherer(agent).SetClone(originalObjectGUID);
        }
    }
}