using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ResourceServer.Resources;
using Npgsql;

namespace ResourceServer.Migrations
{
    public class Migration
    {
        private static String query;
        public static void createDB()
        {
            using (var connection = new NpgsqlConnection(AppSettingProvider.migString))
            {
                connection.Open();
                query = "CREATE DATABASE TrueHomeDB";
                connection.Query(query);
                Console.WriteLine("Created new database");
                createTables();
            }
        }

        private static void createTables()
        {
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                createRoleTable(connection);
                createUserTable(connection);
                createApartmentTable(connection);   
            }    
        }

        private static void createUserTable(NpgsqlConnection connection)
        {
            query = "CREATE TABLE public.User(" +
                    "ID_User       SERIAL," +
                    "Login         varchar (100) NOT NULL," +
                    "Email         varchar (100) NOT NULL," +
                    "IDRole        INTEGER," +
                    "CONSTRAINT pk_user PRIMARY KEY(ID_User)," +
                    "CONSTRAINT fk_role FOREIGN KEY(IDRole) REFERENCES Role(ID_Role) ON DELETE RESTRICT" +
                    ");";

            connection.Query(query);
        }
        private static void createApartmentTable(NpgsqlConnection connection)
        {
            query = "CREATE TABLE public.Apartment (" +
                    "ID_Ap         SERIAL," +
                    "Name            varchar (100) NOT NULL," +
                    "City            varchar (100) NOT NULL," +
                    "Street          varchar (100) NOT NULL," +
                    "ApartmentNumber varchar (20)  NOT NULL," +
                    "ImgThumb        varchar (200),"          +
                    "ImgList         character varying(200)[]," +
                    "Rate            numeric (2,1),"          +
                    "Lat             numeric (9,7) NOT NULL," +
                    "Long            numeric (10,7)NOT NULL," +
                    "IDUser          INTEGER,"                +       
                    "CONSTRAINT pk_apartment PRIMARY KEY(ID_Ap)," +
                    "CONSTRAINT fk_user FOREIGN KEY(IDUser) REFERENCES public.User(ID_User) ON DELETE RESTRICT" +
                    ");";

            connection.Query(query);
        }
        private static void createRoleTable(NpgsqlConnection connection)
        {
            query = "CREATE TABLE public.Role (" +
                    "ID_Role       SERIAL," +
                    "RoleName      varchar (30) NOT NULL," +
                    "CONSTRAINT pk_role PRIMARY KEY (ID_Role)" +
                    ");";

            connection.Query(query);
        }
    }
}
