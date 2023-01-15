using System.Numerics;
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
        var capture = new WasapiLoopbackCapture();
        capture.DataAvailable += (s, a) =>
        {
            //var str = GetDirectionWithPhaseDifference(capture.WaveFormat, a.Buffer);
            double pan = getStereoPan(capture.WaveFormat, a.Buffer);
            if (pan == Double.NaN)
            {
                return;
            }
            double StereoPanToDegree(double value)
            {
                // Convert the stereo panning value to a degree between 0 and 180
                double degree = (value + 1) * 90;

                // Rotate the degree so the top of the circle represents the front
                degree = 90 - degree;

                // Make sure the degree is within the range [0, 360)
                if (degree < 0)
                    degree += 360;
                return degree;
            }

            var d = StereoPanToDegree(pan);
            _overlayForm.Invoke((MethodInvoker)delegate
            {
                _overlayForm.TriggerOverlayIndicator(d);
            });
        };

        capture.RecordingStopped += (s, a) =>
        {
            capture.Dispose();
        };

        capture.StartRecording();
        while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
        {
            Thread.Sleep(10);
        }
    }
    
    double getStereoPan(WaveFormat waveFormat, byte[] buffer)
    {
        if (waveFormat.Channels != 2)
        {
            throw new ArgumentException("Wave format must be stereo", nameof(waveFormat));
        }
        var left = new float[buffer.Length / sizeof(float)];
        var right = new float[buffer.Length / sizeof(float)];
        var samples = new float[buffer.Length / sizeof(float)];
        Buffer.BlockCopy(buffer, 0, samples, 0, buffer.Length);

        for (int i = 0; i < samples.Length; i += 2)
        {
            left[i / 2] = samples[i];
            right[i / 2] = samples[i + 1];
        }
        var leftAmplitude = left.Select(x => Math.Abs(x)).Max();
        var rightAmplitude = right.Select(x => Math.Abs(x)).Max();
        var stereoPan = leftAmplitude - rightAmplitude;
        stereoPan /= leftAmplitude + rightAmplitude;
        return stereoPan;
    }
    
    // string GetDirectionWithPhaseDifference(WaveFormat waveFormat, byte[] buffer)
    // {
    //     var samples = new float[buffer.Length / sizeof(float)];
    //     Buffer.BlockCopy(buffer, 0, samples, 0, buffer.Length);
    //     var left = new float[samples.Length / 2];
    //     var right = new float[samples.Length / 2];
    //
    //     for (int i = 0; i < samples.Length; i += 2)
    //     {
    //         left[i / 2] = samples[i];
    //         right[i / 2] = samples[i + 1];
    //     }
    //     var leftPhase = GetPhase(left);
    //     var rightPhase = GetPhase(right);
    //     var phaseDifference = rightPhase - leftPhase;
    //     if (phaseDifference > Math.PI)
    //     {
    //         phaseDifference = 2 * Math.PI - phaseDifference;
    //     }
    //     if (phaseDifference < Math.PI / 2)
    //     {
    //         return "Front";
    //     }
    //     else if (phaseDifference < Math.PI)
    //     {
    //         return "Right";
    //     }
    //     else
    //     {
    //         return "Left";
    //     }
    // }
    //
    // double GetPhase(float[] samples)
    // {
    //     var complex = new Complex[samples.Length];
    //     for (int i = 0; i < samples.Length; i++)
    //     {
    //         complex[i] = new Complex(samples[i], 0);
    //     }
    //     Accord.Math.FourierTransform.FFT(complex, Accord.Math.FourierTransform.Direction.Forward);
    //     var maxIndex = 0;
    //     var maxAmplitude = 0.0;
    //     for (int i = 0; i < complex.Length; i++)
    //     {
    //         if (complex[i].Magnitude > maxAmplitude)
    //         {
    //             maxAmplitude = complex[i].Magnitude;
    //             maxIndex = i;
    //         }
    //     }
    //     return Math.Atan2(complex[maxIndex].Imaginary, complex[maxIndex].Real);
    // }

}