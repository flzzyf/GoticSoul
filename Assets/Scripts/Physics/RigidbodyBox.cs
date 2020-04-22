using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidEvent();

//最后执行
//[DefaultExecutionOrder(100)]
public class RigidbodyBox : RaycastController
{
    //该物体每帧的移动量
    public Vector2 velocity;

    public Vector2 movingForce;

    public float gravity = -20;

    public bool isOnGround { get { return collisions.below; } }

    //默认面向右边
    int facingDir = 1;

    void FixedUpdate()
    {
        //计算施加在该物体上的力
        CalculateForces(ref velocity);

        //对该物体施加它身上的（玩家移动力）
        ApplyForces((velocity + movingForce) * Time.fixedDeltaTime);

		UpdateLandAndLeaveEvent();
    }

    //计算施加在该物体上的力
    void CalculateForces(ref Vector2 v)
    {
        //如果上下有接触物体则设置y速度为0
        if ((collisions.below && v.y < 0) || (collisions.above))
            velocity.y = 0;

        //摩擦力，只计算在在这之前的力
        CalculateFriction(ref v);

        //施加重力
        v.y += gravity * Time.fixedDeltaTime;
    }

    //对该物体施加它身上的力
    void ApplyForces(Vector2 v)
    {
        //更新四个方向的光束源点
        UpdateRaycastOrigins();
        //重置碰撞
        collisions.Reset();
        //之前速度
        collisions.velocityOld = v;

        //下坡
        if (v.y < 0)
            DescendSlope(ref v);

        //水平碰撞判定（为了爬墙，即便是0也判断）
        //if(v.x != 0)
        HorizontalCollisions(ref v);

        //竖直射线判定，判定当前移动方向上有没有被阻挡
        VerticalRaycast(ref v);

        transform.Translate(v);
    }

    #region 水平和竖直碰撞判定
    //水平碰撞判定
    void HorizontalCollisions(ref Vector2 v)
    {
        if(v.x != 0)
        facingDir = v.x > 0 ? 1 : -1;

        //速度正负方向
        float directionX = facingDir;
        //光束长度
        float rayLength = Mathf.Abs(v.x) + skinWidth;

        if (Mathf.Abs(v.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;  //刚好能检测到相邻物体的距离
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigin.bottomLeft : raycastOrigin.bottomRight;

            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            //碰到物体
            if (hit)
            {
                if (hit.distance == 0)
                    continue;

                //坡角度
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClambAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        v = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    //进入新的坡
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        v.x -= distanceToSlopeStart * directionX;
                    }
                    //爬坡
                    ClampSlope(ref v, slopeAngle);
                    v.x += distanceToSlopeStart * directionX;
                }
                //爬不动坡
                if (!collisions.clambingSlope || slopeAngle > maxClambAngle)
                {
                    //根据距离确定下一步移动距离
                    v.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                    //在爬坡
                    if (collisions.clambingSlope)
                    {
                        v.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(v.x);
                    }

                    //若方向向左则为真
                    collisions.left = directionX == -1;
                    //向右
                    collisions.right = directionX == 1;
                }
            }

        }
    }

    void VerticalRaycast(ref Vector2 v)
    {
        //速度正负方向，默认是负的
        int dir = v.y > 0 ? 1 : -1;
        //光束长度
        float rayLength = Mathf.Abs(v.y) + skinWidth;

        //遍历每道光束
        for (int i = 0; i < verticalRayCount; i++)
        {
            //根据速度方向确定光束方向
            Vector2 rayOrigin = (dir == -1) ? raycastOrigin.bottomLeft : raycastOrigin.topLeft;

            //光束源点偏移
            rayOrigin += Vector2.right * (verticalRaySpacing * i + v.x);

            //发出光束
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.up * dir, rayLength, collisionMask);

            //有障碍物
            if (hit)
            {
                v.y = (hit.distance - skinWidth) * dir;

                rayLength = hit.distance;

                //若方向向下则为真
                collisions.below = dir == -1;
                //向右
                collisions.above = dir == 1;
            }
        }
    }

    #endregion

    #region 上下坡

    //最大可攀爬角度
    public float maxClambAngle = 60;
    //最大下坡角度（超过会直接掉下来
    public float maxDescendAngle = 55;

    void ClampSlope(ref Vector2 v, float slopeAngle)
    {
        //在平地上应该移动的水平距离，若有障碍物则为0
        float moveDistance = Mathf.Abs(v.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (v.y > climbVelocityY)
        {
            //print("Jumping");
        }
        else
        {
            //计算在斜坡上的xy轴移动距离
            v.y = climbVelocityY;
            v.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(v.x);
            //设置为接触地面
            collisions.below = true;
            collisions.clambingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 v)
    {
        //速度正负方向
        float directionX = Mathf.Sign(v.x);

        Vector2 rayOrigin = (directionX == -1) ? raycastOrigin.bottomRight : raycastOrigin.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            //坡角度
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            //坡角在合适范围内
            if (slopeAngle != 0 && slopeAngle <= maxClambAngle)
            {
                //在下坡
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    //
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(v.x))
                    {
                        float moveDistance = Mathf.Abs(v.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        v.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(v.x);
                        v.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }

            }
        }
    }

    #endregion

    #region 射线碰撞判定

    

    #endregion

    #region 碰撞信息
    public CollisionInfo collisions;
    public struct CollisionInfo
    {
        //上下左右接触物体
        public bool above, below;
        public bool left, right;

        //正在上下坡
        public bool clambingSlope;
        public bool descendingSlope;
        //坡度
        public float slopeAngle, slopeAngleOld;

        public Vector3 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;

            clambingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    #endregion

    public void AddForce(Vector2 force)
    {
        velocity += force;
    }

    #region 阻力

    //空气阻力
    public float airFriction = .1f;
    //地面阻力
    public float groundFriction = .2f;

    //计算阻力
    void CalculateFriction(ref Vector2 v)
    {
		float friction = isOnGround ? groundFriction : airFriction;

		//速度减去阻力，但不少于0

		if (v.x != 0)
			v.x = Mathf.Sign(v.x) * Mathf.Max(0, Mathf.Abs(v.x) - friction * Time.fixedDeltaTime);

		if (v.y != 0 && !isOnGround)
			v.y = Mathf.Sign(v.y) * Mathf.Max(0, Mathf.Abs(v.y) - friction * Time.fixedDeltaTime);
	}

	#endregion

	//判定射线是否射中物体
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 dir, float distance, LayerMask layer)
	{
		RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, layer);

		Debug.DrawLine(origin, origin + dir * distance, Color.green);

		if (hit)
			Debug.DrawLine(origin, origin + dir * hit.distance, Color.red);

		return hit;
	}

	#region 着地事件

	//落地和离地事件
	public VoidEvent onLand;
	public VoidEvent onLeaveGround;

	bool lastIsOnGround;

	void UpdateLandAndLeaveEvent()
	{
		if(isOnGround != lastIsOnGround)
		{
			if(isOnGround)
			{
				onLand?.Invoke();
			}
			else
			{
				onLeaveGround?.Invoke();
			}
		}

		lastIsOnGround = isOnGround;
	}

	#endregion

	private void OnDrawGizmos()
	{
		//Gizmos.DrawWireCube((Vector2)boxCollider.transform.position + boxCollider.offset, boxCollider.transform.localScale * boxCollider.size);
	}
}
