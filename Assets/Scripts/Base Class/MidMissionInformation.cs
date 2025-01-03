using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class MidMissionInformation
{
    public string AreaName;
    public Vector2 PlayerPosition;
    public bool PlayerIsFlipped;
    public float PlayerCurrentHealth;
    public Dictionary<Type, int> ToolChargesRemaining = new();
    public Dictionary<string, UnitState> GameObjectPathsToUnits = new();
    public Dictionary<string, InteractableState> GameObjectPathsToInteractables = new();
    public Dictionary<string, DestructibleState> GameObjectPathsToDestructibles = new();
    public List<string> ClassesAndMethodsToExecute = new();

    public class UnitState {
        public Vector2 Position;
        public bool KnockedOut;
        public bool IsFlipped;
        public bool IsActive;
    }
    public class InteractableState {
        public int InteractCount;
        public Vector2 Position;
        public Sprite Sprite;
        public bool IsActive;
    }
    public class DestructibleState {
        public Vector2 Position;
        public bool IsActive;
    }
}
