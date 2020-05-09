using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class audiotoshader : MonoBehaviour
{
    [Header("Effect Configuration")]
    [SerializeField] public float vol = 1.0f;
 	[SerializeField, Range(0f, 10f)] float m_gain = 10.0f; // 音量に掛ける倍率
 	Material _material;
 	private Renderer _renderer;


    AudioClip clip;
    int head = 0;
    const int samplingFrequency = 44100;
    const int lengthSeconds = 1;
    float[] processBuffer = new float[256];
    float[] microphoneBuffer = new float[lengthSeconds * samplingFrequency];


    float m_volumeRate; // 音量(0-1)

    // Start is called before the first frame update
    void Start()
    {
    	_renderer = GetComponent<Renderer>();
    	var originalMaterial = new Material(_renderer.material);

        clip = Microphone.Start(null, true, 1, samplingFrequency);
        while (Microphone.GetPosition(null) < 0) { }
    }

    // Update is called once per frame
    void Update()
    {
    	Debug.Log(m_volumeRate);

    	var position = Microphone.GetPosition(null);
        if (position < 0 || head == position) {
            return;
        }

        clip.GetData(microphoneBuffer, 0);
        while (GetDataLength(microphoneBuffer.Length, head, position) > processBuffer.Length) {
            var remain = microphoneBuffer.Length - head;
            if (remain < processBuffer.Length) {
                Array.Copy(microphoneBuffer, head, processBuffer, 0, remain);
                Array.Copy(microphoneBuffer, 0, processBuffer, remain, processBuffer.Length - remain);
            } else {
                Array.Copy(microphoneBuffer, head, processBuffer, 0, processBuffer.Length);
            }

            head += processBuffer.Length;
            if (head > microphoneBuffer.Length) {
                head -= microphoneBuffer.Length;
            }
        }

        float sum = 0f;
        for (int i = 0; i < processBuffer.Length; ++i)
        {
            sum += Mathf.Abs(processBuffer[i]); // データ（波形）の絶対値を足す
        }
        // データ数で割ったものに倍率をかけて音量とする
        m_volumeRate = Mathf.Clamp01(sum * m_gain / (float)processBuffer.Length);

    	vol = m_volumeRate;

        _renderer.material.SetFloat("_Volume", vol);

        if(vol > 0.4){
        	_renderer.material.SetFloat("_FRG", 1.0f);
        }else{
        	_renderer.material.SetFloat("_FRG", 0);
        }


    }

    static int GetDataLength(int bufferLength, int head, int tail) {
        if (head < tail) {
            return tail - head;
        } else {
            return bufferLength - head + tail;
        }
    }
}
