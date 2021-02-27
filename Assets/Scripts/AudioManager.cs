using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioMixerGroup Mixer;
    private static bool isMuted = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Mute()
    {
        if (isMuted)
        {
            instance.Mixer.audioMixer.SetFloat("MasterVolume", 0);
        }
        else
        {
            instance.Mixer.audioMixer.SetFloat("MasterVolume", -80);
        }
        isMuted = !isMuted;
    }
}
