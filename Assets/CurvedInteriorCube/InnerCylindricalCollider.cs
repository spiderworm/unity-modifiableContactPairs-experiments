using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public enum InnerCylindricalColliderAxis {
    x, y, z
}

public class InnerCylindricalCollider : MonoBehaviour
{

    public Vector3 worldCylinderPoint = new Vector3(0, 0, 0);
    public float worldCylinderRadius = 1f;
    public InnerCylindricalColliderAxis worldCylinderAxis = InnerCylindricalColliderAxis.x;

    private Vector3 myPosition;

    void Start() {
        myPosition = transform.position;

        Physics.ContactModifyEvent += ModificationCallback;

        var collider = GetComponent<Collider>();
        if (collider) {
            collider.hasModifiableContacts = true;
        }

    }

    void ModificationCallback(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs) {
        var ballRadius = 0.5f;
        
        var innerRadialPoint = new Vector3(worldCylinderPoint.x, worldCylinderPoint.y, worldCylinderPoint.z);

        foreach (ModifiableContactPair pair in contactPairs) {
            for (int i = 0; i < pair.contactCount; i++) {
                var rampPoint = pair.GetPoint(i);
                var normal = pair.GetNormal(i);
                var separation = pair.GetSeparation(i);

                var ballPoint = rampPoint + (normal * separation);

                switch (worldCylinderAxis) {
                    case InnerCylindricalColliderAxis.x:
                        innerRadialPoint.x = pair.position.x;
                        break;
                    case InnerCylindricalColliderAxis.y:
                        innerRadialPoint.y = pair.position.y;
                        break;
                    case InnerCylindricalColliderAxis.z:
                        innerRadialPoint.z = pair.position.z;
                        break;
                }

                var innerRadialNormal = (pair.position - innerRadialPoint).normalized;

                ballPoint = pair.position + (ballRadius * innerRadialNormal);

                rampPoint = innerRadialPoint + (worldCylinderRadius * innerRadialNormal);
                normal = -1 * innerRadialNormal;
                separation = worldCylinderRadius - (ballPoint - innerRadialPoint).magnitude;

                pair.SetPoint(i, rampPoint);
                pair.SetNormal(i, normal);
                pair.SetSeparation(i, separation);
            }
        }
    }
}
