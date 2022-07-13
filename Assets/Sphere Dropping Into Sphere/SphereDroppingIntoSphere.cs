using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class SphereDroppingIntoSphere : MonoBehaviour {
    public GameObject otherSphere;
    private Collider otherCollider;
    private float ignoreAboveY;

    void Start() {
        otherCollider = otherSphere.GetComponent<Collider>();
        ignoreAboveY = transform.position.y;

        Physics.ContactModifyEvent += ModificationCallback;

        var collider = GetComponent<Collider>();
        if (collider) {
            collider.hasModifiableContacts = true;
        }
    }

    void Update() {}

    void ModificationCallback(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs) {
        var ballDiameter = 1;

        foreach (ModifiableContactPair pair in contactPairs) {
            for (int i = 0; i < pair.contactCount; i++) {
                var rampPoint = pair.GetPoint(i);
                if (rampPoint.y >= ignoreAboveY) {
                    pair.IgnoreContact(i);
                } else {
                    var normal = pair.GetNormal(i);
                    var separation = pair.GetSeparation(i);

                    var ballPoint = rampPoint + (normal * separation);
                    //Debug.DrawLine(ballPoint, rampPoint, Color.red, 500);

                    if (pair.GetSeparation(i) < -ballDiameter) {
                        pair.IgnoreContact(i);
                    } else {
                        separation = -ballDiameter - separation;
                        normal = normal * -1;
                        ballPoint = rampPoint + (normal * separation);
                        //Debug.DrawLine(ballPoint, rampPoint, Color.blue, 500);

                        pair.SetNormal(i, normal);
                        pair.SetSeparation(i, separation);
                    }
                }
            }
        }
    }
}
