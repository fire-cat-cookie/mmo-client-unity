using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour{

    public Texture2D defaultCursor;
    public Texture2D attackCursor;

    private PlayerService playerService;
    
	void Awake(){
        playerService = FindObjectOfType<PlayerService>();
	}
	
    void Start() {
        UseDefaultCursor();
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Interactable");
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layerMask)) {
            Player player = hit.transform.gameObject.GetComponent<Player>();
            if (player != playerService.GetMainPlayer() && player.CharInfo.Alive) {
                Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
            }
        } else {
            UseDefaultCursor();
        }
    }

    private void UseDefaultCursor() {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

    }

}
