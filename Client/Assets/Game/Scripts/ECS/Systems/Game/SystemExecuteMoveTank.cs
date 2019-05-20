using Entitas;
using Lockstep.Math;
using UnityEngine.SocialPlatforms;

namespace Lockstep.Game.Systems.Game   {
    public class SystemExecuteMoveTank : IExecuteSystem {
        private readonly GameContext _gameContext;
        readonly IGroup<GameEntity> _moveRequest;

        public SystemExecuteMoveTank(Contexts contexts, IServiceContainer serviceContainer){
            _gameContext = contexts.game;
            _moveRequest = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagTank,
                GameMatcher.MoveRequest,
                GameMatcher.Move));
        }

        public void Execute(){
            foreach (var entity in _moveRequest.GetEntities()) {
                var deltaTime = Define.DeltaTime;
                var mover = entity.move;
                var dir = mover.dir;
                var pos = mover.pos;
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
                maxMoveDist = LMath.Min(maxMoveDist, dist, dist2);

                var diffPos = maxMoveDist * dirVec;
                pos = pos + diffPos;
                mover.pos = pos;
            }
        }
    }

}