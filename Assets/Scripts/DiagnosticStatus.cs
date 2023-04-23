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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ROS2
{

/// <summary>
/// An example class provided for testing of basic ROS2 communication
/// </summary>
public class DiagnosticSubscriber : MonoBehaviour
{
    private ROS2UnityComponent ros2;
    private ROS2Node node;
    private ISubscription<diagnostic_msgs.msg.DiagnosticStatus> diagnostic_sub;
    private diagnostic_msgs.msg.DiagnosticStatus message;

    public string topicName = "/diagnostic";

    void Start()
    {
        ros2 = GetComponent<ROS2UnityComponent>();
        if (node == null && ros2.Ok())
        {
            node = ros2.CreateNode("diagnostic_node");
            diagnostic_sub = node.CreateSubscription<diagnostic_msgs.msg.DiagnosticStatus>(topicName, OnDiagnosticCallback, new QualityOfServiceProfile(QosPresetProfile.SENSOR_DATA));    
        }
    }

    void Update()
    {   
        if (message != null)
        {
            var bus_voltage = message.Values[0].Value;
            var current = message.Values[1].Value;
            var power = message.Values[2].Value;
            var percentage = message.Values[3].Value;

            var textMesh = GetComponent<TMPro.TextMeshProUGUI>();
            textMesh.text = percentage;
            switch (message.Level)
            {
                case diagnostic_msgs.msg.DiagnosticStatus.OK:
                    textMesh.color = Color.green;
                    break;
                case diagnostic_msgs.msg.DiagnosticStatus.WARN:
                    textMesh.color = Color.yellow;
                    break;
                case diagnostic_msgs.msg.DiagnosticStatus.ERROR:
                    textMesh.color = Color.red;
                    break;
                case diagnostic_msgs.msg.DiagnosticStatus.STALE:
                    break;
                default:
                    break;
            }
        }
    }

    void OnDiagnosticCallback(diagnostic_msgs.msg.DiagnosticStatus message)
    {
        switch (message.Level)
        {
            case diagnostic_msgs.msg.DiagnosticStatus.OK:
                Debug.LogFormat("{0}: {1}", message.Name, message.Message);
                break;
            case diagnostic_msgs.msg.DiagnosticStatus.WARN:
                Debug.LogFormat("{0}: {1}", message.Name, message.Message);
                break;
            case diagnostic_msgs.msg.DiagnosticStatus.ERROR:
                Debug.LogFormat("{0}: {1}", message.Name, message.Message);
                break;
            case diagnostic_msgs.msg.DiagnosticStatus.STALE:
                Debug.LogFormat("{0}: {1}", message.Name, message.Message);
                break;
            default:
                break;
        }
        foreach (var value in message.Values)
        {
            Debug.LogFormat("{0}: {1}", value.Key, value.Value);
        }
        this.message = message;
    }   
}

}  // namespace ROS2
