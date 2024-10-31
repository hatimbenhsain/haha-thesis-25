EXTERNAL pause(time)
EXTERNAL stopSinging()
EXTERNAL continueSinging()
EXTERNAL restartSinging()
EXTERNAL loadLevel(destination)
EXTERNAL goToNextLevel()

=== beginning ===
~ stopSinging()
Teacher: Hey. How's the current for you?
+   [Fine.]
    MC: I think it's fine.
+   [Too hot.]
    MC: Way too hot.
+   [Too cold.]
    MC: Really cold.
- Teacher: I see.
~ stopSinging()
I don't think I agree.
~ continueSinging()
I'm going to the party next.
-> END

=== ambientDialogue ===
# ambient
Teacher: I like this. # time: 3
~ pause(4)
Teacher: Do you? # time: 3
~ pause(4)
MC: Feels pretty good. #time: 3
-> END

