using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  CameraControl
{
    public class BatteryLevelLabel : IObserver
    {
        public string batterylevel;
   
        public void Update(Observable observable, CameraEvent e)
        {
            CameraEvent.Type eventType = CameraEvent.Type.NONE;

            if ((eventType = e.GetEventType()) == CameraEvent.Type.PROPERTY_CHANGED)
            {
                uint propertyID = (uint)e.GetArg();

                if (propertyID == EDSDKLib.EDSDK.PropID_BatteryLevel)
                {

                    //Update property
                    switch (eventType)
                    {
                        case CameraEvent.Type.PROPERTY_CHANGED:

                            CameraModel model = (CameraModel)observable;
                            string infoText = "AC power";
                            if (0xffffffff != model.BatteryLebel)
                            {
                                infoText = model.BatteryLebel.ToString() + "%";
                            }

                            batterylevel = infoText;
                            break;
                    }
                }
            }
        }
    }
    
}
