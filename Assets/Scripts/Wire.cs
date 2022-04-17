using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Wire : MonoBehaviour
{
    public int wireLength = 20;

    [SerializeField] private Transform StartPoint;
    [SerializeField] private Transform EndPoint;

    private LineRenderer lineRenderer;
    private List<WireSegment> wireSegments = new List<WireSegment>();
    private float wireSegmentLength = 0.05f;
    private float lineWidth = 0.01f;


    void Start()
    {
        if (StartPoint != null || EndPoint != null)
        {
            Vector3 wireStartPoint = StartPoint.position;

            for (int i = 0; i < wireLength; i++)
            {
                wireSegments.Add(new WireSegment(wireStartPoint));
                wireStartPoint.y -= wireSegmentLength;
            }
        }
        else
        {
            Debug.LogError("Please assign Start/End point of the " + gameObject.name + " wire");
        }
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        if (StartPoint != null && EndPoint != null)
        {
            DrawRope();
        }
    }
    private void FixedUpdate()
    {
        if (StartPoint != null && EndPoint != null)
        {
            Simulate();
        }
    }
    private void Simulate()
    {
        Vector3 forceGravity = new Vector3(0f, -1f);

        for (int i = 1; i < wireLength; i++)
        {
            WireSegment firstSegment = wireSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            wireSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        WireSegment firstSegment = wireSegments[0];
        firstSegment.posNow = StartPoint.position;
        wireSegments[0] = firstSegment;


        WireSegment endSegment = wireSegments[wireSegments.Count - 1];
        endSegment.posNow = EndPoint.position;
        wireSegments[wireSegments.Count - 1] = endSegment;

        for (int i = 0; i < wireLength - 1; i++)
        {
            WireSegment firstSeg = wireSegments[i];
            WireSegment secondSeg = wireSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - wireSegmentLength);
            Vector3 changeDir = Vector3.zero;

            if (dist > wireSegmentLength)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < wireSegmentLength)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                wireSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                wireSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                wireSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] wirePositions = new Vector3[this.wireLength];
        for (int i = 0; i < this.wireLength; i++)
        {
            wirePositions[i] = this.wireSegments[i].posNow;
        }

        lineRenderer.positionCount = wirePositions.Length;
        lineRenderer.SetPositions(wirePositions);
    }

    public struct WireSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public WireSegment(Vector3 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}
