using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public interface IUnitService : IService {
        void CreateBullet(LVector2 pos, EDir dir, int type, GameEntity owner);
        void CreateEnemy(LVector2 pos, int type);
        void CreateCamp(LVector2 pos, int type);
        void CreatePlayer(byte actorId, int type);
        void DropItem(LFloat rate);
        
        void TakeDamage(GameEntity atker, GameEntity suffer);
    }

}