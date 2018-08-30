using UnityEngine;
using System.Collections;

namespace Main.AI.Pathfindidng
{
    public class Unit : MonoBehaviour
    {
        private const float minPathUpdateTime = .2f;
        private const float pathUpdateMoveThreshold = .5f;
        private bool isSelected = false;
        public Transform target;
        public float speed = 20;
        public float turnSpeed = 3;
        public float turnDst = 5;
        public float stoppingDst = 10;
        private Path path;
        private Coroutine lastRoutine = null;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
            }
        }

        void Start()
        {
            StartCoroutine(UpdatePath());
        }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDst, stoppingDst);

                if (lastRoutine != null)
                {
                    StopCoroutine(lastRoutine);
                }

                lastRoutine = StartCoroutine(FollowPath());
            }
        }

        private IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }
            PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);

                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    targetPosOld = target.position;
                }
            }
        }

        private IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            transform.LookAt(path.lookPoints[0]);

            float speedPercent = 1;

            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }

                if (followingPath)
                {
                    if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                    {
                        speedPercent = Mathf.Clamp(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst, 0.01f, 1);
                        //if (speedPercent < 0.01f)
                        //{
                        //    followingPath = false;
                        //}
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
                }

                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            if (path != null)
            {
                path.DrawGizmos();
            }
        }
    }
}