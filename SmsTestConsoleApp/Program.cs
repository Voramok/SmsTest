using System;
using System.IO;
using NLog;
using SmsTestConsoleApp.Interfaces;
using SmsTestConsoleApp.Services;
using Npgsql;
using Microsoft.Extensions.Configuration;
using SmsTestConsoleApp.Models;

namespace SmsTestConsoleApp
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static string baseAddress = "http://localhost";
        private static string userName = "YourUsername";
        private static string password = "YourPassword";
        private static GetMenuResponse getMenuResponse;
        private static SendOrderResponse sendOrderResponse;

        internal static void Main(string[] args)
        {
            string logDirectory = "Logs";

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                logger.Info($"Директория '{logDirectory}' была создана.");
            }
            else
            {
                logger.Info($"Директория '{logDirectory}' уже существует.");
            }

            Console.WriteLine("Введите текст (нажмите Ctrl+C для выхода):");

            while (true)
            {
                try
                {
                    string choice = string.Empty;
                    while (choice != "1" && choice != "2")
                    {
                        Console.WriteLine("Выберите библиотеку:");
                        Console.WriteLine("1. SmsTestHttpLib 1");
                        Console.WriteLine("2. SmsTestGrpcLib 2");
                        choice = Console.ReadLine();
                    }

                    ISmsTestServiceClient smsTestServiceClient = null;
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Выбрана SmsTestHttpLib");
                            smsTestServiceClient = new HttpSmsTestServiceClient(baseAddress, userName, password);
                            break;
                        case "2":
                            Console.WriteLine("Выбрана SmsTestGrpcLib");
                            smsTestServiceClient = new GrpcSmsTestServiceClient(baseAddress);
                            break;
                    }

                    string choice2 = string.Empty;
                    while (choice2 != "1" && choice2 != "2")
                    {
                        Console.WriteLine("Выберите метод:");
                        Console.WriteLine("1. GetMenu 1");
                        Console.WriteLine("2. SendOrder 2");
                        choice2 = Console.ReadLine();
                    }


                    switch (choice2)
                    {
                        case "1":
                            Console.WriteLine("Выбран GetMenu");
                            //Funcs asynk if they need later
                            getMenuResponse = smsTestServiceClient.GetMenuAsync(true).Result;

                            Console.WriteLine("GetMenu результат:");

                            if (string.IsNullOrEmpty(getMenuResponse.ErrorMessage))
                            {
                                foreach (var menuItem in getMenuResponse.MenuItems)
                                {
                                    Console.WriteLine($"{menuItem.Name}-{menuItem.Article}-{menuItem.Price}");
                                }
                            } else {
                                logger.Fatal(getMenuResponse.ErrorMessage);
                                Environment.Exit(0);
                            }
                            break;
                        case "2":
                            Console.WriteLine("Выбран SendOrder");

                            Order order = null;

                            order = new Order();
                            Console.WriteLine("Введите заказ по примеру: \"Код1: Количество1; Код2: Количество2; Код3: Количество3;…\"");
                            var orderStr = Console.ReadLine();

                            if (string.IsNullOrEmpty(orderStr))
                            {
                                Console.WriteLine("Строка заказа не может быть пустой");
                            }

                            var menuItems = orderStr.Split(";");
                            foreach (var menuItem in menuItems)
                            {
                                if (string.IsNullOrEmpty(menuItem))
                                {
                                    break;
                                }

                                var menuItemArr = menuItem.Split(":");
                                if (menuItemArr.Length != 2)
                                {
                                    Console.WriteLine($"Неверно заполнена позиция {menuItem}");
                                    break;
                                }

                                double count;
                                double.TryParse(menuItemArr[1], out count);
                                if (count <= 0.0)
                                {
                                    Console.WriteLine($"Количество должно быть > 0 {menuItem}");
                                    break;
                                }

                                var tempMenuItem = new OrderItem()
                                {
                                    Id = menuItemArr[0],
                                    Quantity = count
                                };

                                order.MenuItems.Add(tempMenuItem);
                            }

                            //Funcs asynk if they need later
                            sendOrderResponse = smsTestServiceClient.SendOrderAsync(order).Result;

                            if (string.IsNullOrEmpty(sendOrderResponse.ErrorMessage))
                            {
                                Console.WriteLine("УСПЕХ");
                            }
                            else
                            {
                                logger.Fatal(sendOrderResponse.ErrorMessage);
                            }
                            break;
                    }
                   

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    string connectionString = configuration.GetConnectionString("DefaultConnection");

                    CreateDatabase(connectionString);
                    CreateTable(connectionString);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Exiting...");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static void CreateDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            var databaseName = builder.Database;

            builder.Database = "postgres";

            using (var conn = new NpgsqlConnection(builder.ConnectionString))
            {
                conn.Open();

                // Формируем SQL-запрос для создания базы данных
                var createDatabaseQuery = $"CREATE DATABASE \"{ databaseName}\"";

                using (var command = new NpgsqlCommand(createDatabaseQuery, conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        logger.Info($"DB '{databaseName}' created.");
                    }
                    catch (NpgsqlException ex) when (ex.SqlState == "42P04")
                    {
                        logger.Error($"Error: DB '{databaseName}' is exist.");
                    }
                    catch (NpgsqlException ex)
                    {
                        logger.Error($"DB creation error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error: {ex.Message}");
                    }
                }
            }
        }

        private static void CreateTable(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            using (var conn = new NpgsqlConnection(builder.ConnectionString))
            {
                conn.Open();

                using (var command = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS menu_table (
                id SERIAL PRIMARY KEY,
                name VARCHAR(255) NOT NULL
            )", conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        logger.Info("Table 'menu_table' created.");
                    }
                    catch (NpgsqlException ex)
                    {
                        logger.Error($"Table creation error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}
