﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public static Transform Location;

	public float Speed = 5.0f;
	public float Gravity = 10.0f;
	public float DragNum = 0.05f; //number to account for any aspect of drag
	public float[] JumpBursts = { 5.0f, 3.0f, 1.0f }; //each respective value is the nth jump: 0 is single jump, 1 is double jump, etc
	public float AirSpeed = 5.0f; //air strafing speed
	public float HalfHeight = 1.0f; //height from ground to origin
	public float GroundedMargin = 0.1f; //margin of error for ground check
	public float JumpDelay = 0.1f; //how long to wait after jumping before being grounded resets jumpCount (needed because game thinks you are still grounded a frame or two after jumping)

	private Rigidbody rb; //rigidbody of parent, which just translates; no rotation
	private int jumpCount;
	private float jumpTime;
	private Vector2 prevHorizontalSpeed;
	private Vector3 moveDir;

	void Start () {
		Location = transform.parent;
		rb = transform.parent.GetComponent<Rigidbody>();
		jumpCount = 0;
	}
	
	void Update () {
		if (jumpTime > 0)
			jumpTime -= Time.deltaTime;

		if (isGrounded()) {
			jumpCount = 0;
			moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
			moveDir = transform.TransformDirection(moveDir).normalized;
			moveDir *= Speed;
		}
		else {
			moveDir *= DragNum;
			Vector3 strafeDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
			strafeDirection = transform.TransformDirection(strafeDirection).normalized;
			strafeDirection *= AirSpeed;
			moveDir += strafeDirection;
		}

		if (Input.GetButtonDown("Jump") && jumpCount < JumpBursts.Length) {
			if (jumpCount == 0)
				jumpTime = JumpDelay;
			moveDir.y = JumpBursts[jumpCount];
			jumpCount++;
		}
		else
			moveDir.y = rb.velocity.y;

		rb.velocity = moveDir;
	}

	void FixedUpdate() {
		rb.AddForce(Vector3.down * Gravity);
	}

	private bool isGrounded () {
		if (jumpTime > 0)
			return false;
		int layerMask = ~(1 << 8);
		return Physics.Raycast(transform.position, Vector3.down, HalfHeight + GroundedMargin, layerMask);
	}
}
