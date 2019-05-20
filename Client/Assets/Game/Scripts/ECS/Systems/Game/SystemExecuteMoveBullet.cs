using Entitas;
using Lockstep.Math;
using UnityEngine.SocialPlatforms;

namespace Lockstep.Game.Systems.Game  {

    public class SystemExecuteMoveBullet : IExecuteSystem {
        readonly IGroup<GameEntity> _bulletGroup;

        public SystemExecuteMoveBullet(Contexts contexts, IServiceContainer serviceContainer){
            _bulletGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagBullet,
                GameMatcher.MoveRequest,
                GameMatcher.Move));
        }

        public void Execute(){
            foreach (var entity in _bulletGroup.GetEntities()) {
                var move = entity.move;
                var dirVec = DirUtil.GetDirLVec(entity.dir.value);
                var offset = (move.moveSpd * Define.DeltaTime) * dirVec;
                entity.position.value = offset;
            }
        }
    }
}