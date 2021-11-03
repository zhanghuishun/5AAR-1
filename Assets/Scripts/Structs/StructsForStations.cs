using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct location {
	public float lat;
	public float lng;
}
[System.Serializable]
public struct geometry {
	public location loc;
}
[System.Serializable]
public struct station {
	public geometry geo;
	public string name;
}
[System.Serializable]
public struct attribution {
	public List<station> stations;
}