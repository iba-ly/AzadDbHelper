using System.Collections.Generic;
using System.Data;

namespace AzadDbHelper.DotNetFramework
{
    public interface IdbConncector
    {
        int DeleteFrom(string TableName, List<Field> ColumnEqValue);
        DataTable exec(string queiry);
        DataTable getAllFrom(string tableName);
        DataTable getAllFrom(string tableName, List<Field> filds);
        DataTable getAllFromInnerJoin(List<string> tableNames, List<KeyValuePair<string, string>> InnerOn);
        DataTable getAllFromInnerJoin(List<string> tableNames, List<KeyValuePair<string, string>> InnerOn, List<Field> Where);
        DataTable getAllFromInnerJoin(string tableName, string secondTableName, List<KeyValuePair<string, string>> InnerOn);
        DataTable getAllFromInnerJoin(string tableName, string secondTableName, List<KeyValuePair<string, string>> InnerOn, List<Field> Where);
        DataTable getAllFromSp(string PeocName, List<Field> filds);
        DataTable getAllFromWhere(string tableName, List<Field> attrs, List<Field> field, bool allAttriputes = true, bool and = true);
        int getResultFromSp(string PeocName, List<Field> filds);
        int InsertIntoTable(string TableName, List<Field> ColumnsWithValues);
        bool TestConnection();
        int UpdateTable(string TableName, List<Field> Attr, List<Field> Where);
    }
}