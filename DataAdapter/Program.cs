using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace DataAdapter
{
    //open the connection with the database as late as possible, and close the connection as fast as possible
    internal class Program
    {
        static void Main(string[] args)
        {

            TestRead_Insert();

            Console.ReadKey();
        }

        public static void TestRead_Insert()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build();
            var connection = new SqlConnection(configuration.GetSection("constr").Value);

            //select all
            var sql_select = "SELECT * FROM WALLETS";
            SqlCommand command = new SqlCommand(sql_select, connection);
            PrintAllWallets(command, connection);



            //insert new wallet 
            var sql_insert = "INSERT INTO WALLETS (Holder, Balance) VALUES" +
                $"(@Holder, @Balance);" +
                $"SELECT CAST(Scope_identity() AS int)";
            SqlCommand insertCommand = new SqlCommand(sql_insert, connection);
            //Program.insertNewWallet(insertCommand, connection);

            //update existing wallet
            //var sql_update = $"UPDATE Wallets SET Holder = @Holder, balance = @Balance " +
            //    $"WHERE id = @Id";
            //SqlCommand updateCommand = new SqlCommand(sql_update, connection);
            //Program.updatExistingWallet(updateCommand, connection);



            var sql_delete = $"DELETE FROM Wallets WHERE Id = 1";
            SqlCommand deleteCommnd = new SqlCommand(sql_delete, connection);
            Program.deleteWallet(deleteCommnd, connection);
            PrintAllWallets(command, connection);

        }
        public static void PrintAllWallets(SqlCommand command, SqlConnection connection)
        {
            command.CommandType = CommandType.Text;
            connection.Open(); //==================================open connection==============
            SqlDataReader reader = command.ExecuteReader();

            Wallet wallet;
            while (reader.Read())
            {
                wallet = new Wallet
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Holder"),
                    Balance = reader.GetDecimal("Balance")
                };
                Console.WriteLine(wallet);
            }
            connection.Close();//==================================close connection=============
        }
        public static void insertNewWallet(SqlCommand command, SqlConnection connection)
        {
            Console.WriteLine("please enter name and balance:");
            var walletToInsert = new Wallet
            {
                Name = Console.ReadLine(),
                Balance = Convert.ToDecimal(Console.ReadLine()),
            };

            SqlParameter holderParameter = new SqlParameter
            {
                ParameterName = "@Holder",
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input,
                Value = walletToInsert.Name
            };

            SqlParameter balanceParameter = new SqlParameter
            {
                ParameterName = "@Balance",
                SqlDbType = SqlDbType.Decimal,
                Direction = ParameterDirection.Input,
                Value = walletToInsert.Balance
            };
            command.Parameters.Add(holderParameter);
            command.Parameters.Add(balanceParameter);

            command.CommandType = CommandType.Text;

            connection.Open();
            walletToInsert.Id = (int)command.ExecuteScalar();

            Console.WriteLine($"wallet for {walletToInsert.Name} added successfully");

            connection.Close();

        }
        public static void updatExistingWallet(SqlCommand command, SqlConnection connection)
        {

            SqlParameter holderParameter = new SqlParameter
            {
                ParameterName = "@Holder",
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input,
                Value = "mahmoud"
            };

            SqlParameter balanceParameter = new SqlParameter
            {
                ParameterName = "@Balance",
                SqlDbType = SqlDbType.Decimal,
                Direction = ParameterDirection.Input,
                Value = 0
            };
            SqlParameter IdParameter = new SqlParameter
            {
                ParameterName = "@Id",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = 1
            };
            command.Parameters.Add(IdParameter);
            command.Parameters.Add(holderParameter);
            command.Parameters.Add(balanceParameter);

            command.CommandType = CommandType.Text;

            connection.Open();
            if (command.ExecuteNonQuery() > 0)
                Console.WriteLine("Wallet was updated successfully");


            connection.Close();

        }
        public static void deleteWallet(SqlCommand command, SqlConnection connection)
        {
            SqlParameter idParameter = new SqlParameter
            {
                ParameterName = "@Id",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = 1
            };
            command.Parameters.Add(idParameter);
            connection.Open();
            if (command.ExecuteNonQuery() > 0)
                Console.WriteLine($"Wallet with id {idParameter.Value} deleted successfully");
            connection.Close();
        }
    }
}
