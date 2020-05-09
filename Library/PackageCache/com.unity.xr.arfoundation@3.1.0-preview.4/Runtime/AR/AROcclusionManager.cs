using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// The manager for the occlusion subsystem.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ARUpdateOrder.k_OcclusionManager)]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/api/UnityEngine.XR.ARFoundation.AROcclusionManager.html")]
    public sealed class AROcclusionManager : SubsystemLifecycleManager<XROcclusionSubsystem, XROcclusionSubsystemDescriptor>
    {
        /// <summary>
        /// The list of occlusion texture infos.
        /// </summary>
        /// <value>
        /// The list of occlusion texture infos.
        /// </value>
        readonly List<ARTextureInfo> m_TextureInfos = new List<ARTextureInfo>();

        /// <summary>
        /// The list of occlustion textures.
        /// </summary>
        /// <value>
        /// The list of occlustion textures.
        /// </value>
        readonly List<Texture2D> m_Textures = new List<Texture2D>();

        /// <summary>
        /// The list of occlustion texture property IDs.
        /// </summary>
        /// <value>
        /// The list of occlustion texture property IDs.
        /// </value>
        readonly List<int> m_TexturePropertyIds = new List<int>();

        /// <summary>
        /// An event which fires each time a occlusioncamera frame is received.
        /// </summary>
        public event Action<AROcclusionFrameEventArgs> frameReceived;

        /// <summary>
        /// The mode for generating the human segmentation stencil texture.
        /// </summary>
        /// <value>
        /// The mode for generating the human segmentation stencil texture.
        /// </value>
        public SegmentationStencilMode humanSegmentationStencilMode
        {
            get => m_HumanSegmentationStencilMode;
            set
            {
                m_HumanSegmentationStencilMode = value;
                if (enabled && subsystem != null)
                {
                    subsystem.humanSegmentationStencilMode = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("The mode for generating human segmentation stencil texture.\n\n"
                 + "Disabled -- No human stencil texture produced.\n"
                 + "Fastest -- Minimal rendering quality. Minimal frame computation.\n"
                 + "Medium -- Medium rendering quality. Medium frame computation.\n"
                 + "Best -- Best rendering quality. Increased frame computation.")]
        SegmentationStencilMode m_HumanSegmentationStencilMode = SegmentationStencilMode.Fastest;

        /// <summary>
        /// The mode for generating the human segmentation depth texture.
        /// </summary>
        /// <value>
        /// The mode for generating the human segmentation depth texture.
        /// </value>
        public SegmentationDepthMode humanSegmentationDepthMode
        {
            get => m_HumanSegmentationDepthMode;
            set
            {
                m_HumanSegmentationDepthMode = value;
                if (enabled && subsystem != null)
                {
                    subsystem.humanSegmentationDepthMode = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("The mode for generating human segmentation depth texture.\n\n"
                 + "Disabled -- No human depth texture produced.\n"
                 + "Fastest -- Minimal rendering quality. Minimal frame computation.\n"
                 + "Best -- Best rendering quality. Increased frame computation.")]
        SegmentationDepthMode m_HumanSegmentationDepthMode = SegmentationDepthMode.Fastest;

        /// <summary>
        /// The human segmentation stencil texture.
        /// </summary>
        /// <value>
        /// The human segmentation stencil texture, if any. Otherwise, <c>null</c>.
        /// </value>
        public Texture2D humanStencilTexture
        {
            get
            {
                if ((subsystem != null) && subsystem.TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor))
                {
                    ARTextureInfo textureInfo = new ARTextureInfo(humanStencilDescriptor);
                    return textureInfo.texture;
                }
                return null;
            }
        }

        /// <summary>
        /// The human segmentation depth texture.
        /// </summary>
        /// <value>
        /// The human segmentation depth texture, if any. Otherwise, <c>null</c>.
        /// </value>
        public Texture2D humanDepthTexture
        {
            get
            {
                if ((subsystem != null) && subsystem.TryGetHumanDepth(out XRTextureDescriptor humanDepthDescriptor))
                {
                    ARTextureInfo textureInfo = new ARTextureInfo(humanDepthDescriptor);
                    return textureInfo.texture;
                }
                return null;
            }
        }

        /// <summary>
        /// Callback before the subsystem is started (but after it is created).
        /// </summary>
        protected override void OnBeforeStart()
        {
            subsystem.humanSegmentationStencilMode = m_HumanSegmentationStencilMode;
            subsystem.humanSegmentationDepthMode = m_HumanSegmentationDepthMode;
        }

        /// <summary>
        /// Callback as the manager is being updated.
        /// </summary>
        public void Update()
        {
            if (subsystem != null)
            {
                UpdateTexturesInfos();
                InvokeFrameReceived();
            }
        }

        /// <summary>
        /// Pull the texture descriptors from the occlusion subsystem, and update the texture information maintained by
        /// this component.
        /// </summary>
        void UpdateTexturesInfos()
        {
            var textureDescriptors = subsystem.GetTextureDescriptors(Allocator.Temp);
            try
            {
                int numUpdated = Math.Min(m_TextureInfos.Count, textureDescriptors.Length);

                // Update the existing textures that are in common between the two arrays.
                for (int i = 0; i < numUpdated; ++i)
                {
                    m_TextureInfos[i] = ARTextureInfo.GetUpdatedTextureInfo(m_TextureInfos[i], textureDescriptors[i]);
                }

                // If there are fewer textures in the current frame than we had previously, destroy any remaining unneeded
                // textures.
                if (numUpdated < m_TextureInfos.Count)
                {
                    for (int i = numUpdated; i < m_TextureInfos.Count; ++i)
                    {
                        m_TextureInfos[i].Reset();
                    }
                    m_TextureInfos.RemoveRange(numUpdated, (m_TextureInfos.Count - numUpdated));
                }
                // Else, if there are more textures in the current frame than we have previously, add new textures for any
                // additional descriptors.
                else if (textureDescriptors.Length > m_TextureInfos.Count)
                {
                    for (int i = numUpdated; i < textureDescriptors.Length; ++i)
                    {
                        m_TextureInfos.Add(new ARTextureInfo(textureDescriptors[i]));
                    }
                }
            }
            finally
            {
                if (textureDescriptors.IsCreated)
                {
                    textureDescriptors.Dispose();
                }
            }
        }

        /// <summary>
        /// Invoke the occlusion frame received event with the updated textures and texture property IDs.
        /// </summary>
        void InvokeFrameReceived()
        {
            if (frameReceived != null)
            {
                int numTextureInfos = m_TextureInfos.Count;

                m_Textures.Clear();
                m_TexturePropertyIds.Clear();

                m_Textures.Capacity = numTextureInfos;
                m_TexturePropertyIds.Capacity = numTextureInfos;

                for (int i = 0; i < numTextureInfos; ++i)
                {
                    m_Textures.Add(m_TextureInfos[i].texture);
                    m_TexturePropertyIds.Add(m_TextureInfos[i].descriptor.propertyNameId);
                }

                subsystem.GetMaterialKeywords(out List<string> enabledMaterialKeywords, out List<string>disabledMaterialKeywords);

                AROcclusionFrameEventArgs args = new AROcclusionFrameEventArgs();
                args.textures = m_Textures;
                args.propertyNameIds = m_TexturePropertyIds;
                args.enabledMaterialKeywords = enabledMaterialKeywords;
                args.disabledMaterialKeywords = disabledMaterialKeywords;

                frameReceived(args);
            }
        }
    }
}
