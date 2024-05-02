using mmo_shared.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private Player player;

    void Awake() {
        player = GetComponent<Player>();
    }

	void Start () {
    }
	
	void Update () {
        Movement();
    }

    void FixedUpdate() {
    }

    private void Movement() {
        if (IsMoving()) {
            float distance = Vector3.Distance(player.Destination, transform.position);
            if (distance <= 0.1 && distance >= -0.1) {
                //destination reached
                player.SetDestination(transform.position);
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, player.Destination, player.CharInfo.Velocity * Time.deltaTime);
            var yPos = Terrain.activeTerrain.SampleHeight(transform.position);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            transform.LookAt(new Vector3(player.Destination.x, transform.position.y, player.Destination.z));
        }
    }

    public bool IsMoving() {
        return player.Destination != Vector3.zero && player.Destination != transform.position;
    }
}
