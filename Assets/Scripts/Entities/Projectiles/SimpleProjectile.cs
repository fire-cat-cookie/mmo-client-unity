using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour{

    public delegate void HandleCollision(Actor actor);
    
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
        Actor actor = collider.gameObject.GetComponent<Actor>();
        if (actor != null) {
            CollisionHandler.Invoke(actor);
        }
    }

}
