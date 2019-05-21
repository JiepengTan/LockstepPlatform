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
                GameMatcher.AI,
                GameMatcher.Skill
            ));
        }

        public void Execute(){
            foreach (var entity in _AIGroup.GetEntities()) {
                var aiInfo = entity.aI;
                aiInfo.timer += Define.DeltaTime;
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
                        entity.dir.value = (EDir) (allWalkableDir[LRandom.Range(0, count)]);
                        entity.move.isChangedDir = true;
                    }
                }

                //Fire skill
                var isNeedFire = LRandom.value < aiInfo.fireRate;
                if (isNeedFire) {
                    if (entity.skill.cdTimer <= LFloat.zero) {
                        entity.skill.cdTimer = entity.skill.cd;
                        //Fire
                        _unitService.CreateBullet(entity.pos.value, entity.dir.value, (int) entity.skill.bulletId,
                            entity);
                    }
                }
            }
        }
    }
}