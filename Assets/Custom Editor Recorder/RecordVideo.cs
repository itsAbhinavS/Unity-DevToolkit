#if UNITY_EDITOR

using System.IO;
using Unity.VisualScripting;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine.InputSystem;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// Handles manual video recording from the Unity Game View using the Unity Recorder API.
    /// Recording can be started and stopped via configurable input actions.
    /// This script is editor-only and intended for gameplay capture, testing, or demo recording.
    /// </summary>
    public class RecordVideo : MonoBehaviour
    {
        RecorderController m_RecorderController;

        #region Video Recording Settings
        /// <summary>
        /// Resolution of the recorded video (width x height).
        /// </summary>
        [Space(20)]
        [Header("Video Resolution")]
        [SerializeField] private Vector2 m_RecordResolution = new Vector2(1920, 1080);

        /// <summary>
        /// Determines whether audio should be recorded.
        /// </summary>
        [Space(20)]
        [Header("Audio Setting")]
        [SerializeField] private bool m_RecordAudio = false;

        /// <summary>
        /// Input action used to start video recording.
        /// Defaults to the 'Q' key if not assigned.
        /// </summary>
        [Space(20)]
        [Header("Start Recording Input")]
        [SerializeField] private InputAction startRecordingInput;

        /// <summary>
        /// Input action used to stop video recording.
        /// Defaults to the 'E' key if not assigned.
        /// </summary>
        [Header("Stop Recording Input")]
        [SerializeField] private InputAction stopRecordingInput;

        /// <summary>
        /// Base name of the output video file.
        /// Additional metadata like resolution, FPS, and take number are appended automatically.
        /// </summary>
        [Space(20)]
        [Header("Name of the output video file (without extension)")]
        [SerializeField] private string outputFileName = "MyRecording";

        /// <summary>
        /// Relative folder path where recorded videos will be saved.
        /// </summary>
        [Space(20)]
        [Header("Output folder path relative to project root. Leave empty for default 'SampleRecordings'")]
        [SerializeField] private string outputFolderPath = "SampleRecordings";

        /// <summary>
        /// Codec format used for video encoding (MP4 / WEBM).
        /// </summary>
        [Space(20)]
        [Header("Output codec format")]
        [SerializeField] private CoreEncoderSettings.OutputCodec outputCodec = CoreEncoderSettings.OutputCodec.MP4;

        /// <summary>
        /// Frame rate of the recorded video.
        /// </summary>
        [Space(20)]
        [Header("Frame rate for the recording")]
        [SerializeField] private float frameRate = 60.0f;

        /// <summary>
        /// Quality level used during video encoding.
        /// </summary>
        [Space(20)]
        [Header("Video encoding quality")]
        [SerializeField] private CoreEncoderSettings.VideoEncodingQuality encodingQuality = CoreEncoderSettings.VideoEncodingQuality.High;
        #endregion Video Recording Settings

        /// <summary>
        /// Recorder settings instance used to configure video recording.
        /// </summary>
        internal MovieRecorderSettings m_Settings = null;

        /// <summary>
        /// Returns the final output video file including its extension.
        /// </summary>
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


        /// <summary>
        /// Initializes and starts the video recording process.
        /// Creates the output directory, configures recorder settings,
        /// increments the take number, and begins recording.
        /// </summary>
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
        
        /// <summary>
        /// Enables and binds input actions for starting and stopping recording.
        /// Assigns default keys if no bindings are provided.
        /// </summary>
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

        /// <summary>
        /// Disables and unbinds all recording-related input actions.
        /// </summary>
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

        /// <summary>
        /// Callback invoked when the start recording input is triggered.
        /// </summary>
        private void OnStartRecordPressed(InputAction.CallbackContext context) => Initialize();

        /// <summary>
        /// Callback invoked when the stop recording input is triggered.
        /// Stops recording and logs the output file path.
        /// </summary>
        private void OnStopRecordPressed(InputAction.CallbackContext context)
        {
            m_RecorderController.StopRecording();
            Debug.Log($"RECORDING SAVED TO : {OutputFile.FullName}");
        }
        #endregion Start/Stop Recording



        /// <summary>
        /// Returns the appropriate file extension based on the selected output codec.
        /// </summary>
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