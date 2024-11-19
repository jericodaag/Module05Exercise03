using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module07DataAccess.Model;
using MySql.Data.MySqlClient;

namespace Module07DataAccess.Services
{
    public class PersonalService
    {
        private readonly string _connectionString;

        public PersonalService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Personal>> GetAllPersonalAsync()
        {
            var personalService = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Retrieve data including new fields
                var cmd = new MySqlCommand("SELECT * FROM tblpersonal", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Gender = reader.GetString("Gender"),
                            ContactNo = reader.GetString("Contactno"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                                ? string.Empty
                                : reader.GetString("Address"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                ? string.Empty
                                : reader.GetString("Email")
                        });
                    }
                }
            }
            return personalService;
        }

        public async Task<bool> AddPersonalAsync(Personal newPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "INSERT INTO tblpersonal (Name, Gender, ContactNo, Address, Email) " +
                        "VALUES (@Name, @Gender, @ContactNo, @Address, @Email)", conn);

                    cmd.Parameters.AddWithValue("@Name", newPerson.Name);
                    cmd.Parameters.AddWithValue("@Gender", newPerson.Gender);
                    cmd.Parameters.AddWithValue("@ContactNo", newPerson.ContactNo);
                    cmd.Parameters.AddWithValue("@Address",
                        string.IsNullOrEmpty(newPerson.Address) ? DBNull.Value : newPerson.Address);
                    cmd.Parameters.AddWithValue("@Email",
                        string.IsNullOrEmpty(newPerson.Email) ? DBNull.Value : newPerson.Email);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePersonalAsync(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblPersonal WHERE ID = @ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdatePersonalAsync(Personal person)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "UPDATE tblpersonal " +
                        "SET Name = @Name, Gender = @Gender, ContactNo = @ContactNo, " +
                        "Address = @Address, Email = @Email " +
                        "WHERE ID = @ID", conn);

                    cmd.Parameters.AddWithValue("@ID", person.ID);
                    cmd.Parameters.AddWithValue("@Name", person.Name);
                    cmd.Parameters.AddWithValue("@Gender", person.Gender);
                    cmd.Parameters.AddWithValue("@ContactNo", person.ContactNo);
                    cmd.Parameters.AddWithValue("@Address",
                        string.IsNullOrEmpty(person.Address) ? DBNull.Value : person.Address);
                    cmd.Parameters.AddWithValue("@Email",
                        string.IsNullOrEmpty(person.Email) ? DBNull.Value : person.Email);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<Personal> GetPersonalByIdAsync(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM tblpersonal WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Personal
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Gender = reader.GetString("Gender"),
                            ContactNo = reader.GetString("Contactno"),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                                ? string.Empty
                                : reader.GetString("Address"),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                ? string.Empty
                                : reader.GetString("Email")
                        };
                    }
                }
            }
            return null;
        }
    }
}