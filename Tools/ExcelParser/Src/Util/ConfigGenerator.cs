using Excel;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;

namespace CSVGenCode {
    public abstract class ConfigGenerator {
        public string curDealPath;
        public string curDealExlName;

        public void Run(List<string> InputPaths, string csvDir, string bytesDir, string codeDir,
            string templateFilePath){
            var paths = new Dictionary<string, string>();
            if (Directory.Exists(csvDir)) {
                Directory.Delete(csvDir, true);
            }

            if (Directory.Exists(bytesDir)) {
                Directory.Delete(bytesDir, true);
            }

            if (Directory.Exists(codeDir)) {
                Directory.Delete(codeDir, true);
            }

            templateFilePath = templateFilePath.Replace("\\", "/");
            Debug.LogError(templateFilePath);
            var templeteStr = File.ReadAllText(templateFilePath);
            foreach (var InputPath in InputPaths) {
                Util.Walk(InputPath, "*.xls|*.xlsx",
                    (path) => { SaveToCSV(path, bytesDir, csvDir, codeDir, templeteStr); });
            }
        }

        public void SaveToCSV(string inputPath, string bytesDir, string csvDir, string codeDir, string templateStr){
            Debug.LogError("Deal:" + inputPath);
            var mResultSet = Open(inputPath);
            curDealPath = inputPath;
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            curDealExlName = fileName;
            var csvPath = Path.Combine(csvDir, fileName + ".csv");
            var bytesPath = Path.Combine(bytesDir, fileName + ".bytes");
            var codePath = Path.Combine(codeDir, "Table_" + curDealExlName + ".cs");
            ConvertToCSV(mResultSet, csvPath, Encoding.UTF8);
            ConvertToBytes(mResultSet, bytesPath, Encoding.UTF8);
            GenCode(mResultSet, codePath, templateStr, Encoding.UTF8);
        }

        public void GenCode(DataSet mResultSet, string codePath, string templateStr, Encoding encoding){
            DataTable mSheet;
            int colCount;
            int rowCount;
            List<TableField> heads;
            List<int> keyIdx;
            CollectHeadInfo(mResultSet, out mSheet, out colCount, out rowCount, out heads, out keyIdx);
            var tableName = curDealExlName;
            StringBuilder sb = new StringBuilder();
            foreach (var item in heads) {
                var head = item;
                var attriStr = GetAttriDeclare(head);
                sb.Append(attriStr);
            }

            var declareStr = sb.ToString();
            sb = new StringBuilder();
            foreach (var item in heads) {
                var head = item;
                var attriStr = GetAttriAssign(head);
                sb.Append(attriStr);
            }

            var assignStr = sb.ToString();
            var finalStr = templateStr.Replace("#TableName", tableName)
                    .Replace("#PropertyDeclare", declareStr)
                    .Replace("#PropertyAssign", assignStr)
                ;
            SaveTo(codePath, finalStr, Encoding.UTF8);
        }

        protected abstract string GetAttriDeclare(TableField info);

        protected abstract string GetAttriAssign(TableField info);

        protected Dictionary<ETableBaseType, string> typeToAssignTypeStr = new Dictionary<ETableBaseType, string> {
            {ETableBaseType.Byte, "Byte"},
            {ETableBaseType.Short, "Int16"},
            {ETableBaseType.Int, "Int32"},
            {ETableBaseType.Long, "Int64"},
            {ETableBaseType.Bool, "Boolean"},
            {ETableBaseType.Float, "Single"},
            {ETableBaseType.String, "String"},
        };

        public DataSet Open(string excelFile){
            FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
            IExcelDataReader mExcelReader = null;
            if (excelFile.EndsWith("xls")) {
                mExcelReader = ExcelReaderFactory.CreateBinaryReader(mStream);
            }
            else {
                mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
            }

            return mExcelReader.AsDataSet();
        }

        int curRow = 0;
        int curCol;
        string curCellStr;
        string keyIdxStr;

        public string DebugSttring(){
            return string.Format("path:{0} row = {1}  col= {2} cell = {3} keyStr{4}", curDealExlName, curRow, curCol,
                curCellStr, keyIdxStr);
        }

        public void ConvertToBytes(DataSet mResultSet, string outputPath, Encoding encoding){
            DataTable mSheet;
            int colCount;
            int rowCount;
            List<TableField> heads;
            List<int> keyIdx;
            CollectHeadInfo(mResultSet, out mSheet, out colCount, out rowCount, out heads, out keyIdx);
            MemoryStream mstream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mstream);
            var rowsCount = rowCount - 5;
            var columnsCount = colCount;
            writer.Write((ushort) rowsCount);
            writer.Write((byte) columnsCount);
            for (int i = 0; i < heads.Count; i++) {
                var item = heads[i];
                var val = 0;
                val |= (item.isBase ? 1 : 0) << 0;
                val |= (item.isList ? 1 : 0) << 1;
                val |= ((int) (item.define) & 0x3) << 2;
                val |= ((int) (item.fieldType) & 0xf) << 4;
                writer.Write((byte) val);
            }

            for (int i = 0; i < rowsCount; i++) {
                ulong Key = 0;
                curRow = i + 5;

                List<int> keys = new List<int>();

                for (int kidx = 0; kidx < keyIdx.Count; kidx++) {
                    var cellStr = mSheet.Rows[i + 5][keyIdx[kidx] + 1].ToString();
                    if (string.IsNullOrEmpty(cellStr)) {
                        break;
                    }

                    keyIdxStr = cellStr;
                    keys.Add(int.Parse(cellStr));
                }

                switch (keyIdx.Count) {
                    case 1:
                        Key = TableHelper.GetKey(keys[0]);
                        break;
                    case 2:
                        Key = TableHelper.GetKey(keys[0], keys[1]);
                        break;
                    case 3:
                        Key = TableHelper.GetKey(keys[0], keys[1], keys[2]);
                        break;
                    case 4:
                        Key = TableHelper.GetKey(keys[0], keys[1], keys[2], keys[3]);
                        break;
                    default:
                        throw new Exception("keyIdx.Count out of Range[1,4]" + keyIdx.Count);
                        break;
                }

                writer.Write(Key); //key
                var lenStartIdx = mstream.Length;
                writer.Write(0); //len 先站位
                //write content
                for (int col = 0; col < heads.Count; col++) {
                    var head = heads[col];
                    curCol = col;
                    var cellStr = mSheet.Rows[i + 5][col + 1].ToString();
                    curCellStr = cellStr;
                    if (head.isList) {
                        switch (head.fieldType) {
                            case ETableBaseType.Byte:
                                TableHelper.WriteListByte(writer, cellStr);
                                break;
                            case ETableBaseType.Short:
                                TableHelper.WriteListInt16(writer, cellStr);
                                break;
                            case ETableBaseType.Int:
                                TableHelper.WriteListInt32(writer, cellStr);
                                break;
                            default:
                                throw new Exception("不支持除Int 之外的List");
                        }
                    }
                    else {
                        switch (head.fieldType) {
                            case ETableBaseType.Byte:
                                TableHelper.WriteByte(writer, cellStr);
                                break;
                            case ETableBaseType.Short:
                                TableHelper.WriteInt16(writer, cellStr);
                                break;
                            case ETableBaseType.Int:
                                TableHelper.WriteInt32(writer, cellStr);
                                break;
                            case ETableBaseType.Long:
                                TableHelper.WriteInt64(writer, cellStr);
                                break;
                            case ETableBaseType.Bool:
                                TableHelper.WriteBoolean(writer, cellStr);
                                break;
                            case ETableBaseType.Float:
                                TableHelper.WriteSingle(writer, cellStr);
                                break;
                            case ETableBaseType.String:
                                TableHelper.WriteString(writer, cellStr);
                                break;
                            default:
                                break;
                        }
                    }
                }

                var rowLen = mstream.Length - lenStartIdx - 4;
                mstream.Seek(lenStartIdx, SeekOrigin.Begin);
                writer.Write(rowLen); //len 先站位
                mstream.Seek(0, SeekOrigin.End);
            }

            writer.Flush();

            SaveTo(outputPath, mstream.ToArray());
        }

        private static void CollectHeadInfo(DataSet mResultSet, out DataTable mSheet, out int colCount,
            out int rowCount, out List<TableField> heads, out List<int> keyIdxs){
            if (mResultSet.Tables.Count < 1)
                throw new Exception("Tables.Count < 1");

            mSheet = mResultSet.Tables[0];
            if (mSheet.Rows.Count < 5)
                throw new Exception("Rows.Count < 5");

            rowCount = mSheet.Rows.Count;
            colCount = mSheet.Columns.Count;
            int maxColIdx = 0;
            for (int col = 1; col < colCount; col++) {
                var _str = mSheet.Rows[4][col];
                if (_str == null || string.IsNullOrEmpty(_str.ToString())) {
                    break;
                }

                maxColIdx = col;
            }


            StringBuilder stringBuilder = new StringBuilder();
            //get the header
            heads = new List<TableField>();
            int rowIdx = 0;
            rowIdx = 1;
            for (int col = 1; col <= maxColIdx; col++) {
                var filed = new TableField();
                var str = "";
                int idx = 1;
                //parse key info
                str = mSheet.Rows[idx++][col].ToString().ToLower();
                filed.SetDefine(str);
                //comment 
                str = mSheet.Rows[idx++][col].ToString();
                filed.comment = str;
                //type
                str = mSheet.Rows[idx++][col].ToString().ToLower();
                if (str.EndsWith("[]")) {
                    filed.isList = true;
                    str = str.Replace("[]", "");
                }

                filed.SetFieldType(str);
                //filedName
                str = mSheet.Rows[idx++][col].ToString();
                filed.filedName = str;
                heads.Add(filed);
            }

            keyIdxs = new List<int>();
            for (int i = 0; i < heads.Count; i++) {
                var item = heads[i];
                if (item.define == TableDefine.Key) {
                    keyIdxs.Add(i);
                }
            }

            if (keyIdxs.Count == 0) {
                keyIdxs.Add(0);
            }

            if (keyIdxs.Count > 4) {
                throw new Exception("KeyCount 超过4");
            }

            if (keyIdxs[keyIdxs.Count - 1] != (keyIdxs.Count - 1)) {
                //throw new Exception("Key 不连续 不是前面列");
            }

            foreach (var item in keyIdxs) {
                var head = heads[item];
                if (head.fieldType == ETableBaseType.String
                    || head.fieldType == ETableBaseType.Bool
                    || head.fieldType == ETableBaseType.Float
                ) {
                    throw new Exception("不支持非整形的Key");
                }
            }

            if ((colCount - 1) != heads.Count) {
                //throw new Exception("columnsCount != heads.Count");
                Console.WriteLine("colCount = {0} , heads.Count = {1}", colCount, heads.Count);
            }

            for (int i = 5; i < rowCount; i++) {
                foreach (var keyidx in keyIdxs) {
                    var cellStr = mSheet.Rows[i][keyidx + 1].ToString();
                    if (string.IsNullOrEmpty(cellStr)) {
                        rowCount = i;
                        return;
                    }
                }
            }
        }

        public void ConvertToCSV(DataSet mResultSet, string outputPath, Encoding encoding){
            DataTable mSheet = mResultSet.Tables[0];

            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < rowCount; i++) {
                for (int j = 0; j < colCount; j++) {
                    var cell = mSheet.Rows[i][j];
                    var str = cell.ToString();
                    if (str.Contains(",")) {
                        str = "\"" + str + "\"";
                    }
                    sb.Append(str + ",");
                }

                sb.AppendLine();
            }

            SaveTo(outputPath, sb.ToString(), encoding);
        }

        private static void SaveTo(string path, byte[] bytes){
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllBytes(path, bytes);
        }

        private static void SaveTo(string path, string str, Encoding encoding){
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, str, encoding);
        }
    }
}