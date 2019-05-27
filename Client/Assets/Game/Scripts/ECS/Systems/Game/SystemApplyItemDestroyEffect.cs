using DesperateDevs.Utils;
using Entitas;
using Lockstep.ECS.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game.Systems.Game {
    public class SystemApplyItemDestroyEffect : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _destroyedGroup;
        readonly IGroup<GameEntity> _allEnmey;

        public SystemApplyItemDestroyEffect(Contexts contexts, IServiceContainer serviceContainer) : base(contexts,
            serviceContainer){
            _destroyedGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.Destroyed,
                GameMatcher.LocalId,
                GameMatcher.ItemType));
            _allEnmey = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagEnemy));
        }


        public void Execute(){
            foreach (var entity in _destroyedGroup.GetEntities()) {
                _audioService.PlayMusicGetItem();
                var actor = _actorContext.GetEntityWithId(entity.itemType.killerActorId);
                var player = _gameContext.GetEntityWithLocalId(actor.gameLocalId.value);
                UnityEngine.Debug.Assert(actor != null, " player's tank have no owner");
                if (player != null && !player.isDestroyed && player.unit.health != 0) {
                    actor.score.value = actor.score.value + 500;
                    switch (entity.itemType.type) {
                        case EItemType.Boom:
                            OnTriggerBoom(actor, player, _allEnmey);
                            break;
                        case EItemType.Upgrade:
                            OnTriggerUpgrade(actor, player, _allEnmey);
                            break;
                        case EItemType.AddLife:
                            OnTriggerAddLife(actor, player, _allEnmey);
                            break;
                    }
                }
            }
        }

        public void TriggerEffect(EItemType type, ActorEntity actor, IGroup<GameEntity> _allEnemy){ }

        void OnTriggerBoom(ActorEntity actor, GameEntity player, IGroup<GameEntity> _allEnemy){
            foreach (var tank in _allEnemy.GetEntities()) {
                if (tank.isDestroyed || tank.unit.health == 0) continue;
                tank.unit.killerLocalId = player.localId.value;
                tank.unit.health = 0;
                tank.isDestroyed = true;
            }
        }

        void OnTriggerUpgrade(ActorEntity actor, GameEntity player, IGroup<GameEntity> _allEnemy){
            _unitService.Upgrade(player);
        }

        void OnTriggerAddLife(ActorEntity actor, GameEntity player, IGroup<GameEntity> _allEnemy){
            actor.life.value = actor.life.value + 1;
        }
    }
}