using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_Ignis : Quest {
    public Quest_Ignis() {
        Icon = "UI/Ignis";
        Objectives = new() { 
        new(this, 0), 
        new(this, 10),
        new(this, 20),
        new(this, 30),
        new(this, 40),
        new(this, 50),
        new(this, 60),
        new(this, 70),
        new(this, 80),
        new(this, 100),
        new(this, 110),
        new(this, 120),
        new(this, 130),
        new(this, 140),
        new(this, 150),
        new(this, 200),
        new(this, 210),
        new(this, 220),
        new(this, 230),
        new(this, 240)};
    }
}