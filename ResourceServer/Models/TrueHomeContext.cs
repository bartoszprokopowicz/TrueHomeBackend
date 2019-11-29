using Dapper;
using Npgsql;
using ResourceServer.JSONModels;
using ResourceServer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceServer.Models
{
    internal class TrueHomeContext
    {
        private static String query;

        //Get all rentings
        internal static IList<Renting> getAllRentings() {
            query = $"SELECT * FROM \"renting\"";

            IList<Renting> rentingList = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                rentingList = connection.Query<Renting>(query).ToList();
            }
            return rentingList;
        }
        
        internal static async Task<int> createRenting(Renting renting) {
            query = "INSERT INTO renting " +
                    "(IDAp, IDUser, date_from, date_to) " +
                    "VALUES " +
                    $"({renting.IDAp}, '{renting.IDUser}', '{renting.date_from}', '{renting.date_to}') " +
                    "RETURNING ID_Renting;";

            int id = -1;
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                id = await connection.ExecuteScalarAsync<int>(query, renting);
            }
            return id;
        }

        internal static Renting getRentingByUser(string id)
        {
            query = $"SELECT * FROM renting WHERE IDUser = '{id}';";
            Renting renting = null;
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                renting = connection.Query<Renting>(query).FirstOrDefault();
            }
            return renting;
        }

        internal static void deleteRenting(int id)
        {
            query = "DELETE FROM \"renting\" " +
                    $"WHERE ID_Renting = {id};";

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                connection.Execute(query);
            }
        }        
        internal static void updateRenting(Renting renting)
        {
            query = "UPDATE renting SET " +
                    $"date_to = '{renting.date_to}' " +
                    $"WHERE ID_Renting = '{renting.ID_Renting}';";

            var id = -1;
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                id = connection.ExecuteScalar<int>(query, renting);
            }
        }
        
        //Get User by Login
        internal static User getUserByLogin(string login)
        {
            query = $"SELECT * FROM \"user\" WHERE Login = {login};";

            User user = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                user = connection.Query<User>(query).FirstOrDefault();
            }
            return user;
        }
        internal static IList<Apartment> getUserApartmentList(string userID)
        {
            query = "SELECT * FROM Apartment " +
                    $"WHERE IDUser = '{userID}'";

            IList<Apartment> apartmentList = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                apartmentList = connection.Query<Apartment>(query).ToList();
            }

            return apartmentList;
        }
        //Get User by ID
        internal static User getUser(string userID)
        {
            query = $"SELECT * FROM \"user\" WHERE ID_User = '{userID}';";

            User user = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                user = connection.Query<User>(query).FirstOrDefault();
            }
            return user;
        }

        //Add new user
        internal static async Task addUser(User user)
        {
            user.isBlocked = false;

            query = "INSERT INTO \"user\" " +
                    "(ID_User, Login, Email, isBlocked, IDRole)" +
                    "VALUES " +
                    $"('{user.ID_User}','{user.Login}','{user.Email}',{user.isBlocked},{user.IDRole});";

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                await connection.ExecuteAsync(query);
            }
        }

        internal static string getPhoneNumber(string userID)
        {
            query = "SELECT PhoneNumber FROM \"personaldata\" " +
                    $"WHERE IDUser = '{userID}';";

            string phonenum = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                phonenum = connection.Query<string>(query).FirstOrDefault();
            }

            return phonenum;
        }

        internal static void setPhoneNumber(string phoneNum, string userID)
        {
            query = "UPDATE PersonalData SET " +
                    $"PhoneNumber = '{phoneNum}' " +
                    $"WHERE IDUser = '{userID}';";
            
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                connection.Execute(query);
            }
        }

        internal static PersonalData getPersonalDataByUserID(string userID)
        {
            query = "SELECT * FROM PersonalData AS PD " +
                    "LEFT JOIN \"user\" AS U " +
                    "ON PD.IDUser = U.ID_User " +
                    $"WHERE PD.IDUser = '{userID}';";

            PersonalData ps = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                ps = connection.Query<PersonalData>(query).FirstOrDefault();
            }
            return ps;
        }

        //Add new user
        internal static async Task addPersonalData(PersonalData personalData)
        {
            query = "INSERT INTO PersonalData " +
                    "(FirstName, LastName, BirthDate, IDUser)" +
                    "VALUES " +
                    $"('{personalData.FirstName}','{personalData.LastName}'," +
                    $"'{personalData.BirthDate}','{personalData.IDUser}');";

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                await connection.ExecuteAsync(query);
            }
        }

        //Get Apartment by id
        internal static Apartment getApartment(int id)
        {
            query = $"SELECT * FROM get_apartment({id});";

            Apartment apartment = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                apartment = connection.Query<Apartment>(query).FirstOrDefault();
            }

            return apartment;
        }
        
        //Get all Apartments
        internal static IList<Apartment> getAllApartments()
        {
            query = "SELECT * FROM get_all_apartments();";

            IList<Apartment> apartment = null;

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                apartment = connection.Query<Apartment>(query).ToList();
            }
            return apartment;
        }

        //Get with limit and offset Apartments
        internal static ApartmentListJSON getApartments(int limit, int offset)
        {
            query = $"SELECT * FROM get_all_apartments() ORDER BY ID_Ap ASC LIMIT {limit} OFFSET {offset};";

            IList<Apartment> apartments = null;
            ApartmentListJSON apJson = new ApartmentListJSON();

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                apartments = connection.Query<Apartment>(query).ToList();
            }

            if (apartments.Count <= limit)
            {
                apJson.hasMore = false;
                apJson.apartmentsList = apartments;
            } else
            {
                apartments.RemoveAt(limit);
                apJson.hasMore = true;
                apJson.apartmentsList = apartments;
            }

            return apJson;
        }

        //Update Apartment
        internal static void updateApartment(Apartment ap)
        {
            query = @"UPDATE Apartment SET " +
                    "Name = @Name," +
                    "ImgThumb = @ImgThumb," +
                    "ImgList = @ImgList," +
                    "Description = @Description " +
                    "WHERE ID_Ap = @ID_Ap;";


            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                connection.Execute(query, ap);
            }
        }

        //Create Apartment
        internal static async Task<int> createApartment(Apartment ap)
        {
            query = @"INSERT INTO Apartment " +
                    "(Name,City,Street,ApartmentNumber,ImgThumb,ImgList,Lat,Long,IDUser,Description)" +
                    " VALUES " +
                    "(@Name,@City,@Street,@ApartmentNumber,@ImgThumb,@ImgList,@Lat,@Long,@IDUser, @Description)" +
                    "RETURNING ID_Ap";

            int id;
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                id = await connection.ExecuteScalarAsync<int>(query, ap);
            }

            return id;
        }
        //Delete Apartment
        internal static bool deleteApartment(int? id)
        {
            bool isSuccess = false;
            query = "DELETE FROM Apartment" +
                    $" WHERE id_ap = {id};";

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                connection.Execute(query);
                isSuccess = true;
            }
            return isSuccess;
        }

        //Get with limit and offset Ratings
        internal static RatingJSON getRatings(int id, int limit, int offset)
        {
            query = $"SELECT * FROM rating WHERE IDAp = {id} ORDER BY ID_Rating ASC LIMIT {limit} OFFSET {offset};";

            IList<Rating> ratings = null;
            var ratJson = new RatingJSON();

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                ratings = connection.Query<Rating>(query).ToList();
            }

            if (ratings.Count <= limit)
            {
                ratJson.hasMore = false;
                ratJson.ratingsList = ratings;
            }
            else
            {
                ratings.RemoveAt(limit);
                ratJson.hasMore = true;
                ratJson.ratingsList = ratings;
            }

            return ratJson;
        }
        //Create Rating
        internal static async Task<int> createRating(Rating rat)
        {
            query = @"INSERT INTO rating " +
                    "(Owner,Location,Standard,Price,Description,IDUser,IDAp)" +
                    " VALUES " +
                    "(@Owner,@Location,@Standard,@Price,@Description,@IDUser,@IDAp)" +
                    "RETURNING ID_Rating";

            int id;
            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                id = await connection.ExecuteScalarAsync<int>(query, rat);
            }

            return id;
        }
        //Update Rating
        internal static void updateRating(Rating rat)
        {
            query = @"UPDATE Rating SET " +
                    "Owner = @Owner," +
                    "Location = @Location," +
                    "Standard = @Standard," +
                    "Price = @Price," +
                    "Description = @Description," +
                    "IDUser = @IDUser," +
                    "IDAp = @IDAp," +
                    "WHERE ID_Rating = @ID_Rating;";

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                connection.Execute(query, rat);
            }
        }

        //Delete Rating
        internal static void deleteRating(int id)
        {
            query = "DELETE FROM rating" +
                    $" WHERE id_Rating = {id};";

            using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            {
                connection.Open();
                connection.Execute(query);
            }
        }

        //Add picture reference
        internal static void AddPictureRefAsync(int id, string fileName)
        {
            var apartment = getApartment(id);

            if (apartment.ImgList == null)
            {
                apartment.ImgList = new[] {fileName};
                apartment.ImgThumb = fileName;
            }
            else
                apartment.ImgList = apartment.ImgList
                    ?.Concat(new[] {fileName}).ToArray();

            updateApartment(apartment);

            //TODO: make this work instead of loading whole apartment object
            //query = @"SELECT ImgList FROM Apartment WHERE id_ap = @id;";
            //var updQuery = @"UPDATE Apartment SET ImgList = @imgList WHERE id_ap = @id;";

            //using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            //{
            //    connection.Open();
            //    var imgList = connection.Query<string[]>(query, new {id}).FirstOrDefault();
            //    imgList.Append(fileName);
            //    connection.Execute(updQuery, new {imgList, id});
            //}
        }
        //Delete picture reference
        internal static void DeletePictureRefAsync(int id, string fileName)
        {
            var apartment = getApartment(id);
            apartment.ImgList = apartment.ImgList.Where(file => file != fileName).ToArray();
            updateApartment(apartment);

            //TODO: make this work instead of loading whole apartment object
            //query = @"SELECT ImgList FROM Apartment WHERE id_ap = @id;";
            //var updQuery = @"UPDATE Apartment SET ImgList = @imgList WHERE id_ap = @id;";

            //using (var connection = new NpgsqlConnection(AppSettingProvider.connString))
            //{
            //    connection.Open();
            //    var imgList = connection.Query<string[]>(query, new {id}).FirstOrDefault();
            //    imgList.Append(fileName);
            //    connection.Execute(updQuery, new {imgList, id});
            //}
        }
    }
}
