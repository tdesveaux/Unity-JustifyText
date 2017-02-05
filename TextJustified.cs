using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

[ExecuteInEditMode]
public class TextJustified : Text
{

    [SerializeField]
    public bool m_Justified = false;

    private string m_JustifiedText = string.Empty;
    private string m_lastJustifiedText = string.Empty;

    /// <summary>
    /// Text that's being displayed by the Text.
    /// </summary>

    public override string text
    {
        get
        {
            if (m_Justified)
            {
                if (m_lastJustifiedText != m_Text)
                {
                    m_lastJustifiedText = m_Text;
                    StartJustification();
                }

                while (justifying);

                return (m_JustifiedText);
            }
            else
                return m_Text;
        }
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                if (String.IsNullOrEmpty(m_Text))
                {
                    return;
                }
                m_Text = "";
                SetVerticesDirty();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    TextGenerationSettings settings;
    private bool justifying = false;

    public void StartJustification()
    {

        justifying = true;
        Vector2 extents = GetComponent<RectTransform>().rect.size;

        settings = GetGenerationSettings(extents);

        StartCoroutine("Justify");
    }

    IEnumerator Justify()
    {

        string line;

        cachedTextGenerator.Populate(m_Text, settings);

        UILineInfo[] lines = cachedTextGenerator.GetLinesArray();

        string newText = "";
        for (int i = 0; i < lines.Length; i++)
        {
            if (i < lines.Length - 1)
            {
                line = m_Text.Substring(lines[i].startCharIdx, lines[i + 1].startCharIdx - lines[i].startCharIdx);

                if (line[line.Length - 1] == '\n')
                    newText += line;
                else
                    newText += FillLine(line);
            }
            else
            {
                line = m_Text.Substring(lines[i].startCharIdx);
                newText += line;
            }

        }

        m_JustifiedText = newText;

        justifying = false;
        yield return new WaitForEndOfFrame();
    }

    string  FillLine(string line)
    {
        string result = "";
        string trial = "";
        List<string> words = new List<string>(line.Split(' '));

        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].Length <= 0)
            {
                words.RemoveAt(i);
                i--;
            }
        }

        if (words.Count <= 1)
            return (line);

        int spaceCount = 0;

        cachedTextGenerator.Populate(line, settings);
        while (cachedTextGenerator.lineCount == 1)
        {
            ++spaceCount;
            result = trial;
            trial = GenerateSentenceWithSpace(words, spaceCount) + " ";
            cachedTextGenerator.Populate(trial, settings);
        }
        spaceCount--;

        int additionalSpaces = 0;

        trial = result;
        cachedTextGenerator.Populate(line, settings);
        while (cachedTextGenerator.lineCount == 1)
        {
            ++additionalSpaces;
            result = trial;
            trial = GenerateSentenceWithExtraSpaces(words, spaceCount, additionalSpaces) + " ";
            cachedTextGenerator.Populate(trial, settings);
        }

        return (result);
    }

    string  GenerateSentenceWithSpace(List<string> words, int spaceCount = 1)
    {
        string result = "";

        for (int i = 0; i < words.Count - 1; i++)
        {
            result += words[i];

            for (int s = 0; s < spaceCount; s++)
                result += " ";
        }
        result += words[words.Count - 1];

        return (result);
    }

    string GenerateSentenceWithExtraSpaces(List<string> words, int spaceCount, int extraSpaces)
    {
        string result = "";

        if (extraSpaces < 2)
        {
            result = GenerateSentenceWithSpace(words, spaceCount);
            if (extraSpaces == 0)
                return (result);
            return (result.Insert(result.IndexOf(' '), " "));
        }

        float addIncr =  (float)(words.Count - 1) / (float)extraSpaces;
        float addIndex = addIncr;
        int added = 0;

        for (int i = 0; i < words.Count - 1; i++)
        {
            result += words[i];

            for (int s = 0; s < spaceCount; s++)
                result += " ";

            if ((i + 1) == Mathf.RoundToInt(addIndex))
            {
                result += " ";
                addIndex += addIncr;
                added++;
            }
        }
        result += words[words.Count - 1];

        return (result);
    }
}
