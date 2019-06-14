using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game  {
    public class SystemSkillUpdate :BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _skillGroup;

        public SystemSkillUpdate(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer){
            _skillGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.Skill));
        }

        public void Execute(){
            foreach (var entity in _skillGroup.GetEntities()) {
                var skill = entity.skill;
                skill.cdTimer -= _gameStateService.DeltaTime;
                if (skill.cdTimer < 0) {
                    skill.cdTimer = LFloat.zero;
                }
            }
        }
    }
}