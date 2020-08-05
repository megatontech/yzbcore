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
    public class Equipment_status_logsRepository : IEquipment_status_logsRepository
    {
        private readonly IConnectionProvider _provider;

        public Equipment_status_logsRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task<IEnumerable<Equipment_status_logs>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = "select * from equipment_status_logs order by create_time ";
                query += "limit @Skip , @Take ";

                return await connection.QueryAsync<Equipment_status_logs>(query, parameters);
            }
        }
        public void Add(Equipment_status_logs admin)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "INSERT INTO equipment_status_logs (serial, temperature, humidity, power_status, create_time, update_time)"
                                + " VALUES(@serial, @temperature, @humidity, @power_status, @create_time, @update_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, admin);
            }
        }

        public IEnumerable<Equipment_status_logs> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Equipment_status_logs>("SELECT * FROM equipment_status_logs");
            }
        }

        public Equipment_status_logs GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM equipment_status_logs"
                               + " WHERE Id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Equipment_status_logs>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM equipment_status_logs"
                             + " WHERE ProductId = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Equipment_status_logs prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "UPDATE equipment_status_logs SET  (serial, temperature, humidity, power_status, create_time, update_time)"
                                + " VALUES(@serial, @temperature, @humidity, @power_status, @create_time, @update_time) WHERE Id = @Id";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Equipment_status_logs invoice)
        {
            using (var connection = _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE equipment_status_logs SET  (serial, temperature, humidity, power_status, create_time, update_time)"
                                + " VALUES(@serial, @temperature, @humidity, @power_status, @create_time, @update_time) WHERE Id=@Id";
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

        public Equipment_status_logs GetByserial(string seial)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = @"SELECT
                                        *
                                    FROM

                                        equipment_status_logs
                                    WHERE

                                        serial = @seial
                                    ORDER BY

                                        create_time DESC LIMIT 1 ";
                dbConnection.Open();
                return dbConnection.Query<Equipment_status_logs>(sQuery, new { seial = seial }).FirstOrDefault();
            }
        }
    }
    public interface IEquipment_status_logsRepository
    {
        public void Update(Equipment_status_logs prod);
        public IEnumerable<Equipment_status_logs> GetAll();
        public Equipment_status_logs GetByID(int id);
        public Task<IEnumerable<Equipment_status_logs>> List(int page, int pageSize);
        public void Add(Equipment_status_logs admin);
        public void Delete(int id);
        public Task UpdateAsync(Equipment_status_logs invoice);
        public Equipment_status_logs GetByserial(string seial);
    }
}
