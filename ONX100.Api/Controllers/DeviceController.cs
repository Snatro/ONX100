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


    // PWR ON
    [HttpPost("power/on")]
    public async Task<IActionResult> PowerOn()
    {
        var response = await _driver.PowerOn();

        return Ok(response);
    }


    // PWR OFF
    [HttpPost("power/off")]
    public async Task<IActionResult> PowerOff()
    {
        var response = await _driver.PowerOff();

        return Ok(response);
    }


    // PWR ?
    [HttpGet("power")]
    public async Task<IActionResult> GetPowerState()
    {
        var status = await _driver.GetPowerState();

        return Ok(status);
    }


    // IN <n>
    [HttpPost("input/{input}")]
    public async Task<IActionResult> SetInput(int input)
    {
        if (input < 1 || input > 4)
        {
            return BadRequest("Input must be between 1 and 4.");
        }

        var response = await _driver.SetInput(input);

        return Ok(response);
    }


    // IN ?
    [HttpGet("input")]
    public async Task<IActionResult> GetInput()
    {
        var input = await _driver.GetInput();

        return Ok(input);
    }


    // VOL <n>
    [HttpPost("volume/{volume}")]
    public async Task<IActionResult> SetVolume(int volume)
    {
        if (volume < 0 || volume > 100)
        {
            return BadRequest("Volume must be between 0 and 100.");
        }

        var response = await _driver.SetVolume(volume);

        return Ok(response);
    }


    // VOL ?
    [HttpGet("volume")]
    public async Task<IActionResult> GetVolume()
    {
        var volume = await _driver.GetVolume();

        return Ok(volume);
    }


    // MUTE ON
    [HttpPost("mute/on")]
    public async Task<IActionResult> EnableMute()
    {
        var response = await _driver.SetMute(true);

        return Ok(response);
    }


    // MUTE OFF
    [HttpPost("mute/off")]
    public async Task<IActionResult> DisableMute()
    {
        var response = await _driver.SetMute(false);

        return Ok(response);
    }


    // MUTE ?
    [HttpGet("mute")]
    public async Task<IActionResult> GetMute()
    {
        var mute = await _driver.GetMute();

        return Ok(mute);
    }


    // Current device state
    [HttpGet("status")]
    public async Task<ActionResult<UnitPropertiesStatus>> GetStatus()
    {
        var status = await _driver.GetStatus();

        return Ok(status);
    }
}