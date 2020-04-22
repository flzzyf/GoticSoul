using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
	[HideInInspector]
	public BoxCollider2D boxCollider;

	public virtual void Start()
	{
		boxCollider = GetComponent<BoxCollider2D>();

		//更新四个方向的光束源点
		CalculateRaySpacing();
	}

	//边缘厚度，一个很小的值
	public const float skinWidth = .015f;

	//水平和竖直的射线数量
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	//水平和竖直的射线间距
	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	//碰撞层
	public LayerMask collisionMask;

	//四个角落的碰撞源点
	public struct RaycastOrigin
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public RaycastOrigin raycastOrigin;

	//更新四个方向的光束源点
	public void UpdateRaycastOrigins()
	{
		Bounds bounds = boxCollider.bounds;
		//边缘缩进一定的厚（这样能区别地面和左右的物体）
		bounds.Expand(skinWidth * -2);

		raycastOrigin.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigin.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigin.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigin.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	//计算光束间距
	public void CalculateRaySpacing()
	{
		Bounds bounds = boxCollider.bounds;
		//边缘缩进一定的厚度
		bounds.Expand(skinWidth * -2);

		//至少两道光
		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
		//计算水平和垂直方向上光束的间距
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
}
