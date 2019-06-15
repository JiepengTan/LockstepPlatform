using Entitas;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game {

    public static class UnitUtil {
       
        #region IUnitService

        public void TakeDamage(GameEntity bullet, GameEntity suffer){
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
            CreateItem(new LVector2(x, y), _randomService.Range(0, _config.itemPrefabs.Count));
        }

        private void CreateItem(LVector2 createPos, int type){
            CreateUnit(createPos, _config.itemPrefabs, type, EDir.Up, transParentItem);
        }

        public void CreateCamp(LVector2 createPos, int type = 0){
            CreateUnit(createPos + GameConfig.TankBornOffset, _config.CampPrefabs, 0, EDir.Up, transParentItem);
        }

        public void CreateBullet(LVector2 pos, EDir dir, int type, GameEntity owner){
            var createPos = pos + DirUtil.GetDirLVec(dir) * TankUtil.TANK_HALF_LEN;
            
            var entity = CreateUnit(createPos, _config.bulletPrefabs, type, dir, transParentBullet);
            entity.bullet.ownerLocalId = owner.localId.value;
            entity.unit.camp = owner.unit.camp;
        }

        public void CreateEnemy(LVector2 bornPos){
            var type = _randomService.Range(0, _config.enemyPrefabs.Count);
            CreateEnemy(bornPos, type);
        }

        public void CreateEnemy(LVector2 bornPos, int type){
            var createPos = bornPos + LVector2.right;
            _gameEffectService.ShowBornEffect(createPos);
            _gameAudioService.PlayClipBorn();
            EDir dir = EDir.Down;
            DelayCall(GameConfig.TankBornDelay,
                () => {
                    CreateUnit(createPos, _config.enemyPrefabs, type, dir, transParentEnemy);
                });
        }

        public void CreatePlayer(byte actorId, int type){
            var bornPos = _gameConstStateService.playerBornPoss[actorId%_gameConstStateService.playerBornPoss.Count];
            var createPos = bornPos + GameConfig.TankBornOffset;
            _gameEffectService.ShowBornEffect(createPos);
            _gameAudioService.PlayClipBorn();
            EDir dir = EDir.Up;
            DelayCall(GameConfig.TankBornDelay, () => {
                var entity = CreateUnit(createPos, _config.playerPrefabs, type, dir, transParentPlayer);
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


        private GameEntity CreateUnit<T>(LVector2 createPos, List<T> prefabLst, int type, EDir dir, Transform parent)
            where T : GameConfig.Unit{
            var ecsPrefab = prefabLst[type];
            var assetId = ecsPrefab.asset.assetId;
            var entity = CreateGameEntity();
            ecsPrefab.SetComponentsTo(entity);
            entity.dir.value = dir;
            entity.pos.value = createPos;
            if (!_constStateService.IsVideoLoading) {
                _viewService.BindView(entity, go);
            }
            return entity;
        }

        ///用于惟一标记 GameEntity 用于回滚
        private uint _localIdCounter;

        private GameEntity CreateGameEntity(){
            var entity = _gameContext.CreateEntity();
            entity.AddLocalId(_localIdCounter);
            _localIdCounter++;
            return entity;
        }

        public void Upgrade(GameEntity entity,int playerCount){
            var targetType = entity.unit.detailType +1;
            if (targetType >= playerCount) {
                Debug.Log($"hehe already max level can not upgrade");
                return;
            }
            
            var ecsPrefab = _config.playerPrefabs[targetType];
            var rawPos = entity.pos.value;
            var rawDir = entity.dir.value;
            ecsPrefab.SetComponentsTo(entity);
            entity.pos.value = rawPos;
            entity.dir.value = rawDir;

        }
        public void DelayCall(LFloat delay, Action callback){
            var delayEntity = CreateGameEntity();
            delayEntity.AddDelayCall(delay, FuncUtil.RegisterFunc(callback));
        }

        #endregion
    }
}