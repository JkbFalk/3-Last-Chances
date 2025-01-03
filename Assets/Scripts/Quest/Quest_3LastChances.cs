using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest_3LastChances : Quest {
    public Quest_3LastChances() {
        Icon = "UI/Cycle3";
        Objectives = new() { new(this, 10) {ShouldShowUpInJournal=true}, 
        new(this, 20),
        new(this, 30),
        new(this, 100) {ShouldShowUpInJournal=true, JournalId="Quest_3LastChances_100", ShowInJournalAsFailedWhenCompleted=true},
        new(this, 200) {ShouldShowUpInJournal=true, JournalId="Quest_3LastChances_200", ShowInJournalAsFailedWhenCompleted=true},
        new(this, 300) {ShouldShowUpInJournal=true, JournalId="Quest_3LastChances_300"}};
    }
}