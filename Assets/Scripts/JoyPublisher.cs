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
using UnityEngine.UI;
using Unity.Collections;

namespace ROS2
{

/// <summary>
/// An example class provided for testing of basic ROS2 communication
/// </summary>
public class JoyPublisher : MonoBehaviour
{
    private ROS2UnityComponent ros2;
    private ROS2Node node;
    private IPublisher<sensor_msgs.msg.Joy> joy_pub;

    float timeElapsed;

    public string topicName = "/joy";
    public float publishFrequency = 10f;

    bool motorEnabled = false;
    bool motorToggle = false;
    bool turboEnabled = false;
    bool centerCamera = false;

    [SerializeField]
    private GameObject motorIcon;
    [SerializeField]
    private GameObject turboIcon;


    void Start()
    {
        ros2 = GetComponent<ROS2UnityComponent>();
        if (node == null && ros2.Ok())
        {
            node = ros2.CreateNode("joy_node");
            joy_pub = node.CreatePublisher<sensor_msgs.msg.Joy>(topicName);
        }
    }

    
    void Update()
    {
        updateMotor();

        timeElapsed += Time.deltaTime;
        if (timeElapsed > (1f/publishFrequency))
        {
            var move = getMoveInput();
            var mouse = getMouseInput();
            var arrow = getArrowInput();

            bool LB = Input.GetButton("Enable Turbo");
            bool RB = Input.GetButton("Enable Motor");
            Debug.Log(String.Format("LB RB: {0} {1}", LB, RB));

            bool RS = Input.GetButton("Center Camera");
            Debug.Log(String.Format("RS: {0}", RS));

            turboEnabled = LB;
            motorEnabled = RB || motorToggle;
            centerCamera = RS || Input.GetButton("Fire2");;

            var message = new sensor_msgs.msg.Joy();
            message.Axes = new float[8];
            message.Axes[0] = move.x;
            message.Axes[1] = move.y;
            message.Axes[3] = mouse.x + arrow.x;
            message.Axes[4] = mouse.y + arrow.y;
            message.Buttons = new int[12];
            message.Buttons[4] = turboEnabled ? 1 : 0;
            message.Buttons[5] = motorEnabled ? 1 : 0;
            message.Buttons[10] = centerCamera ? 1 : 0;

            joy_pub.Publish(message);
            timeElapsed = 0;
        }

        updateIcon();
    }


    void updateMotor()
    {
        if (Input.GetKeyDown("space"))
        {
            motorToggle = !motorToggle;
            Debug.Log(String.Format("Motor Toggle: {0}", motorToggle));
        }
    }


    Vector2 getMoveInput()
    {
        float x = Input.GetAxis("Horizontal"); 
        float y = Input.GetAxis("Vertical");
        Debug.Log(String.Format("XY: {0:F2} {1:F2}", x, y));

        return new Vector2(x, y);
    }

    Vector2 getMouseInput(float speed=1.0f)
    {
        float pan = 0f;
        float tilt = 0f;

        bool fire = Input.GetButton("Fire1");
        if (fire)
        {
            pan = speed * Input.GetAxis("Mouse X");
            tilt = speed * Input.GetAxis("Mouse Y");
        }
        Debug.Log(String.Format("Pan-Tilt: {0:F2} {1:F2}", pan, tilt));

        return new Vector2(pan, tilt);
    }

    Vector2 getArrowInput()
    {
        float pan = Input.GetAxis("Pan Camera"); 
        float tilt = Input.GetAxis("Tilt Camera");
        Debug.Log(String.Format("Pan-Tilt: {0:F2} {1:F2}", pan, tilt));

        return new Vector2(pan, tilt);
    }


    void updateIcon()
    {
        if (motorIcon != null)
        {
            if (!motorEnabled)
            {
                var image = motorIcon.GetComponentInChildren<Image>();
                image.color = Color.red;
            }
            else
            {
                var image = motorIcon.GetComponentInChildren<Image>();
                image.color = Color.green;
            }
        }
        if (turboIcon != null)
        {
            turboIcon.SetActive(turboEnabled);
            motorIcon.SetActive(!turboEnabled);
            var image = turboIcon.GetComponentInChildren<Image>();
            image.color = Color.green;
        }
    }
}

}  // namespace ROS2
