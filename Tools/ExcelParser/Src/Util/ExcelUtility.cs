using Excel;
using System.Data;
using System.IO;
using System.Text;

namespace CSVGenCode {
    public class ExcelUtility {
        
        private DataSet mResultSet;
        
        public ExcelUtility(string excelFile) {
            Open(excelFile);
        }

        public void Open(string excelFile) {
            FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
            IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
            mResultSet = mExcelReader.AsDataSet();
        }


        public void ConvertToCSV(string CSVPath, Encoding encoding) {
            if (mResultSet.Tables.Count < 1)
                return;

            DataTable mSheet = mResultSet.Tables[0];

            if (mSheet.Rows.Count < 1)
                return;

            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < rowCount; i++) {
                for (int j = 0; j < colCount; j++) {
                    stringBuilder.Append(mSheet.Rows[i][j] + ",");
                }
                stringBuilder.Append("\r\n");
            }

            using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write)) {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding)) {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }
    }
}