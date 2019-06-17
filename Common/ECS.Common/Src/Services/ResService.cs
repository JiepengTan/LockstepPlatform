using System.Collections;
using System.Collections.Generic;

namespace Lockstep.Game {
    public class ResService : BaseService, IResService {
        private Dictionary<short, string> _id2Path = new Dictionary<short, string>();

        public string GetAssetPath(short assetId){
            if (_id2Path.TryGetValue(assetId, out string path)) {
                return path;
            }

            return null;
        }
    }
}