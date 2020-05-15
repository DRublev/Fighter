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
    public class BonesBinder : MonoBehaviour
    {
        private readonly int BONE_WEIGHT = 1; // We may change it to custom later by making it public

        private SkinnedMeshRenderer meshRenderer;
        private Animation animation;

        private List<Vector2> bonePoses;
        private Mesh mesh;
        private Transform[] bones;
        private Matrix4x4[] bindPoses;

        public BonesBinder(List<Vector2> bonePoses)
        {
            this.bonePoses = bonePoses;
            this.mesh = new Mesh();
            this.bones = new Transform[bonePoses.Count];
            this.bindPoses = new Matrix4x4[bonePoses.Count];
            CalcBonesWeights();
        }

        private void CalcBonesWeights()
        {
            BoneWeight[] bonesWeights = new BoneWeight[this.bones.Length];
            for (int i = 0; i < bonesWeights.Length; i++)
            {
                // I don't exactly know why boneIndex0, just trying
                bonesWeights[i].boneIndex0 = i;
                bonesWeights[i].weight0 = this.BONE_WEIGHT;
            }
            this.mesh.boneWeights = bonesWeights;
        }

        void Start()
        {
            meshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
            animation = gameObject.AddComponent<Animation>();
        }
    }
}
