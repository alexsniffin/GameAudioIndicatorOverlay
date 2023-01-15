using System.Numerics;
using GameAudioIndicatorOverlay.Audio;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace GameAudioIndicatorOverlay;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var overlay = new OverlayForm();

        var ac = new AudioCapture(overlay);
        Thread thread = new Thread(ac.Capture);
        thread.Start();
        
        Application.Run(overlay);
    }
}