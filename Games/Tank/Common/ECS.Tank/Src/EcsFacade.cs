using System.Collections.Generic;
using System.Reflection;
using Entitas;
using Lockstep.ECS;

namespace Lockstep.Game {
    public class EcsFacade :IECSFacadeService {
        private IContexts lastInstance;

        public IContexts CreateContexts(){
            IContexts ctxs;
            if (lastInstance == null) {
                ctxs = Contexts.sharedInstance;
                InitEntityConfigLUT();
            }
            else {
                ctxs = new Contexts();
            }

            lastInstance = ctxs;
            return ctxs;
        }

        void InitEntityConfigLUT(){
            var name2Idx = new Dictionary<string, int>();
            var fileds = typeof(GameComponentsLookup).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var filed in fileds) {
                if (filed.IsLiteral && !filed.IsInitOnly && filed.FieldType == typeof(int)) {
                    name2Idx.Add(filed.Name + "Component", (int) filed.GetRawConstantValue());
                }
            }

            BaseEntitySetter.UpdateEntityConfigLUT(name2Idx);
        }
    }
}