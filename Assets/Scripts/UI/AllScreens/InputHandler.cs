using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum KeyboardContext {
    None, Game, Chat
}

/// <summary>
/// Listens for mouse and keyboard input, decides the context for the input (UI or world), and publishes an event based on the context and input.
/// </summary>
public class InputHandler : MonoBehaviour{

    public delegate void ProcessMove(Vector3 point);
    public delegate void ProcessInteract(GameObject target);
    public delegate void ProcessKey();
    public delegate void SkillHandler(uint skillId);
    public event ProcessMove TerrainMoveCommand = delegate { };
    public event ProcessInteract InteractCommand = delegate { };
    public event ProcessInteract Select = delegate { };
    public event ProcessKey Deselect = delegate { };
    public event SkillHandler SkillPress = delegate { };

    public KeyboardContext keyboardContext = KeyboardContext.None;

    void Update() {
        if (keyboardContext == KeyboardContext.Game) {
            if (Input.GetMouseButtonUp(0)) {
                ProcessLeftClick();
            }
            if (Input.GetMouseButtonUp(1)) {
                ProcessRightClick();
            }
            if (Input.GetKeyUp(KeyCode.Q)) {
                SkillPress(0);
            }
            if (Input.GetKeyUp(KeyCode.W)) {
                SkillPress(1);
            }
        }
    }

    public mmo_shared.Vector2 GetMousePositionOnTerrain() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = (1 << LayerMask.NameToLayer("Terrain"));
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layerMask)) {
            return new mmo_shared.Vector2(hit.point.x, hit.point.z);
        } else {
            return new mmo_shared.Vector2(0,0);
        }
    }

    private void ProcessLeftClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = (1 << LayerMask.NameToLayer("Terrain")) + (1 << LayerMask.NameToLayer("Interactable"));
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layerMask)) {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
                Deselect();
            } else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable")) {
                Select(hit.transform.gameObject);
            }
        }
    }

    private void ProcessRightClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = (1 << LayerMask.NameToLayer("Terrain")) + (1 << LayerMask.NameToLayer("Interactable"));
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layerMask)) {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
                TerrainMoveCommand(hit.point);
            } else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable")) {
                InteractCommand(hit.transform.gameObject);
            }
        }
    }
}
