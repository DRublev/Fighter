using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BoneManager : MonoBehaviour {

	private Animator animator;
	void Start() {
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {

	}

	void OnAnimatorIK() {

	}

	public HumanBodyBones Bone;
	[Range(-1, 1)]
	public float LeftArmAmt = 1;

	[Range(0, 2)]
	public int TargetAxis;

	private void OnValidate() { SetFromTpose(); }

	public void SetFromTpose() {
		var bone = animator.GetBoneTransform(Bone);

		var angleMin = 50;
		var angleMax = 50;

		var ang = Mathf.LerpAngle(angleMin, angleMax, (LeftArmAmt + 1) / 2f);
		Vector3 f = new Vector3();
		f[TargetAxis] = ang;

		bone.localRotation = Quaternion.Euler(f) * bone.localRotation;
	}
}