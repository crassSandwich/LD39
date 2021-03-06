﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

	public Vector3 DefaultRotation; //Euler angle rotation of gun when nothing is in the crosshair /(ie, aiming to the sky)
	public float RotationSpeed; //how quickly the gun should rotate toward the object hit by the crosshair
	public float ShotDelay;
	public AudioClip[] Sounds;

	private Camera cam;
	private BulletController bullet;
	private Transform model; //transform of the model of the current gun. this is where bullets are shot from
	private Vector3 crosshairLocation = new Vector3(0.5f, 0.5f); //normalized position in viewport space where the crosshair is. in viewport space, the bottom left is (0,0) and the top right is (1,1). should usually (if not always) be in the middle, or (.5,.5)
	private AudioSource aus;
	private float shotTime = 0.0f;

	void Start() {
		cam = (Camera) GetComponentInChildren(typeof(Camera));
		model = transform.Find("Model");
		bullet = ((GameObject) Resources.Load("Bullet")).GetComponent<BulletController>();
		aus = transform.parent.GetComponent<AudioSource>();
	}

	void Update() {
		rotateGun(hitPoint());
		shotTime -= Time.deltaTime;
		if (shotTime <= 0.0f && Input.GetButtonDown("Fire1")) {
			aus.PlayOneShot(RandHelp.Choose(Sounds));
			shotTime = ShotDelay;
			shoot();
		}
	}

	//point in world space hit by crosshair
	//returns null if nothing was hit
	private Vector3? hitPoint() {
		RaycastHit hit;
		int mask = ~(1 << 8) & ~(1 << 9); //ignore any casts with player or bullets
		Ray ray = cam.ViewportPointToRay(crosshairLocation);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
			return hit.point;
		else
			return null;
	}

	//face the gun toward point
	//if the point is null, face toward the default instead
	private void rotateGun(Vector3? point) {
		Quaternion rot;
		if (point != null) {
			rot = Quaternion.LookRotation((Vector3) point - model.position);
			model.rotation = Quaternion.RotateTowards(model.rotation, rot, RotationSpeed * Time.deltaTime);
		}
		else {
			rot = Quaternion.Euler(DefaultRotation);
			model.localRotation = Quaternion.RotateTowards(model.localRotation, rot, RotationSpeed * Time.deltaTime);
		}
	}

	//spawn the projectile toward the crosshairs
	private void shoot() {
		Vector3? target = hitPoint();
		BulletController bc = Instantiate(bullet, model.position, model.rotation);//.GetComponent<BulletController>();
		if (target != null)
			bc.Initialize(((Vector3) target - model.position).normalized);
		else
			bc.Initialize(model.forward.normalized);
	}

}
