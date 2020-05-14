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

    public interface ISkeleton
    {
        Vector4[] Bones
        {
            get;
            set;
        }
        OutputFormat Format
        {
            get;
            set;
        }
        void ParseBonesToFormat();
    }

    static class SkeletonMapper
    {
        private static Dictionary<OutputFormat, string[]> BoneMap = new Dictionary<OutputFormat, string[]>();
        static string[] BODY_25_Map = new string[]
        {
            "Nose",
            "Neck",
            "RShoulder",
            "RElbow",
            "RWrist",
            "LShoulder",
            "LElbow",
            "LWrist",
            "MidHip",
            "RHip",
            "RKnee",
            "RAnkle",
            "LHip",
            "LKnee",
            "LAnkle",
            "REye",
            "LEye",
            "REar",
            "LEar",
            "LBigToe",
            "LSmallToe",
            "LHeel",
            "RBigToe",
            "RSmallToe",
            "RHeel",
            "Background"
        };
    }

    // Base class for Skeleton objects
    public class Skeleton
    {
        private OutputFormat format = OutputFormat.BODY_25;
        public Skeleton(OutputFormat format)
        {
            this.format = format;
        }
    }
}
