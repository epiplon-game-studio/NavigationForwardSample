using UnityEngine;
using UnityEngine.AI;
using Deviant.Utils;

[RequireComponent(typeof(NavMeshAgent))]
public class Character : MonoBehaviour
{
    public Camera _mainCamera;
    public float speed = 0.6f;

    NavMeshAgent agent;
    int pathIndex = 0;
    Vector3 _direction;
    Vector3 targetPosition;
    Vector3 _velocity;

    bool wasPending;
    int corners;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        NavMesh.onPreUpdate += OnNavmeshPreUpdate;
    }

    void Update()
    {
        SetTarget();

        //if (!agent.pathPending && wasPending)
        //    pathIndex = 0;

        // locks agent from moving
        agent.nextPosition = transform.position;

        if (pathIndex < agent.path.corners.Length)
        {
            targetPosition = agent.path.corners[pathIndex];

            var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            _direction = (targetPoint - transform.position).normalized;
            if (_direction != Vector3.zero)
            {
                var _lookRotation = Quaternion.LookRotation(_direction);

                Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, Time.deltaTime * 360);
                var angle = Quaternion.Angle(nextRotation, transform.rotation);
                //animator.SetFloat(rotationParameter, Mathf.Clamp(angle, -1, 1));
                transform.rotation = nextRotation;
            }

            var d = Vector2.Dot(transform.forward.XZ(), _direction.XZ());

            if (d > 0.9)
            {
                // Move foward
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * speed);
            }
            else
            {
                // Stays in place
                agent.nextPosition = transform.position;
            }

            if (transform.position.XZ() == targetPosition.XZ())
                pathIndex++;

        }

        
    }

    Ray ray;
    void SetTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    void OnNavmeshPreUpdate()
    {
        if ((!agent.pathPending && wasPending)
            || corners != agent.path.corners.Length)
            pathIndex = 0;


        wasPending = agent.pathPending;
        corners = agent.path.corners.Length;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            var size = Vector3.one * 0.2f;
            for (int i = 0; i < agent.path.corners.Length; i++)
            {
                Gizmos.DrawCube(agent.path.corners[i], size);
                if (i > 0)
                    Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i - 1]);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
            Gizmos.DrawRay(ray);

            Gizmos.DrawLine(transform.position, _direction);
        }
    }

    private void OnGUI()
    {
        var rect = new Rect(0, 0, 300, 300);
        GUI.Label(rect, $"Direction: {_direction}");
        rect.y += 20;
        GUI.Label(rect, $"Index: {pathIndex}");
        rect.y += 20;
        GUI.Label(rect, $"Corners Length: {agent.path.corners.Length}");
        rect.y += 20;
        GUI.Label(rect, $"Path Pending: {agent.pathPending}");
        rect.y += 20;
        GUI.Label(rect, $"Path Status: {agent.pathStatus}");
    }
}
