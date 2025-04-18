using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_3LastChances : Quest {
    public Quest_3LastChances() {
        Icon = "UI/Cycle3";
        Objectives = new() { new(this, 10), 
        new(this, 20),
        new(this, 30),
        new(this, 100) {JournalId="Quest_3LastChances_100"},
        new(this, 200) {JournalId="Quest_3LastChances_200"},
        new(this, 300) {JournalId="Quest_3LastChances_300"}};
    }
}