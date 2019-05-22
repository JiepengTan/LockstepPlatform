using Entitas;
using Lockstep.Math;
using UnityEngine.SocialPlatforms;

namespace Lockstep.Game.Systems.Game   {
    public class SystemExecuteMoveTank :BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _moveRequest;

        public SystemExecuteMoveTank(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer){

            _moveRequest = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.MoveRequest,
                GameMatcher.TagTank,
                GameMatcher.Move));
        }

        public void Execute(){
            foreach (var entity in _moveRequest.GetEntities()) {
                var deltaTime = GameConfig.DeltaTime;
                var mover = entity.move;
                var dir = entity.dir.value;
                var pos = entity.pos.value;
                var moveSpd = mover.moveSpd;

                //can move 判定
                var dirVec = DirUtil.GetDirVec(dir).ToLVector2();
                var moveDist = (moveSpd * deltaTime);
                var fTargetHead = pos + (TankUtil.TANK_HALF_LEN + moveDist) * dirVec;
                var fPreviewHead = pos + (TankUtil.TANK_HALF_LEN + TankUtil.FORWARD_HEAD_DIST) * dirVec;

                LFloat maxMoveDist = moveSpd * deltaTime;
                var headPos = pos + (TankUtil.TANK_HALF_LEN) * dirVec;
                var dist = CollisionUtil.GetMaxMoveDist(dir, headPos, fTargetHead);
                var dist2 = CollisionUtil.GetMaxMoveDist(dir, headPos, fPreviewHead);
                maxMoveDist =LMath.Max(LFloat.zero,LMath.Min(maxMoveDist, dist, dist2)) ;

                var diffPos = maxMoveDist * dirVec;
                pos = pos + diffPos;
                entity.pos.value = pos;
            }
        }
    }

}