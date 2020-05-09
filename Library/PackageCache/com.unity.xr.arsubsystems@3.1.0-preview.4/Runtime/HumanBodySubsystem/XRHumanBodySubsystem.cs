using System;
using Unity.Collections;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines an interface for interacting with human body functionality.
    /// </summary>
    public abstract class XRHumanBodySubsystem : TrackingSubsystem<XRHumanBody, XRHumanBodySubsystemDescriptor>
    {
        /// <summary>
        /// The implementation specific provider of human body functionality.
        /// </summary>
        /// <value>
        /// The implementation specific provider of human body functionality.
        /// </value>
        Provider m_Provider;

        /// <summary>
        /// Whether 2D human body pose estimation is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if 2D human body pose estimation is enabled. Otherwise, <c>false</c>.
        /// </value>
        public bool humanBodyPose2DEstimationEnabled
        {
            get => m_HumanBodyPose2DEstimationEnabled;
            set
            {
                if ((m_HumanBodyPose2DEstimationEnabled != value)
                    && m_Provider.TrySetHumanBodyPose2DEstimationEnabled(value))
                {
                    m_HumanBodyPose2DEstimationEnabled = value;
                }
            }
        }
        bool m_HumanBodyPose2DEstimationEnabled = false;

        /// <summary>
        /// Whether 3D human body pose estimation is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if 3D human body pose estimation is enabled. Otherwise, <c>false</c>.
        /// </value>
        public bool humanBodyPose3DEstimationEnabled
        {
            get => m_HumanBodyPose3DEstimationEnabled;
            set
            {
                if ((m_HumanBodyPose3DEstimationEnabled != value)
                    && m_Provider.TrySetHumanBodyPose3DEstimationEnabled(value))
                {
                    m_HumanBodyPose3DEstimationEnabled = value;
                }
            }
        }
        bool m_HumanBodyPose3DEstimationEnabled = false;

        /// <summary>
        /// Whether 3D human body scale estimation is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if 3D human body scale estimation is enabled. Otherwise, <c>false</c>.
        /// </value>
        public bool humanBodyPose3DScaleEstimationEnabled
        {
            get => m_HumanBodyPose3DScaleEstimationEnabled;
            set
            {
                if ((m_HumanBodyPose3DScaleEstimationEnabled != value)
                    && m_Provider.TrySetHumanBodyPose3DScaleEstimationEnabled(value))
                {
                    m_HumanBodyPose3DScaleEstimationEnabled = value;
                }
            }
        }
        bool m_HumanBodyPose3DScaleEstimationEnabled = false;

        /// <summary>
        /// Construct the subsystem by creating the functionality provider.
        /// </summary>
        public XRHumanBodySubsystem() => m_Provider = CreateProvider();

        /// <summary>
        /// Start the subsystem by starting the provider.
        /// </summary>
        protected sealed override void OnStart() => m_Provider.Start();

        /// <summary>
        /// Stop the subsystem by stopping the provider.
        /// </summary>
        protected sealed override void OnStop() => m_Provider.Stop();

        /// <summary>
        /// Destroy the subsystem by desstroying the provider.
        /// </summary>
        protected sealed override void OnDestroyed() => m_Provider.Destroy();

        /// <summary>
        /// Query the provider for the trackable changes.
        /// </summary>
        /// <param name="allocator">The memory allocator to use for allocating the arrays.</param>
        /// <returns>
        /// The trackable human body changes.
        /// </returns>
        public override TrackableChanges<XRHumanBody> GetChanges(Allocator allocator)
            => m_Provider.GetChanges(XRHumanBody.defaultValue, allocator);

        /// <summary>
        /// Query the provider for the skeleton joints for the requested trackable identifier.
        /// </summary>
        /// <param name="trackableId">The human body trackable identifier for which to query.</param>
        /// <param name="allocator">The memory allocator to use for the returned arrays.</param>
        /// <param name="skeleton">The array of skeleton joints to update and returns.</param>
        public void GetSkeleton(TrackableId trackableId, Allocator allocator, ref NativeArray<XRHumanBodyJoint> skeleton)
            => m_Provider.GetSkeleton(trackableId, allocator, ref skeleton);

        /// <summary>
        /// Gets the human body pose 2D joints for the current frame.
        /// </summary>
        /// <param name="allocator">The allocator to use for the returned array memory.</param>
        /// <returns>
        /// The array of body pose 2D joints.
        /// </returns>
        /// <remarks>
        /// The returned array may be empty if the system is not enabled for human body pose 2D or if the system
        /// does not detect a human in the camera image.
        /// </remarks>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human body
        /// pose 2D.</exception>
        public NativeArray<XRHumanBodyPose2DJoint> GetHumanBodyPose2DJoints(Allocator allocator)
            => m_Provider.GetHumanBodyPose2DJoints(default(XRHumanBodyPose2DJoint), Screen.width, Screen.height,
                                                   Screen.orientation, allocator);

        /// <summary>
        /// Create the implementation specific functionality provider.
        /// </summary>
        /// <returns>
        /// The implementation specific functionality provider.
        /// </returns>
        protected abstract Provider CreateProvider();

        /// <summary>
        /// Register the descriptor for the human body subsystem implementation.
        /// </summary>
        /// <param name="humanBodySubsystemCinfo">The human body subsystem implementation construction information.
        /// </param>
        /// <returns>
        /// <c>true</c> if the descriptor was registered. Otherwise, <c>false</c>.
        /// </returns>
        public static bool Register(XRHumanBodySubsystemCinfo humanBodySubsystemCinfo)
        {
            XRHumanBodySubsystemDescriptor humanBodySubsystemDescriptor = XRHumanBodySubsystemDescriptor.Create(humanBodySubsystemCinfo);

            return SubsystemRegistration.CreateDescriptor(humanBodySubsystemDescriptor);
        }

        protected abstract class Provider
        {
            /// <summary>
            /// Method to be implemented by the provider to start the functionality.
            /// </summary>
            public virtual void Start() { }

            /// <summary>
            /// Method to be implemented by the provider to stop the functionality.
            /// </summary>
            public virtual void Stop() { }

            /// <summary>
            /// Method to be implemented by the provider to destroy itself and release any resources.
            /// </summary>
            public virtual void Destroy() { }

            /// <summary>
            /// Method to be implemented by the provider to sets whether human body pose 2D estimation is enabled.
            /// </summary>
            /// <param name="enabled">Whether the human body pose 2D estimation should be enabled.</param>
            /// <returns>
            /// <c>true</c> if the human body pose 2D estimation is set to the given value. Otherwise, <c>false</c>.
            /// </returns>
            /// <exception cref="System.NotSupportedException">Thrown when setting the human body pose 2D estimation to
            /// <c>true</c> if the implementation does not support human body pose 2D estimation.</exception>
            public virtual bool TrySetHumanBodyPose2DEstimationEnabled(bool enabled)
            {
                if (enabled)
                {
                    throw new NotSupportedException("Setting human body pose 2D estimation to enabled is not "
                                                    + "supported by this implementation");
                }

                return false;
            }

            /// <summary>
            /// Method to be implemented by the provider to sets whether human body pose 3D estimation is enabled.
            /// </summary>
            /// <param name="enabled">Whether the human body pose 3D estimation should be enabled.</param>
            /// <returns>
            /// <c>true</c> if the human body pose 3D estimation is set to the given value. Otherwise, <c>false</c>.
            /// </returns>
            /// <exception cref="System.NotSupportedException">Thrown when setting the human body pose 3D estimation to
            /// <c>true</c> if the implementation does not support human body pose 3D estimation.</exception>
            public virtual bool TrySetHumanBodyPose3DEstimationEnabled(bool enabled)
            {
                if (enabled)
                {
                    throw new NotSupportedException("Setting human body pose 3D estimation to enabled is not "
                                                    + "supported by this implementation");
                }

                return false;
            }

            /// <summary>
            /// Method to be implemented by the provider to sets whether 3D human body scale estimation is enabled.
            /// </summary>
            /// <param name="enabled">Whether the 3D human body scale estimation should be enabled.</param>
            /// <returns>
            /// <c>true</c> if the 3D human body scale estimation is set to the given value. Otherwise, <c>false</c>.
            /// </returns>
            /// <exception cref="System.NotSupportedException">Thrown when setting the 3D human body scale estimation to
            /// <c>true</c> if the implementation does not support 3D human body scale estimation.</exception>
            public virtual bool TrySetHumanBodyPose3DScaleEstimationEnabled(bool enabled)
            {
                if (enabled)
                {
                    throw new NotSupportedException("Setting 3D human body scale estimation to enabled is not "
                                                    + "supported by this implementation");
                }

                return false;
            }

            /// <summary>
            /// Method to be implemented by the provider to query for the set of human body changes.
            /// </summary>
            /// <param name="defaultHumanBody">The default human body.</param>
            /// <param name="allocator">The memory allocator to use for the returns trackable changes.</param>
            /// <returns>
            /// The set of human body changes.
            /// </returns>
            /// <exception cref="System.NotSupportedException">Thrown for platforms not supporting human body pose
            /// estimation.</exception>
            public abstract TrackableChanges<XRHumanBody> GetChanges(XRHumanBody defaultHumanBody, Allocator allocator);

            /// <summary>
            /// Method to be implemented by the provider to get the skeleton joints for the requested trackable identifier.
            /// </summary>
            /// <param name="trackableId">The human body trackable identifier for which to query.</param>
            /// <param name="allocator">The memory allocator to use for the returned arrays.</param>
            /// <param name="skeleton">The array of skeleton joints to update and return.</param>
            /// <exception cref="System.NotSupportedException">Thrown for platforms not supporting human body pose 3D.
            /// </exception>
            public virtual void GetSkeleton(TrackableId trackableId, Allocator allocator, ref NativeArray<XRHumanBodyJoint> skeleton)
                => throw new NotSupportedException("Skeletons are not supported by this implementation.");

            /// <summary>
            /// Method to be implemented by the provider to get the human body pose 2D joints for the current frame.
            /// </summary>
            /// <param name="defaultHumanBodyPose2DJoint">The default value for the body pose 2D joint.</param>
            /// <param name="screenWidth">The width of the screen, in pixels.</param>
            /// <param name="screenHeight">The height of the screen, in pixels.</param>
            /// <param name="screenOrientation">The orientation of the device so that the joint positions may be
            /// adjusted as required.</param>
            /// <param name="allocator">The allocator to use for the returned array memory.</param>
            /// <returns>
            /// The array of body pose 2D joints.
            /// </returns>
            /// <remarks>
            /// The returned array may be empty if the system is not enabled for human body pose 2D or if the system
            /// does not detect a human in the camera image.
            /// </remarks>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human body
            /// pose 2D.</exception>
            public virtual NativeArray<XRHumanBodyPose2DJoint> GetHumanBodyPose2DJoints(XRHumanBodyPose2DJoint defaultHumanBodyPose2DJoint,
                                                                                        int screenWidth,
                                                                                        int screenHeight,
                                                                                        ScreenOrientation screenOrientation,
                                                                                        Allocator allocator)
                => throw new NotSupportedException("human body pose 2D is not supported by this implementation");
        }
    }
}
