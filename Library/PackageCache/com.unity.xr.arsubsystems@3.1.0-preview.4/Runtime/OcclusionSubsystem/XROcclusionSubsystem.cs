using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Defines an interface for interacting with occlusion functionality.
    /// </summary>
    public abstract class XROcclusionSubsystem : XRSubsystem<XROcclusionSubsystemDescriptor>
    {
        /// <summary>
        /// The implementation specific provider of occlusion functionality.
        /// </summary>
        /// <value>
        /// The implementation specific provider of occlusion functionality.
        /// </value>
        Provider m_Provider;

        /// <summary>
        /// Specifies the human segmentation stencil mode.
        /// </summary>
        /// <value>
        /// The human segmentation stencil mode.
        /// </value>
        /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation stencil mode to
        /// enabled if the implementation does not support human segmentation.</exception>
        public SegmentationStencilMode humanSegmentationStencilMode
        {
            get => m_Provider.humanSegmentationStencilMode;
            set => m_Provider.humanSegmentationStencilMode = value;
        }

        /// <summary>
        /// Specifies the human segmentation depth mode.
        /// </summary>
        /// <value>
        /// The human segmentation depth mode.
        /// </value>
        /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation depth mode to
        /// enabled if the implementation does not support human segmentation.</exception>
        public SegmentationDepthMode humanSegmentationDepthMode
        {
            get => m_Provider.humanSegmentationDepthMode;
            set => m_Provider.humanSegmentationDepthMode = value;
        }

        /// <summary>
        /// Construct the subsystem by creating the functionality provider.
        /// </summary>
        public XROcclusionSubsystem() => m_Provider = CreateProvider();

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
        /// Gets the human stencil texture descriptor.
        /// </summary>
        /// <param name="humanStencilDescriptor">The human stencil texture descriptor to be populated, if available
        /// from the provider.</param>
        /// <returns>
        /// <c>true</c> if the human stencil texture descriptor is available and is returned. Otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human stencil
        /// texture.</exception>
        public bool TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor)
            => m_Provider.TryGetHumanStencil(out humanStencilDescriptor);

        /// <summary>
        /// Gets the human depth texture descriptor.
        /// </summary>
        /// <param name="humanDepthDescriptor">The human depth texture descriptor to be populated, if available from
        /// the provider.</param>
        /// <returns>
        /// <c>true</c> if the human depth texture descriptor is available and is returned. Otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human depth
        /// texture.</exception>
        public bool TryGetHumanDepth(out XRTextureDescriptor humanDepthDescriptor)
            => m_Provider.TryGetHumanDepth(out humanDepthDescriptor);

        /// <summary>
        /// Gets the occlusion texture descriptors associated with the current AR frame.
        /// </summary>
        /// <param name="allocator">The allocator to use when creating the returned <c>NativeArray</c>.</param>
        /// <returns>An array of texture descriptors.</returns>
        /// <remarks>
        /// The caller owns the returned <c>NativeArray</c> and is responsible for calling <c>Dispose</c> on it.
        /// </remarks>
        public NativeArray<XRTextureDescriptor> GetTextureDescriptors(Allocator allocator)
            => m_Provider.GetTextureDescriptors(default(XRTextureDescriptor), allocator);

        /// <summary>
        /// Get the enabled and disabled shader keywords for the material.
        /// </summary>
        /// <param name="enabledKeywords">The keywords to enable for the material.</param>
        /// <param name="disabledKeywords">The keywords to disable for the material.</param>
        public void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            => m_Provider.GetMaterialKeywords(out enabledKeywords, out disabledKeywords);

        /// <summary>
        /// Create the implementation specific functionality provider.
        /// </summary>
        /// <returns>
        /// The implementation specific functionality provider.
        /// </returns>
        protected abstract Provider CreateProvider();

        /// <summary>
        /// Register the descriptor for the occlusion subsystem implementation.
        /// </summary>
        /// <param name="occlusionSubsystemCinfo">The occlusion subsystem implementation construction information.
        /// </param>
        /// <returns>
        /// <c>true</c> if the descriptor was registered. Otherwise, <c>false</c>.
        /// </returns>
        public static bool Register(XROcclusionSubsystemCinfo occlusionSubsystemCinfo)
        {
            XROcclusionSubsystemDescriptor occlusionSubsystemDescriptor = XROcclusionSubsystemDescriptor.Create(occlusionSubsystemCinfo);

            return SubsystemRegistration.CreateDescriptor(occlusionSubsystemDescriptor);
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
            /// Property to be implemented by the provider to get/set the human segmentation stencil mode.
            /// </summary>
            /// <value>
            /// The human segmentation stencil mode.
            /// </value>
            /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation stencil mode
            /// to enabled if the implementation does not support human segmentation.</exception>
            public virtual SegmentationStencilMode humanSegmentationStencilMode
            {
                get => SegmentationStencilMode.Disabled;
                set
                {
                    if (value.Enabled())
                    {
                        throw new NotSupportedException("Setting human segmentation stencil to enabled is not "
                                                        + "supported by this implementation");
                    }
                }
            }

            /// <summary>
            /// Property to be implemented by the provider to get/set the human segmentation depth mode.
            /// </summary>
            /// <value>
            /// The human segmentation depth mode.
            /// </value>
            /// <exception cref="System.NotSupportedException">Thrown when setting the human segmentation depth mode
            /// to enabled if the implementation does not support human segmentation.</exception>
            public virtual SegmentationDepthMode humanSegmentationDepthMode
            {
                get => SegmentationDepthMode.Disabled;
                set
                {
                    if (value.Enabled())
                    {
                        throw new NotSupportedException("Setting human segmentation depth to enabled is not supported "
                                                        + "by this implementation");
                    }
                }
            }

            /// <summary>
            /// Method to be implemented by the provider to get the human stencil texture descriptor.
            /// </summary>
            /// <param name="humanStencilDescriptor">The human stencil texture descriptor to be populated, if
            /// available.</param>
            /// <returns>
            /// <c>true</c> if the human stencil texture descriptor is available and is returned. Otherwise,
            /// <c>false</c>.
            /// </returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human
            /// stencil texture.</exception>
            public virtual bool TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor)
                => throw new NotSupportedException("human stencil texture is not supported by this implementation");

            /// <summary>
            /// Method to be implemented by the provider to get the human depth texture descriptor.
            /// </summary>
            /// <param name="humanDepthDescriptor">The human depth texture descriptor to be populated, if available.
            /// </param>
            /// <returns>
            /// <c>true</c> if the human depth texture descriptor is available and is returned. Otherwise,
            /// <c>false</c>.
            /// </returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support human
            /// depth texture.</exception>
            public virtual bool TryGetHumanDepth(out XRTextureDescriptor humanDepthDescriptor)
                => throw new NotSupportedException("human depth texture is not supported by this implementation");

            /// <summary>
            /// Method to be implemented by the provider to get the occlusion texture descriptors associated with the
            /// current AR frame.
            /// </summary>
            /// <param name="defaultDescriptor">The default descriptor value.</param>
            /// <param name="allocator">The allocator to use when creating the returned <c>NativeArray</c>.</param>
            /// <returns>An array of the occlusion texture descriptors.</returns>
            public virtual NativeArray<XRTextureDescriptor> GetTextureDescriptors(XRTextureDescriptor defaultDescriptor,
                                                                                  Allocator allocator)
                => new NativeArray<XRTextureDescriptor>(0, allocator);

            /// <summary>
            /// Method to be implemented by the provider to get the enabled and disabled shader keywords for the
            /// material.
            /// </summary>
            /// <param name="enabledKeywords">The keywords to enable for the material.</param>
            /// <param name="disabledKeywords">The keywords to disable for the material.</param>
            public virtual void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            {
                enabledKeywords = null;
                disabledKeywords = null;
            }
        }
    }
}
