#if UNITY_EDITOR

using System.IO;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine.InputSystem;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// This example shows how to set up a recording session via script.
    /// To use this example, add the CaptureScreenShotExample component to a GameObject.
    ///
    /// Entering playmode will display a "Capture ScreenShot" button.
    ///
    /// Recorded images are saved in [Project Folder]/SampleRecordings
    /// </summary>
    public class RecordVideo : MonoBehaviour
    {
        public static RecordVideo Instance;

        RecorderController m_RecorderController;
        public bool m_RecordAudio = false;
        internal MovieRecorderSettings m_Settings = null;

        private InputAction qKeyAction;
        private InputAction eKeyAction;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(Instance);
            }
        }

        public FileInfo OutputFile
        {
            get
            {
                var fileName = m_Settings.OutputFile + ".mp4";
                return new FileInfo(fileName);
            }
        }

        private void OnEnable()
        {
            // Q keyinput
            qKeyAction = new InputAction("QKey", binding: "<Keyboard>/q");
            qKeyAction.performed += OnQKeyPressed;
            qKeyAction.Enable();

            // E keyinput
            eKeyAction = new InputAction("EKey", binding: "<Keyboard>/e");
            eKeyAction.performed += OnEKeyPressed;
            eKeyAction.Enable();
        }

        internal void Initialize()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "SampleRecordings"));

            // Video
            m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            m_Settings.name = "My Video Recorder";
            m_Settings.Enabled = true;

            // This example performs an MP4 recording
            m_Settings.EncoderSettings = new CoreEncoderSettings
            {
                EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High,
                Codec = CoreEncoderSettings.OutputCodec.MP4
            };
            m_Settings.CaptureAlpha = true;

            m_Settings.ImageInputSettings = new GameViewInputSettings
            {
                //OutputWidth = 1440,
                //OutputHeight = 2560,
                OutputWidth = 1080,
                OutputHeight = 1920
            };

            // Simple file name (no wildcards) so that FileInfo constructor works in OutputFile getter.
            m_Settings.OutputFile = mediaOutputFolder.FullName + "/" + "video";

            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 60.0f;

            RecorderOptions.VerboseMode = false;
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            Debug.Log($"Started recording for file {OutputFile.FullName}");
        }

        private void OnDisable()
        {
            qKeyAction.Disable();
        }

        private void OnQKeyPressed(InputAction.CallbackContext context)
        {
            Initialize();
        }
        private void OnEKeyPressed(InputAction.CallbackContext context)
        {
            m_RecorderController.StopRecording();
        }

    }

}

#endif



