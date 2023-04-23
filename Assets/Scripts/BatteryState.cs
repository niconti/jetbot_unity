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
public class BatteryState : MonoBehaviour
{
    private ROS2UnityComponent ros2;
    private ROS2Node node;
    private ISubscription<sensor_msgs.msg.BatteryState> battery_state_sub;
    private sensor_msgs.msg.BatteryState message;

    float LOW_CHARGE = 15f;
    float CRITICAL_CHARGE = 5f;

    public string topicName = "/battery_state";

    [SerializeField]
    private GameObject powerIcon;
    // [SerializeField]
    // private TextMeshProUGUI textMesh;

    void Start()
    {
        ros2 = GetComponent<ROS2UnityComponent>();
        if (node == null && ros2.Ok())
        {
            node = ros2.CreateNode("battery_state_node");
            battery_state_sub = node.CreateSubscription<sensor_msgs.msg.BatteryState>(topicName, OnBatteryStateCallback, new QualityOfServiceProfile(QosPresetProfile.SENSOR_DATA));    
        }
    }

    void Update()
    {   
        if (message != null)
        {
            switch (message.Power_supply_status)
            {
                case sensor_msgs.msg.BatteryState.POWER_SUPPLY_STATUS_CHARGING:
                    powerIcon.SetActive(true);
                    break;
                case sensor_msgs.msg.BatteryState.POWER_SUPPLY_STATUS_DISCHARGING:
                    powerIcon.SetActive(false);
                    break;
            }

            var textMesh = GetComponentInChildren<TextMeshProUGUI>();
            float percentage = message.Percentage * 100f;
            textMesh.text = String.Format("{0:F0}%", percentage);
            textMesh.color = Color.green;
            if (percentage < LOW_CHARGE) textMesh.color = Color.yellow;
            if (percentage < CRITICAL_CHARGE) textMesh.color = Color.red;
        }
    }

    void OnBatteryStateCallback(sensor_msgs.msg.BatteryState message)
    {
        this.message = message;
    }   
}

}  // namespace ROS2
