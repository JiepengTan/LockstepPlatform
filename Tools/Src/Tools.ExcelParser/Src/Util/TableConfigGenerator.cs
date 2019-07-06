namespace CSVGenCode {
    public class TableConfigGenerator : ConfigGenerator {
        string attriDeclareTemplet =
            @"
        /// #Comment
        public #AttriType #AttriName;";

        string attriAssignTemplet =
            @"      
            table.#AttriName = bytesData.Read#AttriReadType();";

        protected override string GetAttriDeclare(TableField info){
            return attriDeclareTemplet.Replace("#AttriName", info.filedName)
                .Replace("#Comment",
                    info.isList ? (info.comment + "  !" + info.fieldType.ToString().ToLower() + " List") : info.comment)
                .Replace("#AttriType", info.isList ? "string" : info.fieldType.ToString().ToLower());
        }

        protected override string GetAttriAssign(TableField info){
            return attriAssignTemplet.Replace("#AttriName", info.filedName)
                .Replace("#AttriReadType", (info.isList ? "List" : "") + typeToAssignTypeStr[info.fieldType]
                );
        }
    }
}