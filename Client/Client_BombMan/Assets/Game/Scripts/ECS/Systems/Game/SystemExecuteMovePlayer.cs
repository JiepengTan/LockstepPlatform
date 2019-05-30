using Entitas;
using Lockstep.Math;
using UnityEngine.SocialPlatforms;

namespace Lockstep.Game.Systems.Game {
    public class SystemExecuteMovePlayer : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _moveRequest;

        public SystemExecuteMovePlayer(Contexts contexts, IServiceContainer serviceContainer) : base(contexts,
            serviceContainer){
            _moveRequest = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.MoveRequest,
                GameMatcher.Move,
                GameMatcher.ActorId
            ));
        }

        public void Execute(){
            foreach (var entity in _moveRequest.GetEntities()) {
                var mover = entity.move;
                var pos = entity.pos.value;
                if (mover.isChangedDir) {
                    var idir = (int) (entity.dir.value);
                    var isUD = idir % 2 == 0;
                    if (isUD) {
                        pos.x = CollisionUtil.RoundIfNear(pos.x + TankUtil.TANK_HALF_LEN, TankUtil.SNAP_DIST) -
                                TankUtil.TANK_HALF_LEN;
                    }
                    else {
                        pos.y = CollisionUtil.RoundIfNear(pos.y + TankUtil.TANK_HALF_LEN, TankUtil.SNAP_DIST) -
                                TankUtil.TANK_HALF_LEN;
                    }
                }

                entity.pos.value = pos;
                mover.isChangedDir = false;
                entity.RemoveMoveRequest();
            }
        }
    }
}