using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
	public LayerMask passengerMask;

	public override void Start()
	{
		base.Start();

		globalWayPoints = new Vector2[localWayPoints.Length + 1];
		globalWayPoints[0] = transform.position;

		for (int i = 0; i < localWayPoints.Length; i++)
		{
			globalWayPoints[i + 1] = (Vector2)transform.position + localWayPoints[i];
		}
	}

	void FixedUpdate()
	{
		UpdateRaycastOrigins();

		Vector2 velocity = CalculatePlatformMovement();

		MovePassengers(velocity);
		transform.Translate(velocity);
	}

	//移动平台上的物体
    void MovePassengers(Vector2 velocity)
	{
		//该帧移动过的对象
		HashSet<Transform> movedPassengers = new HashSet<Transform>();
		
		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);

		//垂直移动
		if(velocity.y != 0)
		{
			float rayLength = Mathf.Abs(velocity.y) + skinWidth;

			//遍历每道光束
			for (int i = 0; i < verticalRayCount; i++)
			{
				//根据速度方向确定光束方向
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigin.bottomLeft : raycastOrigin.topLeft;

				//光束源点偏移
				rayOrigin += Vector2.right * (verticalRaySpacing * i);

				//发出光束
				RaycastHit2D hit = Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				//有障碍物
				if (hit)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);

						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						hit.transform.Translate(new Vector2(pushX, pushY));
					}
				}
			}
		}

		//水平移动
		if(velocity.x != 0)
		{
			float rayLength = Mathf.Abs(velocity.x) + skinWidth;

			if (Mathf.Abs(velocity.x) < skinWidth)
			{
				rayLength = 2 * skinWidth;  //刚好能检测到相邻物体的距离
			}

			for (int i = 0; i < horizontalRayCount; i++)
			{
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigin.bottomLeft : raycastOrigin.bottomRight;

				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				//碰到物体
				if (hit)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);

						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = 0;

						hit.transform.Translate(new Vector2(pushX, pushY));
					}
				}
			}
		}

		//平台水平或竖直移动时
		if(directionY == -1 || velocity.y == 0 && velocity.x != 0)
		{
			float rayLength = skinWidth * 2;

			//遍历每道光束
			for (int i = 0; i < verticalRayCount; i++)
			{
				//根据速度方向确定光束方向
				Vector2 rayOrigin = raycastOrigin.topLeft + Vector2.right * (verticalRaySpacing * i);

				//发出光束
				RaycastHit2D hit = Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

				//有障碍物
				if (hit)
				{
					if (!movedPassengers.Contains(hit.transform))
					{
						movedPassengers.Add(hit.transform);

						float pushX = velocity.x;
						float pushY = velocity.y;

						hit.transform.Translate(new Vector2(pushX, pushY));
					}
				}
			}
		}
	}

	//判定射线是否射中物体
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 dir, float distance, LayerMask layer)
	{
		RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, layer);

		Debug.DrawLine(origin, origin + dir * distance, Color.green);

		if (hit)
			Debug.DrawLine(origin, origin + dir * hit.distance, Color.red);

		return hit;
	}

	#region 路径点移动

	public Vector2[] localWayPoints;
	Vector2[] globalWayPoints;

	public BoxCollider2D boxCollider2D;

	public float speed = 3;

	int currentWayPointIndex = 0;

	//当前路径点间移动比例（0-1）
	float percentBetweenWayPoints;

	//计算平台移动量
	Vector2 CalculatePlatformMovement()
	{
		int nextWayPointIndex = currentWayPointIndex >= globalWayPoints.Length - 1 ? 0 : currentWayPointIndex + 1;

		float distance = Vector2.Distance(globalWayPoints[currentWayPointIndex], globalWayPoints[nextWayPointIndex]);
		percentBetweenWayPoints += Time.fixedDeltaTime * speed / distance;

		Vector2 pos = Vector2.Lerp(globalWayPoints[currentWayPointIndex], globalWayPoints[nextWayPointIndex], percentBetweenWayPoints);

		if(percentBetweenWayPoints >= 1)
		{
			percentBetweenWayPoints = 0;

			currentWayPointIndex = nextWayPointIndex;
		}

		return pos - (Vector2)transform.position;
	}


	#endregion

	private void OnDrawGizmos()
	{
		if(localWayPoints != null)
		{
			for (int i = 0; i < localWayPoints.Length; i++)
			{
				Vector2 pos = Application.isPlaying ? globalWayPoints[i] : (Vector2)transform.position + localWayPoints[i];
				Gizmos.DrawWireCube(pos, transform.localScale);
			}
		}
	}
}
