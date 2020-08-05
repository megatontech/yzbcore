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
    public class MemberRepository: IMemberRepository
    {
        private readonly IConnectionProvider _provider;

        public MemberRepository(IConnectionProvider provider)
        {
            _provider = provider;
        }
        public async Task<IEnumerable<Member>> List(int page, int pageSize)
        {
            using (var connection = _provider.GetDbConnection())
            {
                var parameters = new { Skip = (page - 1) * pageSize, Take = pageSize };

                var query = "select * from member order by create_time ";
                query += "limit @Skip , @Take ";

                return await connection.QueryAsync<Member>(query, parameters);
            }
        }
        public void Add(Member member)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "INSERT INTO member (username, nick_name, avatar, mobile, warning_mobile, create_time,update_time)"
                                + " VALUES(@username, @nick_name, @avatar, @mobile, @warning_mobile, @create_time,@update_time)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, member);
            }
        }
        public int GetAllCount()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.ExecuteScalar<int>("SELECT count(*) FROM member");
            }
        }
        public IEnumerable<Member> GetAll()
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                dbConnection.Open();
                return dbConnection.Query<Member>("SELECT * FROM member");
            }
        }
        public Member GetBymobile(string mobile) 
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM member"
                               + " WHERE mobile = @mobile";
                dbConnection.Open();
                return dbConnection.Query<Member>(sQuery, new { mobile = mobile }).FirstOrDefault();
            }
        }
        public Member GetByID(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "SELECT * FROM member"
                               + " WHERE id = @Id";
                dbConnection.Open();
                return dbConnection.Query<Member>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = "DELETE FROM member"
                             + " WHERE id = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Member prod)
        {
            using (IDbConnection dbConnection = _provider.GetDbConnection())
            {
                string sQuery = @"UPDATE `member` SET   `username` = @username,
                                                         `nick_name` = @nick_name,
                                                         `avatar` = @avatar,
                                                         `mobile` = @mobile,
                                                         `warning_mobile` = @warning_mobile,
                                                         `create_time` = @create_time,
                                                         `update_time` = @update_time"
                                + "  WHERE `id` = @id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, prod);
            }
        }

        public async Task UpdateAsync(Member invoice)
        {
            using (var connection = _provider.GetDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "UPDATE Invoices SET  (`username`, `nick_name`, `avatar`, `mobile`, `warning_mobile`, `create_time`,`update_time`)"
                                + " VALUES(@username, @nick_name, @avatar, @mobile, @warning_mobile, @create_time,@update_time) WHERE id = @id";
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
    public interface IMemberRepository
    {
        public void Update(Member prod);
        public int GetAllCount();
        public IEnumerable<Member> GetAll();
        public Member GetByID(int id);
        public Member GetBymobile(string mobile);
        
        public Task<IEnumerable<Member>> List(int page, int pageSize);
        public void Add(Member member);
        public void Delete(int id);
        public Task UpdateAsync(Member invoice);
    }
}
