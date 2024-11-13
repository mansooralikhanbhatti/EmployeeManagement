using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDBContext _context;

        public EmployeesController(EmployeeDBContext context)
        {
            _context = context;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetEmployees(string name = null, DateTime? dateOfJoining = null, string email = null)
        {
            var employees = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                employees = employees.Where(e => e.Name.Contains(name));

            if (dateOfJoining.HasValue)
                employees = employees.Where(e => e.DateOfJoining.Date == dateOfJoining.Value.Date);

            if (!string.IsNullOrEmpty(email))
                employees = employees.Where(e => e.Email.Contains(email));

            var result = await employees.Where(e => !e.IsDeleted).ToListAsync();
            return Ok(result);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEmployees), new { id = employee.EmpId }, employee);
            }
            return BadRequest(ModelState);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmpId)
                return BadRequest();

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null || existingEmployee.IsDeleted)
                return NotFound();

            existingEmployee.Name = employee.Name;
            existingEmployee.Age = employee.Age;
            existingEmployee.Email = employee.Email;
            existingEmployee.UpdatedAt = System.DateTime.UtcNow;

            _context.Entry(existingEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || employee.IsDeleted)
                return NotFound();

            employee.IsDeleted = true;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}