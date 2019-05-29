using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using DesperateDevs.Utils;
using Lockstep.Logging;
using Debug = UnityEngine.Debug;
using ICloneable = Lockstep.ECS.ICloneable;

namespace Lockstep.ECS{
    public static class ECSExtension {
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