#if UNITY_EDITOR

using System.IO;
using Unity.VisualScripting;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine.InputSystem;

namespace UnityEngine.Recorder.Examples
{
    public class RecordVideo : MonoBehaviour
    {
        RecorderController m_RecorderController;

        [Space(20)]
        [Header("Video Recording Settings")]
        [SerializeField] private Vector2 m_RecordResolution = new Vector2(1920, 1080);
        [SerializeField] private bool m_RecordAudio = false;

        [Space(20)]
        [Header("Start Recording Input")]
        [SerializeField] private InputAction startRecordingInput;
        [Header("Stop Recording Input")]
        [SerializeField] private InputAction stopRecordingInput;

        [Space(20)]
        [Header("Name of the output video file (without extension)")]
        [SerializeField] private string outputFileName = "MyRecording";
        
        [Space(20)]
        [Header("Output folder path relative to project root. Leave empty for default 'SampleRecordings'")]
        [SerializeField] private string outputFolderPath = "SampleRecordings";
        
        [Space(20)]
        [Header("Output codec format")]
        [SerializeField] private CoreEncoderSettings.OutputCodec outputCodec = CoreEncoderSettings.OutputCodec.MP4;

        [Space(20)]
        [Header("Frame rate for the recording")]
        [SerializeField] private float frameRate = 60.0f;

        [Space(20)]
        [Header("Video encoding quality")]
        [SerializeField] private CoreEncoderSettings.VideoEncodingQuality encodingQuality = CoreEncoderSettings.VideoEncodingQuality.High;

        internal MovieRecorderSettings m_Settings = null;

        public FileInfo OutputFile
        {
            get
            {
                var extension = GetFileExtension(outputCodec);
                var fileName = m_Settings.OutputFile + extension;
                return new FileInfo(fileName);
            }
        }


        private void OnEnable()
        {
            EnableRecordingInput();
        }
        private void OnDisable()
        {
            DisableRecordingInput();
        }


        internal void Initialize()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            
            string folderPath = string.IsNullOrEmpty(outputFolderPath) ? "SampleRecordings" : outputFolderPath;
            var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", folderPath));

            
            // Create folder if it doesn't exist
            if (!mediaOutputFolder.Exists)
            {
                mediaOutputFolder.Create();
            }

            
            // Video settings
            m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            m_Settings.name = "My Video Recorder";
            m_Settings.Enabled = true;

            
            // Encoder settings
            m_Settings.EncoderSettings = new CoreEncoderSettings
            {
                EncodingQuality = encodingQuality,
                Codec = outputCodec
            };
            m_Settings.CaptureAlpha = true;

            m_Settings.ImageInputSettings = new GameViewInputSettings
            {
                OutputWidth = (int)m_RecordResolution.x,
                OutputHeight = (int)m_RecordResolution.y
            };


            // Output file path
            int takeCount = PlayerPrefs.GetInt("VideoTakes", 0);
            takeCount++;
            PlayerPrefs.SetInt("VideoTakes", takeCount);
            string take = takeCount.ToString();
            m_Settings.OutputFile = $"{mediaOutputFolder.FullName}/{outputFileName}_Resolution{m_RecordResolution.x}x{m_RecordResolution.y}_Fps{frameRate}_Take{take}";


            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = frameRate;

            RecorderOptions.VerboseMode = false;
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            Debug.Log($"STARTED RECORDING");
        }



        #region Start/Stop Recording
        private void EnableRecordingInput()
        {
            // START RECORDING
            if (startRecordingInput == null || startRecordingInput.bindings.Count == 0)
            {
                startRecordingInput = new InputAction(
                    "StartRecording",
                    InputActionType.Button,
                    "<Keyboard>/q"
                );
            }

            startRecordingInput.performed += OnStartRecordPressed;
            startRecordingInput.Enable();


            // STOP RECORDING
            if (stopRecordingInput == null || stopRecordingInput.bindings.Count == 0)
            {
                stopRecordingInput = new InputAction(
                    "StopRecording",
                    InputActionType.Button,
                    "<Keyboard>/e"
                );
            }

            stopRecordingInput.performed += OnStopRecordPressed;
            stopRecordingInput.Enable();
        }

        private void DisableRecordingInput()
        {
            if (startRecordingInput != null)
            {
                startRecordingInput.performed -= OnStartRecordPressed;
                startRecordingInput.Disable();
            }

            if (stopRecordingInput != null)
            {
                stopRecordingInput.performed -= OnStopRecordPressed;
                stopRecordingInput.Disable();
            }
        }
        private void OnStartRecordPressed(InputAction.CallbackContext context)
        {
            Initialize();
        }
        private void OnStopRecordPressed(InputAction.CallbackContext context)
        {
            m_RecorderController.StopRecording();
            Debug.Log($"RECORDING SAVED TO : {OutputFile.FullName}");
        }
        #endregion Start/Stop Recording



        private string GetFileExtension(CoreEncoderSettings.OutputCodec codec)
        {
            switch (codec)
            {
                case CoreEncoderSettings.OutputCodec.MP4:
                    return ".mp4";
                case CoreEncoderSettings.OutputCodec.WEBM:
                    return ".webm";
                default:
                    return ".mp4";
            }
        }
    }

}

#endif