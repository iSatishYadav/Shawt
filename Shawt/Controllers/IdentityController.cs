using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shawt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    // GET: api/Identity
    [HttpGet]
    [Authorize]
    public IEnumerable<string> Get()
    {
        return (User.Identity as ClaimsIdentity)?.Claims?.Select(x => $"{x.Type}:{x.Value} {Environment.NewLine}");
    }

}
