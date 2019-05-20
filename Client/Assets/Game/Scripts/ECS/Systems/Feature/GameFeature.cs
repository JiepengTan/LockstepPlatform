using Lockstep.Game.Systems.Game;

namespace Lockstep.Game.Features {
    sealed class GameFeature : Feature {
        public GameFeature(Contexts contexts, IServiceContainer services) : base("Game"){
            Add(new SystemUpdateAI(contexts, services));
            Add(new SystemExecuteFire(contexts, services));
            Add(new SystemSkillUpdate(contexts, services));
            Add(new SystemExecuteMoveBullet(contexts, services));
            Add(new SystemExecuteMoveTank(contexts, services));
            Add(new SystemExecuteMovePlayer(contexts, services));
            
            Add(new SystemEnemyBorn(contexts, services));
            
        }
    }
}