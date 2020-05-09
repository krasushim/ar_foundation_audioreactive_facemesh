using System;

namespace UnityEngine.XR.ARSubsystems
{
    public struct XROcclusionSubsystemCinfo : IEquatable<XROcclusionSubsystemCinfo>
    {
        /// <summary>
        /// Specifies an identifier for the provider implementation of the subsystem.
        /// </summary>
        /// <value>
        /// The identifier for the provider implementation of the subsystem.
        /// </value>
        public string id { get; set; }

        /// <summary>
        /// Specifies the provider implementation type to use for instantiation.
        /// </summary>
        /// <value>
        /// Specifies the provider implementation type to use for instantiation.
        /// </value>
        public Type implementationType { get; set; }

        /// <summary>
        /// Specifies if the current subsystem supports human segmentation stencil image.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports human segmentation stencil image. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanSegmentationStencilImage { get; set; }

        /// <summary>
        /// Specifies if the current subsystem supports human segmentation depth image.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports human segmentation depth image. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanSegmentationDepthImage { get; set; }

        public bool Equals(XROcclusionSubsystemCinfo other)
            => (id.Equals(other.id) && implementationType.Equals(other.implementationType)
                && supportsHumanSegmentationStencilImage.Equals(other.supportsHumanSegmentationStencilImage)
                && supportsHumanSegmentationDepthImage.Equals(other.supportsHumanSegmentationDepthImage));

        public override bool Equals(System.Object obj)
            => ((obj is XROcclusionSubsystemCinfo) && Equals((XROcclusionSubsystemCinfo)obj));

        public static bool operator ==(XROcclusionSubsystemCinfo lhs, XROcclusionSubsystemCinfo rhs)
            => lhs.Equals(rhs);

        public static bool operator !=(XROcclusionSubsystemCinfo lhs, XROcclusionSubsystemCinfo rhs)
            => !lhs.Equals(rhs);

        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + ((id == null) ? 0 : id.GetHashCode());
                hashCode = (hashCode * 486187739) + ((implementationType == null) ? 0 : implementationType.GetHashCode());
                hashCode = (hashCode * 486187739) + supportsHumanSegmentationStencilImage.GetHashCode();
                hashCode = (hashCode * 486187739) + supportsHumanSegmentationDepthImage.GetHashCode();
            }
            return hashCode;
        }
    }

    /// <summary>
    /// Descritptor for the XROcclusionSubsystem.
    /// </summary>
    public class XROcclusionSubsystemDescriptor : SubsystemDescriptor<XROcclusionSubsystem>
    {
        XROcclusionSubsystemDescriptor(XROcclusionSubsystemCinfo occlusionSubsystemCinfo)
        {
            id = occlusionSubsystemCinfo.id;
            subsystemImplementationType = occlusionSubsystemCinfo.implementationType;
            supportsHumanSegmentationStencilImage = occlusionSubsystemCinfo.supportsHumanSegmentationStencilImage;
            supportsHumanSegmentationDepthImage = occlusionSubsystemCinfo.supportsHumanSegmentationDepthImage;
        }

        /// <summary>
        /// Specifies if the current subsystem supports human segmentation stencil image.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports human segmentation stencil image. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanSegmentationStencilImage { get; private set; }

        /// <summary>
        /// Specifies if the current subsystem supports human segmentation depth image.
        /// </summary>
        /// <value>
        /// <c>true</c> if the current subsystem supports human segmentation depth image. Otherwise, <c>false</c>.
        /// </value>
        public bool supportsHumanSegmentationDepthImage { get; private set; }

        /// <summary>
        /// Creates the occlusion subsystem descriptor from the construction info.
        /// </summary>
        /// <param name="occlusionSubsystemCinfo">The occlusion subsystem descriptor constructor information.</param>
        internal static XROcclusionSubsystemDescriptor Create(XROcclusionSubsystemCinfo occlusionSubsystemCinfo)
        {
            if (String.IsNullOrEmpty(occlusionSubsystemCinfo.id))
            {
                throw new ArgumentException("Cannot create occlusion subsystem descriptor because id is invalid",
                                            "occlusionSubsystemCinfo");
            }

            if ((occlusionSubsystemCinfo.implementationType == null)
                || !occlusionSubsystemCinfo.implementationType.IsSubclassOf(typeof(XROcclusionSubsystem)))
            {
                throw new ArgumentException("Cannot create occlusion subsystem descriptor because implementationType is invalid",
                                            "occlusionSubsystemCinfo");
            }

            return new XROcclusionSubsystemDescriptor(occlusionSubsystemCinfo);
        }
    }
}
