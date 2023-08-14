using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzadDbHelper.DotNetFramework
{
    public class dbConncector : IdbConncector
    {
        private SqlConnection _Connection;
        private String _ConnectionString;
        private SqlDataAdapter _adpter;
        //

        #region Constructor
        //public dbConncector()
        //{
        //    _ConnectionString = ConfigurationManager.ConnectionStrings["tawdeef_ConnectionString"].ToString();
        //    _Connection = new SqlConnection(_ConnectionString);
        //    //InitDbConnetion();
        //}
        public dbConncector(String connStr)
        {
            _ConnectionString = connStr;
            _Connection = new SqlConnection(connStr);
        }
        public dbConncector(SqlConnection con)
        {
            _Connection = con;
            _ConnectionString = con.ConnectionString;
        }
        #endregion

        #region delete

        #region CORE
        private int deleteAllFrom(String tableName, List<Field> field)
        {
            String _q = "";
            try
            {
                String where = "";
                int columns = field.Count();
                for (int i = 0; i < columns - 1; i++)
                {
                    where += " " + field[i].Column + " = @" + field[i].Column;
                }
                where += " " + field[columns - 1].Column + " = @" + field[columns - 1].Column;
                //

                _q = "DELETE FROM " + tableName + " WHERE " + where;
                //
                SqlCommand com = new SqlCommand(_q, _Connection);
                columns = field.Count();
                for (int i = 0; i < columns - 1; i++)
                {
                    com.Parameters.AddWithValue(field[i].Column, field[i].Value);
                }
                com.Parameters.AddWithValue(field[columns - 1].Column, field[columns - 1].Value);
                //
                com.Connection.Open();
                int state = com.ExecuteNonQuery();
                com.Connection.Close();
                return state;
            }
            catch (Exception ex)
            {
                throw new Exception(_q + " <br> " + ex.Message + "<br>" + ex.Source + "<br>" + ex.StackTrace);
            }
        }
        #endregion
        public int DeleteFrom(String TableName, List<Field> ColumnEqValue)
        {
            foreach (Field f in ColumnEqValue)
            {
                f.Value = removeSqlInjection(f.Value.ToString());
            }
            return deleteAllFrom(TableName, ColumnEqValue);
        }
        #endregion

        #region INSERT

        #region CORE
        private int InsertInto(String TableName, List<Field> fields)
        {
            String _q = "";
            try
            {
                _q = " INSERT INTO " + TableName + " ( ";
                SqlCommand com = new SqlCommand();
                int clmns = fields.Count;
                String tmp = "";
                for (int i = 0; i < clmns - 1; i++)
                {
                    _q += fields[i].Column + ", ";
                    tmp += "@" + fields[i].Column + ", ";
                    com.Parameters.AddWithValue("@" + fields[i].Column, fields[i].Value);
                }
                _q += fields[clmns - 1].Column + " ) VALUES ( ";
                tmp += "@" + fields[clmns - 1].Column + " )";
                com.Parameters.AddWithValue("@" + fields[clmns - 1].Column, fields[clmns - 1].Value);
                //
                com.Connection = _Connection;
                com.CommandText = _q + tmp;
                com.Connection.Open();
                int state = com.ExecuteNonQuery();
                com.Connection.Close();
                return state;
            }
            catch (Exception ex)
            {
                throw new Exception(_q + "\r" + ex.Message);
            }
        }
        #endregion
        public int InsertIntoTable(String TableName, List<Field> ColumnsWithValues)
        {
            foreach (Field f in ColumnsWithValues)
            {
                f.Value = removeSqlInjection(f.Value.ToString());
            }
            return InsertInto(TableName, ColumnsWithValues);
        }
        #endregion

        #region UPDATE

        #region Core
        private int Update(String tableName, List<Field> attrs, List<Field> Where)
        {
            String _q = "";
            String qWhere = "";
            String attr = "";
            int attrsCount = attrs.Count;
            int whereCount = Where.Count;
            int cumCpomt = attrsCount + whereCount;
            //SqlParameter[] phars = new SqlParameter[cumCpomt];
            SqlCommand com = new SqlCommand();
            try
            {
                for (int i = 0; i < attrs.Count - 1; i++)
                {
                    attr += attrs[i].Column + "=@" + attrs[i].Column + ", ";
                    com.Parameters.AddWithValue("@" + attrs[i].Column, attrs[i].Value);
                }
                attr += attrs[attrsCount - 1].Column + "=@" + attrs[attrsCount - 1].Column;
                com.Parameters.AddWithValue("@" + attrs[attrsCount - 1].Column, attrs[attrsCount - 1].Value);
                //
                for (int i = 0; i < Where.Count - 1; i++)
                {
                    qWhere += Where[i].Column + "=@" + Where[i].Column + " AND ";
                    com.Parameters.AddWithValue("@" + Where[i].Column, Where[i].Value);
                }
                attrsCount++;
                qWhere += Where[whereCount - 1].Column + "=@" + Where[whereCount - 1].Column;
                com.Parameters.AddWithValue("@" + Where[whereCount - 1].Column, Where[whereCount - 1].Value);
                //
                _q = "UPDATE " + tableName + " SET " + attr + " WHERE " + qWhere;
                //
                com.Connection = _Connection;
                com.CommandText = _q;
                com.Connection.Open();
                int state = com.ExecuteNonQuery();
                com.Connection.Close();
                //
                return state;
            }
            catch (Exception ex)
            {
                throw new Exception("Error : Query[" + "UPDATE " + tableName + " SET " + attr + " WHERE " + qWhere + "]" + ex.StackTrace + ":" + ex.Message + ":,{0}");
            }
        }
        #endregion
        public int UpdateTable(String TableName, List<Field> Attr, List<Field> Where)
        {
            foreach (Field field in Attr)
            {
                field.Value = removeSqlInjection(field.Value.ToString());
            }
            return Update(TableName, Attr, Where);
        }

        #endregion

        //
        public DataTable exec(String queiry)
        {
            String _q = queiry;
            try
            {
                _adpter = new SqlDataAdapter(_q, _ConnectionString);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]", "getAllFrom", _q, ex.Message));
            }
        }
        //
        public DataTable getAllFrom(String tableName)
        {
            String _q = "";
            try
            {
                tableName = removeSqlInjection(tableName);
                _q = "SELECT * FROM " + tableName;
                _adpter = new SqlDataAdapter(_q, _ConnectionString);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                //dt.Rows.Cast<>
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]", nameof(getAllFrom), _q, ex.Message));
            }
        }
        //
        public DataTable getAllFromInnerJoin(String tableName, String secondTableName, List<KeyValuePair<string, string>> InnerOn)
        {
            String _q = "";
            try
            {
                int num = InnerOn.Count;
                tableName = removeSqlInjection(tableName);
                _q = $"SELECT * FROM {tableName} inner join {secondTableName} on ";
                for (int i = 0; i < num - 1; i++)
                {
                    _q += $" {tableName}.{InnerOn[i].Key} = {secondTableName}.{InnerOn[i].Value} and";
                }
                _q += $" {tableName}.{InnerOn[num - 1].Key} = {secondTableName}.{InnerOn[num - 1].Value} ";

                _adpter = new SqlDataAdapter(_q, _ConnectionString);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                //dt.Rows.Cast<>
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]", nameof(getAllFrom), _q, ex.Message));
            }
        }
        //
        public DataTable getAllFromInnerJoin(List<string> tableNames, List<KeyValuePair<string, string>> InnerOn, List<Field> Where)
        {
            String _q = "";
            try
            {
                int num = InnerOn.Count;
                bool isOk = tableNames.Count == InnerOn.Count - 1;
                //tableName = removeSqlInjection(tableName);
                _q = $"SELECT * FROM {tableNames[0]}";
                for (int i = 0; i < num - 1; i++)
                {
                    _q += $" inner join {tableNames[i + 1]} on {InnerOn[i].Key} = {InnerOn[i].Value}";
                }
                _q += $" inner join {tableNames[num]} on {InnerOn[num - 1].Key} = {InnerOn[num - 1].Value} ";

                //
                var com = new SqlCommand();
                com.Connection = new SqlConnection(_ConnectionString);
                String where = "where  ";
                int columns = Where.Count();
                for (int i = 0; i < columns - 1; i++)
                {
                    where += " " + Where[i].Column + " = @" + Where[i].Column + " and ";
                    com.Parameters.AddWithValue(Where[i].Column, Where[i].Value);

                }
                where += " " + Where[columns - 1].Column + " = @" + Where[columns - 1].Column;
                com.Parameters.AddWithValue(Where[columns - 1].Column, Where[columns - 1].Value);
                com.CommandText = _q += where;
                _adpter = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                //dt.Rows.Cast<>
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]", nameof(getAllFrom), _q, ex.Message));
            }
        }
        //
        public DataTable getAllFromInnerJoin(List<string> tableNames, List<KeyValuePair<string, string>> InnerOn)
        {
            String _q = "";
            try
            {
                int num = InnerOn.Count;
                bool isOk = tableNames.Count == InnerOn.Count - 1;
                //tableName = removeSqlInjection(tableName);
                _q = $"SELECT * FROM {tableNames[0]}";
                for (int i = 0; i < num - 1; i++)
                {
                    _q += $" inner join {tableNames[i + 1]} on {InnerOn[i].Key} = {InnerOn[i].Value}";
                }
                _q += $" inner join {tableNames[num]} on {InnerOn[num - 1].Key} = {InnerOn[num - 1].Value} ";

                _adpter = new SqlDataAdapter(_q, _ConnectionString);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                //dt.Rows.Cast<>
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]", nameof(getAllFrom), _q, ex.Message));
            }
        }
        //
        public DataTable getAllFromInnerJoin(String tableName, String secondTableName, List<KeyValuePair<string, string>> InnerOn, List<Field> Where)
        {
            String _q = "";
            try
            {
                int num = InnerOn.Count;
                tableName = removeSqlInjection(tableName);
                _q = $"SELECT * FROM {tableName} inner join {secondTableName} on ";
                for (int i = 0; i < num - 1; i++)
                {
                    _q += $" {tableName}.{InnerOn[i].Key} = {secondTableName}.{InnerOn[i].Value} and";
                }
                _q += $" {tableName}.{InnerOn[num - 1].Key} = {secondTableName}.{InnerOn[num - 1].Value} ";

                //
                var com = new SqlCommand();
                String where = "and ";
                int columns = Where.Count();
                for (int i = 0; i < columns - 1; i++)
                {
                    where += " " + Where[i].Column + " = @" + Where[i].Column + " and ";
                    com.Parameters.AddWithValue(Where[i].Column, Where[i].Value);

                }
                where += " " + Where[columns - 1].Column + " = @" + Where[columns - 1].Column;
                com.Parameters.AddWithValue(Where[columns - 1].Column, Where[columns - 1].Value);

                com.CommandText = _q + where;
                com.Connection = new SqlConnection(_ConnectionString);
                _adpter = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                //dt.Rows.Cast<>
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]", nameof(getAllFrom), _q, ex.Message));
            }
        }
        //
        public DataTable getAllFrom(String tableName, List<Field> filds)
        {
            return getAllFromWhere(tableName, null, filds);
        }
        //
        public DataTable getAllFromSp(String PeocName, List<Field> filds)
        {
            String _q = "";
            try
            {
                _q = "Exec " + PeocName + " ";
                int fildsCount = filds.Count;
                for (int i = 0; i < filds.Count - 1; i++)
                {
                    _q += "@" + filds[i].Column + ", ";
                }
                _q += "@" + filds[fildsCount - 1].Column;
                SqlCommand comm = new SqlCommand(_q, new SqlConnection(_ConnectionString));
                for (int i = 0; i < filds.Count - 1; i++)
                {
                    comm.Parameters.AddWithValue("@" + filds[i].Column, filds[i].Value);
                }
                comm.Parameters.AddWithValue("@" + filds[fildsCount - 1].Column, filds[fildsCount - 1].Value);
                _adpter = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                _adpter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                String values = "";
                foreach (Field item in filds)
                {
                    values += item.Value + ", ";
                }
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]\rValues [{3}]", "getAllFromSp", _q, ex.Message, values));
            }

        }
        //
        public int getResultFromSp(String PeocName, List<Field> filds)
        {
            String _q = "";
            try
            {
                _q = "Exec " + PeocName + " ";
                int fildsCount = filds.Count;
                for (int i = 0; i < filds.Count - 1; i++)
                {
                    _q += "@" + filds[i].Column + ", ";
                }
                _q += "@" + filds[fildsCount - 1].Column;
                SqlCommand comm = new SqlCommand(_q, new SqlConnection(_ConnectionString));
                for (int i = 0; i < filds.Count - 1; i++)
                {
                    comm.Parameters.AddWithValue("@" + filds[i].Column, filds[i].Value);
                }
                comm.Parameters.AddWithValue("@" + filds[fildsCount - 1].Column, filds[fildsCount - 1].Value);
                comm.Connection.Open();
                int state = comm.ExecuteNonQuery();
                comm.Connection.Close();
                return state;
            }
            catch (Exception ex)
            {
                String values = "";
                foreach (Field item in filds)
                {
                    values += item.Value + ", ";
                }
                throw new Exception(String.Format("Error in {0} method\rQuery is [{1}]\rError Message :[{2}]\rValues [{3}]", "getResultFromSp", _q, ex.Message, values));
            }

        }
        //
        public DataTable getAllFromWhere(String tableName, List<Field> attrs, List<Field> field, bool allAttriputes = true, bool and = true)
        {
            foreach (Field f in field)
            {
                f.Value = removeSqlInjection(f.Value.ToString());
            }
            //attrs
            String logical = " or ";
            if (and)
            {
                logical = " and ";
            }
            String attr = " * ";
            if (attrs != null)
            {
                attr = " ";
                int attrCount = attrs.Count;
                for (int i = 0; i < attrCount - 1; i++)
                {
                    attr += attrs[i].Column + ", ";
                }
                attr += attrs[attrCount - 1].Column;
            }
            //
            String where = "";
            int columns = field.Count();
            for (int i = 0; i < columns - 1; i++)
            {
                where += " " + field[i].Column + " = @" + field[i].Column + logical;
            }
            where += " " + field[columns - 1].Column + " = @" + field[columns - 1].Column;
            //
            String _q;
            _q = "SELECT " + attr + " FROM " + tableName + " WHERE " + where;
            //
            SqlCommand com = new SqlCommand(_q, _Connection);
            columns = field.Count();
            for (int i = 0; i < columns - 1; i++)
            {
                com.Parameters.AddWithValue(field[i].Column, field[i].Value);
            }
            com.Parameters.AddWithValue(field[columns - 1].Column, field[columns - 1].Value);
            //
            _adpter = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            try
            {
                _adpter.Fill(dt);
            }

            catch (Exception ex)
            {
                throw new Exception(_q + " <br> " + ex.Message + "<br>" + ex.Source + "<br>" + ex.StackTrace);
            }
            return dt;
        }
        //
        public bool TestConnection()
        {
            try
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    _Connection.Open();
                    _Connection.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        //
        private String removeSqlInjection(String q)
        {
            return q.Replace("'", "").Replace("--", "").Replace(";", "");
        }
    }
}
public class Field
{
    public String Column;
    public Object Value;
    public Object secValue;
    //private String hashType = "HASHBYTES('MD5', ## )";
    public String Equalzation;
    private int sides;
    public Field(String c, Object v = null, String eq = "=", int si = 2)
    {
        this.Column = c;
        this.Value = v;
        this.Equalzation = eq;
        this.sides = si;
    }
    //public String getSucre()
    //{
    //    //return secValue = hashType.Replace("##", Value);
    //}
    public String getForQuery()
    {
        String returnText;
        returnText = this.Column + this.Equalzation;
        if (Equalzation.Contains("="))
        {
            returnText += "@" + this.Column;
        }
        else
        {
            switch (sides)
            {
                case 0:
                    returnText += "'" + "%" + this.Value + "'";
                    break;
                case 1:
                    returnText += "'" + this.Value + "%'";
                    break;
                case 2:
                    returnText += "'" + "%" + this.Value + "%'";
                    break;
            }
        }
        return returnText;
    }
    public String getSmallForQuery()
    {
        return this.Column + "=@" + this.Column;
    }
    public SqlParameter getParhameter()
    {
        return new SqlParameter("@" + this.Column, this.Value);
    }
}