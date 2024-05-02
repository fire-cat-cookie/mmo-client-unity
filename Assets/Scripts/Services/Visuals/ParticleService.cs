using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleService : MonoBehaviour{
    
	public void PlayParticles(GameObject prefab, Vector3 position, float lifetime) {
        var particle = GameObject.Instantiate(prefab, position, Quaternion.identity);
        particle.transform.Translate(new Vector3(0, 0.2f, 0));

        //little hack that moves the particles closer to the camera, so they don't clip through the target.
        particle.transform.position = Vector3.MoveTowards(particle.transform.position, Camera.main.transform.position, 0.5f);

        GameObject.Destroy(particle, lifetime);
    }
	
}
