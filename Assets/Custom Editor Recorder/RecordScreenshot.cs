#if UNITY_EDITOR

using System.IO;
using UnityEditor.Recorder;
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
    public class RecordScreenshot : MonoBehaviour
    {
        public static RecordScreenshot Instance;

        RecorderController m_RecorderController;

        private InputAction qKeyAction;

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

        private void OnEnable()
        {
            // keyboard input
            qKeyAction = new InputAction("QKey", binding: "<Keyboard>/q");
            qKeyAction.performed += OnQKeyPressed;
            qKeyAction.Enable();

            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            var mediaOutputFolder = Path.Combine(Application.dataPath, "..", "SampleRecordings");

            // Image
            var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorder.name = "My Image Recorder";
            imageRecorder.Enabled = true;
            imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            imageRecorder.CaptureAlpha = false;

            imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, "image_") + DefaultWildcard.Take;

            imageRecorder.imageInputSettings = new GameViewInputSettings
            {
                OutputWidth = 1440,
                OutputHeight = 2560,
            };

            // Setup Recording
            controllerSettings.AddRecorderSettings(imageRecorder);
            controllerSettings.SetRecordModeToSingleFrame(0);
        }

        void OnDisable()
        {
            qKeyAction.Disable();
        }

        private void OnQKeyPressed(InputAction.CallbackContext context)
        {
            TakeScreenshot();
        }

        private void TakeScreenshot()
        {
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();
        }
    }

}

#endif



