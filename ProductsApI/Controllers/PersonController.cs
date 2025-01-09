using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApI.Data;
using ProductsApI.Models;

namespace ProductsApI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ProductContext _context;

        public PersonController(ProductContext context)
        {
            _context = context;
        }

        // Scenario 1: Create a Person without a User
        [HttpPost("create-person")]
        public async Task<ActionResult<Person>> CreatePerson(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, person);
        }

        // Scenario 2: Create a Person and link to an existing User
        [HttpPost("create-person-with-user")]
        public async Task<ActionResult<Person>> CreatePersonWithUser(Person person)
        {
            var existingUser = await _context.Users.FindAsync(person.UserId);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            person.User = existingUser;
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, person);
        }

        // Scenario 3: Create a Person and register as a new User
        [HttpPost("register-person-as-user")]
        public async Task<ActionResult<Person>> RegisterPersonAsUser(Person person)
        {
            var user = new User
            {
                UserType = person.User.UserType
                // Initialize additional user properties
            };

            person.User = user;
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPersonById), new { id = person.Id }, person);
        }

        // Scenario 4: Assign User credentials to an existing Person
        [HttpPost("{personId}/assign-user")]
        public async Task<IActionResult> AssignUserToPerson(int personId, User user)
        {
            var person = await _context.Persons.FindAsync(personId);
            if (person == null)
            {
                return NotFound("Person not found.");
            }

            person.User = user;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Get all Persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons()
        {
            return await _context.Persons.Include(p => p.User).ToListAsync();
        }

        // Get a Person by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPersonById(int id)
        {
            var person = await _context.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return person;
        }

        // Update a Person
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, Person updatedPerson)
        {
            if (id != updatedPerson.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedPerson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete a Person
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }

}
