using Entitas;

namespace Lockstep.Game.Systems.Game {
    public class SystemExecuteFire :BaseSystem,IExecuteSystem{
        readonly IGroup<GameEntity> _fireReqGroup;

        public SystemExecuteFire(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer)
        {                         
            _fireReqGroup = contexts.game.GetGroup(GameMatcher.AllOf(
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
                _gameUnitService.CreateBullet(entity.pos.value,entity.dir.value,(int)skill.bulletId,entity);
            }
        }
    }
}