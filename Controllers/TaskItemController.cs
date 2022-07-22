using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace TaskManagement.Controllers
{
    [Route("task")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly TaskContext _context;
        private readonly IConfiguration _env;

        public TaskItemController(IConfiguration env)
        {
            _env = env;
        }

        // GET: api/TaskItem
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
        // {
        //     if (_context.TaskItems == null)
        //     {
        //         return NotFound();
        //     }
        //     return await _context.TaskItems.ToListAsync();
        // }

        // GET: api/TaskItem/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        // {
        //     if (_context.TaskItems == null)
        //     {
        //         return NotFound();
        //     }
        //     var taskItem = await _context.TaskItems.FindAsync(id);

        //     if (taskItem == null)
        //     {
        //         return NotFound();
        //     }

        //     return taskItem;
        // }

        // PUT: api/TaskItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        // {
        //     if (id != taskItem.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(taskItem).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!TaskItemExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: api/TaskItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItem taskItem)
        // {

        //   if (_context.TaskItems == null)
        //   {
        //       return Problem("Entity set 'TaskContext.TaskItems'  is null.");
        //   }
        //     _context.TaskItems.Add(taskItem);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        // }


        [HttpPost]
        public void PostTaskItem(TaskItem taskItem)
        {
            {
                taskItem.TaskStatus = "STARTED";

                var factory = new ConnectionFactory()
                {
                    HostName = _env.GetSection("RABBITMQ_HOST").Value,
                    Port = Convert.ToInt32(_env.GetSection("RABBITMQ_PORT").Value),
                    UserName = _env.GetSection("RABBITMQ_USER").Value,
                    Password = _env.GetSection("RABBITMQ_PASSWORD").Value

                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "tasks", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    string message = string.Empty;
                    message = JsonSerializer.Serialize(taskItem);

                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "", routingKey: "tasks", basicProperties: null, body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }


        // DELETE: api/TaskItem/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteTaskItem(int id)
        // {
        //     if (_context.TaskItems == null)
        //     {
        //         return NotFound();
        //     }
        //     var taskItem = await _context.TaskItems.FindAsync(id);
        //     if (taskItem == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.TaskItems.Remove(taskItem);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool TaskItemExists(int id)
        // {
        //     return (_context.TaskItems?.Any(e => e.Id == id)).GetValueOrDefault();
        // }
    }
}
