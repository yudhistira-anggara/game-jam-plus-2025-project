using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameJam
{
    public partial class AudioManager : Node
    {
        public static AudioManager Instance { get; private set; }

        private AudioStreamPlayer2D _musicPlayer;
        private AudioStreamPlayer2D _audioPlayer;

        private Dictionary<string, AudioStream> _loadedAudio = [];
        private AudioStream _currentStream { get; set; }

        public float GlobalVolume { get; set; } = 0;
        private float MusicVolumeReal { get; set; } = -5;
        public float MusicVolume { get; set; } = -5;
        private float SFXVolumeReal { get; set; } = -5;
        public float SFXVolume { get; set; } = -5;

        public override void _Ready()
        {
            Instance = this;

            _musicPlayer = new AudioStreamPlayer2D();
            _audioPlayer = new AudioStreamPlayer2D();

            _musicPlayer.VolumeDb = MusicVolumeReal;
            _musicPlayer.Autoplay = true;

            _audioPlayer.VolumeDb = SFXVolumeReal;
            _audioPlayer.Autoplay = true;

            AddChild(_musicPlayer);
            AddChild(_audioPlayer);
        }

        public void PlayMusic(AudioStream soundEffect)
        {
            //
        }

        public List<string> LoadFromDirectory(string path)
        {
            List<string> allPaths = [];
            var dir = DirAccess.Open(path);

            foreach (string file in dir.GetFiles())
            {
                string fullPath = $"{path}/{file}";
                if (Utils.IsValidAudioExtension(fullPath))
                {
                    allPaths.Add(fullPath);
                }
            }

            foreach (string sub in dir.GetDirectories())
            {
                string subPath = $"{path}/{sub}";
                if (Utils.IsValidAudioExtension(subPath))
                {
                    allPaths.AddRange(LoadFromDirectory(subPath));
                }
            }

            return allPaths;
        }

        public void LoadSFX(string path)
        {
            if (_loadedAudio.ContainsKey(path))
                return;

            AudioStream audio = ResourceLoader.Load<AudioStream>(path);
            if (audio != null)
                _loadedAudio[path] = audio;
            else
                GD.PrintErr();
        }

        public void LoadAndPlaySFX(string path)
        {
            AudioStream audio;
            if (_loadedAudio.TryGetValue(path, out AudioStream value))
                audio = value;
            else
            {
                audio = ResourceLoader.Load<AudioStream>(path);

                if (audio != null)
                    _loadedAudio[path] = audio;
                else
                    GD.PrintErr();
            }

            PlaySFX(audio);
        }

        public void PlaySFX(string name)
        {
            if (_loadedAudio.ContainsKey(name))
            {
                _audioPlayer.Stream = _loadedAudio[name];
                _audioPlayer.Play();
            }
            else
                LoadAndPlaySFX(name);
        }

        public void PlaySFX(AudioStream soundEffect)
        {
            _audioPlayer.Stream = soundEffect;
            _audioPlayer.Play();
        }

        public void PauseResumeAudio(bool resume = true)
        {
            _audioPlayer.Stop();
            _musicPlayer.Stop();
        }

        public void SetGlobalVolume(float volume)
        {
            GlobalVolume += volume;
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolumeReal = GlobalVolume + SFXVolume;
            SFXVolumeReal += volume;
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolumeReal = GlobalVolume + MusicVolume;
            SFXVolumeReal += volume;
        }
    }
}