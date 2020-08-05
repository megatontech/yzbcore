using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using yzbcore.Models;
using yzbcore.Repository;

namespace yzbcore.Bussiness
{
    public class Sms_logsRepository: ISms_logsRepository
    {
        private readonly IConnectionProvider _provider;

        public Sms_logsRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task<IEnumerable<Sms_logs>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = "select * from sms_logs order by create_time ";
                query += "limit @Skip , @Take ";

                return await connection.QueryAsync<Sms_logs>(query, parameters);
            }
        }
        public void Add(Sms_logs admin)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "INSERT INTO sms_logs (mobile, code, end_time,create_time,update_time)"
                                + " VALUES(@mobile, @code, @end_time, @create_time,@update_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, admin);
            }
        }

        public IEnumerable<Sms_logs> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Sms_logs>("SELECT * FROM sms_logs");
            }
        }

        public Sms_logs GetBymobile(string mobile) 
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = @"SELECT * FROM sms_logs"
                               + @" WHERE mobile = @mobile  ORDER BY
                                    create_time DESC LIMIT 1 ";
                dbConnection.Open();
                return dbConnection.Query<Sms_logs>(sQuery, new { mobile = mobile }).FirstOrDefault();
            }
        }
        public Sms_logs GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM sms_logs"
                               + " WHERE id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Sms_logs>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM sms_logs"
                             + " WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Sms_logs prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "UPDATE sms_logs SET  (mobile, code, end_time,create_time)"
                                + " VALUES(@mobile, @code, @end_time, @create_time) WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Sms_logs invoice)
        {
            using (var connection = _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE sms_logs SET  (mobile, code, end_time,create_time)"
                                + " VALUES(@mobile, @code, @end_time, @create_time) WHERE id = @Id";
                    await connection.ExecuteAsync(query, invoice, transaction);
                    var lineIds = new List<int>();

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
    public interface ISms_logsRepository
    {
        public void Update(Sms_logs prod);
        public IEnumerable<Sms_logs> GetAll();
        public Sms_logs GetByID(int id);
        public Task<IEnumerable<Sms_logs>> List(int page, int pageSize);
        public void Add(Sms_logs admin);
        public void Delete(int id);
        public Sms_logs GetBymobile(string mobile);
        public Task UpdateAsync(Sms_logs invoice);
    }
}
