using System;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Defines which keyword will be used when framing a function window.
    /// </summary>
    public enum FrameType
    {
        /// <summary>
        /// Specifies that the default frame type will be used, which is ROWS.
        /// </summary>
        Default,
        /// <summary>
        /// Specifies that the ROWS keyword will be used.
        /// </summary>
        Row,
        /// <summary>
        /// Specifies that the RANGE keyword will be used.
        /// </summary>
        Range,
    }

    /// <summary>
    /// Converts between representations of frame types.
    /// </summary>
    internal sealed class FrameTypeConverter
    {
        /// <summary>
        /// Initializes a new instance of a FrameTypeConverter.
        /// </summary>
        public FrameTypeConverter()
        {
        }

        /// <summary>
        /// Converts the given value to its string representation.
        /// </summary>
        /// <param name="value">The type of the frame.</param>
        /// <returns>The string representing the given frame type.</returns>
        public string ToString(FrameType value)
        {
            switch (value)
            {
                case FrameType.Default:
                case FrameType.Row:
                    return "ROWS";
                case FrameType.Range:
                    return "RANGE";
                default:
                    throw new ArgumentException("Encountered an unknown windowed function frame type.");
            }
        }
    }
}
