using LitJson;

namespace Lockstep.Game {
    public static class JsonExtension {
        public static string ToJson(this object obj){
            return obj == null ? "null" : JsonMapper.ToJson(obj);
        }
    }
}