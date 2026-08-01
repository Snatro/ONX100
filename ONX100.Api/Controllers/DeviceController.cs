using Microsoft.AspNetCore.Mvc;
using ONX100.Driver;
using ONX100.Models;

namespace ONX100.Api.Controllers;

[ApiController]
[Route("api/device")]
public class DeviceController : ControllerBase
{
    private readonly ProjectorDriver _driver;

    public DeviceController(ProjectorDriver driver)
    {
        _driver = driver;
    }

    [HttpPost("power/on")]
    public async Task<IActionResult> PowerOn()
    {
        try
        {
            var response = await _driver.PowerOn();
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("power/off")]
    public async Task<IActionResult> PowerOff()
    {
        try
        {
            var response = await _driver.PowerOff();
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("power")]
    public async Task<IActionResult> GetPowerState()
    {
        try
        {
            var status = await _driver.GetPowerState();
            return Ok(status);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("input/{input}")]
    public async Task<IActionResult> SetInput(int input)
    {
        if (input < 1 || input > 4)
            return BadRequest("Input must be between 1 and 4.");

        try
        {
            var response = await _driver.SetInput(input);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("input")]
    public async Task<IActionResult> GetInput()
    {
        try
        {
            var input = await _driver.GetInput();
            return Ok(input);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("volume/{volume}")]
    public async Task<IActionResult> SetVolume(int volume)
    {
        if (volume < 0 || volume > 100)
            return BadRequest("Volume must be between 0 and 100.");

        try
        {
            var response = await _driver.SetVolume(volume);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("volume")]
    public async Task<IActionResult> GetVolume()
    {
        try
        {
            var volume = await _driver.GetVolume();
            return Ok(volume);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("mute/on")]
    public async Task<IActionResult> EnableMute()
    {
        try
        {
            var response = await _driver.SetMute(true);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("mute/off")]
    public async Task<IActionResult> DisableMute()
    {
        try
        {
            var response = await _driver.SetMute(false);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("mute")]
    public async Task<IActionResult> GetMute()
    {
        try
        {
            var mute = await _driver.GetMute();
            return Ok(mute);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("status")]
    public async Task<ActionResult<UnitPropertiesStatus>> GetStatus()
    {
        try
        {
            var status = await _driver.GetStatus();
            return Ok(status);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}