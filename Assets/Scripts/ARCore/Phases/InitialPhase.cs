using GoogleARCore.Examples.CloudAnchors;
using Photon.Pun;

namespace ARCore.Phases
{
    public class InitialPhase : Phase
    {
        private readonly MasterPositioningPhase _masterPositioningPhase;
        private readonly NonMasterPositioningPhase _nonMasterPositioningPhase;

        public InitialPhase(PhaseManager phaseManager,
            MasterPositioningPhase masterPositioningPhase,
            NonMasterPositioningPhase nonMasterPositioningPhase) : base(phaseManager)
        {
            _masterPositioningPhase = masterPositioningPhase;
            _nonMasterPositioningPhase = nonMasterPositioningPhase;
        }

        public override void OnEnter()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhaseManager.ChangePhase(_masterPositioningPhase);
            }
            else
            {
                PhaseManager.ChangePhase(_nonMasterPositioningPhase);
            }
        }
    }
}
