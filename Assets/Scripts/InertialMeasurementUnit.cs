using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RosSharp.RosBridgeClient.MessageTypes.Std;
using UnityEngine;


public struct InertialMeasurementUnit
{
    private GameObject m_Parent;

    public InertialMeasurementUnit(GameObject parent)
    {
        m_Parent = parent;
    }


}
