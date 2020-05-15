using System;
using System.Collections.Generic;

using UnityEngine;

// Output format here: https://github.com/CMU-Perceptual-Computing-Lab/openpose/blob/master/doc/output.md#output-format
namespace Assets.Scripts
{
    public enum OutputFormat
    {
        BODY_25,
        HAND
    }

    /// <summary>
    /// Use this to create skeleton from coordinates and to handle pose changing.
    /// Note that it's not tested yet
    /// </summary>
    /// <param name="bonePoses">List of { x, y } coords. Bones will be created at this coordinates</param>

    /* 
        I'd like to pass one of skeleton types (see Types.cs), but i dont know how to do this
        I tried  smth like BonesBinder<T> where T : IConvertible
        But in this case i cannot take a name from enum (see SetBonesPosition to understand what i mean) 
    */
    public class BonesBinder : MonoBehaviour
    {
        private readonly int BONE_WEIGHT = 1; // We may change it to custom later by making it public

        private SkinnedMeshRenderer meshRenderer;
        private Animation animation;

        private List<Vector2> bonePoses;
        private Mesh mesh;
        private Transform[] bones;
        private Matrix4x4[] bindPoses;

        // It's needed to be in separate class
        private AnimationCurve defaultAnimationCurve = new AnimationCurve();

        public BonesBinder(ref List<Vector2> bonePoses)
        {
            this.bonePoses = bonePoses;
            this.mesh = new Mesh();
            this.bones = new Transform[bonePoses.Count];
            this.bindPoses = new Matrix4x4[bonePoses.Count];
            CalcBonesWeights();
            SetBonesPosition();

            defaultAnimationCurve.keys = new Keyframe[]
            {
                new Keyframe(0, 0, 0, 0), new Keyframe(1, 3, 0, 0), new Keyframe(2, 0.0F, 0, 0)
            };
        }

        private void CalcBonesWeights()
        {
            BoneWeight[] bonesWeights = new BoneWeight[bones.Length];
            for (int i = 0; i < bonesWeights.Length; i++)
            {
                // I don't exactly know why boneIndex0, just trying
                bonesWeights[i].boneIndex0 = i;
                bonesWeights[i].weight0 = BONE_WEIGHT;
            }
            mesh.boneWeights = bonesWeights;
        }

        // Create GameObjects for all bones and set their position and rotation. Set mesh's bindposes
        private void SetBonesPosition()
        {
            for (int i = 0; i < bones.Length; i++)
            {
                string boneName = GetBoneNameByIndex(i);
                bones[i] = new GameObject(boneName).transform;
                bones[i].parent = transform;
                bones[i].localRotation = Quaternion.identity;
                bones[i].localPosition = GetBoneLocalPosition();
                // Make matrix relative to the root
                bindPoses[i] = bones[i].worldToLocalMatrix * transform.worldToLocalMatrix;
            }
            mesh.bindposes = bindPoses;

            meshRenderer.bones = bones;
            meshRenderer.sharedMesh = mesh;
        }

        // Here we need to think about bonePoses to local position mechanism
        private Vector3 GetBoneLocalPosition()
        {
            return Vector3.zero;
        }

        private string GetBoneNameByIndex(int index)
        {
            return ((Body25) index).ToString();
        }

        ///<summary>
        /// Use it to add animation for the bone, Initial animation on start, for example
        ///<summary>
        ///<param name="toBone">Bone to animate. It will get bone name and find GameObject with this name</param>
        // I think we need another class (decoratior) to hanlde animation logic
        public void AddAnimationClip(Body25 toBone, Type? type, WrapMode? mode, AnimationCurve? curve)
        {
            AnimationCurve actualCurve = curve != null ? curve : defaultAnimationCurve;
            AnimationClip clip = new AnimationClip();
            clip.SetCurve(toBone.ToString(), type != null ? type : typeof(Transform), "localPosition.y", actualCurve);
            clip.legacy = true;

            clip.wrapMode = mode != null ? (WrapMode) mode : WrapMode.Once;
            animation.AddClip(clip, "clip"); // Should i pass name too?
            animation.Play();
        }

        public void UpdateBonePositions(Vector3[] newVertices, Vector2[] newUV, int[] newTriangles)
        {
            mesh.Clear();

            mesh.vertices = newVertices;
            mesh.uv = newUV;
            mesh.triangles = newTriangles;
        }

        void Start()
        {
            meshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
            animation = gameObject.AddComponent<Animation>();
        }
    }
}
