// Copyright 2019-2021 Robotec.ai.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;
using Unity.Collections;

namespace ROS2
{

/// <summary>
/// An example class provided for testing of basic ROS2 communication
/// </summary>
public class ImuPublisher : MonoBehaviour
{
    private ROS2UnityComponent ros2;
    private ROS2Node node;
    private IPublisher<sensor_msgs.msg.Imu> imu_pub;

    public string topicName = "/imu";

    void Start()
    {
        ros2 = GetComponent<ROS2UnityComponent>();
        if (node == null && ros2.Ok())
        {
            node = ros2.CreateNode("imu_node");
            imu_pub = node.CreatePublisher<sensor_msgs.msg.Imu>(topicName);
        }
    }

    void Update()
    {   
        Input.gyro.enabled = true;
        
        Quaternion attitude = Input.gyro.attitude;
        float roll  = attitude.eulerAngles.x;
        float pitch = attitude.eulerAngles.y;
        float yaw   = attitude.eulerAngles.z;
        Debug.LogFormat("Gyro RPY: {0:F1} {1:F1} {2:F1}", roll, pitch, yaw);

        // transform.rotation = GyroToUnity(Input.gyro.attitude);
        
        var message = new sensor_msgs.msg.Imu();
        message.Orientation.X = attitude.x;
        message.Orientation.Y = attitude.y;
        message.Orientation.Z = attitude.z;
        message.Orientation.W = attitude.w;
        imu_pub.Publish(message);
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }
  
}

}  // namespace ROS2
