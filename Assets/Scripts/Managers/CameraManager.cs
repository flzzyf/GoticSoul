using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour{
	//镜头跟随目标
	public Transform followedTarget;
	//偏移
	public Vector3 followOffset;
	//跟随平滑度
	public float followSmooth = .1f;

	Camera cam;

	private void Awake() {
		cam = Camera.main;
	}

	private void Start() {
		MoveToTarget();
	}

	private void FixedUpdate() {
		MoveToTarget();
	}

	void MoveToTarget() {
		if (followedTarget != null) {
			cam.transform.position = Vector3.Lerp(cam.transform.position, followedTarget.position + followOffset, followSmooth);
		}
	}
}
