using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour{

    public delegate void HandleCollision(Player player);
    
    public Vector3 Direction { get; set; }
    public float Velocity { get; set; }
    public float RemainingLifespan { get; set; }
    public HandleCollision CollisionHandler { get; set; }

    void Update() {
        RemainingLifespan -= Time.deltaTime;
        if (RemainingLifespan <= 0) {
            Destroy(gameObject);
            return;
        }
        gameObject.transform.Translate(Direction * Velocity * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider collider) {
        Player player = collider.gameObject.GetComponent<Player>();
        if (player != null) {
            CollisionHandler.Invoke(player);
        }
    }

}
