using System.IO;
using ARCore.Phases;
using ARCore.Phases.Combat;
using ARCore.Phases.Instantiating;
using GoogleARCore.Examples.CloudAnchors;
using Photon;
using Photon.Countdown;
using Photon.Pun;
using UI;
using UI.Combat;
using UnityEngine;
using WFC;

namespace ARCore
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] private CloudAnchorsExampleController anchorsExampleController;

        [Header("UI")]
        [SerializeField] private NetworkUIController networkUi;
        [SerializeField] private PlayerUI playerUI;
        [SerializeField] private EndGameScreen endGameScreen;

        [Header("Test flags")]
        [SerializeField] private bool skipAR;
        [SerializeField] private bool skipGameArea;
        [SerializeField] private bool skipCombat;

        [Header("Settings")]
        [SerializeField] private float countdownTime;
        [SerializeField] private FinishCountdownEvent finishCountdownEvent;

        private Phase _currentState;
        private ObstacleGenerator _obstacleGenerator;
        public ObstacleGenerator ObstacleGenerator =>
            _obstacleGenerator ? _obstacleGenerator : _obstacleGenerator = FindObjectOfType<ObstacleGenerator>();

        public EndGameScreen EndGameScreen => endGameScreen;

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)
                InstantiateObstacleGenerator();
        }

        private void Start()
        {
            PhotonRoom.Instance.OnOpponentDisconnect += OpponentDisconnect;
            InitializePhases();
        }

        public void ChangePhase(Phase newPhase)
        {
            _currentState?.OnExit();
            _currentState = newPhase;
            _currentState.OnEnter();
        }

        private void InitializePhases()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ChangePhase(InstantiateMasterPhases());
            }
            else
            {
                ChangePhase(InstantiateNonMasterPhases());
            }
        }

        private void InstantiateObstacleGenerator()
        {
            _obstacleGenerator = PhotonNetwork
                .Instantiate(Path.Combine("WFC Prefabs", "Obstacle Generator"), Vector3.zero, Quaternion.identity)
                .GetComponent<ObstacleGenerator>();
        }

        private NonMasterPositioningPhase InstantiateNonMasterPhases()
        {
            var combatPhase = new CombatPhase(this, playerUI, skipCombat, countdownTime, finishCountdownEvent);
            var nonMasterInstantiatingPhase =
                new NonMasterInstantiatingPhase(this, networkUi, combatPhase);
            return new NonMasterPositioningPhase(this, networkUi, anchorsExampleController,
                nonMasterInstantiatingPhase);
        }

        private MasterPositioningPhase InstantiateMasterPhases()
        {
            var combatPhase = new CombatPhase(this, playerUI, skipCombat, countdownTime, finishCountdownEvent);
            var masterInstantiatingPhase =
                new MasterInstantiatingPhase(this, networkUi, anchorsExampleController, ObstacleGenerator,
                    combatPhase, skipAR);
            return new MasterPositioningPhase(this, networkUi, anchorsExampleController, masterInstantiatingPhase, skipGameArea);
        }

        private void OpponentDisconnect()
        {
            _currentState.OpponentLeft();
        }
    }
}
