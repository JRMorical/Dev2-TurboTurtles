using UnityEngine;

public class pathPlatform : MonoBehaviour
{
    public enum LoopType
    {
        Loop,
        PingPong
    }

    [Header("Path")]
    [SerializeField] Transform[] points;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] LoopType loopType = LoopType.Loop;
    [SerializeField] float pointReachDistance = 0.05f;

    int currentIndex = 0;
    int direction = 1;

    Vector3 lastPosition;
    CharacterController rider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (points.Length > 0)
        {
            transform.position = points[0].position;
            currentIndex = points.Length > 1 ? 1 : 0;
        }

        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (points == null || points.Length < 2)
            return;

        Vector3 target = points[currentIndex].position;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        Vector3 delta = transform.position - lastPosition;

        if (rider != null && delta != Vector3.zero)
        {
            rider.Move(delta);
        }

        if (Vector3.Distance(transform.position, target) <= pointReachDistance)
        {
            AdvanceIndex();
        }

        lastPosition = transform.position;
    }

    void AdvanceIndex()
    {
        if (loopType == LoopType.Loop)
        {
            currentIndex = (currentIndex + 1) % points.Length;
        }
        else if (loopType == LoopType.PingPong)
        {
            if (currentIndex == points.Length - 1)
                direction = -1;
            else if (currentIndex == 0)
                direction = 1;

            currentIndex += direction;
        }
    }

    public void SetRider(CharacterController controller)
    {
        rider = controller;
    }

    public void ClearRider(CharacterController controller)
    {
        if (rider == controller)
            rider = null;
    }
}
