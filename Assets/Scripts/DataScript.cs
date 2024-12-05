using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[System.Serializable] public struct WordInfo
{
    public string En;
    public string Au;
    public string Ru;
    public WordInfo(string[] w)
    {
        En = w[0];
        Au = w[1];
        Ru = w[2];
    }
}

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class DataScript : ScriptableObject
{
    [SerializeField] int level;
    public int CurrentTopicIndex;
    [SerializeField] List<string> data;
    [SerializeField] List<string> topics;
    [SerializeField] List<WordInfo> words;
    [SerializeField] int[] itemIndex = new int[8];
    [SerializeField] float[] scrollbarValue = new float[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
    public int TestType;
    [SerializeField] string[] testTopics = new string[4] { "1", "1", "1", "1" };
    [SerializeField] List<TestInfo> results = new List<TestInfo>();
    List<int> remainInd = new List<int>();
    TestInfo test;
    int predInd;
    int ppredInd;
    System.Random r = new System.Random();
    List<int> testInd = new List<int>();
    public bool OptAudioEnRu;
    public int OptTopicName;
    public int OptVolume = 10;
    public Button mainButton;

    public int S1ItemIndex
    {
        get => itemIndex[level];
        set => itemIndex[level] = value;
    }
    public float S1ScrollbarValue
    {
        get => scrollbarValue[level];
        set => scrollbarValue[level] = value;
    }

    public int S2ItemIndex
    {
        get => itemIndex[level + 4];
        set => itemIndex[level + 4] = value;
    }
    public float S2ScrollbarValue
    {
        get => scrollbarValue[level + 4];
        set => scrollbarValue[level + 4] = value;
    }

    void Awake()
    {
        data = new List<string>(Resources.LoadAll<TextAsset>("Data")
        .Select(e => e.name));
        LoadPrefabs();
        LoadPrefs();
        SetLevel(level);
    }

    public int ResultCount { get => results.Count; }
    public string TestTypeToString(int testType)
    {
        switch (testType)
        {
            case 0:
                return "En-Ru";
            case 1:
                return "Ru-En";
            default:
                return "Au-Ru";
        }
    }

    public string CompleteResultInfo(int i)
    {
        var r = results[i];
        return $@"Начало тестирования:
            {r.StartTime}
            Вид тестирования: {TestTypeToString(r.Type)}
            Уровень и темы: {r.Level + 1}:{r.Topics}
            Количество слов: {r.WordCount}
            Вопросов в тесте: {r.Questions}
            Правильных ответов: {r.Answers}
            Ошибочных ответов: {r.Errors}
            Рейтинг и оценка: {r.Rating * 100:f1} [{r.Mark}]";
    }

    public string Result(int i)
    {
        var r = results[i];
        return $"{r.StartTime} {r.Level + 1}:{r.Topics} "
        + $"{TestTypeToString(r.Type)} {r.Rating * 100:f0}\u00A0[{r.Mark}]";
    }

    public int OptMainButtonFontSize
    {
        get => mainButton.GetComponentInChildren<Text>().fontSize;
        set => mainButton.GetComponentInChildren<Text>().fontSize = value;
    }

    public int GetHeight(Component comp) =>
        (int)comp.GetComponent<RectTransform>().sizeDelta.y;

    public void SetHeight(Component comp, int value)
    {
        RectTransform rt = comp.GetComponent<RectTransform>();
        Vector2 sd = rt.sizeDelta;
        sd.y = value;
        rt.sizeDelta = sd;
    }

    public int OptMainButtonHeight
    {
        get => GetHeight(mainButton);
        set => SetHeight(mainButton, value);
    }

    public int Level { get => level; }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
        topics = new List<string>(data
        .Where(e => e.StartsWith((newLevel + 1) + ".")));
    }

    public int TopicCount { get => topics.Count; }

    public string Topic(int i)
    {
        string s = topics[i].Remove(0, 2);
        if (OptTopicName == 0 || level == 3)
            return s;
        var m = Regex.Match(s, @"(\d\d\.)(.*) \((.*)\)");
        return m.Groups[1].Value + m.Groups[OptTopicName + 1].Value;
    }

    void Reset() => Awake();

    public void GetWords(int topicIndex, bool reset = true)
    {
        if (reset)
            words.Clear();
        string text = Resources.Load<TextAsset>("Data/"
        + topics[topicIndex]).text;
        foreach (var str in text.Split('\n'))
        {
            string[] w = str.Split('#');
            if (w.Length == 3)
                words.Add(new WordInfo(w));
        }
    }
    public int WordCount { get => words.Count; }
    public string Word(int i) => $"{words[i].En}  \u2013  {words[i].Ru}";
    public void PlayAudio(int wordIndex)
    {
        var audio = Camera.main.GetComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>("Sounds/" + words[wordIndex].Au);
        audio.volume = OptVolume / 10.0f;
        audio.Play();
    }

    public void InitTest()
    {
        words.Clear();
        foreach (int ind in TestTopics)
            GetWords(ind, false);
        remainInd.Clear();
        remainInd.AddRange(Enumerable.Range(0, words.Count));
        remainInd.AddRange(Enumerable.Range(0, words.Count));
        test = new TestInfo(this);
        predInd = -1;
        ppredInd = -1;
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("TestType", TestType);
        for (int i = 0; i < 8; i++)
        {
            PlayerPrefs.SetInt($"ItemIndex{i}", itemIndex[i]);
            PlayerPrefs.SetFloat($"ScrollbarValue{i}", scrollbarValue[i]);
        }
        for (int i = 0; i < 4; i++)
            PlayerPrefs.SetString($"TestTopics{i}", testTopics[i]);
        PlayerPrefs.SetInt("OptAudioEnRu", OptAudioEnRu ? 1 : 0);
        PlayerPrefs.SetInt("OptTopicName", OptTopicName);
        PlayerPrefs.SetInt("OptVolume", OptVolume);
        PlayerPrefs.SetInt("OptMainButtonFontSize", OptMainButtonFontSize);
        PlayerPrefs.SetInt("OptMainButtonHeight", OptMainButtonHeight);
        PlayerPrefs.SetInt("ResultsCount", results.Count);
        for (int i = 0; i < results.Count; i++)
            PlayerPrefs.SetString($"Results{i}", results[i].ToString());
    }
    void LoadPrefs()
    {
        level = PlayerPrefs.GetInt("Level", 0);
        TestType = PlayerPrefs.GetInt("TestType", 0);
        for (int i = 0; i < 8; i++)
        {
            itemIndex[i] = PlayerPrefs.GetInt($"ItemIndex{i}", 0);
            scrollbarValue[i] = PlayerPrefs.GetFloat($"ScrollbarValue{i}", 0);
        }
        for (int i = 0; i < 4; i++)
            testTopics[i] = PlayerPrefs.GetString($"TestTopics{i}", "1");
        OptAudioEnRu = PlayerPrefs.GetInt("OptAudioEnRu", 0) == 1;
        OptTopicName = PlayerPrefs.GetInt("OptTopicName", 0);
        OptVolume = PlayerPrefs.GetInt("OptVolume", 10);
        OptMainButtonFontSize = PlayerPrefs.GetInt("OptMainButtonFontSize", 10);
        OptMainButtonHeight = PlayerPrefs.GetInt("OptMainButtonHeight", 25);
        results.Clear();
        var cnt = PlayerPrefs.GetInt("ResultsCount", 0);
        for (int i = 0; i < cnt; i++)
            results.Add(new TestInfo(PlayerPrefs.GetString($"Results{i}", "")));
    }


    string getTitle() =>
        $"Вопрос {test.Answers + 1} из {test.Questions}\nРейтинг: {test.Rating * 100:f2}";
    float getProgress() => (float)test.Answers / test.Questions;

    public void SetNavigationDown(Selectable from, Selectable to)
    {
        var nav = from.navigation;
        nav.selectOnDown = to;
        from.navigation = nav;
    }

    void LoadPrefabs()
    {
        mainButton = Resources.Load<GameObject>("Prefabs/MainButton")
        .GetComponent<Button>();
    }

    public bool NextQuestion(string[] labels, out string title, out float progress)
    {
        if (test.Answers == test.Questions)
        {
            title = $"Итоговый рейтинг: {test.Rating * 100:f2}\nОценка: {test.Mark}";
            progress = 1;
            return false;
        }
        //(1)
        title = getTitle();
        progress = getProgress();
        //(2)
        int q = -1;
        for (int i = 0; i < 10; i++)
        {
            q = r.Next(remainInd.Count);
            if (remainInd[q] != predInd && remainInd[q] != ppredInd)
                break;
        }
        int qInd = remainInd[q];
        //(3)
        remainInd.RemoveAt(q);
        ppredInd = predInd;
        predInd = qInd;
        //(4)
        testInd.Clear();
        testInd.Add(qInd);
        for (int i = 0; i < 4; i++)
        {
            int ind = r.Next(words.Count);
            while (testInd.Contains(ind))
                ind = r.Next(words.Count);
            testInd.Add(ind);
        }
        testInd.Insert(r.Next(1, 6), qInd);
        //(5)
        if (TestType == 1)
        {
            labels[0] = words[testInd[0]].Ru;
            for (int i = 1; i < 6; i++)
                labels[i] = words[testInd[i]].En;
        }
        else
        {
            labels[0] = TestType == 0 ? words[testInd[0]].En : "[Audio]";
            if (TestType == 2 || OptAudioEnRu)
                PlayAudio(testInd[0]);
            for (int i = 1; i < 6; i++)
                labels[i] = words[testInd[i]].Ru;
        }
        return true;
    }

    public string CheckAnswer(int ansInd, ref string title, ref float progress)
    {
        if (testInd[ansInd] == testInd[0])
        {
            test.Answers++;
            return "";
        }
        test.Errors++;
        test.Questions += 2;
        remainInd.Add(testInd[ansInd]);
        remainInd.Add(testInd[0]);
        title = getTitle();
        progress = getProgress();
        if (TestType == 2)
            PlayAudio(testInd[ansInd]);
        if (TestType == 1)
            return $"{words[testInd[ansInd]].En} \u2013 {words[testInd[ansInd]].Ru}";
        return $"{words[testInd[ansInd]].Ru} \u2013 {words[testInd[ansInd]].En}";
    }

    public SortedSet<int> TestTopics
    {
        get
        {
            if (testTopics[level] == null || testTopics[level] == "")
                return new SortedSet<int>();
            return new SortedSet<int>(testTopics[level]
            .Split(',').Select(e => int.Parse(e) - 1));
        }
        set => testTopics[level] = string.Join(",", value.Select(e => e + 1));
    }

    public void AdditionalTestAction()
    {
        if (TestType != 1)
            PlayAudio(testInd[0]);
    }

    public void SaveResult()
    {
        if (test.Mark > 2)
            results.Add(test);
    }

    public string TestTopicsToString() => testTopics[level];
}
