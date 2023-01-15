using System.Numerics;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace GameAudioIndicatorOverlay.Audio;

public class AudioCapture
{
    private OverlayForm _overlayForm;

    public AudioCapture(OverlayForm overlayForm)
    {
        _overlayForm = overlayForm;
    }

    public void Capture()
    {
        var enumerator = new MMDeviceEnumerator();
        var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        var capture = new WasapiLoopbackCapture(device);
        
        List<double> _panValues = new List<double>();
        int _averageCount = 3;

        capture.DataAvailable += (s, a) =>
        {
            double pan = getStereoPan(capture.WaveFormat, a.Buffer);
            if (Double.IsNaN(pan))
            {
                return;
            }
            
            _panValues.Add(pan);
            if (_panValues.Count > _averageCount)
            {
                var averagePan = _panValues.Average();
                _overlayForm.Invoke((MethodInvoker)delegate
                {
                    _overlayForm.UpdateIndicator((float) averagePan);
                    _overlayForm.UpdateText(Math.Round(averagePan, 5).ToString());
                });
                _panValues.Clear();
            }
        };

        capture.RecordingStopped += (s, a) =>
        {
            capture.Dispose();
        };

        capture.StartRecording();
        while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
        {
            Thread.Sleep(300);
        }
    }

    float getStereoPan(WaveFormat waveFormat, byte[] buffer)
    {
        var channels = waveFormat.Channels;
        var samplesPerChannel = buffer.Length / sizeof(float) / channels;
        var samples = new float[buffer.Length / sizeof(float)];
        Buffer.BlockCopy(buffer, 0, samples, 0, buffer.Length);

        var rmsPerChannel = new float[channels];
        for (int i = 0; i < samplesPerChannel; i++)
        {
            for (int j = 0; j < channels; j++)
            {
                rmsPerChannel[j] += samples[i * channels + j] * samples[i * channels + j];
            }
        }

        for (int i = 0; i < channels; i++)
        {
            rmsPerChannel[i] = (float)Math.Sqrt(rmsPerChannel[i] / samplesPerChannel);
        }

        var stereoPan = (rmsPerChannel[0] - rmsPerChannel[1]) / (rmsPerChannel[0] + rmsPerChannel[1]);
        return -1 * stereoPan;
    }
}