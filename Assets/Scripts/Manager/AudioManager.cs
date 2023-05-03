using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct AudioStruct
    {
        public EnumAudios enumAudio;
        public AudioData audioData;
    }

    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public float volume;
    }

    [SerializeField] private List<AudioStruct> m_ListAudio;
    [SerializeField] private List<AudioData> m_ListBackgroundMusic;
    private Dictionary<EnumAudios, AudioData> m_DictAudio;

    private Coroutine m_CoroutineMusicBackground;

    public static AudioManager m_Instance;

    private void Awake()
    {
        m_Instance = this;

        m_DictAudio = new Dictionary<EnumAudios,AudioData>();

        foreach(AudioStruct value in m_ListAudio)
        {
            m_DictAudio.Add(value.enumAudio, value.audioData);
        }
    }

    private void Start()
    {
        GameObject gameObjectAudioInstance = Pool.m_Instance.GetObject(EnumAudios.instance);
        m_CoroutineMusicBackground = StartCoroutine(CoroutineMusicBackground(gameObjectAudioInstance));
    }

    private void OnDisable()
    {
        if(m_CoroutineMusicBackground != null)
        {
            StopCoroutine(m_CoroutineMusicBackground);
        }
    }

    public void PlaySoundAt(Vector3 pos, EnumAudios clip)
    {
        GameObject objectAudioSource = Pool.m_Instance.GetObject(EnumAudios.instance);
        objectAudioSource.transform.position = pos;

        AudioSource audioSource = objectAudioSource.GetComponent<AudioSource>();
        audioSource.clip = m_DictAudio[clip].clip;
        audioSource.volume = m_DictAudio[clip].volume;

        StartCoroutine(CoroutinePlaySoundAt(objectAudioSource));
    }

    public void PlaySoundContinueAt(Vector3 pos, EnumAudios clip)
    {
        GameObject objectAudioSource = Pool.m_Instance.GetObject(EnumAudios.instance);
        objectAudioSource.transform.position = pos;

        AudioSource audioSource = objectAudioSource.GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = m_DictAudio[clip].clip;
        audioSource.volume = m_DictAudio[clip].volume;

        objectAudioSource.SetActive(true);
    }

    private IEnumerator CoroutinePlaySoundAt(GameObject gameObject)
    {
        gameObject.SetActive(true);

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        yield return new WaitForSeconds(audioSource.clip.length);

        gameObject.transform.position = Vector3.zero;
        audioSource.clip = null;
        audioSource.volume = 1;
        audioSource.loop = false;
        Pool.m_Instance.RemoveObject(gameObject, EnumAudios.instance);
    }

    private IEnumerator CoroutineMusicBackground(GameObject gameObject)
    {
        while(true)
        {
            if(m_ListBackgroundMusic.Count > 0)
            {
                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                int randomIndex = Random.Range(0, m_ListBackgroundMusic.Count);
                audioSource.clip = m_ListBackgroundMusic[randomIndex].clip;
                audioSource.volume = m_ListBackgroundMusic[randomIndex].volume;

                gameObject.SetActive(true);

                yield return new WaitForSeconds(audioSource.clip.length);
            }
            else
            {
                yield return null;
            }
            
            gameObject.SetActive(false);
        }
    }
}
