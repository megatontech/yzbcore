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
    public class BirdhouseRepository: IBirdhouseRepository
    {
        private readonly IConnectionProvider _provider;

        public BirdhouseRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task<IEnumerable<Birdhouse>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = "select * from birdhouse order by create_time ";
                query += "limit @Skip , @Take ";

                return await connection.QueryAsync<Birdhouse>(query, parameters);
            }
        }
        public void Add(Birdhouse admin)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = @"INSERT INTO birdhouse (`member_id`, `name`, `type`, 
`equipment_id`, `min_temperature`, `max_temperature`, `min_humidity`, `max_humidity`,
 `warning_ammonia_concentration`, `warning_negative_pressure`, `create_time`, `update_time`, 
`delete_time`)"
                                + @" VALUES(@member_id, @name,@type, @equipment_id, @min_temperature,@max_temperature,@min_humidity,@max_humidity,
@warning_ammonia_concentration,@warning_negative_pressure,@create_time,@update_time,@delete_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, admin);
            }
        }

        public IEnumerable<Birdhouse> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Birdhouse>("SELECT * FROM birdhouse");
            }
        }
        public IEnumerable<Birdhouse> GetAllByMember(int member_id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM birdhouse"
                              + " WHERE member_id = @member_id";
                dbConnection.Open();
                return dbConnection.Query<Birdhouse>(sQuery, new { member_id = member_id });
            }
        }
        public Birdhouse GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM birdhouse"
                               + " WHERE id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Birdhouse>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM birdhouse"
                             + " WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Birdhouse prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "UPDATE birdhouse SET name = @name,"
                               + " type = @type, equipment_id= @equipment_id," +
                               " min_temperature= @min_temperature, max_temperature= @max_temperature, min_humidity= @min_humidity, max_humidity= @max_humidity,"+
                               " warning_ammonia_concentration= @warning_ammonia_concentration, warning_negative_pressure= @warning_negative_pressure, create_time= @create_time, update_time= @update_time"
                               + " WHERE id = @id";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Birdhouse invoice)
        {
            using (var connection = _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE birdhouse SET name = @name,"
                               + " type = @type, equipment_id= @equipment_id," +
                               " min_temperature= @min_temperature, max_temperature= @max_temperature, min_humidity= @min_humidity, max_humidity= @max_humidity," +
                               " warning_ammonia_concentration= @warning_ammonia_concentration, warning_negative_pressure= @warning_negative_pressure, create_time= @create_time, update_time= @update_time"
                               + " WHERE id = @id";
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
    public interface IBirdhouseRepository
    {
        public void Update(Birdhouse prod);
        public IEnumerable<Birdhouse> GetAll();
        public IEnumerable<Birdhouse> GetAllByMember(int member_id);
        public Birdhouse GetByID(int id);
        public Task<IEnumerable<Birdhouse>> List(int page, int pageSize);
        public void Add(Birdhouse admin);
        public void Delete(int id);
        public Task UpdateAsync(Birdhouse invoice);
    }
}
