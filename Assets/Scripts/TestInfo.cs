using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestInfo
{
    public int Answers;
    public int Errors;
    public int Questions;
    public int Type;
    public int Level;
    public string Topics;
    public int WordCount;
    public string StartTime;
    public TestInfo(DataScript data)
    {
        Answers = 0;
        Errors = 0;
        Questions = 2 * data.WordCount;
        Type = data.TestType;
        Level = data.Level;
        Topics = data.TestTopicsToString();
        WordCount = data.WordCount;
        StartTime = System.DateTime.Now.ToString("yy.MM.dd HH:mm");
    }
    public TestInfo(string info)
    {
        var s = info.Split('|');
        if (s.Length != 8)
            return;
        Answers = int.Parse(s[0]);
        Errors = int.Parse(s[1]);
        Questions = int.Parse(s[2]);
        Type = int.Parse(s[3]);
        Level = int.Parse(s[4]);
        Topics = s[5];
        WordCount = int.Parse(s[6]);
        StartTime = s[7];
    }

    public override string ToString() =>
        $"{Answers}|{Errors}|{Questions}|{Type}|{Level}|{Topics}|{WordCount}|{StartTime}";
    public float Rating
    {
        get => (Answers > Errors) ?
        Mathf.Pow((float)(Answers - Errors) / Questions, 2) : 0;
    }
    public int Mark
    {
        get
        {
            float r = Rating;
            if (r < 0.6f)
                return 2;
            if (r < 0.7f)
                return 3;
            if (r < 0.85f)
                return 4;
            return 5;
        }
    }
}