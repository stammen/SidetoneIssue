using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Render;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SidetoneIssue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool m_isRecording = false;
        private AudioGraph m_audioGraph;
        private AudioDeviceInputNode m_deviceInputNode;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_isRecording)
            {
                StopRecording();
                recordButton.Content = "Record";
            }
            else
            {
                // start the recording task
                await StartRecording();
                recordButton.Content = "Stop";
            }
        }

        public async Task StartRecording()
        {
            try
            {
                // Construct the audio graph
                var result = await AudioGraph.CreateAsync(new AudioGraphSettings(AudioRenderCategory.Other));

                if (result.Status != AudioGraphCreationStatus.Success)
                {
                    throw new Exception("AudioGraph creation error: " + result.Status);
                }

                m_audioGraph = result.Graph;

                var inputResult = await m_audioGraph.CreateDeviceInputNodeAsync(MediaCategory.Other);
                if (inputResult.Status != AudioDeviceNodeCreationStatus.Success)
                {
                    throw new Exception("AudioGraph CreateDeviceInputNodeAsync error: " + inputResult.Status);
                }

                m_deviceInputNode = inputResult.DeviceInputNode;
                m_audioGraph.QuantumStarted += Node_QuantumStarted;
                m_audioGraph.Start();
                m_isRecording = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AudioInput Start Exception: " + ex.Message);
            }
        }

        private void Node_QuantumStarted(AudioGraph graph, object args)
        {

        }

        public void StopRecording()
        {
            if (m_audioGraph != null)
            {
                m_audioGraph.Stop();
                m_audioGraph.Dispose();
                m_audioGraph = null;
            }
            m_isRecording = false;
        }
    }
}
