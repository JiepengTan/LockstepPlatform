using System;
using System.Collections.Generic;
using Entitas;
using DesperateDevs.Utils;
using Lockstep.Math;


namespace Lockstep.ECS{
    public static class ExtensionUtil {
        public static void CopyTo(this Entity srcEntity,Entity destEntity, int componentIndex)
        {
            var component1 = srcEntity.GetComponent(componentIndex);
            Stack<IComponent> componentPool = destEntity.GetComponentPool(componentIndex);
            IComponent backupComponent = null;
            if (componentPool.Count <= 0) {
                if (component1 is ICloneable cloneComp) {
                    backupComponent =  cloneComp.Clone() as IComponent;
                }
                else 
                {
                    backupComponent =  (IComponent) Activator.CreateInstance(component1.GetType());
                    component1.CopyPublicMemberValues(backupComponent);
                }
            }
            else {
                backupComponent = componentPool.Pop();
                if (component1 is ICloneable cloneComp) {
                    cloneComp.CopyTo(backupComponent);
                }
                else 
                {
                    component1.CopyPublicMemberValues(backupComponent);
                }
            }
            destEntity.AddComponent(componentIndex, backupComponent);
        }
    }
}
namespace Lockstep.Serialization {
    public static class ExtensionSerializer {
        public static void PutLFloat(this Serializer serializer, LFloat val){
            serializer.PutInt32(val._val);
        }    
        public static void PutLVector2(this Serializer serializer, LVector2 val){
            serializer.PutInt32( val._x);
            serializer.PutInt32( val._y);
        }      
        public static void PutLVector3(this Serializer serializer, LVector3 val){
            serializer.PutInt32( val._x);
            serializer.PutInt32( val._y);
            serializer.PutInt32( val._z);
        }


        public static LFloat GetLFloat(this Deserializer reader){
            var x = reader.GetInt32();
            return new LFloat(true,x);
        }

        public static LVector2 GetLVector2(this Deserializer reader){
            var x = reader.GetInt32();
            var y = reader.GetInt32();
            return new LVector2(true,x,y);
        }

        public static LVector3 GetLVector3(this Deserializer reader){
            var x = reader.GetInt32();
            var y = reader.GetInt32();
            var z = reader.GetInt32();
            return new LVector3(true,x,y,z);
        }
    }
}