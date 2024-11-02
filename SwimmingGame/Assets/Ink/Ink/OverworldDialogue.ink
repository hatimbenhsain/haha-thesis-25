EXTERNAL pause(time)
EXTERNAL stopSinging()
EXTERNAL continueSinging()
EXTERNAL restartSinging()
EXTERNAL loadLevel(destination)
EXTERNAL goToNextLevel()
EXTERNAL nextBrain()  //Switch the brain/behavior used by NPC

=== beginning ===
~ stopSinging()
Teacher: Hey. \\pause\\pause\\pauseHow's the current for you? #speed: 50
+   [Fine.]
    MC: I think it's fine.
+   [Too hot.]
    MC: Way too hot.
+   [Too cold.]
    MC: Really cold.
- Teacher: I see. #speed: slow
~ stopSinging()
I don't think I agree. #speed: fast
~ continueSinging()
I'm going to the party next. #speed: normal
-> END

=== ambientDialogue ===
# ambient
Teacher: I like this. # time: 3
~ pause(4)
Teacher: Do you? # time: 3
~ pause(4)
MC: Feels pretty good. #time: 3
-> END

=== teacherBeforeSex ===
~ stopSinging()
Teacher: How did you find the party?
MC: It was alright...
Teacher: What's wrong?
MC: Nothing.
MC: ...
Teacher: Are you still upset that they left you?
MC: No! I don't care about that anymore.
MC: I just want to forget.
~pause(2)
Teacher: Do you wanna race?
MC: Now?
Teacher: When else?
+   [Let's race.]
    MC: Ok let's do it.
    Teacher: We can start slow.
    ~ nextBrain()
+   [No.]
    MC: I'm not in the mood.
    Teacher: Let me know if you change your mind. I might be here still.
- -> END

=== teacherDuringRace1 ===
# ambient
Teacher: Are you good? # time: 2
MC: I'm fine. # time: 2
-> END

=== teacherDuringRace2 ===
# ambient
Teacher: Do you feel it building? # time: 2
MC: Feel what? # time: 2
Teacher: Oh. Sorry. I wasn't thinking... # time: 4
MC: Oh! No, I get it. I think I do.
-> END

=== teacherDuringRace3 ===
# ambient
Teacher: So have you done it before? # time: 3
MC: No # time: 2
Teacher: Would you like to try it? With me? # time: 6
MC: Y-yeah. Okay. I think it's coming out. #time: 6
Teacher: Let's go somewhere else. #time: 4
~ loadLevel("SexPrototype")
-> END












