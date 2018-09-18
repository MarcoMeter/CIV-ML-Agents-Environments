using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubes : MonoBehaviour {

    public float intervall = 1.0f;
    private float nextSpawn = 0f;

    public Transform[] prefabs;

    public float moveSpeed = 3f;
    private float npos = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time >= nextSpawn)
        {
            nextSpawn = Time.time + intervall;
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform.position, Quaternion.identity);
            npos = Random.Range(-0.4f, 0.4f);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, npos), Time.deltaTime * moveSpeed);
	}
}
