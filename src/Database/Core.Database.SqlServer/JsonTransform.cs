using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Database.SqlServer
{
    public class JsonTransform
    {
        [SqlFunction(DataAccess = DataAccessKind.Read, IsDeterministic = true)]
        [return: SqlFacet(MaxSize = -1)]
        public static void FunctionFromQueryToJson([SqlFacet(IsNullable = false, MaxSize = -1)] SqlString selectStatement, [SqlFacet(IsNullable = false)] SqlBoolean useDoubleQuotes, ref SqlString data)
        {
            try
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = new SqlConnection("context connection=true");
                connection.Open();
                new SqlDataAdapter() { SelectCommand = new SqlCommand((string)selectStatement, connection) }.Fill(dataTable);
                char quoteChar = useDoubleQuotes == (SqlBoolean)true ? '"' : '\'';
                string str = Serialize(dataTable, quoteChar);
                connection.Close();
                data = (SqlString)str;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + (string)selectStatement);
            }
            
        }

        [SqlProcedure]
        [return: SqlFacet(MaxSize = -1)]
        public static void ProcedureFromQueryToJson([SqlFacet(IsNullable = false, MaxSize = -1)] SqlString selectStatement, [SqlFacet(IsNullable = false)] SqlBoolean useDoubleQuotes)
        {
            DataTable dataTable = new DataTable();
            SqlConnection selectConnection = new SqlConnection("context connection=true");
            selectConnection.Open();
            new SqlDataAdapter((string)selectStatement, selectConnection).Fill(dataTable);
            char quoteChar = useDoubleQuotes == (SqlBoolean)true ? '"' : '\'';
            string s = Serialize(dataTable, quoteChar);
            selectConnection.Close();
            SqlDataRecord record = new SqlDataRecord(new SqlMetaData[1]
                                                         {
                                                             new SqlMetaData("JSONData", SqlDbType.NVarChar, 4000L)
                                                         });
            if (dataTable.Rows.Count == 0)
            {
                SqlContext.Pipe.Send(record);
            }
            else
            {
                bool flag = false;
                StringReader stringReader = new StringReader(s);
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    record.SetValue(0, (object)str);
                    if (!flag)
                    {
                        SqlContext.Pipe.SendResultsStart(record);
                        flag = true;
                    }
                    else
                        SqlContext.Pipe.SendResultsRow(record);
                }
                if (flag)
                    SqlContext.Pipe.SendResultsEnd();
            }
        }

        [SqlProcedure]
        public static SqlInt32 ProcedureFromJsonToTable([SqlFacet(IsNullable = false, MaxSize = -1)] SqlString jsonText, [SqlFacet(IsNullable = false, MaxSize = 128)] SqlString templateTableName)
        {
            DataTable dataTable = new DataTable();
            SqlConnection selectConnection = new SqlConnection("context connection=true");
            selectConnection.Open();
            new SqlDataAdapter(string.Format("SELECT * FROM {0} WHERE 1=2", (object)(string)templateTableName), selectConnection).Fill(dataTable);
            Deserialize((string)jsonText, ref dataTable);
            List<SqlMetaData> list = new List<SqlMetaData>(dataTable.Columns.Count);
            foreach (DataColumn dataColumn in (InternalDataCollectionBase)dataTable.Columns)
            {
                SqlDbType dbType = GetDbType(dataColumn.DataType);
                SqlMetaData sqlMetaData = SqlDbType.Binary != dbType && SqlDbType.Image != dbType && (SqlDbType.NText != dbType && SqlDbType.NVarChar != dbType) && (SqlDbType.Text != dbType && SqlDbType.Udt != dbType && (SqlDbType.VarBinary != dbType && SqlDbType.VarChar != dbType)) && SqlDbType.Variant != dbType && SqlDbType.Xml != dbType ? new SqlMetaData(dataColumn.ColumnName, dbType) : new SqlMetaData(dataColumn.ColumnName, dbType, (long)dataColumn.MaxLength);
                list.Add(sqlMetaData);
            }
            SqlDataRecord record = new SqlDataRecord(list.ToArray());
            SqlContext.Pipe.SendResultsStart(record);
            foreach (DataRow dataRow in (InternalDataCollectionBase)dataTable.Rows)
            {
                for (int ordinal = 0; ordinal < dataTable.Columns.Count; ++ordinal)
                    record.SetValue(ordinal, dataRow.ItemArray[ordinal]);
                SqlContext.Pipe.SendResultsRow(record);
            }
            SqlContext.Pipe.SendResultsEnd();
            return (SqlInt32)dataTable.Rows.Count;
        }

        [SqlProcedure]
        public static SqlInt32 ProcedureJsonBulkCopy([SqlFacet(IsNullable = false, MaxSize = -1)] SqlString jsonText, [SqlFacet(IsNullable = false, MaxSize = 128)] SqlString destinationTableName, [SqlFacet(IsNullable = true, MaxSize = 128)] SqlString destinationServerName, [SqlFacet(IsNullable = true, MaxSize = 128)] SqlString destinationDatabaseName, [SqlFacet(IsNullable = true, MaxSize = 128)] SqlString destinationUserName, [SqlFacet(IsNullable = true, MaxSize = 128)] SqlString destinationPassword, [SqlFacet(IsNullable = true)] SqlInt32 batchSize, [SqlFacet(IsNullable = true)] SqlInt32 timeout, [SqlFacet(IsNullable = true)] SqlBoolean checkConstraints, [SqlFacet(IsNullable = true)] SqlBoolean keepIdentity, [SqlFacet(IsNullable = true)] SqlBoolean keepNulls, [SqlFacet(IsNullable = true)] SqlBoolean tableLock, [SqlFacet(IsNullable = true)] SqlBoolean fireTriggers, [SqlFacet(IsNullable = true)] SqlBoolean useInternalTransaction)
        {
            DataTable dataTable = new DataTable();
            SqlConnection sqlConnection = new SqlConnection("context connection=true");
            
            sqlConnection.Open();
            string str1 = destinationServerName.IsNull ? sqlConnection.DataSource : (string)destinationServerName;
            string str2 = destinationDatabaseName.IsNull ? sqlConnection.Database : (string)destinationDatabaseName;
            new SqlDataAdapter(string.Format("SELECT * FROM {0} WHERE 1=2", (object)(string)destinationTableName), sqlConnection).Fill(dataTable);
            if (sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();
            Deserialize((string)jsonText, ref dataTable);
            string str3;
            if (!destinationUserName.IsNull && !destinationPassword.IsNull)
                str3 = string.Format("Server={0};Database={1};Username={2},;Password={3}", (object)str1, (object)str2, (object)(string)destinationUserName, (object)(string)destinationPassword);
            else
                str3 = string.Format("Server={0};Database={1};Trusted_Connection=Yes", (object)str1, (object)str2);
            sqlConnection.ConnectionString = str3;
            sqlConnection.Open();
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default;
            if (checkConstraints.IsNull && keepIdentity.IsNull && (keepNulls.IsNull && tableLock.IsNull) && fireTriggers.IsNull && useInternalTransaction.IsNull)
            {
                copyOptions = SqlBulkCopyOptions.Default;
            }
            else
            {
                if ((bool)checkConstraints)
                    copyOptions |= SqlBulkCopyOptions.CheckConstraints;
                if ((bool)keepIdentity)
                    copyOptions |= SqlBulkCopyOptions.KeepIdentity;
                if ((bool)keepNulls)
                    copyOptions |= SqlBulkCopyOptions.KeepNulls;
                if ((bool)tableLock)
                    copyOptions |= SqlBulkCopyOptions.TableLock;
                if ((bool)fireTriggers)
                    copyOptions |= SqlBulkCopyOptions.FireTriggers;
                if ((bool)useInternalTransaction)
                    copyOptions |= SqlBulkCopyOptions.UseInternalTransaction;
            }
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection, copyOptions, (SqlTransaction)null);
            sqlBulkCopy.DestinationTableName = (string)destinationTableName;
            if (!batchSize.IsNull)
                sqlBulkCopy.BatchSize = (int)batchSize;
            if (!timeout.IsNull)
                sqlBulkCopy.BulkCopyTimeout = (int)timeout;
            sqlBulkCopy.WriteToServer(dataTable);
            sqlConnection.Close();
            return (SqlInt32)dataTable.Rows.Count;
        }

        private static string Serialize(DataTable dataTable, char quoteChar)
        {
            JsonSerializer jsonSerializer = new JsonSerializer()
            {
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateParseHandling = DateParseHandling.DateTime,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            jsonSerializer.Converters.Add((JsonConverter)new DataTableConverter());
            StringWriter stringWriter = new StringWriter();
            JsonTextWriter jsonTextWriter1 = new JsonTextWriter((TextWriter)stringWriter);
            jsonTextWriter1.Formatting = Formatting.None;
            jsonTextWriter1.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            
            jsonTextWriter1.QuoteChar = quoteChar;
            
            JsonTextWriter jsonTextWriter2 = jsonTextWriter1;
            jsonSerializer.Serialize((JsonWriter)jsonTextWriter2, (object)dataTable);
            string str = stringWriter.ToString();
            jsonTextWriter2.Close();
            stringWriter.Close();
            return str;
        }

        private static void Deserialize(string jsonText, ref DataTable dataTable)
        {
            JsonSerializer jsonSerializer = new JsonSerializer()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            JsonTextReader jsonTextReader = new JsonTextReader((TextReader)new StringReader(jsonText));
            dataTable = (DataTable)jsonSerializer.Deserialize((JsonReader)jsonTextReader, typeof(DataTable));
            jsonTextReader.Close();
        }

        private static SqlDbType GetDbType(Type t)
        {
            SqlParameter sqlParameter = new SqlParameter();
            TypeConverter converter = TypeDescriptor.GetConverter((object)sqlParameter.DbType);
            if (converter.CanConvertFrom(t))
            {
                object obj = converter.ConvertFrom((object)t.Name);
                if (obj != null)
                    sqlParameter.DbType = (DbType)obj;
            }
            else
            {
                try
                {
                    object obj = converter.ConvertFrom((object)t.Name);
                    if (obj != null)
                        sqlParameter.DbType = (DbType)obj;
                }
                catch { }
            }
            return sqlParameter.SqlDbType;
        }
    }
}