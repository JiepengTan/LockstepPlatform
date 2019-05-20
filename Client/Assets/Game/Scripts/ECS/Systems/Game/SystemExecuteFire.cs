using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemExecuteFire :IExecuteSystem{
        private readonly GameContext _gameContext;
        readonly IGroup<GameEntity> _fireReqGroup;

        public SystemExecuteFire(Contexts contexts, IServiceContainer serviceContainer)
        {                                             
            _gameContext = contexts.game;                    

            _fireReqGroup = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.FireRequest,
                GameMatcher.LocalId, 
                GameMatcher.Skill));
        }    

        public void Execute()
        {
            foreach (var entity in _fireReqGroup.GetEntities())
            {
                entity.isFireRequest = false;
                var skill = entity.skill;
                if (skill.cdTimer > 0) {//
                    continue;
                }
                skill.cdTimer = skill.cd;
                UnitUtil.CreateBullet(entity.position.value,entity.dir.value,skill.bulletId,entity);
            }
        }
    }
}