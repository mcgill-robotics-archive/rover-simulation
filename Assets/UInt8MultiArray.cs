﻿namespace RosSharp.RosBridgeClient.MessageTypes.Std
{
    public class UInt8MultiArray : Message
    {
        public const string RosMessageName = "std_msgs/UInt8MultiArray";

        //  Please look at the MultiArrayLayout message definition for
        //  documentation on all multiarrays.
        public MultiArrayLayout layout { get; set; }
        //  specification of data layout
        public byte[] data { get; set; }
        //  array of data

        public UInt8MultiArray()
        {
            this.layout = new MultiArrayLayout();
            this.data = new byte[0];
        }

        public UInt8MultiArray(MultiArrayLayout layout, byte[] data)
        {
            this.layout = layout;
            this.data = data;
        }
    }
}