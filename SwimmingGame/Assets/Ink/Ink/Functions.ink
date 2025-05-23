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
EXTERNAL overrideRotationWithSpeed(targetName, rotationSpeed)
EXTERNAL changeDialogueView(viewIndex)
EXTERNAL loadInt(name)
EXTERNAL loadBool(name)
EXTERNAL loadString(name)
EXTERNAL loadFloat(name)
EXTERNAL saveValue(name,value)
EXTERNAL fadeIn(time)
EXTERNAL fadeOut(time)
EXTERNAL setFMODGlobalParameter(name,value)
EXTERNAL changeDesire(text)
EXTERNAL clearScreen()
EXTERNAL changeStartKnot(name)
EXTERNAL activateBorder(name,b)

VAR world=0

=== nextBrainKnot ===
~ nextBrain()
-> END

=== function muffleNPCsVolume()
~ setFMODGlobalParameter("npcSingingVolume",0.3)

=== function restoreNPCsVolume()
~ setFMODGlobalParameter("npcSingingVolume",1)


=== npcStart ===
~ stopSinging()
~ pauseTutorial(true)
~ muffleNPCsVolume()
~ setDialogueBubble("standard")
{
    -world==1:
        ~ activateBorder("floral",true)
    -world==2:
        ~ activateBorder("rave",true)
}
->->

=== npcEnd ===
~pauseTutorial(false)
~ continueSinging()
~ restoreNPCsVolume()
~ activateBorder("floral",false)
~ activateBorder("rave",false)
->->



//Colors:
//Eelor
//# color: 6D6787
//Geluu
//# color: 95B79B
//Horma
//# color: 7E0D13
//Fonsh
//#color: 99AFAA
//Shrimptux
//# color: 1F7A6E
//Beloo
//# color: 1d1c29
//Popero
//# color: 966382
//Xrys
//# color: 2b6136