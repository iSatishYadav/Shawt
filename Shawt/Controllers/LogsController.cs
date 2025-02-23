using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shawt.Data;

namespace Shawt.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LogsController(LinksContext context) : ControllerBase
{

    // GET: api/Logs/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Log>> GetLog(Guid id)
    {
        var log = await context.Log.FindAsync(id);

        if (log == null)
        {
            return NotFound();
        }

        return log;
    }
}
