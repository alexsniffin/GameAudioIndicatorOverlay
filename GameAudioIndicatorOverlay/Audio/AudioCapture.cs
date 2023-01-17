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

        var panValues = new List<double>();
        const int averageCount = 3;

        var audioFileBuffer = File.ReadAllBytes("./shots1.mp3");
        capture.DataAvailable += (s, a) =>
        {
            var rmsPercentChannels = getChannelPercentages(capture.WaveFormat, a.Buffer, 0.000000001f)
                .Select(x => x * 100).ToArray();

            double pan = getStereoPan(capture.WaveFormat, a.Buffer);
            if (double.IsNaN(pan))
            {
                _overlayForm.Invoke((MethodInvoker)delegate
                {
                    _overlayForm.UpdateIndicator(0.0f, rmsPercentChannels);
                    _overlayForm.UpdateText("quiet");
                });
                return;
            }

            panValues.Add(pan);
            if (panValues.Count <= averageCount) return;

            var averagePan = panValues.Average();

            _overlayForm.Invoke((MethodInvoker)delegate
            {
                _overlayForm.UpdateIndicator((float)averagePan, rmsPercentChannels);
                _overlayForm.UpdateText(string.Join(",", rmsPercentChannels.Select((val, idx) => $"{idx}: {val}")));
            });
            panValues.Clear();
        };

        capture.RecordingStopped += (s, a) => { capture.Dispose(); };

        capture.StartRecording();
        while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
        {
            Thread.Sleep(300);
        }
    }
    
    float[] getChannelPercentages(WaveFormat waveFormat, byte[] buffer, float threshold)
    {
        var channels = waveFormat.Channels;
        var samplesPerChannel = buffer.Length / sizeof(float) / channels;
        var samples = new float[buffer.Length / sizeof(float)];
        Buffer.BlockCopy(buffer, 0, samples, 0, buffer.Length);
        var rmsPerChannel = new float[channels];
        for (var i = 0; i < samplesPerChannel; i++)
        {
            for (var j = 0; j < channels; j++)
            {
                rmsPerChannel[j] += samples[i * channels + j] * samples[i * channels + j];
            }
        }

        // check if the RMS amplitude of any channel is below the threshold
        var isBelowThreshold = true;
        for (var i = 0; i < channels; i++)
        {
            var rms = Math.Sqrt(rmsPerChannel[i] / samplesPerChannel);
            if (rms > threshold)
            {
                isBelowThreshold = false;
                break;
            }
        }

        if (isBelowThreshold)
        {
            var zeroes = new float[channels];
            for (var i = 0; i < channels; i++)
            {
                zeroes[i] = 0;
            }

            return zeroes;
        }

        for (var i = 0; i < channels; i++)
        {
            rmsPerChannel[i] = (float)Math.Sqrt(rmsPerChannel[i] / samplesPerChannel);
        }

        return rmsPerChannel;
    }

    float getStereoPan(WaveFormat waveFormat, byte[] buffer)
    {
        var channels = waveFormat.Channels;
        var samplesPerChannel = buffer.Length / sizeof(float) / channels;
        var samples = new float[buffer.Length / sizeof(float)];
        Buffer.BlockCopy(buffer, 0, samples, 0, buffer.Length);

        var rmsPerChannel = new float[channels];
        for (var i = 0; i < samplesPerChannel; i++)
        {
            for (var j = 0; j < channels; j++)
            {
                rmsPerChannel[j] += samples[i * channels + j] * samples[i * channels + j];
            }
        }

        for (var i = 0; i < channels; i++)
        {
            rmsPerChannel[i] = (float)Math.Sqrt(rmsPerChannel[i] / samplesPerChannel);
        }

        var stereoPan = (rmsPerChannel[0] - rmsPerChannel[1]) / (rmsPerChannel[0] + rmsPerChannel[1]);
        return -1 * stereoPan;
    }
}