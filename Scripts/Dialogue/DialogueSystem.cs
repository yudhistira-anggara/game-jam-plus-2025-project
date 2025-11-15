using Godot;
using System;

namespace GameJam
{
    /*
    Example dialogue .json file format
    [
        {
            "id": "test_dialogue_01",
            "contents": [
                {
                    "name": "Person A",
                    "options": {
                        "voice": "person_a_talk.wav",
                        "script": "", // for dialogue requiring special scripting, if required (can be empty)
                    },
                    "text": "Hello, world!"
                },
                {
                    "name": "Person B",
                    "script": "",
                    "text": "Hi, Person A!"
                }
            ]
        },
        {
            "id": "test_dialogue_02",
            "contents": [
                {
                }
            ]
        }
    ]
    */
    public partial class DialogueSystem : Node
    {
        //
    }
}