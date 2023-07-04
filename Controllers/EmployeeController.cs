using lastonegooo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace lastonegooo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            string query = @"
                INSERT INTO [dbo].[Employee] (EmpName, EmpContact, EmpEmail, EmpAddress)
                VALUES (@EmpName, @EmpContact, @EmpEmail, @EmpAddress);
                SELECT SCOPE_IDENTITY();
            ";

            int newEmployeeId;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("EmployeeDbConnection")))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmpName", employee.EmpName);
                    command.Parameters.AddWithValue("@EmpContact", employee.EmpContact);
                    command.Parameters.AddWithValue("@EmpEmail", employee.EmpEmail);
                    command.Parameters.AddWithValue("@EmpAddress", employee.EmpAddress);
                    object result = command.ExecuteScalar();
                    newEmployeeId = Convert.ToInt32(result);
                }
                connection.Close();
            }

            return Ok(new { EmployeeId = newEmployeeId, Message = "Employee added successfully" });
        }

        [HttpGet("{employeeId}")]
        public IActionResult GetEmployee(int employeeId)
        {
            string query = @"
                SELECT * FROM [dbo].[Employee]
                WHERE EmpId = @EmployeeId
            ";

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("EmployeeDbConnection")))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
                connection.Close();
            }

            if (table.Rows.Count == 0)
            {
                return NotFound("Employee not found");
            }

            DataRow row = table.Rows[0];
            Employee employee = new Employee
            {
                EmpId = (int)row["EmpId"],
                EmpName = row["EmpName"].ToString(),
                EmpContact = row["EmpContact"].ToString(),
                EmpEmail = row["EmpEmail"].ToString(),
                EmpAddress = row["EmpAddress"].ToString()
            };

            return Ok(employee);
        }

        [HttpPut("{employeeId}")]
        public IActionResult EditEmployee(int employeeId, Employee employee)
        {
            string query = @"
                UPDATE [dbo].[Employee]
                SET EmpName = @EmpName,
                    EmpContact = @EmpContact,
                    EmpEmail = @EmpEmail,
                    EmpAddress = @EmpAddress
                WHERE EmpId = @EmployeeId
            ";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("EmployeeDbConnection")))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmpName", employee.EmpName);
                    command.Parameters.AddWithValue("@EmpContact", employee.EmpContact);
                    command.Parameters.AddWithValue("@EmpEmail", employee.EmpEmail);
                    command.Parameters.AddWithValue("@EmpAddress", employee.EmpAddress);
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound("Employee not found");
                    }
                }
                connection.Close();
            }

            return Ok("Employee updated successfully");
        }

        [HttpDelete("{employeeId}")]
        public IActionResult DeleteEmployee(int employeeId)
        {
            string query = @"
                DELETE FROM [dbo].[Employee]
                WHERE EmpId = @EmployeeId
            ";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("EmployeeDbConnection")))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound("Employee not found");
                    }
                }
                connection.Close();
            }

            return Ok("Employee deleted successfully");
        }
    }
}
