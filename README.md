# ONX100
--- 

## RUN PROJECT 

Inside solution directory in command prompt run the following line
```
dotnet run --project ONX100.Api
```
After which you can see Swagger UI on link 

localhost:7205/Swagger in browser.


For testing you can also run 

```
dotnet test

```




# Missed opportunity

1. Dropped response, currently there is no implementation for how to work around dropped response, what to do. My way of thinking would be to reconnect
2. WARM / COOL response and behaviour, They were noted as responses but nothing was implemented around them
3. UI design: I had made a basic UI just to showcase information, with more time and some feedback it could have been done better. Some components are not responding
to unforeseen behavior so it might seem clunky.
