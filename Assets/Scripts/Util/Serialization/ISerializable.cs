using Rover.Util.IOStream;

namespace Rover
{
    /// <summary>
    /// Implement this interface for all custom ROS message types
    /// </summary>
    public interface ISerializable
    {
        void Serialize(ByteArrayOutputStream ostream);
        void Deserialize(ByteArrayInputStream istream);
    }
}

