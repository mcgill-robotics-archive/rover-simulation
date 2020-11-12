namespace RosSharp.RosBridgeClient.MessageTypes.Std
{
    public class MultiArrayDimension : Message
    {
        public const string RosMessageName = "std_msgs/MultiArrayDimension";

        public string label { get; set; }
        //  label of given dimension
        public uint size { get; set; }
        //  size of given dimension (in type units)
        public uint stride { get; set; }
        //  stride of given dimension

        public MultiArrayDimension()
        {
            this.label = "";
            this.size = 0;
            this.stride = 0;
        }

        public MultiArrayDimension(string label, uint size, uint stride)
        {
            this.label = label;
            this.size = size;
            this.stride = stride;
        }
    }
}