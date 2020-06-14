using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D bc2d;
    private Rigidbody2D rb2d;
    private float inverseMoveTime;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        bc2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {   
        // The sqrt of distance between current postion and destionation
        float sqrtRemainingDistance = (transform.position - end).sqrMagnitude;

        // rb2d move inverseMoveTime * Time.deltaTime every frame, so we use a while loop to control our player
        // can move to destionation 
        while (sqrtRemainingDistance > float.Epsilon)
        {
            // move straight a rb2d to postion end.
            Vector3 newPos = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPos);
            sqrtRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        // current postion
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        bc2d.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        bc2d.enabled = true;
        // if player doesnt collide board, it move to end, and return true;
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    // Move out component and do somethings if this component get a hit or not 
    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        // hit is null, means component move successfully;
        if (hit.transform == null)
            return;
        // component collide a generic component T, maybe wall, maybe enemy, so something different occured by OnCantMove,
        // maybe decrease hitpoint.
        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null) OnCantMove(hitComponent);
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
