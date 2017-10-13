using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBands : MonoBehaviour
{
  [Range(0.0f, 0.5f)]
  public float maxHeight = 0.3f;
  public static AudioBands Instance;
  public float[] amplitudes = new float[8];
  public float averageAmplitude;

  private AudioSource audioSource;
  private float highestAmplitude;

  private float[] samplesLeft = new float[512];
  private float[] samplesRight = new float[512];
  private float[] bandBuffer = new float[8];
  private float[] bufferDecrease = new float[8];
  private float[] freqBandHighest = new float[8];

  void Awake()
  {
    if (Instance == null)
    {
        Instance = this;
    }
    else if (Instance != this)
    {
        Destroy(gameObject);
    }
  }

  void Start ()
  {
    audioSource = GetComponent<AudioSource>();
    InitializeFreqBandHighest();
  }

  void Update ()
  {
    GetSpectrumDataFromAudioSource();
    CreateAmplitudes();
    CreateAverageAmplitude();
  }

  private void InitializeFreqBandHighest()
  {
    for (int i = 0; i < 8; i++)
    {
      freqBandHighest[i] = 5;
    }
  }

  private void GetSpectrumDataFromAudioSource()
  {
    audioSource.GetSpectrumData(samplesLeft, 0, FFTWindow.Blackman);
    audioSource.GetSpectrumData(samplesRight, 1, FFTWindow.Blackman);
  }

  private void CreateAmplitudes()
  {
    int count = 0;

    for (int i = 0; i < 8; i++)
    {
        float average = 0;
        int sampleCount = (int)Mathf.Pow(2, i) * 2;

        if (i == 7)
        {
            sampleCount +=2;
        }

        for (int j = 0; j < sampleCount; j++)
        {
            average += samplesLeft[count] + samplesRight[count] * (count + 1);
            count++;
        }

        average /= count;
        float currentBand = average * 10;

        if (currentBand > bandBuffer[i])
        {
            bandBuffer[i] = currentBand;
            bufferDecrease[i] = 0.005f;
        }

        if (currentBand < bandBuffer[i])
        {
            bandBuffer[i] -= bufferDecrease[i];
            bufferDecrease[i] *= 1.2f;
        }

        if (bandBuffer[i] < 0)
        {
            bandBuffer[i] = 0;
        }

        if (currentBand > freqBandHighest[i])
        {
            freqBandHighest[i] = currentBand;
        }

        amplitudes[i] = (bandBuffer[i] / freqBandHighest[i]) * maxHeight;
    }
  }

  void CreateAverageAmplitude()
  {
    float currentAmplitude = 0;

    for (int i = 0; i < 8; i++)
    {
        currentAmplitude += amplitudes[i];
    }

    if (currentAmplitude > highestAmplitude)
    {
        highestAmplitude = currentAmplitude;
    }

    averageAmplitude = currentAmplitude / highestAmplitude;
  }
}

