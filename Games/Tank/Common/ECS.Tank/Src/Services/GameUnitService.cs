using System.Collections.Generic;
using System;
using Entitas;
using Lockstep.ECS;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    [System.Serializable]
    public partial class GameUnitService : BaseGameService, IGameUnitService {
        private ActorContext _actorContext;
        private GameContext _gameContext;

        public override void DoAwake(IServiceContainer services){
            var contexts = services.GetService<IConstStateService>().Contexts as Contexts;
            _actorContext = contexts.actor;
            _gameContext = contexts.game;
        }

        ///用于惟一标记 GameEntity 用于回滚
        private uint _localIdCounter;

        public void TakeDamage(IEntity pbullet, IEntity psuffer){
            var bullet = pbullet as GameEntity;
            var suffer = psuffer as GameEntity;
            if (suffer.isDestroyed) return;
            if (suffer.unit.health <= bullet.unit.damage) {
                bullet.unit.health -= suffer.unit.health;
                suffer.unit.health = 0;
                suffer.unit.killerLocalId = bullet.bullet.ownerLocalId;
                suffer.isDestroyed = true;
            }
            else {
                suffer.unit.health -= bullet.unit.damage;
                bullet.unit.health = 0;
                bullet.isDestroyed = true;
            }
        }

        public void DropItem(LFloat rate){
            if (_randomService.value >= rate) {
                return;
            }

            var min = _gameConstStateService.mapMin;
            var max = _gameConstStateService.mapMax;
            var x = _randomService.Range(min.x + 4, max.x - 4);
            var y = _randomService.Range(min.y + 4, max.y - 4);
            CreateItem(new LVector2(x, y), _randomService.Range(0, _gameConfigService.itemPrefabs.Count));
        }

        private void CreateItem(LVector2 createPos, int type){
            CreateUnit(createPos, _gameConfigService.itemPrefabs, type, EDir.Up);
        }

        public void CreateCamp(LVector2 createPos, int type = 0){
            CreateUnit(createPos + _gameConfigService.TankBornOffset,
                _gameConfigService.CampPrefabs, 0, EDir.Up);
        }

        public void CreateBullet(LVector2 pos, EDir dir, int type, IEntity pEntity){
            var owner = pEntity as GameEntity;
            var createPos = pos + DirUtil.GetDirLVec(dir) * TankUtil.TANK_HALF_LEN;

            var entity = CreateUnit(createPos, _gameConfigService.bulletPrefabs, type, dir);
            entity.bullet.ownerLocalId = owner.localId.value;
            entity.unit.camp = owner.unit.camp;
        }

        public void CreateEnemy(LVector2 bornPos){
            var type = _randomService.Range(0, _gameConfigService.enemyPrefabs.Count);
            CreateEnemy(bornPos, type);
        }

        public void CreateEnemy(LVector2 bornPos, int type){
            var createPos = bornPos + LVector2.right;
            _gameEffectService.ShowBornEffect(createPos);
            _gameAudioService.PlayClipBorn();
            EDir dir = EDir.Down;
            DelayCall(base._gameConfigService.TankBornDelay,
                () => { CreateUnit(createPos, _gameConfigService.enemyPrefabs, type, dir); });
        }

        public void CreatePlayer(byte actorId, int type){
            var bornPos = _gameConstStateService.playerBornPoss[actorId % _gameConstStateService.playerBornPoss.Count];
            var createPos = bornPos + base._gameConfigService.TankBornOffset;
            _gameEffectService.ShowBornEffect(createPos);
            _gameAudioService.PlayClipBorn();
            EDir dir = EDir.Up;
            DelayCall(base._gameConfigService.TankBornDelay, () => {
                var entity = CreateUnit(createPos, _gameConfigService.playerPrefabs, type, dir);
                var actor = _actorContext.GetEntityWithId(actorId);
                if (actor != null) {
                    actor.ReplaceGameLocalId(entity.localId.value);
                    entity.ReplaceActorId(actorId);
                }
                else {
                    Debug.LogError(
                        $"can not find a actor after create a game player actorId:{actorId} localId{entity.localId.value}");
                }
            });
        }


        private GameEntity CreateUnit<T>(LVector2 createPos, List<T> prefabLst, int type, EDir dir)
            where T : BaseEntitySetter{
            var ecsPrefab = prefabLst[type] as ConfigUnit;
            var assetId = ecsPrefab.asset.assetId;
            var entity = CreateGameEntity();
            ecsPrefab.SetComponentsTo(entity);
            entity.dir.value = dir;
            entity.pos.value = createPos;
            if (!_constStateService.IsVideoLoading) {
                _viewService.BindView(entity, (ushort) assetId, createPos);
            }

            return entity;
        }


        public void Upgrade(IEntity iEntity){
            var entity = iEntity as GameEntity;
            var playerCount = _gameConfigService.playerPrefabs.Count;
            var targetType = entity.unit.detailType + 1;
            if (targetType >= playerCount) {
                Debug.Log($"hehe already max level can not upgrade");
                return;
            }

            var ecsPrefab = _gameConfigService.playerPrefabs[targetType] as ConfigUnit;
            var rawPos = entity.pos.value;
            var rawDir = entity.dir.value;
            ecsPrefab.SetComponentsTo(entity);
            entity.pos.value = rawPos;
            entity.dir.value = rawDir;
            if (!_constStateService.IsVideoLoading) {
                _viewService.DeleteView(entity.localId.value);
                _viewService.BindView(entity, (ushort) ecsPrefab.asset.assetId, rawPos,
                    DirUtil.GetDirDeg(rawDir));
            }
        }

        public void DelayCall(LFloat delay, Action callback){
            var delayEntity = CreateGameEntity();
            delayEntity.AddDelayCall(delay, FuncUtil.RegisterFunc(callback));
        }

        private GameEntity CreateGameEntity(){
            var entity = _gameContext.CreateEntity();
            entity.AddLocalId(_localIdCounter);
            _localIdCounter++;
            return entity;
        }
    }
}