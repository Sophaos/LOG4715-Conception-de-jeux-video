using UnityEngine;
using System.Collections;

namespace EasyDestuctibleWall {
    public class DestructionManager : MonoBehaviour {
      
        [SerializeField]
        private float health = 100f;

      
        [SerializeField]
        private float impactMultiplier = 2.25f;
        [SerializeField]
        private float twistMultiplier = 0.0025f;

        private Rigidbody cachedRigidbody;

        private void Awake() {
            cachedRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
     
            if(health <= 0f) {
                foreach(Transform child in transform) {
                    child.gameObject.GetComponent<BrickHealth>().WallDestructed =true;
                    Rigidbody spawnRB = child.gameObject.AddComponent<Rigidbody>();
                    child.parent = null;
                    // Transfer velocity
                    spawnRB.velocity = GetComponent<Rigidbody>().GetPointVelocity(child.position);
                    // Transfer torque
                    spawnRB.AddTorque(GetComponent<Rigidbody>().angularVelocity, ForceMode.VelocityChange);
                }
                Destroy(gameObject); // Destroy this now empty chunk object
            }
        }


        void OnCollisionEnter(Collision collision) {        
            if(collision.gameObject.tag == "Player") {
                if (collision.gameObject.GetComponentInChildren<Playerhealth>().BigMode) {
                    health = 0;
                }
             
                
            } 
        }
    }
}