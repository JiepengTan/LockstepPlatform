using System.Collections.Generic;
using Entitas;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game.Systems.Game {
    public class SystemUpdateAI : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _AIGroup;

        public SystemUpdateAI(Contexts contexts, IServiceContainer serviceContainer) : base(contexts, serviceContainer){
            _AIGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.AI
            ));
        }

        public void Execute(){
            foreach (var entity in _AIGroup.GetEntities()) {
                var aiInfo = entity.aI;
                aiInfo.timer += GameConfig.DeltaTime;
                if (aiInfo.timer < aiInfo.updateInterval) {
                    continue;
                }

                aiInfo.timer = LFloat.zero;
                Vector2Int dir = Vector2Int.zero;
                var curPos = entity.pos.value;
                var headPos = TankUtil.GetHeadPos(entity.pos.value, entity.dir.value);
                var isReachTheEnd = CollisionUtil.HasColliderWithBorder(entity.dir.value, headPos);
                if (isReachTheEnd) {
                    List<int> allWalkableDir = new List<int>();
                    for (int i = 0; i < (int) (EDir.EnumCount); i++) {
                        var vec = DirUtil.GetDirLVec((EDir) i) * TankUtil.TANK_HALF_LEN;
                        var pos = curPos + vec;
                        if (!CollisionUtil.HasCollider(pos)) {
                            allWalkableDir.Add(i);
                        }
                    }

                    var count = allWalkableDir.Count;
                    if (count > 0) {
                        entity.dir.value = (EDir) (allWalkableDir[_randomService.Range(0, count)]);
                        entity.move.isChangedDir = true;
                    }
                }
                
                var iPos = entity.pos.value.Floor();
                if ((entity.pos.value - (iPos.ToLVector2() + TankUtil.UNIT_SIZE)).sqrMagnitude <TankUtil. sqrtTargetDist) {
                    if (_randomService.value > new LFloat(true,200)) {
                        return;
                    }
                    //random change dir if it can 
                    var borderDir = DirUtil.GetDirVec((EDir) ((int) ( entity.dir.value + 1) % (int) EDir.EnumCount));
                    var iBorder1 = iPos + borderDir;
                    var iBorder2 = iPos - borderDir;
                    if (!CollisionUtil.HasCollider(iBorder1)) {
                        entity.dir.value = DirUtil.GetEDirFromVec(borderDir);
                    }
                    else if (!CollisionUtil.HasCollider(iBorder2)) {
                        entity.dir.value = DirUtil.GetEDirFromVec(Vector2Int.zero -borderDir);
                    }
                }
            }
        }
    }
}