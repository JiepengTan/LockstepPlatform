using System.Collections.Generic;
using System.Linq;
using Entitas;
using Lockstep.Game.Interfaces;
using Lockstep.Logging;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game.Features {
    public class BulletUpdateSystem : IExecuteSystem {
        readonly IGroup<GameEntity> _bullets;

        public BulletUpdateSystem(Contexts contexts, IServiceContainer serviceContainer){
            _bullets = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagBullet,
                GameMatcher.Position,
                GameMatcher.Move,
                GameMatcher.Dir));
        }

        public void Execute(){
            foreach (var entity in _bullets.GetEntities()) {
                var move = entity.move;
                var dirVec = DirUtil.GetDirLVec(entity.dir.value);
                var offset = (move.moveSpd * Define.DeltaTime) * dirVec;
                entity.position.value = offset;
            }
        }
    }

    public class AIUpdateSystem : IExecuteSystem {
        readonly IGroup<GameEntity> _AIs;

        public AIUpdateSystem(Contexts contexts, IServiceContainer serviceContainer){
            _AIs = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ActorId,
                GameMatcher.TagEnemy
                ));
        }

        public void Execute(){
            foreach (var entity in _AIs.GetEntities()) {
                var aiInfo = entity.aI;
                aiInfo.timer += Define.DeltaTime;
                if (aiInfo.timer < aiInfo.updateInterval) {
                    continue;
                }

                aiInfo.timer = LFloat.zero;
                Vector2Int dir = Vector2Int.zero;
                var curPos = entity.position.value;
                var headPos = TankUtil.GetHeadPos(entity.position.value, entity.dir.value);
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
                    if (entity.skill.timer <= LFloat.zero) {
                        entity.skill.timer = entity.skill.cd;
                        //Fire
                        UnitUtil.CreateBullet(entity.position.value,entity.dir.value,entity.skill.bulletId,entity);
                    }
                }
            }
        }
    }

    public class SkillUpdateSystem : IExecuteSystem {
        readonly IGroup<GameEntity> skills;

        public SkillUpdateSystem(Contexts contexts, IServiceContainer serviceContainer){
            skills = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.Skill,
                GameMatcher.TagEnemy
            ));
        }

        public void Execute(){
            foreach (var entity in skills.GetEntities()) {
                var timer = entity.skill.timer;
                timer -= Define.DeltaTime;
                if (timer < 0) {
                    entity.skill.timer = LFloat.zero;
                }
            }
        }
    }

    public class PlayerFireSystem  : IExecuteSystem{
        private readonly IResourceService _viewService;
        private readonly GameContext _gameContext;
        private readonly GameStateContext _gameStateContext;   
        private readonly IGroup<InputEntity> _spawnInputs;    

        private uint _localIdCounter;
        private readonly ActorContext _actorContext;

        public PlayerFireSystem(Contexts contexts, IServiceContainer serviceContainer)
        {                                                  
            _viewService = serviceContainer.GetService<IResourceService>();              
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
            _actorContext = contexts.actor;

            _spawnInputs = contexts.input.GetGroup(
                InputMatcher.AllOf(
                    InputMatcher.EntityConfigId,
                    InputMatcher.ActorId,
                    InputMatcher.Coordinate,
                    InputMatcher.Tick));
        }       

        public void Execute()
        {                                                             
            foreach (var input in _spawnInputs.GetEntities().Where(entity => entity.tick.value == _gameStateContext.tick.value))
            {           
                var actor = _actorContext.GetEntityWithId(input.actorId.value);
                var nextEntityId = actor.entityCount.value;

                var e = _gameContext.CreateEntity();        

                Log.Trace(this, actor.id.value + " -> " + nextEntityId);

                //composite primary key
                e.AddId(nextEntityId);
                e.AddActorId(input.actorId.value);

                //unique id for internal usage
                e.AddLocalId(_localIdCounter);
                
                //some default components that every game-entity must have
                //_viewService.LoadView(e, input.entityConfigId.value);


                actor.ReplaceEntityCount(nextEntityId + 1);
                _localIdCounter += 1;
            }                                                                                    
        }    
    }

    public class GameInitSystem : Entitas.IInitializeSystem {
        //Load map Create camp and other Entity
        //Create Players
        public void Initialize(){ }
    }

    public class PlayerMoveSystem {
        
    }

    public class EnemyBoreSystem {
        
    }
}