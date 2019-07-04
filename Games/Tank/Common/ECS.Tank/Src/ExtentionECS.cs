//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using System;
using System.Collections.Generic;
using Lockstep.Serialization;
namespace Lockstep.ECS.Input{public partial class ActorIdComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class ActorIdComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class AIComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class AssetComponent : BaseComponent{}};
namespace Lockstep.ECS.Actor{public partial class BackupComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class BackupComponent : BaseComponent{}};
namespace Lockstep.ECS.GameState{public partial class BackupCurFrameComponent : BaseComponent{}};
namespace Lockstep.ECS.GameState{public partial class BeforeExecuteHashCodeComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class BornPointComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class BulletComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class ColliderComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class DelayCallComponent : BaseComponent{}};
namespace Lockstep.ECS.Input{public partial class DestroyedComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class DestroyedComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class DirComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class DropRateComponent : BaseComponent{}};
namespace Lockstep.ECS.Input{public partial class EntityConfigIdComponent : BaseComponent{}};
namespace Lockstep.ECS.Actor{public partial class EntityCountComponent : BaseComponent{}};
namespace Lockstep.ECS.Input{public partial class FireComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class FireRequestComponent : BaseComponent{}};
namespace Lockstep.ECS.Actor{public partial class GameLocalIdComponent : BaseComponent{}};
namespace Lockstep.ECS.Snapshot{public partial class HashCodeComponent : BaseComponent{}};
namespace Lockstep.ECS.Debug{public partial class HashCodeComponent : BaseComponent{}};
namespace Lockstep.ECS.GameState{public partial class HashCodeComponent : BaseComponent{}};
namespace Lockstep.ECS.Actor{public partial class IdComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class ItemTypeComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class LifeComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class LocalIdComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class MoveComponent : BaseComponent{}};
namespace Lockstep.ECS.Input{public partial class MoveDirComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class MoveRequestComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class OwnerComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class PosComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class ScoreComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class SkillComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class TagBulletComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class TagCampComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class TagEnemyComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class TagTankComponent : BaseComponent{}};
namespace Lockstep.ECS.Input{public partial class TickComponent : BaseComponent{}};
namespace Lockstep.ECS.Snapshot{public partial class TickComponent : BaseComponent{}};
namespace Lockstep.ECS.Debug{public partial class TickComponent : BaseComponent{}};
namespace Lockstep.ECS.GameState{public partial class TickComponent : BaseComponent{}};
namespace Lockstep.ECS.Game{public partial class UnitComponent : BaseComponent{}};


#if !DONT_USE_GENERATE_CODE

namespace Lockstep.ECS.Input{
    [System.Serializable]
    public partial class ActorIdComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadByte();
        }

        public override void CopyTo(object comp){
            var dst = (ActorIdComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new ActorIdComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (ActorIdComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class ActorIdComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadByte();
        }

        public override void CopyTo(object comp){
            var dst = (ActorIdComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new ActorIdComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (ActorIdComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class AIComponent  {
        public override void Serialize(Serializer writer){
			writer.PutLFloat(fireRate);
			writer.PutLFloat(timer);
			writer.PutLFloat(updateInterval);
        }
    
        public override void Deserialize(Deserializer reader){
			fireRate = reader.GetLFloat();
			timer = reader.GetLFloat();
			updateInterval = reader.GetLFloat();
        }

        public override void CopyTo(object comp){
            var dst = (AIComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.fireRate = fireRate;
			dst.timer = timer;
			dst.updateInterval = updateInterval;
        }
        
        public override object Clone(){
            var dst = new AIComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (AIComponent) obj;
            if (dst == null) return false;
			if (fireRate != dst.fireRate) return false;
			if (timer != dst.timer) return false;
			if (updateInterval != dst.updateInterval) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class AssetComponent  {
        public override void Serialize(Serializer writer){
			writer.Write((int)(assetId));
        }
    
        public override void Deserialize(Deserializer reader){
			assetId = (Lockstep.Game.EAssetID)reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (AssetComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.assetId = assetId;
        }
        
        public override object Clone(){
            var dst = new AssetComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (AssetComponent) obj;
            if (dst == null) return false;
			if (assetId != dst.assetId) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Actor{
    [System.Serializable]
    public partial class BackupComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(actorId);
			writer.Write(tick);
        }
    
        public override void Deserialize(Deserializer reader){
			actorId = reader.ReadByte();
			tick = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (BackupComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.actorId = actorId;
			dst.tick = tick;
        }
        
        public override object Clone(){
            var dst = new BackupComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (BackupComponent) obj;
            if (dst == null) return false;
			if (actorId != dst.actorId) return false;
			if (tick != dst.tick) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class BackupComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(localEntityId);
			writer.Write(tick);
        }
    
        public override void Deserialize(Deserializer reader){
			localEntityId = reader.ReadUInt32();
			tick = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (BackupComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.localEntityId = localEntityId;
			dst.tick = tick;
        }
        
        public override object Clone(){
            var dst = new BackupComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (BackupComponent) obj;
            if (dst == null) return false;
			if (localEntityId != dst.localEntityId) return false;
			if (tick != dst.tick) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.GameState{
    [System.Serializable]
    public partial class BackupCurFrameComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (BackupCurFrameComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new BackupCurFrameComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (BackupCurFrameComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.GameState{
    [System.Serializable]
    public partial class BeforeExecuteHashCodeComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt64();
        }

        public override void CopyTo(object comp){
            var dst = (BeforeExecuteHashCodeComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new BeforeExecuteHashCodeComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (BeforeExecuteHashCodeComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class BornPointComponent  {
        public override void Serialize(Serializer writer){
			writer.PutLVector2(coord);
        }
    
        public override void Deserialize(Deserializer reader){
			coord = reader.GetLVector2();
        }

        public override void CopyTo(object comp){
            var dst = (BornPointComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.coord = coord;
        }
        
        public override object Clone(){
            var dst = new BornPointComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (BornPointComponent) obj;
            if (dst == null) return false;
			if (coord != dst.coord) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class BulletComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(canDestoryGrass);
			writer.Write(canDestoryIron);
			writer.Write(ownerLocalId);
        }
    
        public override void Deserialize(Deserializer reader){
			canDestoryGrass = reader.ReadBoolean();
			canDestoryIron = reader.ReadBoolean();
			ownerLocalId = reader.ReadUInt32();
        }

        public override void CopyTo(object comp){
            var dst = (BulletComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.canDestoryGrass = canDestoryGrass;
			dst.canDestoryIron = canDestoryIron;
			dst.ownerLocalId = ownerLocalId;
        }
        
        public override object Clone(){
            var dst = new BulletComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (BulletComponent) obj;
            if (dst == null) return false;
			if (canDestoryGrass != dst.canDestoryGrass) return false;
			if (canDestoryIron != dst.canDestoryIron) return false;
			if (ownerLocalId != dst.ownerLocalId) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class ColliderComponent  {
        public override void Serialize(Serializer writer){
			writer.PutLFloat(radius);
			writer.PutLVector2(size);
        }
    
        public override void Deserialize(Deserializer reader){
			radius = reader.GetLFloat();
			size = reader.GetLVector2();
        }

        public override void CopyTo(object comp){
            var dst = (ColliderComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.radius = radius;
			dst.size = size;
        }
        
        public override object Clone(){
            var dst = new ColliderComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (ColliderComponent) obj;
            if (dst == null) return false;
			if (radius != dst.radius) return false;
			if (size != dst.size) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class DelayCallComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(callBack);
			writer.PutLFloat(delayTimer);
        }
    
        public override void Deserialize(Deserializer reader){
			callBack = reader.ReadInt32();
			delayTimer = reader.GetLFloat();
        }

        public override void CopyTo(object comp){
            var dst = (DelayCallComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.callBack = callBack;
			dst.delayTimer = delayTimer;
        }
        
        public override object Clone(){
            var dst = new DelayCallComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (DelayCallComponent) obj;
            if (dst == null) return false;
			if (callBack != dst.callBack) return false;
			if (delayTimer != dst.delayTimer) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Input{
    [System.Serializable]
    public partial class DestroyedComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (DestroyedComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new DestroyedComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (DestroyedComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class DestroyedComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (DestroyedComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new DestroyedComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (DestroyedComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class DirComponent  {
        public override void Serialize(Serializer writer){
			writer.Write((int)(value));
        }
    
        public override void Deserialize(Deserializer reader){
			value = (Lockstep.Game.EDir)reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (DirComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new DirComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (DirComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class DropRateComponent  {
        public override void Serialize(Serializer writer){
			writer.PutLFloat(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.GetLFloat();
        }

        public override void CopyTo(object comp){
            var dst = (DropRateComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new DropRateComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (DropRateComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Input{
    [System.Serializable]
    public partial class EntityConfigIdComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (EntityConfigIdComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new EntityConfigIdComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (EntityConfigIdComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Actor{
    [System.Serializable]
    public partial class EntityCountComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadUInt32();
        }

        public override void CopyTo(object comp){
            var dst = (EntityCountComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new EntityCountComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (EntityCountComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Input{
    [System.Serializable]
    public partial class FireComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (FireComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new FireComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (FireComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class FireRequestComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (FireRequestComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new FireRequestComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (FireRequestComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Actor{
    [System.Serializable]
    public partial class GameLocalIdComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadUInt32();
        }

        public override void CopyTo(object comp){
            var dst = (GameLocalIdComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new GameLocalIdComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (GameLocalIdComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Debug{
    [System.Serializable]
    public partial class HashCodeComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt64();
        }

        public override void CopyTo(object comp){
            var dst = (HashCodeComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new HashCodeComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (HashCodeComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.GameState{
    [System.Serializable]
    public partial class HashCodeComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt64();
        }

        public override void CopyTo(object comp){
            var dst = (HashCodeComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new HashCodeComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (HashCodeComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Snapshot{
    [System.Serializable]
    public partial class HashCodeComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt64();
        }

        public override void CopyTo(object comp){
            var dst = (HashCodeComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new HashCodeComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (HashCodeComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Actor{
    [System.Serializable]
    public partial class IdComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadByte();
        }

        public override void CopyTo(object comp){
            var dst = (IdComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new IdComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (IdComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class ItemTypeComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(killerActorId);
			writer.Write((int)(type));
        }
    
        public override void Deserialize(Deserializer reader){
			killerActorId = reader.ReadByte();
			type = (Lockstep.ECS.Game.EItemType)reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (ItemTypeComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.killerActorId = killerActorId;
			dst.type = type;
        }
        
        public override object Clone(){
            var dst = new ItemTypeComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (ItemTypeComponent) obj;
            if (dst == null) return false;
			if (killerActorId != dst.killerActorId) return false;
			if (type != dst.type) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class LifeComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (LifeComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new LifeComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (LifeComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class LocalIdComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadUInt32();
        }

        public override void CopyTo(object comp){
            var dst = (LocalIdComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new LocalIdComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (LocalIdComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class MoveComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(isChangedDir);
			writer.PutLFloat(maxMoveSpd);
			writer.PutLFloat(moveSpd);
        }
    
        public override void Deserialize(Deserializer reader){
			isChangedDir = reader.ReadBoolean();
			maxMoveSpd = reader.GetLFloat();
			moveSpd = reader.GetLFloat();
        }

        public override void CopyTo(object comp){
            var dst = (MoveComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.isChangedDir = isChangedDir;
			dst.maxMoveSpd = maxMoveSpd;
			dst.moveSpd = moveSpd;
        }
        
        public override object Clone(){
            var dst = new MoveComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (MoveComponent) obj;
            if (dst == null) return false;
			if (isChangedDir != dst.isChangedDir) return false;
			if (maxMoveSpd != dst.maxMoveSpd) return false;
			if (moveSpd != dst.moveSpd) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Input{
    [System.Serializable]
    public partial class MoveDirComponent  {
        public override void Serialize(Serializer writer){
			writer.Write((int)(value));
        }
    
        public override void Deserialize(Deserializer reader){
			value = (Lockstep.Game.EDir)reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (MoveDirComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new MoveDirComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (MoveDirComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class MoveRequestComponent  {
        public override void Serialize(Serializer writer){
			writer.Write((int)(value));
        }
    
        public override void Deserialize(Deserializer reader){
			value = (Lockstep.Game.EDir)reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (MoveRequestComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new MoveRequestComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (MoveRequestComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class OwnerComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(localId);
        }
    
        public override void Deserialize(Deserializer reader){
			localId = reader.ReadUInt32();
        }

        public override void CopyTo(object comp){
            var dst = (OwnerComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.localId = localId;
        }
        
        public override object Clone(){
            var dst = new OwnerComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (OwnerComponent) obj;
            if (dst == null) return false;
			if (localId != dst.localId) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class PosComponent  {
        public override void Serialize(Serializer writer){
			writer.PutLVector2(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.GetLVector2();
        }

        public override void CopyTo(object comp){
            var dst = (PosComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new PosComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (PosComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class ScoreComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (ScoreComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new ScoreComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (ScoreComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class SkillComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(bulletId);
			writer.PutLFloat(cd);
			writer.PutLFloat(cdTimer);
			writer.Write(isNeedFire);
        }
    
        public override void Deserialize(Deserializer reader){
			bulletId = reader.ReadInt32();
			cd = reader.GetLFloat();
			cdTimer = reader.GetLFloat();
			isNeedFire = reader.ReadBoolean();
        }

        public override void CopyTo(object comp){
            var dst = (SkillComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.bulletId = bulletId;
			dst.cd = cd;
			dst.cdTimer = cdTimer;
			dst.isNeedFire = isNeedFire;
        }
        
        public override object Clone(){
            var dst = new SkillComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (SkillComponent) obj;
            if (dst == null) return false;
			if (bulletId != dst.bulletId) return false;
			if (cd != dst.cd) return false;
			if (cdTimer != dst.cdTimer) return false;
			if (isNeedFire != dst.isNeedFire) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class TagBulletComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (TagBulletComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new TagBulletComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TagBulletComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class TagCampComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (TagCampComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new TagCampComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TagCampComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class TagEnemyComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (TagEnemyComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new TagEnemyComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TagEnemyComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class TagTankComponent  {
        public override void Serialize(Serializer writer){

        }
    
        public override void Deserialize(Deserializer reader){

        }

        public override void CopyTo(object comp){
            var dst = (TagTankComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }

        }
        
        public override object Clone(){
            var dst = new TagTankComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TagTankComponent) obj;
            if (dst == null) return false;

            return true;
        }
    }
}


namespace Lockstep.ECS.Debug{
    [System.Serializable]
    public partial class TickComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadUInt32();
        }

        public override void CopyTo(object comp){
            var dst = (TickComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new TickComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TickComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.GameState{
    [System.Serializable]
    public partial class TickComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (TickComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new TickComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TickComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Input{
    [System.Serializable]
    public partial class TickComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (TickComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new TickComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TickComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Snapshot{
    [System.Serializable]
    public partial class TickComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(value);
        }
    
        public override void Deserialize(Deserializer reader){
			value = reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (TickComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.value = value;
        }
        
        public override object Clone(){
            var dst = new TickComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (TickComponent) obj;
            if (dst == null) return false;
			if (value != dst.value) return false;
            return true;
        }
    }
}


namespace Lockstep.ECS.Game{
    [System.Serializable]
    public partial class UnitComponent  {
        public override void Serialize(Serializer writer){
			writer.Write(damage);
			writer.Write(detailType);
			writer.Write(health);
			writer.Write(killerLocalId);
			writer.Write(name);
			writer.Write((int)(camp));
        }
    
        public override void Deserialize(Deserializer reader){
			damage = reader.ReadInt32();
			detailType = reader.ReadInt32();
			health = reader.ReadInt32();
			killerLocalId = reader.ReadUInt32();
			name = reader.ReadString();
			camp = (Lockstep.Game.ECampType)reader.ReadInt32();
        }

        public override void CopyTo(object comp){
            var dst = (UnitComponent) comp;
            if (dst == null) {
                throw new CopyToUnExceptTypeException(comp == null ? "null" : comp.GetType().ToString());
            }
			dst.damage = damage;
			dst.detailType = detailType;
			dst.health = health;
			dst.killerLocalId = killerLocalId;
			dst.name = name;
			dst.camp = camp;
        }
        
        public override object Clone(){
            var dst = new UnitComponent();
            CopyTo(dst);
            return dst;
        }
        
        public override int GetHashCode(){return base.GetHashCode();}
        public override bool Equals(object obj){
            var dst = (UnitComponent) obj;
            if (dst == null) return false;
			if (damage != dst.damage) return false;
			if (detailType != dst.detailType) return false;
			if (health != dst.health) return false;
			if (killerLocalId != dst.killerLocalId) return false;
			if (name != dst.name) return false;
			if (camp != dst.camp) return false;
            return true;
        }
    }
}


#endif
