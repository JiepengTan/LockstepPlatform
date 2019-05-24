using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lockstep.Logic.Server {
    public class SimpleDatabase {
        
        public Dictionary<string, long> Account2Id = new Dictionary<string, long>();

        void LoadDatabase(){
            Account2Id.Clear();
            var path = DatabasePath;
            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            if (!File.Exists(path)) {
                File.WriteAllText("", path);
            }

            var allLine = File.ReadAllLines(path);
            foreach (var line in allLine) {
                var strs = line.Split(':');
                if (strs.Length != 2) continue;
                if (long.TryParse(strs[1], out long id)) {
                    var account = strs[0];
                    Account2Id[account] = id;
                }
            }
        }

        void FlushCachedData(){
            StringBuilder sb = new StringBuilder();
            foreach (var pair in Account2Id) {
                sb.AppendLine(pair.Key + ":" + pair.Value);
            }

            var path = DatabasePath;
            File.WriteAllText(sb.ToString(), path);
        }
        
        private string _databasePath = null;
        string DatabasePath {
            get {
                if (string.IsNullOrEmpty(_databasePath)) {
                    _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Data/Database.txt");
                }

                return _databasePath;
            }
        }
    }
}