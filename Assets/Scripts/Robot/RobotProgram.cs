using System;
using UnityEngine;

namespace Robots.Samples.Unity
{
    public class RobotProgram : MonoBehaviour
    {
        [Serializable]
        public enum RobotState
        {
            Off = -1,
            Idle = 0,
            Normal = 1
        }

        public static RobotState State { get; set; } = RobotState.Off;
        
        public PlaybackPanel[] playbackPanels;
        
        public string gripperLoadedName;
        public string gripperUnloadedName;
        public Transform robotMeshTarget;
        public GameObject toolPrefab;

        [SerializeField]
        #nullable enable
        private Material? _toolMaterial;
        private Program? _program;
        private UnityMeshPoser? _meshPoser;
        private UnityMeshPoser? _toolMeshPoser;
        private bool _isPlaying;
        private int _programDuration;
        private int _currentTargetIndex;
        private bool _isLoaded;
        private float _currentTime;

        private void OnEnable()
        {
            RobotActions.OnToolLoaded += LoadTool;
            RobotActions.OnToolUnloaded += UnloadTool;
            WebClient.OnProgram += CreateProgramFromObject;
        }

        private void CreateProgramFromObject(IProgram obj)
        {
            _program = obj as Program;

            if (_program == null) return; 
            
            _program.MeshPoser = new UnityMeshPoser(_program.RobotSystem, _toolMaterial, toolPrefab, robotMeshTarget);
        }

        private void OnDisable()
        {
            RobotActions.OnToolLoaded -= LoadTool;
            RobotActions.OnToolUnloaded -= UnloadTool;
        }

        private void Start()
        {
            foreach (var playbackPanel in playbackPanels)
            {
                if (playbackPanel == null)
                {
                    Debug.LogWarning("Playback panel is empty, not able to add listener to it");
                    return;
                }
                playbackPanel.playButton.onClick.AddListener(PlayPlayback);
                playbackPanel.pauseButton.onClick.AddListener(PausePlayback);
                playbackPanel.sliderPanel.slider.onValueChanged.AddListener(value => SetPlaybackTime(value));
            }
        }

        void Update()
        {
            if (_program is null)
                return;

            if (_isPlaying)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime > (float)_program.Duration)
                    _currentTime -= (float)_program.Duration;
                // var time = Mathf.PingPong(Time.time, (float)_program.Duration);
                
                SetPlaybackTime(_currentTime);
            }
        }

        public async void CreateProgramFromJSON(string json)
        {
            _program = await GrasshopperSyncProgram.CreateAsync(json);

            if (_toolMaterial == null)
                throw new ArgumentNullException(nameof(_toolMaterial));

            if(_meshPoser == null)
                _meshPoser = new UnityMeshPoser(_program.RobotSystem, _toolMaterial, toolPrefab, robotMeshTarget);

            _program.MeshPoser = _meshPoser;

            RobotActions.OnProgramDurationUpdated((int)_program.Duration);
            RobotActions.OnTimeUpdated(0);
        }

        void PlayPlayback()
        {
            _isPlaying = true;
        }
        void PausePlayback()
        {
            _isPlaying = false;
            UR_EthernetIPClient.StopMoving();
        }

        /// <summary>
        /// Set's the program current playback time, and alters the target state which can trigger robot events.
        /// </summary>
        /// <param name="time"></param>
        void SetPlaybackTime(float time)
        {
            if (_program is null)
                return;


            _currentTime = time;
            _program.Animate(_currentTime, false);

            RobotActions.OnTimeUpdated(_currentTime);
            UpdateProgramTargetState(_program.CurrentSimulationPose.TargetIndex);
        }

        void UpdateProgramTargetState(int targetIndex)
        {
            if (targetIndex != _currentTargetIndex)
            {
                _currentTargetIndex = targetIndex;
                UpdateLoadedState();
            }
        }

        void UpdateLoadedState()
        {
            if (_program is null)
                return;
            
            // If the current state of a tool contains the name of the defined gripperLoadedName, then it's current state is loaded
            if (_program.Targets[_currentTargetIndex].ProgramTargets[0].Target.Tool.Name.Contains(gripperLoadedName))
            {
                if (_isLoaded == false)
                {
                    RobotActions.OnToolLoaded();
                }
            }
            
            if (_program.Targets[_currentTargetIndex].ProgramTargets[0].Target.Tool.Name.Contains(gripperUnloadedName))
            {
                if (_isLoaded)
                {
                    RobotActions.OnToolUnloaded();
                }
            }
        }

        void LoadTool()
        {
            _isLoaded = true;
            Debug.Log("Loaded");
        }

        void UnloadTool()
        {
            _isLoaded = false;
            Debug.Log("Unloaded");
        }
    }
}