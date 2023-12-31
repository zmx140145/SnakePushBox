﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[AddComponentMenu("UI/ToJ Effects/Character Spacing", 7)]
[RequireComponent(typeof(Text))]
public class CharacterSpacing : BaseMeshEffect
{
	private const string REGEX_TAGS = @"<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

	[SerializeField]
	private float m_Offset = 0f;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected CharacterSpacing () { }

	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		offset = m_Offset;
		base.OnValidate();
	}
	#endif

	public float offset
	{
		get { return m_Offset; }
		set
		{
			if (m_Offset == value)
				return;
			m_Offset = value;
			if (graphic != null)
				graphic.SetVerticesDirty();
		}
	}

	public override void ModifyMesh (VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}

		vh.GetUIVertexStream(m_Verts);

		Text textComponent = GetComponent<Text>();
		List<string> lines = new List<string>();
		for (int i = 0; i < textComponent.cachedTextGenerator.lineCount; i++)
		{
			int startIndex = textComponent.cachedTextGenerator.lines[i].startCharIdx;
			int endIndex = (i < textComponent.cachedTextGenerator.lineCount - 1) ? textComponent.cachedTextGenerator.lines[i + 1].startCharIdx : textComponent.text.Length;
			lines.Add(textComponent.text.Substring(startIndex, endIndex - startIndex));
		}

		float charOffset = offset * (float)textComponent.fontSize / 100f;
		float alignmentFactor = 0;

		IEnumerator matchedTagCollection = null;
		Match currentMatchedTag = null;

		if ((textComponent.alignment == TextAnchor.LowerLeft) || (textComponent.alignment == TextAnchor.MiddleLeft) || (textComponent.alignment == TextAnchor.UpperLeft))
		{
			alignmentFactor = 0f;
		}
		else if ((textComponent.alignment == TextAnchor.LowerCenter) || (textComponent.alignment == TextAnchor.MiddleCenter) || (textComponent.alignment == TextAnchor.UpperCenter))
		{
			alignmentFactor = 0.5f;
		}
		else if ((textComponent.alignment == TextAnchor.LowerRight) || (textComponent.alignment == TextAnchor.MiddleRight) || (textComponent.alignment == TextAnchor.UpperRight))
		{
			alignmentFactor = 1f;
		}

		bool hasToContinue = true;
		int charIndex = 0;
		for (int lineIndex = 0; (lineIndex < lines.Count - 0) && (hasToContinue == true); lineIndex++)
		{
			string line = lines[lineIndex];
			int lineLength = line.Length;

			if (lineLength > textComponent.cachedTextGenerator.characterCountVisible - charIndex)
			{
				lineLength = textComponent.cachedTextGenerator.characterCountVisible - charIndex;
				line = line.Substring(0, lineLength) + " ";
				lineLength++;
			}

			if (textComponent.supportRichText)
			{
				matchedTagCollection = GetRegexMatchedTags(line, out lineLength).GetEnumerator();
				currentMatchedTag = null;
				if (matchedTagCollection.MoveNext())
				{
					currentMatchedTag = (Match)matchedTagCollection.Current;
				}
			}

			bool lineEndsWithEmptyChar = (lines[lineIndex].Length > 0) && ((lines[lineIndex][lines[lineIndex].Length - 1] == ' ') || (lines[lineIndex][lines[lineIndex].Length - 1] == '\n'));

			float alignmentOffset = -(lineLength - 1 - (lineEndsWithEmptyChar ? 1 : 0)) * charOffset * alignmentFactor;
			float visibleCharInLineIndex = 0;
			for (int charInLineIndex = 0; (charInLineIndex < line.Length) && (hasToContinue == true); charInLineIndex++)
			{
				if (textComponent.supportRichText)
				{
					if (currentMatchedTag != null && currentMatchedTag.Index == charInLineIndex)
					{
						charInLineIndex += currentMatchedTag.Length - 1;
						charIndex += currentMatchedTag.Length - 1;

						visibleCharInLineIndex--;

						currentMatchedTag = null;
						if (matchedTagCollection.MoveNext())
						{
							currentMatchedTag = (Match)matchedTagCollection.Current;
						}
					}
				}

				if (charIndex * 6 + 5 >= m_Verts.Count)
				{
					hasToContinue = false;
					break;
				}

				for (int i = 0; i < 6; i++)
				{
					UIVertex vert = m_Verts[charIndex * 6 + i];
					vert.position += Vector3.right * (charOffset * visibleCharInLineIndex + alignmentOffset);
					m_Verts[charIndex * 6 + i] = vert;
				}

				charIndex++;
				visibleCharInLineIndex++;
			}
		}

		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}

	private MatchCollection GetRegexMatchedTags(string text, out int lengthWithoutTags)
	{
		MatchCollection matchedTags = Regex.Matches(text, REGEX_TAGS);
		lengthWithoutTags = 0;
		int overallTagLength = 0;

		foreach (Match matchedTag in matchedTags)
		{
			overallTagLength += matchedTag.Length;
		}

		lengthWithoutTags = text.Length - overallTagLength;
		return matchedTags;
	}
}