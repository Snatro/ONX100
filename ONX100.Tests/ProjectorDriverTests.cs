using Moq;
using Xunit;
using ONX100.Driver;
using ONX100.Communication;
using ONX100.Models;

namespace ONX100.Tests;

public class ProjectorDriverTests
{
    private readonly Mock<ITcpClientConnection> _connection;
    private readonly ProjectorDriver _driver;


    public ProjectorDriverTests()
    {
        _connection = new Mock<ITcpClientConnection>();

        _connection
            .Setup(x => x.IsConnected)
            .Returns(true);


        _driver = new ProjectorDriver(
            _connection.Object
        );
    }


    [Fact]
    public async Task PowerOn_ShouldSendPowerOnCommand()
    {
        await _driver.PowerOn();


        _connection.Verify(
            x => x.SendCommand("PWR ON"),
            Times.Once
        );
    }


    [Fact]
    public async Task PowerOff_ShouldSendPowerOffCommand()
    {
        await _driver.PowerOff();


        _connection.Verify(
            x => x.SendCommand("PWR OFF"),
            Times.Once
        );
    }


    [Fact]
    public async Task SetInput_ShouldSendCommand_WhenInputIsValid()
    {
        await _driver.SetInput(3);


        _connection.Verify(
            x => x.SendCommand("IN 3"),
            Times.Once
        );
    }


    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public async Task SetInput_ShouldReturnError_WhenInputIsInvalid(
        int input)
    {
        var result =
            await _driver.SetInput(input);


        Assert.False(result.Success);

        Assert.Equal(
            "ERR 02",
            result.ErrorCode
        );


        _connection.Verify(
            x => x.SendCommand(It.IsAny<string>()),
            Times.Never
        );
    }


    [Fact]
    public async Task SetVolume_ShouldSendCommand_WhenVolumeIsValid()
    {
        await _driver.SetVolume(50);


        _connection.Verify(
            x => x.SendCommand("VOL 50"),
            Times.Once
        );
    }


    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task SetVolume_ShouldReturnError_WhenVolumeIsInvalid(
        int volume)
    {
        var result =
            await _driver.SetVolume(volume);


        Assert.False(result.Success);

        Assert.Equal(
            "ERR 02",
            result.ErrorCode
        );


        _connection.Verify(
            x => x.SendCommand(It.IsAny<string>()),
            Times.Never
        );
    }


    [Fact]
    public async Task GetPowerState_ShouldReturnTrue_WhenPowerIsOn()
    {
        _connection
            .Setup(x => x.Query("PWR ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "PWR ON",
                    Type = MessageType.Acknowledge
                }
            );


        var result =
            await _driver.GetPowerState();


        Assert.True(result);
    }


    [Fact]
    public async Task GetPowerState_ShouldReturnFalse_WhenPowerIsOff()
    {
        _connection
            .Setup(x => x.Query("PWR ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "PWR OFF",
                    Type = MessageType.Acknowledge
                }
            );


        var result =
            await _driver.GetPowerState();


        Assert.False(result);
    }


    [Fact]
    public async Task GetInput_ShouldReturnInputNumber()
    {
        _connection
            .Setup(x => x.Query("IN ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "IN 2",
                    Type = MessageType.Acknowledge
                }
            );


        var result =
            await _driver.GetInput();


        Assert.Equal(
            2,
            result
        );
    }


    [Fact]
    public async Task GetStatus_ShouldReturnCompleteProjectorState()
    {
        _connection
            .Setup(x => x.Query("PWR ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "PWR ON",
                    Type = MessageType.Acknowledge
                }
            );


        _connection
            .Setup(x => x.Query("IN ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "IN 3",
                    Type = MessageType.Acknowledge
                }
            );


        _connection
            .Setup(x => x.Query("VOL ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "VOL 20",
                    Type = MessageType.Acknowledge
                }
            );


        _connection
            .Setup(x => x.Query("MUTE ?"))
            .ReturnsAsync(
                new DeviceMessage
                {
                    Raw = "MUTE OFF",
                    Type = MessageType.Acknowledge
                }
            );


        var result =
            await _driver.GetStatus();


        Assert.True(result.IsPoweredOn);
        Assert.Equal(3, result.InputSource);
        Assert.Equal(32, result.VolumeLevel);
        Assert.False(result.IsMuted);
    }


    [Fact]
    public async Task PowerOn_ShouldThrow_WhenDeviceIsDisconnected()
    {
        _connection
            .Setup(x => x.IsConnected)
            .Returns(false);


        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _driver.PowerOn()
        );
    }
}