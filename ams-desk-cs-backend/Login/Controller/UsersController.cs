using ams_desk_cs_backend.Login.Dto;
using ams_desk_cs_backend.Login.Interface;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ams_desk_cs_backend.Login.Controller;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET api/<UserController>/5
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var result = await _userService.GetUsers();
        return Ok(result.Data);
    }

    // POST api/<AccountsController>
    [HttpPost]
    public async Task<IActionResult> AddUser(UserDto user)
    {
        var result = await _userService.PostUser(user);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return NotFound(result.Message);
        }
        return Ok();
    }

    // PUT api/<UserController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(short id, UserDto user)
    {
        var result = await _userService.ChangeUser(id, user);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok();
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(short id)
    {
        var result = await _userService.DeleteUser(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok();
    }
    [HttpPost("Logout/{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<IActionResult> LogOutUser(short id)
    {
        var result = await _userService.LogOutUser(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok();
    }
    [HttpPost("Logout")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<IActionResult> LogOutAll()
    {
        var result = await _userService.LogOutAll();
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok();
    }
}