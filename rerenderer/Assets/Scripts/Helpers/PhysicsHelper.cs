using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PhysicsHelper
{
    public static bool SphereCastDoubleSided(Vector3 from, Vector3 direction, float radius, out RaycastHit hitInfo, float distance, Collider[] ignoreColliders = null)
    {
        return SphereCastDoubleSided(from, direction, radius, out hitInfo, distance, (hit) => ignoreColliders != null && ignoreColliders.Contains(hit.collider));
    }

    public static bool SphereCastDoubleSided(Vector3 from, Vector3 direction, float radius, out RaycastHit hitInfo, float distance, System.Func<RaycastHit, bool> ignoreFunc = null)
    {
        RaycastHit hitInfoBack;

        direction = direction.normalized;

        var hitFront = SphereCastIgnoreArbitrary(from, direction, radius, out var hitInfoFront, distance, ignoreFunc);
        var hitBack = false;

        // if we hit something from the front, make sure there is no collision backwards between from and hit point
        if (hitFront)
        {
            hitBack = SphereCastIgnoreArbitrary(hitInfoFront.point, (from - hitInfoFront.point), radius, out hitInfoBack, hitInfoFront.distance, ignoreFunc);
            if (hitBack && hitInfoBack.distance < hitInfoFront.distance)
            {
                hitInfo = hitInfoBack;
                hitInfo.distance = hitInfoFront.distance - hitInfo.distance;
                return true;
            }

            hitInfo = hitInfoFront;
            return true;
        }

        // nothing hit from front so try hitting backwards
        hitBack = SphereCastIgnoreArbitrary(hitInfoFront.point, (from - hitInfoFront.point), radius, out hitInfoBack, hitInfoFront.distance, ignoreFunc);
        if (hitBack)
        {
            hitInfo = hitInfoBack;
            hitInfo.distance = distance - hitInfo.distance;
            return true;
        }

        // default
        hitInfo = new RaycastHit();
        return false;
    }

    public static bool SphereCastIgnore(Vector3 from, Vector3 direction, float radius, out RaycastHit hitInfo, float distance, Collider[] ignoreColliders)
    {
        return SphereCastIgnoreArbitrary(from, direction, radius, out hitInfo, distance, (hit) => ignoreColliders != null && ignoreColliders.Contains(hit.collider));
    }

    public static bool SphereCastIgnoreArbitrary(Vector3 from, Vector3 direction, float radius, out RaycastHit hitInfo, float distance, System.Func<RaycastHit, bool> ignoreFunc)
    {
        direction = direction.normalized;
        var totalDistance = 0f;
        var result = false;

        while (true)
        {
            if ((result = Physics.SphereCast(new Ray(from, direction), radius, out hitInfo, distance)) && ignoreFunc != null && ignoreFunc.Invoke(hitInfo))
            {
                result = false;
                from = hitInfo.point;
                totalDistance += hitInfo.distance;
                distance -= hitInfo.distance;
                continue;
            }

            break;
        }

        hitInfo.distance += totalDistance;
        return result;
    }

    public static bool RaycastIgnore(Vector3 from, Vector3 direction, out RaycastHit hitInfo, float distance, Collider[] ignoreColliders)
    {
        direction = direction.normalized;
        var result = false;

        while (true)
        {
            if ((result = Physics.Raycast(new Ray(from, direction), out hitInfo, distance)) && ignoreColliders != null && ignoreColliders.Contains(hitInfo.collider))
            {
                result = false;
                from = hitInfo.point;
                distance -= hitInfo.distance;
                continue;
            }

            break;
        }

        return result;
    }



    public static bool CheckInitialFire(Vector3 barrelPos, Vector3 shotPos, Vector3 direction, float radius, out RaycastHit hitInfo, Collider[] ignoreColliders)
    {
        if (barrelPos != shotPos)
        {
            var barrelDir = (shotPos - barrelPos);
            if (PhysicsHelper.RaycastIgnore(barrelPos, barrelDir, out hitInfo, barrelDir.magnitude + radius, ignoreColliders))
            {
                // if both are on same side of normal then consider it a hit
                // this lets the gun dip below the ground but shoot up out of it in cases where the animation (landing) has the shot spawn pos under the ground
                if (Mathf.Sign(Vector3.Dot(barrelDir, hitInfo.normal)) == Mathf.Sign(Vector3.Dot(direction, hitInfo.normal)))
                    return true;
            }
        }

        // default
        hitInfo = new RaycastHit() { point = shotPos };
        return Physics.OverlapSphere(shotPos, radius)?.Any(x => ignoreColliders == null || !ignoreColliders.Contains(x)) ?? false;
    }
}
