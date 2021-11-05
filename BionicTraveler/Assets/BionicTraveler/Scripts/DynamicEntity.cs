using UnityEngine;
using System.Collections;

public class DynamicEntity : Entity
{
    private Vector3 direction;
    private Vector3 velocity;
    private bool stunned;
    private bool invincible;
    private Vector3 finalTarget;

    /// <summary>
    /// Function for moving a dynamic entity to a target position
    /// </summary>
    /// <param name="target">Target world position to move to</param>
    public void MoveTo(Vector3 target)
    {
        Vector3 pos = gameObject.transform.position;
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(target.y - pos.y, target.x - pos.x));
        if (angle > 315 || angle < 45) { direction = Vector3.right; }
        else if (angle < 135) { direction = Vector3.up; }
        else if (angle < 225) { direction = Vector3.left; }
        else { direction = Vector3.down; }
        base.MoveTo(target);
    }

    private void Move(Vector3 target)
    {
        //use to implement smaller movement steps with pathfinding, if not using NavMesh
    }

    /// <summary>
    /// Makes DEntity invincible for specified number of milliseconds
    /// </summary>
    /// <param name="ms">Number of milliseconds to remain invincible</param>
    public void iFrame(int ms)
    {
        StartCoroutine(iFrameHandler(ms));
    }

    private IEnumerator iFrameHandler(int ms)
    {
        invincible = true;
        yield return new WaitForSeconds(ms / 1000f);
        invincible = false;
    }

    private void StunLock()
    {
        stunned = true;
    }

    /// <summary>
    /// Stagger/Stun the entity for the specified number of milliseconds
    /// </summary>
    /// <param name="ms">The number of milliseconds to stun the entity</param>
    public void Stagger(int ms)
    {
        StartCoroutine(StaggerHandler(ms));
    }

    private IEnumerator StaggerHandler(int ms)
    {
        stunned = true;
        yield return new WaitForSeconds(ms / 1000f);
        stunned = false;
    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
