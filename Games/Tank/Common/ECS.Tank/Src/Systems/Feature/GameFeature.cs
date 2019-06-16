using Lockstep.Game.Systems.Game;

namespace Lockstep.Game.Features {
    sealed class GameFeature : Feature {
        public GameFeature(Contexts contexts, IServiceContainer services) : base("Game"){
            Add(new SystemDelayCall(contexts, services));
            //Spawn
            Add(new SystemEnemyBorn(contexts, services));
            //AI
            Add(new SystemUpdateAI(contexts, services));
            //Skill
            Add(new SystemExecuteFire(contexts, services));
            Add(new SystemSkillUpdate(contexts, services));
            //Move
            Add(new SystemExecuteMoveBullet(contexts, services));
            Add(new SystemExecuteMoveTank(contexts, services));
            Add(new SystemExecuteMovePlayer(contexts, services));
            //CollisionDetected
            Add(new SystemCollisionDetected(contexts, services));
            //Destroy
            Add(new SystemApplyCampDestroyEffect(contexts, services));
            Add(new SystemApplyItemDestroyEffect(contexts, services));
            Add(new SystemApplyPlayerDestroyEffect(contexts, services));
            Add(new SystemApplyEnemyDestroyEffect(contexts, services));
        }
    }
}