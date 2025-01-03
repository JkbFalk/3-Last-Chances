using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_Anima : Quest {
    public Quest_Anima() {
        Icon = "UI/Anima";
        Objectives = new() { 
        new(this, 0) {ShouldShowUpInJournal = true}, 
        new(this, 10),
        new(this, 20),
        new(this, 30),
        new(this, 40) {ShouldShowUpInJournal = true}};
    }
}