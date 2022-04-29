using ApiAuth.Auth;
using ApiAuth.Auth.Model;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ApiAuth.Endpoints;

public class MyAccount: EndpointBaseSync
    .WithoutRequest
    .WithActionResult
{
    private readonly IMapper _mapper;

    public MyAccount(IMapper mapper)
    {
        _mapper = mapper;
    }
    [Authorize]
    [HttpGet("my-account")]
    public override ActionResult Handle()
    {
        if (HttpContext.Items["Account"] is not Account account)
        {
            Log.Error("No account data found in HttpContext.Items");
            return StatusCode(500);
        }

        return Ok(_mapper.Map<MyAccountResult>(account));
    }
}