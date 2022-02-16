using Dapper;
using Prospecto.Data;
using Prospecto.Models;
using System.Data;
using System.Data.SqlClient;

namespace Prospecto.Repositorio
{
    public class CRepository<T>:IRepository<T> where T : class
    {
        private readonly ConnectionConfiguration _context;
        public CRepository(ConnectionConfiguration context)
        {
            _context = context;
        }
        public async Task<int> BDAddAsync(string NomStored, string[] storedParam, string[] cVariables)
        {
            using (SqlConnection con = new SqlConnection(_context.Value))
            {
                int DatoId = 0;
                await con.OpenAsync();
                using (var sqltrans = con.BeginTransaction())
                {
                    //SqlTransaction sqltrans=null;
                    try
                    {
                        var sparam = "";
                        var svalor = "";
                        var param = new DynamicParameters();
                        //------------------Crea los parametros para el stored
                        //sqltrans = con.BeginTransaction();
                        for (var i = 0; i < storedParam.Length; i++)
                        {
                            sparam = storedParam[i];
                            svalor = cVariables[i];
                            param.Add(sparam, svalor);
                        }
                        //-------------------------------------------------
                        DatoId = con.Query<int>(NomStored, param, sqltrans,  commandType: CommandType.StoredProcedure).SingleOrDefault();

                        sqltrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqltrans.Rollback();
                        con.Close();
                        throw ex;
                    }
                    finally
                    {
                        sqltrans.Dispose();
                        con.Close();
                    }
                    return DatoId;
                }
            }
        }

//        using var connection = new SqlConnection("some_sql_connection_string");
//    connection.Execute("insert into ContactForms (name, email) values (@Name, @Email)", new { Name = name, Email = email
//});
//}
        public async Task BDAddFileAsync(DocumentModel model)
        {
            using (SqlConnection con = new SqlConnection(_context.Value))
            {
                await con.OpenAsync();
                using (var sqltrans = con.BeginTransaction())
                {
                    try
                    {
                        string sqlquery = "insert into Documentos(FileName, FileType, FileData, Created, Modified, IdProspecto) " +
                                            "values (@FileName, @FileType, @FileData, @Created, @Modified, @IdProspecto)";
                        
                        con.Execute(sqlquery, new{ FileName=model.FileName, FileType=model.FileType, FileData=model.FileData, Created=model.Created, 
                                                    Modified = model.Modified, IdProspecto=model.IdProspecto }, transaction: sqltrans);

                        sqltrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        sqltrans.Rollback();
                        con.Close();
                        throw ex;
                    }
                    finally
                    {
                        sqltrans.Dispose();
                        con.Close();
                    }
                }
            }
        }

        public async Task<IEnumerable<T>> BDLista(string NomStored, string[] storedParam, string[] cVariables)
        {
            IEnumerable<T> lLista;
            using (SqlConnection con = new SqlConnection(_context.Value))
            //using (var con = _context.CreateConnection())
            {
                try
                {
                    var sparam = "";
                    var svalor = "";
                    var param = new DynamicParameters();
                    //------------------Crea los parametros para el stored
                    for (var i = 0; i < storedParam.Length; i++)
                    {
                        sparam = storedParam[i];
                        svalor = cVariables[i];
                        param.Add(sparam, svalor);
                    }
                    //-------------------------------------------------
                    lLista = await con.QueryAsync<T>(NomStored, param: param, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    con.Close();
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
                return lLista;
            }
 
        }
        public async Task<T> BDBuscarReg(string NameProc, string[] storedParam, string[] cVariables)
        {
            IEnumerable<T> valorReg;
            using (SqlConnection con = new SqlConnection(_context.Value))
            //using (var con = _context.CreateConnection())
            {
                try
                {
                    var sparam = "";
                    var svalor = "";
                    con.Open();
                    var param = new DynamicParameters();
                    //------------------Crea los parametros para el stored
                    for (var i = 0; i < storedParam.Length; i++)
                    {
                        sparam = storedParam[i];
                        svalor = cVariables[i];
                        param.Add(sparam, svalor);
                    }
                    //-------------------------------------------------
                    //valorReg = await con.QueryFirstAsync<T>(NameProc, param: param, commandType: CommandType.StoredProcedure);
                    valorReg = await con.QueryAsync<T>(NameProc, param: param, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    con.Close();
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
                //return valorReg;
                return valorReg.FirstOrDefault();
            }

        }
        public IEnumerable<T> BDListaCombo(string NomStored, string[] storedParam, string[] cVariables)
        {
            IEnumerable<T> lLista;
            using (SqlConnection con = new SqlConnection(_context.Value))
            //using (var con = _context.CreateConnection())
            {
                try
                {
                    var sparam = "";
                    var svalor = "";
                    var param = new DynamicParameters();
                    //------------------Crea los parametros para el stored
                    for (var i = 0; i < storedParam.Length; i++)
                    {
                        sparam = storedParam[i];
                        svalor = cVariables[i];
                        param.Add(sparam, svalor);
                    }
                    //-------------------------------------------------
                    lLista = con.Query<T>(NomStored, param: param, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    con.Close();
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
                return lLista;
            }

        }


    }
}
