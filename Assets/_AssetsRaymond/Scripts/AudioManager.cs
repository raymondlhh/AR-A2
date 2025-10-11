using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioFile
{
    public string name;
    [Range(0f, 1f)]
    public float volume = 1f;
    public AudioClip audioClip;
    
    public AudioFile(string audioName, float audioVolume = 1f, AudioClip clip = null)
    {
        name = audioName;
        volume = audioVolume;
        audioClip = clip;
    }
}

public class AudioManager : MonoBehaviour
{
    [Header("Background Music Files")]
    [SerializeField] private List<AudioFile> backgroundMusicFiles = new List<AudioFile>();
    
    [Header("Narration Files")]
    [SerializeField] private List<AudioFile> narrationFiles = new List<AudioFile>();
    
    [Header("Sound Effects Files")]
    [SerializeField] private List<AudioFile> soundEffectsFiles = new List<AudioFile>();
    
    [Header("Voice Effects Files")]
    [SerializeField] private List<AudioFile> voiceEffectsFiles = new List<AudioFile>();
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource narrationSource;
    [SerializeField] private AudioSource soundEffectsSource;
    [SerializeField] private AudioSource voiceEffectsSource;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize audio sources if not assigned
        InitializeAudioSources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void InitializeAudioSources()
    {
        // Get or create audio sources for each category
        if (backgroundMusicSource == null)
            backgroundMusicSource = GetOrCreateAudioSource("BackgroundMusic");
        if (narrationSource == null)
            narrationSource = GetOrCreateAudioSource("Narration");
        if (soundEffectsSource == null)
            soundEffectsSource = GetOrCreateAudioSource("SoundEffects");
        if (voiceEffectsSource == null)
            voiceEffectsSource = GetOrCreateAudioSource("VoiceEffects");
    }
    
    private AudioSource GetOrCreateAudioSource(string sourceName)
    {
        // Try to find existing audio source in children
        Transform child = transform.Find(sourceName);
        if (child != null)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source == null)
                source = child.gameObject.AddComponent<AudioSource>();
            return source;
        }
        
        // Create new GameObject with AudioSource if not found
        GameObject audioObject = new GameObject(sourceName);
        audioObject.transform.SetParent(transform);
        return audioObject.AddComponent<AudioSource>();
    }
    
    // Helper method to get the appropriate list based on category
    private List<AudioFile> GetAudioFileList(AudioSourceType category)
    {
        switch (category)
        {
            case AudioSourceType.BackgroundMusic:
                return backgroundMusicFiles;
            case AudioSourceType.Narration:
                return narrationFiles;
            case AudioSourceType.SoundEffects:
                return soundEffectsFiles;
            case AudioSourceType.VoiceEffects:
                return voiceEffectsFiles;
            default:
                return soundEffectsFiles;
        }
    }
    
    // Public methods to manage audio files by category
    public void AddAudioFile(string name, AudioSourceType category, float volume = 1f, AudioClip clip = null)
    {
        List<AudioFile> targetList = GetAudioFileList(category);
        targetList.Add(new AudioFile(name, volume, clip));
    }
    
    public void RemoveAudioFile(string name, AudioSourceType category)
    {
        List<AudioFile> targetList = GetAudioFileList(category);
        targetList.RemoveAll(audio => audio.name == name);
    }
    
    public AudioFile GetAudioFile(string name, AudioSourceType category)
    {
        List<AudioFile> targetList = GetAudioFileList(category);
        return targetList.Find(audio => audio.name == name);
    }
    
    public void PlayAudio(string name, AudioSourceType sourceType = AudioSourceType.SoundEffects)
    {
        AudioFile audioFile = GetAudioFile(name, sourceType);
        if (audioFile != null && audioFile.audioClip != null)
        {
            AudioSource source = GetAudioSourceByType(sourceType);
            if (source != null)
            {
                source.clip = audioFile.audioClip;
                source.volume = audioFile.volume;
                source.Play();
            }
        }
    }
    
    // Category-specific management methods
    public void AddBackgroundMusic(string name, float volume = 1f, AudioClip clip = null)
    {
        AddAudioFile(name, AudioSourceType.BackgroundMusic, volume, clip);
    }
    
    public void AddNarration(string name, float volume = 1f, AudioClip clip = null)
    {
        AddAudioFile(name, AudioSourceType.Narration, volume, clip);
    }
    
    public void AddSoundEffect(string name, float volume = 1f, AudioClip clip = null)
    {
        AddAudioFile(name, AudioSourceType.SoundEffects, volume, clip);
    }
    
    public void AddVoiceEffect(string name, float volume = 1f, AudioClip clip = null)
    {
        AddAudioFile(name, AudioSourceType.VoiceEffects, volume, clip);
    }
    
    // Get all audio files from a specific category
    public List<AudioFile> GetAudioFilesByCategory(AudioSourceType category)
    {
        return new List<AudioFile>(GetAudioFileList(category));
    }
    
    // Get all audio files from all categories
    public Dictionary<AudioSourceType, List<AudioFile>> GetAllAudioFilesByCategory()
    {
        return new Dictionary<AudioSourceType, List<AudioFile>>
        {
            { AudioSourceType.BackgroundMusic, new List<AudioFile>(backgroundMusicFiles) },
            { AudioSourceType.Narration, new List<AudioFile>(narrationFiles) },
            { AudioSourceType.SoundEffects, new List<AudioFile>(soundEffectsFiles) },
            { AudioSourceType.VoiceEffects, new List<AudioFile>(voiceEffectsFiles) }
        };
    }
    
    private AudioSource GetAudioSourceByType(AudioSourceType type)
    {
        switch (type)
        {
            case AudioSourceType.BackgroundMusic:
                return backgroundMusicSource;
            case AudioSourceType.Narration:
                return narrationSource;
            case AudioSourceType.SoundEffects:
                return soundEffectsSource;
            case AudioSourceType.VoiceEffects:
                return voiceEffectsSource;
            default:
                return soundEffectsSource;
        }
    }
    
}

public enum AudioSourceType
{
    BackgroundMusic,
    Narration,
    SoundEffects,
    VoiceEffects
}
