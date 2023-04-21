
using UnityEngine;
//using UnityEngine.Audio;

public class BackgroundMusic : MonoBehaviour
{
    //public AudioMixer audioMixer;
    public AudioClip music1;
    public AudioClip music2;
    private AudioSource _audioSource;
    private bool _isPlayingMusic1 = true;
    private float _elapsedTime = 0.1f;
    private const float MusicDuration1 = 300.0f;  // 背景音乐1的时长为5分钟（300秒）
    private const float MusicDuration2 = 300.0f;  // 背景音乐2的时长为5分钟（300秒）

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = music1;
        _audioSource.Play();
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= MusicDuration1 && _isPlayingMusic1)
        {
            // 切换背景音乐为音乐2
            _audioSource.clip = music2;
            _audioSource.Play();
            _isPlayingMusic1 = false;
        }
        else if (_elapsedTime >= MusicDuration1 + MusicDuration2 && !_isPlayingMusic1)
        {
            // 切换背景音乐为音乐1
            _audioSource.clip = music1;
            _audioSource.Play();
            _isPlayingMusic1 = true;
            _elapsedTime = 0.0f;
        }
    }
}
