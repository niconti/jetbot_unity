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

namespace ROS2
{

/// <summary>
/// An example class provided for testing of basic ROS2 communication
/// </summary>
public class ImageSubscriber : MonoBehaviour
{
    private ROS2UnityComponent ros2;
    private ROS2Node node;
    private ISubscription<sensor_msgs.msg.Image> image_sub;
    private sensor_msgs.msg.Image image;
    
    public string topicName = "/image";
    
    private Texture2D texture;


    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }


    void Awake()
    {
        var robotName = GetArg("-robot");
        if (robotName != null)
        {
            topicName = String.Format("/{0}/camera/image_raw", robotName);
        }
        Debug.LogFormat("topicName {0}", topicName);
    }

    void Start()
    {
        ros2 = GetComponent<ROS2UnityComponent>();
        if (node == null && ros2.Ok())
        {
            node = ros2.CreateNode("image_node");
            image_sub = node.CreateSubscription<sensor_msgs.msg.Image>(topicName, OnImageCallback, new QualityOfServiceProfile(QosPresetProfile.SENSOR_DATA));    
        }
    }

    void Update()
    {   
        if (image != null)
        {
            Destroy(texture);
            texture = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.RGB24, false);
            texture.LoadRawTextureData(image.Data);
            texture.Apply();

            GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    void OnImageCallback(sensor_msgs.msg.Image message)
    {
        Debug.LogFormat("Image {0} {1} x {2}", message.Encoding, message.Width, message.Height);
        image = message;
    }   
}

}  // namespace ROS2
