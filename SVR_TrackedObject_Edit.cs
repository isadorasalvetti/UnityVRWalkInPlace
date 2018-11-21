//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
// MODIFYED.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

namespace Valve.VR
{
    public class SVR_TrackedObject_Edit : MonoBehaviour{
        public bool leftFoot;
        
        public enum EIndex
        {
            None = -1,
            Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
            Device1,
            Device2,
            Device3,
            Device4,
            Device5,
            Device6,
            Device7,
            Device8,
            Device9,
            Device10,
            Device11,
            Device12,
            Device13,
            Device14,
            Device15
        }

        public EIndex index;

        public bool isValid { get; private set; }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            if (index == EIndex.None)
                return;

            var i = (int)index;

            isValid = false;
            if (poses.Length <= i)
                return;

            if (!poses[i].bDeviceIsConnected)
                return;

            if (!poses[i].bPoseIsValid)
                return;

            isValid = true;

            //Collect the tracker information and seend to the walking controller.
            /* Modification on original Tracked Objects - Allows script to be placed directly on the movement controller (twice)
               and passes the values directly to the correct script. */
            var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
            if (leftFoot){
            GetComponent<ControllWalking>().currentLeftPosition = pose.pos.y;
            GetComponent<ControllWalking>().currentLeftRotation = pose.rot;
            } else {
            GetComponent<ControllWalking>().currentRightPosition = pose.pos.y;
            GetComponent<ControllWalking>().currentRightRotation = pose.rot;                   
            }

        }

        SteamVR_Events.Action newPosesAction;

        SVR_TrackedObject_Edit()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        private void Awake()
        {
            OnEnable();
        }

        void OnEnable()
        {
            var render = SteamVR_Render.instance;
            if (render == null)
            {
                enabled = false;
                return;
            }

            newPosesAction.enabled = true;
        }

        void OnDisable()
        {
            newPosesAction.enabled = false;
            isValid = false;
        }

        public void SetDeviceIndex(int index)
        {
            if (System.Enum.IsDefined(typeof(EIndex), index))
                this.index = (EIndex)index;
        }
    }
}