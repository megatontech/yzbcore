using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using yzbcore.Models;
using yzbcore.Repository;

namespace yzbcore.Bussiness
{
    public class AdminRepository:IAdminRepository
    {
        
        private readonly IConnectionProvider _provider;

        public AdminRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        
        public async Task<IEnumerable<Admin>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = "select * from admin order by create_time ";
                    query += "limit @Skip , @Take ";

                return await connection.QueryAsync<Admin>(query, parameters);
            }
        }
        public void Add(Admin admin)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "INSERT INTO admin (account, password,create_time )"
                                + " VALUES(@account, @password, @create_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, admin);
            }
        }

        public IEnumerable<Admin> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Admin>("SELECT * FROM admin");
            }
        }
        public int GetAllCount()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.ExecuteScalar<int>("SELECT count(*) FROM admin");
            }
        }

        public Admin GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM admin"
                               + " WHERE id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Admin>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM admin"
                             + " WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Admin prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "UPDATE admin SET password = @password,"
                               + " update_time = @update_time, is_disable= @is_disable"
                               + " WHERE Id = @Id";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Admin invoice)
        {
            using (var connection =  _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE admin SET password=@password, update_time=@update_time ";
                    query += "WHERE Id=@Id";
                    await connection.ExecuteAsync(query, invoice, transaction);
                    //var lineIds = new List<int>();

                    //lineIds.AddRange(invoice.Where(l => l.Id != 0).Select(l => l.Id));

                    //foreach (var line in invoice.InvoiceLines)
                    //{
                    //    if (line.Id == 0)
                    //    {
                    //        query = "INSERT INTO InvoiceLines (LineItem, Amount, Unit, UnitPrice, VatPercent, InvoiceId)";
                    //        query += "VALUES (@LineItem, @Amount, @Unit, @UnitPrice, @VatPercent," + invoice.Id + "); ";
                    //        query += "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    //        var id = await connection.QueryFirstAsync<int>(query, line, transaction);
                    //        lineIds.Add(id);
                    //    }
                    //    else
                    //    {
                    //        query = "UPDATE InvoiceLines SET LineItem=@LineItem,Amount=@Amount,Unit=@Unit,";
                    //        query += "UnitPrice=@UnitPrice,VatPercent=@VatPercent ";
                    //        query += "WHERE Id=@Id";

                    //        await connection.ExecuteAsync(query, line, transaction);
                    //    }
                    //}

                    //if (lineIds.Count > 0)
                    //{
                    //    query = "DELETE FROM InvoiceLines WHERE InvoiceId=" + invoice.Id + " AND ";
                    //    query += "Id NOT IN(" + string.Join(',', lineIds) + ")";

                    //    await connection.ExecuteAsync(query, transaction: transaction);
                    //}

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }
    }
    public interface IAdminRepository 
    {
        public void Update(Admin prod);
        public IEnumerable<Admin> GetAll();
        public Admin GetByID(int id);
        public  Task<IEnumerable<Admin>> List(int page, int pageSize);
        public void Add(Admin admin);
        public void Delete(int id);
        public  Task UpdateAsync(Admin invoice);
        public int GetAllCount();
    }
}
