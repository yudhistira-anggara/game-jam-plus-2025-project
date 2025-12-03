using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;

namespace GameJam
{
	public partial class AudioManager2d : Node2D
	{
		protected Dictionary soundEffectDictionary;
		[export]
		protected Array<SoundEffect> soundEffects;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			foreach (SoundEffect soundEffect in soundEffects)
			{
				soundEffectDictionary[soundEffect.type] = soundEffect;
			}
		}

		public void createLocalAudio(Vector2 location, SoundEffect.SOUND_EFFECT_TYPE type)
		{
			if (!this.soundEffects.ContainsKey(type))
			{
				GD.PushError("Audio Manager failed to find setting for type " + type);
			}

			SoundEffect soundEffect = soundEffectDictionary[type];

			if (! soundEffect.hasOpenLimit())
			{
				GD.PushError("Has no open limit available");
			}

			soundEffect.changeAudioCount(1);

			AudioStreamPlayer2D audio = new();
			AddChild(audio);
			audio.position = location;
			audio.stream = soundEffect.stream;
			audio.volumeDb = (float) soundEffect.volume;
			audio.pitchScale = (float) soundEffect.pitchScale;
			audio.pitchScale += (float) soundEffect.volume;
		}
	}

	public partial class SoundEffect: Resource
	{
		enum SOUND_EFFECT_TYPE
		{

		}

		[export]
		int limit = 5;
		[export]
		SOUND_EFFECT_TYPE type;
		[export]
		AudioStreamMP3 stream;
		[export]
		double volume = 0.0;
		[export]
		double pitchScale = 1.0;
		[export]
		double pitchRandomness = 0.0;

		int audioCount = 0;

		public void changeAudioCount(int amount)
		{
			audioCount = Math.Max(0, audioCount + amount);
		}

		public bool hasOpenLimit()
		{
			return audioCount < limit;
		}

		public void onAudioFinish()
		{
			changeAudioCount(-1);
		}
	}
}