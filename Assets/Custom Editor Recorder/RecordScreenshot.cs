#if UNITY_EDITOR

using System.IO;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine.InputSystem;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// Handles manual screenshot capture from the Unity Game View using the Unity Recorder API.
    /// Screenshots can be captured via a configurable input action.
    /// This script is editor-only and intended for gameplay capture, testing, or demo recording.
    /// </summary>
    public class RecordScreenshot : MonoBehaviour
    {
        RecorderController m_RecorderController;

        #region Screenshot Settings
        /// <summary>
        /// Resolution of the captured screenshot (width x height).
        /// </summary>
        [Space(20)]
        [Header("Screenshot Resolution (Width, Height)")]
        [SerializeField] private Vector2 m_ScreenshotResolution = new Vector2(1920, 1080);

        /// <summary>
        /// Input action used to capture a screenshot.
        /// Defaults to the 'Q' key if not assigned.
        /// </summary>
        [Space(20)]
        [Header("Capture Screenshot Input")]
        [SerializeField] private InputAction captureScreenshotInput;

        /// <summary>
        /// Base name of the output screenshot file.
        /// Additional metadata like resolution and take number are appended automatically.
        /// </summary>
        [Space(20)]
        [Header("Name of the output screenshot file (without extension)")]
        [SerializeField] private string outputFileName = "MyScreenshot";

        /// <summary>
        /// Relative folder path where screenshots will be saved.
        /// </summary>
        [Space(20)]
        [Header("Output folder path relative to project root. Leave empty for default 'SampleRecordings'")]
        [SerializeField] private string outputFolderPath = "SampleRecordings";

        /// <summary>
        /// Image format for the screenshot output.
        /// </summary>
        [Space(20)]
        [Header("Output image format")]
        [SerializeField] private ImageRecorderSettings.ImageRecorderOutputFormat outputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;

        /// <summary>
        /// Determines whether the alpha channel should be captured.
        /// </summary>
        [Space(20)]
        [Header("Capture Alpha Channel")]
        [SerializeField] private bool captureAlpha = false;
        #endregion Screenshot Settings

        /// <summary>
        /// Recorder settings instance used to configure screenshot capture.
        /// </summary>
        internal ImageRecorderSettings m_Settings = null;

        /// <summary>
        /// Returns the final output screenshot file including its extension.
        /// </summary>
        public FileInfo OutputFile
        {
            get
            {
                var extension = GetFileExtension(outputFormat);
                var fileName = m_Settings.OutputFile + extension;
                return new FileInfo(fileName);
            }
        }


        private void OnEnable()
        {
            EnableCaptureInput();
            InitializeRecorder();
        }

        private void OnDisable()
        {
            DisableCaptureInput();
        }

        /// <summary>
        /// Initializes the recorder controller and configures screenshot settings.
        /// Creates the output directory and sets up the image recorder.
        /// </summary>
        internal void InitializeRecorder()
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

            // Image settings
            m_Settings = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            m_Settings.name = "My Screenshot Recorder";
            m_Settings.Enabled = true;
            m_Settings.OutputFormat = outputFormat;
            m_Settings.CaptureAlpha = captureAlpha;

            m_Settings.imageInputSettings = new GameViewInputSettings
            {
                OutputWidth = (int)m_ScreenshotResolution.x,
                OutputHeight = (int)m_ScreenshotResolution.y
            };

            // Output file path with take counter
            int takeCount = PlayerPrefs.GetInt(outputFileName + "ScreenshotTakes", 0);
            string take = takeCount.ToString();
            m_Settings.OutputFile = $"{mediaOutputFolder.FullName}/{outputFileName}_Resolution{m_ScreenshotResolution.x}x{m_ScreenshotResolution.y}_Take{take}";
            takeCount = takeCount + 1;
            PlayerPrefs.SetInt(outputFileName + "ScreenshotTakes", takeCount);

            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToSingleFrame(0);

            RecorderOptions.VerboseMode = false;
        }

        #region Capture Screenshot
        /// <summary>
        /// Enables and binds input action for capturing screenshots.
        /// Assigns default key if no binding is provided.
        /// </summary>
        private void EnableCaptureInput()
        {
            // CAPTURE SCREENSHOT
            if (captureScreenshotInput == null || captureScreenshotInput.bindings.Count == 0)
            {
                captureScreenshotInput = new InputAction(
                    "CaptureScreenshot",
                    InputActionType.Button,
                    "<Keyboard>/q"
                );
            }

            captureScreenshotInput.performed += OnCaptureScreenshotPressed;
            captureScreenshotInput.Enable();
        }

        /// <summary>
        /// Disables and unbinds screenshot capture input action.
        /// </summary>
        private void DisableCaptureInput()
        {
            if (captureScreenshotInput != null)
            {
                captureScreenshotInput.performed -= OnCaptureScreenshotPressed;
                captureScreenshotInput.Disable();
            }
        }

        /// <summary>
        /// Callback invoked when the capture screenshot input is triggered.
        /// Reinitializes the recorder with new take number and captures the screenshot.
        /// </summary>
        private void OnCaptureScreenshotPressed(InputAction.CallbackContext context)
        {
            CaptureScreenshot();
        }

        /// <summary>
        /// Captures a screenshot using the configured settings.
        /// Updates the take counter and logs the output file path.
        /// </summary>
        private void CaptureScreenshot()
        {
            // Reinitialize to update take number
            InitializeRecorder();

            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            Debug.Log($"SCREENSHOT CAPTURED AND SAVED TO: {OutputFile.FullName}");
        }
        #endregion Capture Screenshot


        /// <summary>
        /// Returns the appropriate file extension based on the selected output format.
        /// </summary>
        private string GetFileExtension(ImageRecorderSettings.ImageRecorderOutputFormat format)
        {
            switch (format)
            {
                case ImageRecorderSettings.ImageRecorderOutputFormat.PNG:
                    return ".png";
                case ImageRecorderSettings.ImageRecorderOutputFormat.JPEG:
                    return ".jpg";
                case ImageRecorderSettings.ImageRecorderOutputFormat.EXR:
                    return ".exr";
                default:
                    return ".png";
            }
        }
    }

}

#endif



