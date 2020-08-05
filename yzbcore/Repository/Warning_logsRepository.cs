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
    public class Warning_logsRepository: IWarning_logsRepository
    {
        private readonly IConnectionProvider _provider;

        public Warning_logsRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task<IEnumerable<Warning_logs>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = "select * from warning_logs order by create_time ";
                query += "limit @Skip , @Take ";

                return await connection.QueryAsync<Warning_logs>(query, parameters);
            }
        }
        public void Add(Warning_logs admin)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "INSERT INTO warning_logs (member_id, birdhouse_id, cause, create_time)"
                                + " VALUES(@member_id, @birdhouse_id, @cause, @create_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, admin);
            }
        }

        public IEnumerable<Warning_logs> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Warning_logs>("SELECT * FROM warning_logs");
            }
        }

        public Warning_logs GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM warning_logs"
                               + " WHERE id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Warning_logs>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM warning_logs"
                             + " WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Warning_logs prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "UPDATE warning_logs SET (member_id, birdhouse_id, cause, create_time)"
                                + " VALUES(@member_id, @birdhouse_id, @cause, @create_time)";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Warning_logs invoice)
        {
            using (var connection = _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE warning_logs SET (member_id, birdhouse_id, cause, create_time)"
                                + " VALUES(@member_id, @birdhouse_id, @cause, @create_time)";
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
    public interface IWarning_logsRepository
    {
        public void Update(Warning_logs prod);
        public IEnumerable<Warning_logs> GetAll();
        public Warning_logs GetByID(int id);
        public Task<IEnumerable<Warning_logs>> List(int page, int pageSize);
        public void Add(Warning_logs admin);
        public void Delete(int id);
        public Task UpdateAsync(Warning_logs invoice);
    }
}
