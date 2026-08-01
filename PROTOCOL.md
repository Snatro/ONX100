
# ONX-100 Communication Protocol 


## Overview 

This document describes the reverse-engineered communication protocol used by the ONX-100 simulator.
The device communicates over TCP using ASCII messages terminated by CRLF.
This document describes the actual observed behavior of the simulator, including differences from the vendor documentation.

---


# Connection

```
 "Device": {
   "IpAddress": "127.0.0.1",
   "Port": 4999
 }
```

## Connecting successfully

When connection between server and client is successful, we get a confirmation from Server: 

`[17:41:35.450] client connected: 127.0.0.1:2620`
`[17:41:35.455] tx: *HELLO ONX-100 FW:2.13`

The initial `*HELLO ONX-100 FW:2.13` message is sent by the simulator immediately after a TCP connection is established and should be treated as a handshake rather than a response to a command.

### Disconnect 


After roughly 60 seconds of idle TCP communication between server and client, the simulator closes the communication: 

`[17:42:39.537] tx: BYE`
`[17:42:39.537] idle timeout, closing connection`
`[17:42:39.538] session ended`


If Client closes the connection, we are met with message: 

`[17:46:03.543] client disconnected`
`[17:46:03.544] session ended`


--- 


# Commands

TCP communication between driver and device is separated by a different few types depending on context of request and response 
1. Acknowledge - Commands that modify device state: `PWR ON` `VOL 32` ... Queries retrieve back **acknowledgement** `OK` 
2. Response - Requests where we fetch current value of a specific part: `PWR ?` `RESPONSE: PWR ON` 
3. Error - Error messages `ERR 01` `ERR 02` 
4. Handshake - Connecting and starting communication  `tx: *HELLO ONX-100 FW:2.13`
5. Disconnect - Cutting communication `session ended`

While all types of messages are containing some form of response from Server, only **Acknowledge** and **Response** have direct request from client side. 

## List of commands 

### Power 

`PWR ON / OFF` - Query to turn power on or off
	 RESPONSE: `OK / ERR`
`PWR ?` - Receive current power status of driver device
	 RESPONSE: `PWR ON / OFF / WARM / COOL / ERR`

### Input 

`IN 1 / 2 / 3 / 4` - Set input channel of device
	RESPONSE: `OK / ERR`
`IN ?` - Receive current input channel used
	RESPONSE: `1 / 2 / 3 / 4 / ERR`

### Volume

`VOL 1 - 100`* - Set volume of device
	RESPONSE: `OK / ERR`
`VOL ?` - Receive current volume of device
	 RESPONSE: `VOL 1 - 100 / ERR`

* 1 - 100 is the allowed range of volume. Anything out of the interval is met with `ERR 02` AKA. Invalid parameter. 

### Mute

`MUTE ON / OFF` - Mute the device
	RESPONSE: `OK / ERR`
`MUTE ?` - Receive current status 
	 RESPONSE: `ON / OFF / ERR`



--- 



# Messaging 

Each command must end with symbols ***\r\n***
Messages are sent as ASCII text. 

So a query **PWR ?** is processed as **PWR ? \r\n**

## Message order and commands 

The usual order of communication is client -> server -> client. 

*Example*

`[17:41:39.426] rx: PWR ?`
`[17:41:39.533] tx: PWR OFF`

*rx* - Client side request
*tx* - Server side response
*17:41:39.426* - Time of message sent or received 

## Event messages 

Often times, after server has already sent their acknowledgment or response, it will be followed with event message describing action. 

*Example*

`[17:48:14.583] tx: EVT PWR ON`
`[17:48:16.361] tx: EVT SIGNAL 4 OK`
`[17:48:54.377] tx: EVT SIGNAL 4 LOST`

Event messages are asynchronous and may be received independently of the command that triggered them. They should not be interpreted as direct command responses.

## Error messages 

Seen in documentation ***ONX-100 Presentation Switcher — Control Protocol*** two known error messages are sent from Server. 

**ERR 01** - Unknown command
**ERR 02** - Invalid parameters

There has been a third error message 
**ERR 03**
Which was only sent when a command was concerning input whether it was for checking status or for changing input channel. 

**Reason** it happens, based on observations done on driver through testing, was because Input cannot be reached or changed when projector power state is turned **OFF**. 

During more testing, the behavior was repeated on Power switching back and forth. 

`[18:18:30.515] rx: PWR ?`
`[18:18:30.737] tx: PWR ON`
`[18:18:35.500] rx: PWR ON`
`[18:18:35.651] tx: OK`
`[18:18:43.348] rx: PWR OFF`
`[18:18:43.687] tx: OK`
`[18:18:46.604] rx: PWR ON`
`[18:18:46.880] tx: ERR 03`


## Power status responses

`[18:26:20.462] rx: PWR ?`
`[18:26:20.692] tx: PWR OFF`
`[18:26:24.371] rx: PWR ON`
`[18:26:24.632] tx: OK`
`[18:26:29.794] rx: PWR ?`
`[18:26:29.993] tx: PWR WARM`
`[18:26:32.373] tx: EVT PWR ON`
`[18:26:44.036] rx: PWR OFF`
`[18:26:44.127] tx: OK`
`[18:26:46.199] rx: PWR ?`
`[18:26:46.539] tx: PWR COOL`
`[18:26:49.043] tx: EVT PWR OFF`

There are two instances where response is showcasing a state in between binaries. 
**PWR WARM** is a state when we turn on driver, a few moments until it has been fully turned on. A sort of waiting phase.
In contrast, **PWR COOL** is when we are turning the driver off, making it a cooldown phase until its been turned off.
Both final states are mentioned through event responses, **EVT PWR OFF & ON**

## Dropped responses

Instead of receiving response from Server side, we get a simple message: 
`(response dropped: OK)`

After a dropped response, the client and server can become out of synchronization. 
Observed consequence:

- Following commands may not receive expected responses.
- Reconnecting the TCP session restores normal communication. 

The driver should process one command at a time and wait for the corresponding response before sending another command.


---

# Volume response format

Volume values are sent to the device in decimal format.

Example:

```
VOL 50
```

Response:

```
OK
```

When querying the current volume, the device returns the value in hexadecimal.

Example:

```
VOL ?
VOL 32
```

In this example, `0x32` is hexadecimal and corresponds to decimal **50**.

The driver must therefore convert the returned hexadecimal value to decimal before exposing it to the application.
