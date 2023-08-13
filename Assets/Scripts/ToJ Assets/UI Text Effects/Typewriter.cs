using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(LimitVisibleCharacters))]
public class Typewriter : MonoBehaviour
{
	public float delayBetweenSymbols = 0.1f;
	public AudioClip[] typeSoundEffects;
	public AudioSource audioSourceForTypeEffect;

	private float _timer = 0;
	private LimitVisibleCharacters _limitVisibleCharactersComponent = null;
	private Text _textComponent = null;
	public bool InPlay = false;
	public Action PrintFinish;
	void Start ()
	{

	}

	void Update ()
	{
		audioSourceForTypeEffect.volume = SoundManager.Instance.CurSoundVolume*0.25f;
	}

	private void OnEnable ()
	{
		if (_limitVisibleCharactersComponent == null)
		{
			_limitVisibleCharactersComponent = GetComponent<LimitVisibleCharacters>();
		}
		if (_textComponent == null)
		{
			_textComponent = GetComponent<Text>();
		}

		StopCoroutine("PlayTypewriter");
		StartCoroutine("PlayTypewriter");
	}

	private void OnDisable ()
	{
		StopCoroutine("PlayTypewriter");
	}

	private IEnumerator PlayTypewriter()
	{
		InPlay = true;
		_timer = 0;
		_limitVisibleCharactersComponent.visibleCharacterCount = 0;
		yield return null;
		while (_limitVisibleCharactersComponent.visibleCharacterCount <= _textComponent.text.Length)
		{
			_timer += Time.deltaTime;
			if ((typeSoundEffects != null) && (typeSoundEffects.Length > 0) && (audioSourceForTypeEffect != null) && (_limitVisibleCharactersComponent.visibleCharacterCount != (int)(_timer / delayBetweenSymbols)))
			{
				var it = typeSoundEffects[UnityEngine.Random.Range(0, typeSoundEffects.Length)];
				if (it != null)
				{
					audioSourceForTypeEffect.PlayOneShot(it);
				}
				
			}
			_limitVisibleCharactersComponent.visibleCharacterCount = (int)(_timer / delayBetweenSymbols);
			yield return null;
		}
		_limitVisibleCharactersComponent.visibleCharacterCount = _textComponent.text.Length;
		PrintFinish?.Invoke();
		InPlay = false;
	}
}
