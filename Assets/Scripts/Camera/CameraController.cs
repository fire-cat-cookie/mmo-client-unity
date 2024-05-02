using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float height;

    public delegate void CameraMovedHandler();
    public event CameraMovedHandler CameraMoved = delegate { };

    private PlayerService playerManager;

    private GameObject player;

	void Awake() {
        playerManager = FindObjectOfType<PlayerService>();
        playerManager.MainPlayerFound += (Player player) => { this.player = player.gameObject; };
	}
	
	void LateUpdate () {
        if (player != null) {
            transform.position = player.transform.position + new Vector3(0, height, height * -0.4f);
            CameraMoved();
        }
    }
}
