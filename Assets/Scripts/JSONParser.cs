using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
public static class JSONParser
{
    public static List<Vector2JSON[]> GetBonesList(string array)
    {
        string[] singleArrays = GetSingleDimensionArrays(array);
        string[] vectors;
        Vector2JSON[] vectorJSONs;
        List<Vector2JSON[]> result = new List<Vector2JSON[]>();
        foreach(string element in singleArrays)
        {
            TryParseRegex(out vectors, element, @"\{.*?\}");
            vectorJSONs = new Vector2JSON[vectors.Length];
            for(int i = 0; i<vectors.Length; i++)
            {
                vectorJSONs[i] = JsonUtility.FromJson<Vector2JSON>(vectors[i]);
            }
            result.Add(vectorJSONs);
        }
        return result;
    }
    private static string[] GetSingleDimensionArrays(string array)
    {
        string[] parsed;
        if(!TryParseRegex(out parsed, array, @"\[.*?\]"))
        {
            Debug.Log("JSON PARSER: Couldn't locate sub arrays in: " + array);
            return parsed;
        }
        //Isn't necessary but i'd like to keep everything in the same format
        for(int i = 0; i < parsed.Length; i++)
        {
            parsed[i] = parsed[i].Replace("[[", "[");
            parsed[i] = parsed[i].Replace("]]", "]");
        }
        return parsed;
    }

    private static bool TryParseRegex(out string[] result, string input, string regexString)
    {
        Regex regex = new Regex(regexString);
        MatchCollection matches = regex.Matches(input);
        result = new string[matches.Count];
        if (matches.Count == 0)
            return false;
        for (int i = 0; i < matches.Count; i++)
        {
            result[i] = matches[i].Value;
        }
        return true;
    }
    public static string[] GetBoneMessage(ref string buffer)
    {
        return GetMessage(ref buffer, @"\[\[.*?\]\]");
    }
    public static string[] GetMessage(ref string buffer, string regex)
    {
        string[] result;
        if (!TryParseRegex(out result, buffer, regex))
        {
            Debug.Log("Can't get message by template: " + regex);
        }
        return result;
    }
}

