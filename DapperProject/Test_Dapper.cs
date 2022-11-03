using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DapperProject
{
    internal class Test_Dapper
    {

        public static void TestDapper()
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            IDbConnection db = new SqlConnection(configuration.GetSection("constr").Value);

            //Test_Dapper.SimpleInsert(db, "Hussein attia", 5456);
            //Test_Dapper.Insert_return_id(db, "mohamed hamed ", 15900);
            //Test_Dapper.update(db, 9, "Kerlous", 9900);
            //Test_Dapper.Delete(db, 13);
            //Test_Dapper.ExecuteMultipleQueries(db);
            //Test_Dapper.printAllWallets(db);

            Test_Dapper.DapperWithTransaction(db, 1000, 9, 2);
            Test_Dapper.printAllWallets(db);

            Console.ReadKey();
        }
        public static void DapperWithTransaction(IDbConnection db, decimal transferAmount, int fromId, int toId)
        {
            using (var transactionScope = new TransactionScope())
            {
                var walletFrom = db.QuerySingle<Wallet>("SELECT * FROM Wallets WHERE Id = @Id", new { Id = fromId });
                var walletTo = db.QuerySingle<Wallet>("SELECT * FROM Wallets WHERE Id = @Id", new { Id = toId });
                
                walletFrom.Balance -= transferAmount;
                walletTo.Balance += transferAmount;
                
                db.Execute("UPDATE Wallets SET Balance = @Balance WHERE Id = @Id", new {Balance = walletFrom.Balance, Id = walletFrom.Id});
                db.Execute("UPDATE Wallets SET Balance = @Balance WHERE Id = @Id", new { Balance = walletTo.Balance, Id = walletTo.Id });
                
                transactionScope.Complete();
            }

        }
        public static void ExecuteMultipleQueries(IDbConnection db) //a strong point in dapper
        {
            //execute multiple queries in a single batch
            var sql = "SELECT MIN(Balance) FROM Wallets;" +
                "SELECT MAX(Balance) FROM Wallets;";
            var muliti = db.QueryMultiple(sql);
            Console.WriteLine($"Min = {muliti.ReadSingle<decimal>()}" +
                $"\nMax = {muliti.ReadSingle<decimal>()}");

        }
        public static void Delete(IDbConnection db, int _id)
        {
            var sql = "DELETE FROM Wallets WHERE Id = @Id";
            var walletToDelete = new Wallet { Id = _id};
            var parameters = new { Id = walletToDelete.Id };
            db.Execute(sql, parameters);

        }
        public static void update(IDbConnection db,int _id, string _holder, decimal _balance)
        {
            var walletToUpdate = new Wallet { Id = _id, Holder = _holder, Balance = _balance };
            var sql = "UPDATE Wallets SET Holder = @Holder , Balance = @Balance " +
                "WHERE Id =@Id;";
            var parameters =
                new
                {
                    Id = walletToUpdate.Id,
                    Holder = walletToUpdate.Holder,
                    Balance = walletToUpdate.Balance
                };
            db.Execute(sql, parameters);
        }
        public static void Insert_return_id(IDbConnection db, string _holder, decimal _balance)
        {
            var walletToInsert = new Wallet() { Holder = _holder, Balance = _balance };
            var sql = "INSERT INTO Wallets (Holder, Balance)" +
                "VALUES (@Holder, @Balance)" +
                "SELECT CAST(SCOPE_IDENTITY() AS INT)";
            var parameters =
                new
                { //VALUES (@Holder, @Balance)
                    Holder = walletToInsert.Holder,
                    Balance = walletToInsert.Balance
                };
            walletToInsert.Id = db.Query<int>(sql, parameters).Single();
            Console.WriteLine(walletToInsert);

        }
        public static void SimpleInsert(IDbConnection db, string _holder, decimal _balance)
        {
            var walletToInsert = new Wallet() { Holder = _holder, Balance = _balance };
            var sql = "INSERT INTO Wallets (Holder, Balance) " +
                "VALUES (@Holder, @Balance)";
            db.Execute(sql,
                new
                { //VALUES (@Holder, @Balance)
                    Holder = walletToInsert.Holder,
                    Balance = walletToInsert.Balance
                });
        }
        public static void printAllWallets(IDbConnection db) //binding
        {
            Console.WriteLine("----------------------- using Dynamic Query -------------------");
            var sql = "SELECT * FROM WALLETS";
            var resultDynamic = db.Query(sql);
            foreach (var item in resultDynamic)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("----------------------- using typed Query -------------------");
            var wallets = db.Query<Wallet>(sql);
            foreach (var item in wallets)
            {
                Console.WriteLine(item);
            }
        }
    }
}
