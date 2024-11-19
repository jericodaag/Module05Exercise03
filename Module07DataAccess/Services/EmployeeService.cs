using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Module07DataAccess.Model;
using MySql.Data.MySqlClient;

namespace Module07DataAccess.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var employees = new List<Employee>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM tblEmployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["Name"]),
                            Department = Convert.ToString(reader["Department"]),
                            Position = Convert.ToString(reader["Position"]),
                            ContactNo = Convert.ToString(reader["ContactNo"]),
                            Email = Convert.ToString(reader["Email"]),
                            Address = Convert.ToString(reader["Address"])
                        });
                    }
                }
            }
            return employees;
        }

        public async Task<List<Employee>> SearchEmployeesAsync(string searchQuery)
        {
            var employees = new List<Employee>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(
                    "SELECT * FROM tblEmployee WHERE Name LIKE @Query " +
                    "OR Department LIKE @Query OR Position LIKE @Query " +
                    "OR ContactNo LIKE @Query OR Email LIKE @Query " +
                    "OR Address LIKE @Query", conn);

                cmd.Parameters.AddWithValue("@Query", $"%{searchQuery}%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["Name"]),
                            Department = Convert.ToString(reader["Department"]),
                            Position = Convert.ToString(reader["Position"]),
                            ContactNo = Convert.ToString(reader["ContactNo"]),
                            Email = Convert.ToString(reader["Email"]),
                            Address = Convert.ToString(reader["Address"])
                        });
                    }
                }
            }
            return employees;
        }

        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "INSERT INTO tblEmployee (Name, Department, Position, ContactNo, Email, Address) " +
                        "VALUES (@Name, @Department, @Position, @ContactNo, @Email, @Address)", conn);

                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Department", employee.Department);
                    cmd.Parameters.AddWithValue("@Position", employee.Position);
                    cmd.Parameters.AddWithValue("@ContactNo", employee.ContactNo);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Address", employee.Address ?? string.Empty);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "UPDATE tblEmployee SET Name = @Name, Department = @Department, " +
                        "Position = @Position, ContactNo = @ContactNo, Email = @Email, " +
                        "Address = @Address WHERE ID = @ID", conn);

                    cmd.Parameters.AddWithValue("@ID", employee.ID);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Department", employee.Department);
                    cmd.Parameters.AddWithValue("@Position", employee.Position);
                    cmd.Parameters.AddWithValue("@ContactNo", employee.ContactNo);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Address", employee.Address ?? string.Empty);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblEmployee WHERE ID = @ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee: {ex.Message}");
                return false;
            }
        }
    }
}