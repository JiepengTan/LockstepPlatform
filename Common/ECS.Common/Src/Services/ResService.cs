﻿using System.Collections.Generic;
using System.IO;
using LitJson;

namespace Lockstep.Game {
    public class ResService : BaseService, IResService {
        private Dictionary<ushort, string> _id2Path = new Dictionary<ushort, string>();
        public override void DoStart(){
            base.DoStart();
            var path = _constStateService.ConfigPath + "AssetPath.json";
            var text = File.ReadAllText(path);
            //TODO 
            var content = JsonMapper.ToObject<Dictionary<string, string>>(text);
            foreach (var pair in content) {
                _id2Path[ushort.Parse(pair.Key)] = pair.Value;
            }
        }

        public string GetAssetPath(ushort assetId){
            if (_id2Path.TryGetValue(assetId, out string path)) {
                return path;
            }
            return null;
        }
    }
}