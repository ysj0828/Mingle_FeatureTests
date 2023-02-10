using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.unity.photon
{
    //   public class Member
    //   {
    //     static public string nickname;
    //     static public string phone_number;
    //     static public string thumbnail;
    //     static public string type;
    //     static public string user_id;
    //   }
    //   public class Room
    //   {
    //     static public string id;
    //     static public string thumbnail;
    //     static public string title;

    //   }
    //   public class RoomDadata
    //   {
    //     static public string Room;
    //   }
    public class Infomation : MonoBehaviour
    {
        public static Infomation Instance = null;
        // static public string RoomID = "5aba0250e7d";
        public string RoomID = "5aba0250e7d648b19bf278638721d54e";
        public string NickName = "Default";
        public string character_uuid = "0a395de4191b4799a7b03cf31524b727";
        public string room_template_uuid = "5059cbf382e748439bffc85c8aba358f";
        public string character_preview_uuid = "";
        public string room_preview_uuid = "";
        // static public string Animation;

        public bool Jumping;

        public event Action EmojiChanged = delegate { };

        public void updatePreview(string message)
        {
            Debug.Log("updatePreview");

            JObject json = JObject.Parse(message);

            if (json["character_uuid"] != null) character_preview_uuid = json["character_uuid"].ToString();
            if (json["room_uuid"] != null) room_preview_uuid = json["room_uuid"].ToString();
        }

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }

            else
            {
                Destroy(this.gameObject);
            }
        }

        public Dictionary<string, Vector3> InitialPosition = new Dictionary<string, Vector3>
    {
    {"5059cbf382e748439bffc85c8aba358f", Vector3.zero},
    {"418f435e60674e0b8d00261e9fadffce", Vector3.zero},
    {"0be4a3d51cd84c4485b84f7b6f5cf36c", Vector3.zero},
    {"1f80ec8123534a3aa9c73fc88b7bd0c3", Vector3.zero}
    };

        public Dictionary<string, int> E_Animation = new Dictionary<string, int>
        {
            {"", 0},
            {"HeartEyesShaded", 1},
            {"CryingShaded", 2},
            {"AngryShaded", 5},
            {"LaughingShaded", 3},
            {"CussingShaded", 7},
            {"BlushingShaded", 8},
            {"ShockedShaded", 9},
            {"CoolShaded", 6},
            {"SleepingShaded", 10},
            {"Megaphone", 11},
            {"NoseStreamShaded", 12},
            {"SillyShaded", 4},

            // {"page1_1", 1},
            // {"page1_2", 2},
            // {"page1_3", 3},
            // {"page1_4", 13},
            // {"page1_5", 14},
            // {"page1_6", 15},
            // {"page1_7", 4},
            // {"page1_8", 5},
            // {"page1_9", 6},

            // {"page2_1", 7},
            // {"page2_2", 8},
            // {"page2_3", 9},
            // {"page2_4", 10},
            // {"page2_5", 11},
            // {"page2_6", 12},
            // {"page2_7", 1},
            // {"page2_8", 2},
            // {"page2_9", 3},

            // {"page3_1", 4},
            // {"page3_2", 5},
            // {"page3_3", 6},
            // {"page3_4", 7},
            // {"page3_5", 8},
            // {"page3_6", 9},
            // {"page3_7", 10},
            // {"page3_8", 11},
            // {"page3_9", 12}
        };

        public Dictionary<int, string> E_Animation_inverted = new Dictionary<int, string>
        {
            {0, ""},
            {1, "HeartEyesShaded"},
            {2, "CryingShaded"},
            {3, "AngryShaded"},
            {4, "LaughingShaded"},
            {5, "CussingShaded"},
            {6, "BlushingShaded"},
            {7, "ShockedShaded"},
            {8, "CoolShaded"},
            {9, "SleepingShaded"},
            {10, "Megaphone"},
            {11, "NoseStreamShaded"},
            {12, "SillyShaded"}
        };

        public int Animation = 0;

        //public event Action EmojiChanged = delegate { };
        public string Animation_String
        {
            get => animation_string;
            set
            {
                animation_string = value;
                EmojiChanged?.Invoke();
            }
        }

        private string animation_string;

        public JArray RoomData = null;
        public JArray FriendsData = null;
        public bool isFirst = true;
    }
}
