# 5AAR
 Project for the Advanced User Interfaces course at polimi

## Functions

1. find near bus station
2. return the route to the station with steps
3. show and update step information while the user is walking ([the default precision of unity location api is 10m](https://docs.unity3d.com/ScriptReference/LocationService.Start.html))

#### TODO: 

1. decide which station is our target
2. add a compass and maneuver

 ## Config file

create a config file under ```Asset``` folder in the format below

```xml
<keys>
    <GoogleMapAPIKey>YOUR-GOOGLE-MAP-API-KEY</GoogleMapAPIKey>
</keys>

```

