# 5AAR
 Project for the Advanced User Interfaces course at polimi

## Functions

### ARRecognition Scene

1. recognize the object which scanned by [arkit scanner](https://developer.apple.com/documentation/arkit/content_anchors/scanning_and_detecting_3d_objects)

2. add visual help arrow aside (within 2 sec)

### FindBusStation Scene

1. find near bus station
2. return the route to the station with steps
3. show and update step information while the user is walking [we set it 5m accuracy](https://docs.unity3d.com/ScriptReference/LocationService.Start.html)
4. add arrow toward the temp destination

#### TODO: 

- [ ] decide which station is our target
- [ ] scan the bus ticket validator and test the effect
- [ ] add visual reward when users achieve the temp destination

 ## Config file

create a config file under ```Asset``` folder in the format below

```xml
<keys>
    <GoogleMapAPIKey>YOUR-GOOGLE-MAP-API-KEY</GoogleMapAPIKey>
</keys>

```

