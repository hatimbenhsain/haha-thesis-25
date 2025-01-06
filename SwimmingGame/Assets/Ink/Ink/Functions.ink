EXTERNAL pause(time)
EXTERNAL stopSinging()
EXTERNAL continueSinging()
EXTERNAL restartSinging()
EXTERNAL loadLevel(destination)
EXTERNAL goToNextLevel()
EXTERNAL nextBrain()  //Switch the brain/behavior used by NPC
EXTERNAL toggleSingingMode()
EXTERNAL setDialogueBubble(bubble)
EXTERNAL pauseTutorial(b)
EXTERNAL finishTutorialPart(i)
EXTERNAL switchObject(name,bool)
EXTERNAL switchInterlocutor(name)
EXTERNAL overrideRotation(targetName)
EXTERNAL changeDialogueView(viewIndex)
EXTERNAL loadInt(name)
EXTERNAL loadBool(name)
EXTERNAL loadString(name)
EXTERNAL loadFloat(name)
EXTERNAL saveValue(name,value)
EXTERNAL fadeIn()
EXTERNAL fadeOut()
EXTERNAL setFMODGlobalParameter(name,value)

=== nextBrainKnot ===
~ nextBrain()
-> END

=== function muffleNPCsVolume()
~ setFMODGlobalParameter("npcSingingVolume",0.3)

=== function restoreNPCsVolume()
~ setFMODGlobalParameter("npcSingingVolume",1)