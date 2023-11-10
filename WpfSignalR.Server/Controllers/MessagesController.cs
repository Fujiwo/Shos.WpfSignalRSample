using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WpfSignalR.Server.Controllers;

using Models;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    readonly MessageContext context = new();

    // GET: api/Messages
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        => context.Messages is null ? NotFound()
                                    : await context.Messages.Include(message => message.User)
                                                            .ToListAsync();

    //// GET: api/Messages/5
    //[HttpGet("{id}")]
    //public async Task<ActionResult<Message>> GetMessage(int id)
    //{
    //    if (context.Messages is null)
    //        return NotFound();

    //    var message = await context.Messages.FindAsync(id);

    //    if (message is null)
    //        return NotFound();

    //    return message;
    //}

    //// PUT: api/Messages/5
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutMessage(int id, Message message)
    //{
    //    if (id != message.Id)
    //        return BadRequest();

    //    context.Entry(message).State = EntityState.Modified;

    //    try
    //    {
    //        await context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (MessageExists(id))
    //            throw;
    //        else
    //            return NotFound();
    //    }

    //    return NoContent();
    //}

    //// POST: api/Messages
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPost]
    //public async Task<ActionResult<Message>> PostMessage(Message message)
    //{
    //    if (context.Messages is null)
    //        return Problem("Entity set 'MessageContext.Messages'  is null.");

    //    context.Messages.Add(message);
    //    await context.SaveChangesAsync();

    //    return CreatedAtAction("GetMessage", new { id = message.Id }, message);
    //}

    //// DELETE: api/Messages/5
    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteMessage(int id)
    //{
    //    if (context.Messages is null)
    //        return NotFound();

    //    var message = await context.Messages.FindAsync(id);
    //    if (message is null)
    //        return NotFound();

    //    context.Messages.Remove(message);
    //    await context.SaveChangesAsync();

    //    return NoContent();
    //}

    //bool MessageExists(int id)
    //    => (context.Messages?.Any(message => message.Id == id)).GetValueOrDefault();
}
