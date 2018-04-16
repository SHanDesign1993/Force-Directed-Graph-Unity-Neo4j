using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeoUnity
{
    public class Node : MonoBehaviour
    {
        public int ID;

        [System.NonSerialized]
        public string Type;
        public string Title;
        public string Content;
    }
}
