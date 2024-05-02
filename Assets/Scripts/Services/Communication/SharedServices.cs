using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mmo_shared.Messages;
using mmo_shared;

public class SharedServices : MonoBehaviour {

    public Serializer serializer { get; protected set; } = new Serializer();
}
