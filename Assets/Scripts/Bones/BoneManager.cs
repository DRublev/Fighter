using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BoneManager : MonoBehaviour
{
	public Animator animator;
	public HumanBodyBones Bone;
	public Vector3 Position = new Vector3(0, 0, 0);

	[Range(-1, 1)]
	public float LeftArmAmt = 1;

	[Range(0, 2)]
	public int TargetAxis;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnValidate()
	{
		SetFromTpose();
	}

	public void SetFromTpose()
	{
		var bone = animator.GetBoneTransform(Bone);

		var angleMin = 10;
		var angleMax = 50;

		var ang = Mathf.LerpAngle(angleMin, angleMax, (LeftArmAmt + 1) / 2f);
		Vector3 f = new Vector3();
		f[TargetAxis] = ang;

		bone.localRotation = Quaternion.Euler(f) * bone.localRotation;
		bone.localPosition = Position;
	}
}
