using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using yzbcore.Models;
using yzbcore.Repository;

namespace yzbcore.Bussiness
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly IConnectionProvider _provider;

        public EquipmentRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task<IEnumerable<EquipmentDisp>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = @"SELECT e.id,e.serial,e.`name`,b.`name` bname,m.username,e.bind_status,e.create_time FROM `equipment` e
                                LEFT JOIN birdhouse b on e.birdhouse_id = b.id
                                LEFT JOIN member m on e.member_id = m.id
                                LIMIT @Skip , @Take";

                return await connection.QueryAsync<EquipmentDisp>(query, parameters);
            }
        }

        public async Task<IEnumerable<CacheModel>> CacheList()
        {
            using (var connection = _provider.GetDbConnection())
            {
                var query = @"SELECT
	                            e.serial,
	                            e.`name`,
	                            e.`status`,
	                            e.bind_status,
                                b.`name` birdhouse_name,
	                            b.min_temperature,
	                            b.max_temperature,
	                            b.min_humidity,
	                            b.max_humidity,
	                            b.warning_ammonia_concentration,
	                            b.warning_negative_pressure,
	                            m.warning_mobile,
	                            m.username
                            FROM
	                            equipment e
                            LEFT JOIN birdhouse b ON e.birdhouse_id = b.id
                            LEFT JOIN member m ON e.member_id = m.id;";
                return await connection.QueryAsync<CacheModel>(query);
            }
        }


        public void Add(Equipment equipment)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "INSERT INTO equipment (serial, name, birdhouse_id, member_id, bind_status, status, create_time)"
                                + " VALUES(@serial, @name, @birdhouse_id, @member_id, @bind_status, @status, @create_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, equipment);
            }
        }

        public IEnumerable<Equipment> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Equipment>("SELECT * FROM equipment");
            }
        }
        public int GetAllCount()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.ExecuteScalar<int>("SELECT count(*) FROM equipment");
            }
        }
        public Equipment GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM equipment"
                               + " WHERE id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Equipment>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM equipment"
                             + " WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Equipment prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = @"UPDATE equipment SET `serial` = @serial,
                                                     `name` = @name,
                                                     `birdhouse_id` = @birdhouse_id,
                                                     `member_id` =@member_id,
                                                     `bind_status` =@bind_status,
                                                     `status` = @status,
                                                     `create_time` = @create_time,
                                                     `update_time` = @update_time,
                                                     `delete_time` = @delete_time "
                                + " WHERE id = @id";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Equipment invoice)
        {
            using (var connection = _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE equipment SET  (serial, name, birdhouse_id, member_id, bind_status, status, create_time)"
                                + " VALUES(@serial, @name, @birdhouse_id, @member_id, @bind_status, @status, @create_time) WHERE id=@Id";
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

        public CacheModel GetCacheModelByserial(string serial)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                var query = @"SELECT
	                            e.serial,
	                            e.`name`,
	                            e.`status`,
	                            e.bind_status,
                                b.`name` birdhouse_name,
	                            b.min_temperature,
	                            b.max_temperature,
	                            b.min_humidity,
	                            b.max_humidity,
	                            b.warning_ammonia_concentration,
	                            b.warning_negative_pressure,
	                            m.warning_mobile,
	                            m.username
                            FROM
	                            equipment e
                            LEFT JOIN birdhouse b ON e.birdhouse_id = b.id
                            LEFT JOIN member m ON e.member_id = m.id"
            + " WHERE e.serial = @serial";
                dbConnection.Open();
                return dbConnection.Query<CacheModel>(query, new { serial = serial }).FirstOrDefault();
            }
        }

        public Equipment GetByserial(string serial)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                var query = @"SELECT
	                            e.serial,
	                            e.`name`,
	                            e.`status`,
	                            e.bind_status,
                                b.`name` birdhouse_name,
	                            b.min_temperature,
	                            b.max_temperature,
	                            b.min_humidity,
	                            b.max_humidity,
	                            b.warning_ammonia_concentration,
	                            b.warning_negative_pressure,
	                            m.warning_mobile,
	                            m.username
                            FROM
	                            equipment e
                            LEFT JOIN birdhouse b ON e.birdhouse_id = b.id
                            LEFT JOIN member m ON e.member_id = m.id "
            + " WHERE e.serial = @serial";
                dbConnection.Open();
                return dbConnection.Query<Equipment>(query, new { serial = serial }).FirstOrDefault();
            }
        }

        public Equipment GetBybirdhouse_id(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                var query = @"SELECT
	                            e.serial,
	                            e.`name`,
	                            e.`status`,
	                            e.bind_status,
                                b.`name` birdhouse_name,
	                            b.min_temperature,
	                            b.max_temperature,
	                            b.min_humidity,
	                            b.max_humidity,
	                            b.warning_ammonia_concentration,
	                            b.warning_negative_pressure,
	                            m.warning_mobile,
	                            m.username
                            FROM
	                            equipment e
                            LEFT JOIN birdhouse b ON e.birdhouse_id = b.id
                            LEFT JOIN member m ON e.member_id = m.id "
            + " WHERE e.birdhouse_id  = @birdhouse_id";
                dbConnection.Open();
                return dbConnection.Query<Equipment>(query, new { birdhouse_id = id }).FirstOrDefault();
            }
        }

        public List<float> GethumidityData(string id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                var dte = Util.ToUnixStamp(DateTime.Now.Date.AddDays(1));
                var dts = Util.ToUnixStamp(DateTime.Now.Date);
                var query = @"select humidity as val,hours as hour from(SELECT
	                            HOUR (from_unixtime(create_time)) AS hours,
	                            avg(temperature) AS temperature,
	                            avg(humidity) AS humidity
                            FROM
	                            equipment_status_logs
                            WHERE
	                            serial = @id
                            AND (create_time) <= @dte
                            AND (create_time) >= @dts
                            GROUP BY
	                            hours
                            ORDER BY
	                            hours) temp;";
                dbConnection.Open();
                var cur= (List<curveModel>)dbConnection.Query<curveModel>(query, new { id = id, dte = dte, dts = dts });
                var result = new List<float>();
                for (int i = 0; i < 9; i++)
                {
                    var temp = 0f;
                    if (cur.Any(x => x.hour < ((i + 1) * 3) && x.hour >= ((i) * 3))) { temp = cur.Where(x => x.hour < ((i + 1) * 3) && x.hour >= ((i) * 3)).Average(x => x.val); }
                    result.Add(temp);
                }
                return result;
            }
        }

        public List<float> GettemperatureData(string id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                var dte = Util.ToUnixStamp(DateTime.Now.Date.AddDays(1));
                var dts = Util.ToUnixStamp(DateTime.Now.Date);
                var query = @"select temperature as val,hours as hour  from(SELECT
	                            HOUR (from_unixtime(create_time)) AS hours,
	                            avg(temperature) AS temperature,
	                            avg(humidity) AS humidity
                            FROM
	                            equipment_status_logs
                            WHERE
	                            serial = @id
                            AND (create_time) <= @dte
                            AND (create_time) >= @dts
                            GROUP BY
	                            hours
                            ORDER BY
	                            hours) temp;";
                dbConnection.Open();
                var cur = (List<curveModel>)dbConnection.Query<curveModel>(query, new { id = id, dte = dte, dts = dts });
                var result = new List<float>();
                for (int i = 0; i < 9; i++)
                {
                    var temp = 0f;
                    if (cur.Any(x => x.hour < ((i + 1) * 3) && x.hour >= ((i) * 3))) { temp = cur.Where(x => x.hour < ((i + 1) * 3) && x.hour >= ((i) * 3)).Average(x => x.val); }
                    result.Add(temp);
                }
                return result;
            }
        }
    }
    public interface IEquipmentRepository
    {
        public void Update(Equipment prod);
        public IEnumerable<Equipment> GetAll();
        public Equipment GetByID(int id);
        public Task<IEnumerable<EquipmentDisp>> List(int page, int pageSize);
        public void Add(Equipment equipment);
        public void Delete(int id);
        public int GetAllCount();
        public Task<IEnumerable<CacheModel>> CacheList();
        public Task UpdateAsync(Equipment invoice);
        public Equipment GetByserial(string serial);
        public Equipment GetBybirdhouse_id(int id);

        public CacheModel GetCacheModelByserial(string serial);
        List<float> GethumidityData(string id);
        List<float> GettemperatureData(string id);
    }
    public class curveModel
    {
        public float val{ get; set; }
        public int hour{ get; set; }
    } 
}
