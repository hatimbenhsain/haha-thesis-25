INCLUDE Functions.ink

VAR swimmerCamOn=false
VAR loadCutscene=false
VAR intensity=0

//Include: thoughts about evolution, heartbreak, establish evolution
// maybe something like... i've noticed this thing inside of me 
// or sth growing out of me 


// Main character is swimming in the dark. As the conversation progresses, they begin to see two sexual organs entangling. They have the faces of the Protagonist and the Ex as heads. The faces are obscured until the end of the conversation.
=== intro1 ===
~ stopSinging()
~ pauseTutorial(true)
Ex: So, \\pauseI'm leaving. # time: 3
~ intensity=1
MC: What? # time: 3
Where are you going? # time: 3
Ex: To the surface. # time: 3
MC: I.. # time: 3
Could I come with you? # time: 3
Ex: I don't think that would be right for you. # time: 3
~ pause(2)
MC: Why are you going? Is it.. something I did? # time: 3
Ex: No. \\pauseI just feel the calling. # time: 5
~pause(2)
MC: Will you come back? Every now and then? Or maybe we can still communicate, with telepathy? # time: 5
Ex: ..I don't think it works that way. # time: 3
~pause(4)
Ex: Anyway..
~nextBrain()
I will get going.
~ pause(4)
MC: Hold on..
~ swimmerCamOn=true
~ changeDialogueView(2)
MC: Wait!! #speed: fast
~ pauseTutorial(false)
~ finishTutorialPart(2)
-> END

=== intro2 ===
# ambient
~ intensity=2
~ changeDialogueView(2)
MC: Are you still there?  # time:3
Ex: What is it? # time:3
MC: What does this mean for us..? # time: 5 # speed: slow
Ex: Evidently this has to be our end. # time: 3
MC: And are you sure I can't -- # time: 1.5 # speed: fast
Ex: I don't think you would survive it. # time: 3
-> END

=== intro3 ===
# ambient
~ intensity=3
MC: Did you even like me at all? Or was this just... some sort of a.. a distraction? Just to pass the time?! # time: 5
Ex: Of course I did. \\pauseThis has nothing to do with you. # time: 4
-> END

=== intro4 ===
# ambient
~ intensity=4
MC: Will you miss me at all? # time: 3
Ex: I will likely think about you, \\pauseif I see a rock or a plant or such that has a similar shape to you. \\pauseSometimes, it will ache a little bit. # time: 8
Ex: But mostly, I will be too occupied with all the discoveries I am making to really feel "sorrow". # time: 8
MC: What's it like on the surface? #speed: slow # time: 4
Ex: It's-- # time: 3
~pause(2)
Ex: Do you really want to hear about that? # time: 3
~pause(2)
MC: I guess not. Not really. # time: 3 # speed: slow
-> END

=== intro5 ===
# ambient
~ intensity=5
MC: What is that, up ahead? \\pauseIt looks frightening. # time: 4
Ex: It's part of all the changes that are happening. Everyone is doing it now. # time: 6
MC: But that... that didn't really happen, did it? Between us? # time:5
Ex: No. # time: 3
MC: And neither did this conversation. # time: 3
Ex: But I left you a song. On the coralnet. # time: 3
MC: It was short. # time: 3
~ pause(2)
~ loadCutscene=true 
MC: Wait. \\pauseDoes it have -- # time: 2
-> END





