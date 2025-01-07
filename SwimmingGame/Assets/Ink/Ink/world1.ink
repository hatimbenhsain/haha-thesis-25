INCLUDE Functions.ink

VAR sexIntensity=0
VAR npcsTalkedTo=0
VAR coralTalkedTo=0
VAR coralToTalkToBeforeProgress=3

/* CORALNET */

//I'm calling OP initator for now but could be different
// other ideas: first author, singer

=== coralnetStart ===
~ coralTalkedTo=coralTalkedTo+1
{ coralTalkedTo==coralToTalkToBeforeProgress:
    -> coralnetProgress
}
~ setDialogueBubble("bone")
~ npcsTalkedTo=npcsTalkedTo+1
~ stopSinging()
~ pauseTutorial(true)
~ muffleNPCsVolume()
->->

=== coralnetEnd ===
~pauseTutorial(false)
{ npcsTalkedTo > 2:
    ~finishTutorialPart(6)
}
~ continueSinging()
~ restoreNPCsVolume()
->->

=== coralnet1 ===
-> coralnetStart ->
Coralnet: motif: favorite swimming style?
> what's everyone's favorite swimming style and why?
> personally i like to mix hermit and shark style on the lower half and cerulean top i find it the most efficient.
> i just kind of do what everyone else is doing
> i reaaaally like sliding on walls does anyone else do this
> i like going backwards because i always get surprised when i reach something
> i like sliding on walls too
> i alternate
-> coralnetEnd ->
->END

// I love the i alternate to wrap up everything

=== coralnet2 ===
-> coralnetStart ->
Coralnet: motif: has anyone actually done "it" yet?
> motif.
> yes.
> yeah i have.
> i did it with a bunch of people.
> how does it work exactly?
> so one of you shrinks down and you enter the other person's mouth.
> then you tickle the under of their tongue.
> that's seahorseshit. it's really simple you just have to lie very still next to each other and then it feels good in your stomach.
> hi! actual person who's done it here.
> basically there's two types of people: one with a hole and one with a long hard object. the long object goes into the hole many times.
> all of these sound disgusting. are people really doing this?
> how much chewing are you supposed to do? does anyone know?
-> coralnetEnd ->
-> END

// Love how they are all straight-up misinformation and
// the reference to human sex

=== coralnet3 ===
-> coralnetStart ->
Coralnet: motif: hole in my head
> i woke up with a hole in my head. what do i do?
> you have to be more specific than that.
> it's a hole on the side of my head. i don't know why it's there.
> initiator are you sure you weren't stabbed?
> have you tried putting things in it?
> yes, it feels a little odd.
> and i always feel a sort of unpleasant pressure so i started plugging it
> why?
> just felt right
> how big is it initiator?
> about the size of my finger.
> initiator i have a lot of experience with holes could we meet and i could examine you?
> no?? it's really weird i would rather not or else i wouldn't be on here.
> initiator any update? i think i have one too.
> yes it's still really strange but i feel i am becoming stronger and more deadly
-> coralnetEnd ->
-> END

=== coralnet4 ===
-> coralnetStart ->
Coralnet: motif: ate my lover
> is it abnormal to eat your lover without asking if you could?
> they were annoying me a lot and i didn't want to hear their thoughts anymore.
> my friends are saying it's questionable behavior.
> initiator why didn't you just stop singing with them?
> felt rude
> i think it's ok i've eaten 20 of my past lovers and each time they get tastier
> hi i think you shouldn't date someone if they're not ready to be eaten i think it's ok initiator
-> coralnetEnd ->
-> END

=== coralnet5 ===
-> coralnetStart ->
Coralnet: motif: anyone yearns?
> anyone yearns to be in the next season? i miss the parties
> initiator why dont' you just have a party here
> it's not the same
> initiator that doesn't seem right maybe you hit your heard really hard? have you seen a doctor?
> no
-> coralnetEnd ->
-> END

// HAN: The no at the end is very reddit reply vibe

=== coralnet6 ===
-> coralnetStart ->
Coralnet: motif: what's it like outside?
> motif.
> i don't know
> i don't know
> i tried going but i felt very bad everywhere and had to come back inside.
> has anyone who's traversed come back yet?
> my sibling said they did and there was sharp glass everywhere and everything was blurry and they never got hungry
> but i think they lied
-> coralnetEnd ->
-> END

// Like how a lot of these coralnet foreshadows future events

=== coralnet7 ===
-> coralnetStart ->
Coralnet: motif: i went outside
> i went outside and there was sharp glass everywhere and everything was blurry and i was never hungry
> that's not true my lover said they were born outside and that there wasn't any sharp glass anywhere
> what else did they say?
> they said the ground burns your feet and everything that you try to eat turns into spikes so they had to come here
> none of this is true i went outside and it's the same as here but more magenta
-> coralnetEnd ->
-> END

=== coralnet8 ===
-> coralnetStart ->
Coralnet: motif: i miss my lover
> i miss my lover a lot. what do i do?
> initiator did your lover provide you with most of your food? maybe you should find a good hunter to replace them
> no it wasn't like that
> i think i just really liked their company
> initiator did they have a particularly good voice? maybe you could start going to more concerts and operas
> hi i tried this but i still feel the missing
> what do i do?
-> coralnetEnd ->
-> END

=== coralnet9 ===
-> coralnetStart ->
Coralnet: motif: i see them everywhere
> ever since my lover left me i see them everywhere i always feel their presence even though they're not there
> initiator did you also sing about missing your lover in another coral?
> no? that wasn't me 
> initiator that sucks you should really just try living in the moment
> initator have you tried the entanglement yet? maybe it would fix you
> that sounds scary
-> coralnetEnd ->
-> END

=== coralnet10 ===
-> coralnetStart ->
Coralnet: motif: current order
> does anyone know why the current sometimes is really cold and sometimes too hot?
> i think it's when people emit evil and dark vibrations it makes it cold but sometimes hot and it's perfect when there's only nice people around
> hi i don't know
> what the second person said sounds right
-> coralnetEnd ->
-> END 

=== coralnet11 ===
-> coralnetStart ->
Coralnet: motif: has anyone tried this
> recently i tried wrapping my organ around of a pillar and it felt extremely good
> what organ? what are you talking about
> if you don't know what i'm talking about this isn't for you
> initiator i tried this and i didn't feel anything did you do anything else?
> i just kind of rubbed it a lot and it was really great
> i did this too but instead of a pillar it was a wall and i spread it kind of everywhere
> me too i think i prefer than doing it with other people
> what are you all talking about???
-> coralnetEnd ->
-> END

// love the tone of this part

=== coralnet12 ===
-> coralnetStart ->
Coralnet: motif: keeping the secret
> there's a lot of people who haven't experienced "it" yet and i don't like talking to them because they feel like little children.
> how can we know who else has done it so we can avoid the unchanged?
> initiator since when are we keeping it a secret and why
> i think those of us who had it were chosen and we shouldn't mix with the less-deserving
> chosen for what
> initator chosen by whom
> it doesn't matter can you just answer my question
> i try using a lot of euphemisms... "it" is a good one. i'm also partial to "relations" and "fun"
> has anyone come up with an actual name for it actually??
> what are we talking about???
> hello?
-> coralnetEnd ->
-> END

=== coralnetProgress === //the coralnet to read to progress story
-> coralnetStart ->
Coralnet: motif: my entanglement
~ fadeOut()
> i experienced it today. i'm happy to share my experience.
> oh? how was it? tell us about it.
> it was in the back of our music hall. 
> we were practicing a song for the end-of-season show.
> we felt a tugging from underneath our skin, in the middle top of our back.
> who did you do it with?
> it was my singing partner. not my lover, but someone i've always been deeply intimate with.
> and after the tugging?
> first, there was a burning sensation. then, it started to come out, and it felt cold as ice.
> what was it exactly?
> it was neither tail nor fin nor antenna. it was a tendril of sorts. 
> it was long and sinewy and soft.
> there was a eautiful burts of smaller wriggling tendrils at the end of it, like a flower.
> and what did you do with it?
> at first we just sensed our surroundings
> the organ felt more sensitive than the inside of my mouth.
> then we started feeling each other. we brushed ourselves and we made knots of our bodies.
> we tugged and let go. we followed the current and fought against it.
> we became stickier. it was harder to separate each other.
> our organs were the same color and we couldn't tell whose part was which.
> and how did it feel?
> initiator, how did it feel?
> initiator, tell us how it was.
-> coralnetEnd ->
~ switchObject("Teacher - Library",true)
~ switchInterlocutor("Teacher - Library")
-> teacherAtLibrary

// I like this one very much. I like the recurring reference to how it feels inside one's mouth. Also imagining a world where this part could feel more cut off vibe since the teacher is probably approaching in the middle when MC is reading? Now is also very nice!


/* MAIN STORY */

// Initiated by the teacher as you finish reading coralnetProgress
=== teacherAtLibrary ===
 -> npcStart ->
Teacher: Sounds amazing, doesn't it?
~ fadeIn()
MC: What?
Teacher: The entanglement. \\pauseYou were reading about it just now, right? Have you done it yet?
MC: ...
Teacher: I've done it a couple times now. It wasn't nearly as intense as they described, but maybe I just haven't found the right person.
Teacher: I wonder what happens at the end, maybe a giant sparkly explosion?
MC: Didn't you just say you've done it before...?
Teacher: Yes, but I've never reached this "climax" that I've often heard spoken about. I always stop before.
Teacher: It always felt a bit scary. \\pause\\pauseLike maybe I would burst.
MC: Ah.
Teacher: ...
Teacher: Say, I always see you here. Do you only come to browse or--
MC: I really need to get going.
~ overrideRotation("Roadblock - Library")
~ switchObject("Roadblock - Library",false)
~ pauseTutorial(false)
~ restoreNPCsVolume()
-> END

// Same I feel like here the rest of the interaction is nice but I feel like MC should be more surprised by the teacher being next to them.

// MC finds teacher at diner sitting. They start singing when MC gets nearby to invite conversation
=== teacherAtDiner ===
 -> npcStart ->
Teacher: I'm surprised you've come to talk to me. 
Teacher: I must've been really bothersome at the library.
MC: I was really curt with you.
Teacher: It's alright.
~pause(4)
Teacher: So what brings you here?
Teacher: Can I share some food with you? They have really tasteful salmonds this season.
MC: I'm okay.
~pause(4)
MC: Have you been watching me?
Teacher: What?
MC: At the library. You said you often see me there.
Teacher: Oh, I just meant I notice you there almost everytime I visit.
MC: Right.
~pause(4)
Teacher: Would that be bad? If I watched you?
MC: No.
Teacher: Then I guess I do watch you a little bit.
~pause(4)
Teacher: Would you like to...
MC: Yeah?
Teacher: Go somewhere with fewer other people?
MC: Sure.
Teacher: Ok. Follow me. I'll show you one of my favorite places.
MC: What about your food...?
Teacher: Oh, someone else will eat it.
~ switchObject("Roadblock - Edge",false)
~ nextBrain()
~ restoreNPCsVolume()
~ pauseTutorial(false)
-> END
// MC follows teacher in gameplay portion to edge 2

//Appears if harmonizing while on the way to the edge
=== teacherOnTheWay1 ===
# ambient
Teacher: Are you excited? # time: 3
~ pause(4)
MC: I think. # time: 3
~ pause(4)
Teacher: Me too. #time: 3
-> END

//TO-DO: add more things that they can say on the way.

=== teacherArrivedAtEdge ===
~ switchObject("LevelLoader - Foreplay 1",true)
~ nextBrain()
->END

// MC and Teacher harmonize then sit and talk
=== teacherAtEdge1 ===
~stopSinging()
Teacher: It's nice here, isn't it?
MC: Yeah. I was actually here just now.
Teacher: Oh, haha, wait, really?
MC: Yeah.
Teacher: That makes sense.
~pause(2)
MC: So do you want to try it with me?  #speed: fast
Teacher: Oh!
Teacher: Yes, sure. \\pause\\pauseI mean, that's what I thought this was leading to.
MC: Oh. Sorry. Is it abnormal to talk about it directly?
Teacher: No, not at all!
Teacher: I feel really attracted to you.
+   [Thanks.]
    MC: Ok. Thanks.
    Teacher: ...
+   [Me too.]
    MC: Yeah, me too.
    Teacher: That's good to know, haha.
    Teacher: I mean, I thought so.
+   [I know.]
    MC: Yeah. I could tell. I guess.
    MC: That's cool.
    Teacher: ...Right. You're welcome.
- MC: How do we get started?
Teacher: Ah...\\pause\\pause Well that's the thing, it's been different everytime I've done it. 
Teacher: It would just kind of happen spontaneously. But maybe...
MC: Yes?
Teacher: Maybe we should try just harmonizing for a while? Like they were saying on the coralnet?
MC: Ok. Let's do it.
Teacher: You take the lead.
~ nextBrain()
-> END
// Teacher and MC harmonize a few times

// Maybe add a "I was here just now" vibe on line 249?
// Love the interaction starting at line 254


//After harmonizing a few times
=== teacherAtEdge2 ===
MC: I can feel the... 
Teacher: Yes, me too.
MC: Should we...?
Teacher: Let's keep going.
~nextBrain()
-> END
// Teacher and MC harmonize a few more times, then main act starts

=== teacherAtEdge3 ===
~nextBrain()
~loadLevel("Main Act 1 - 1")
-> END

//Right after the organs come out
=== teacherMainAct1 ===
# ambient
MC: WAIT, stop!!! # time: 2
~loadLevel("Foreplay 1 - 2")
-> END
// Main act interrupted and go back to edge sitting

=== teacherAtEdge4 ===
~stopSinging()
MC: Sorry, \\pausethat was... #speed: slow
Teacher: Are you okay? #speed: slow
MC: Yeah, \\pauseI guess I just didn't expect it to feel that way.
Teacher: Did it hurt?
MC: No.
MC: It was just something I've never felt before.
~pause(2)
Teacher: Right.
~pause(6)
MC: What do we do when the organs are out?
Teacher: I think we can...\\pause just try to wrap around each other?
~pause(2)
Teacher: Do you wanna stop for now?
MC: No I want to keep going. #speed: fast
Teacher: Ok.
Teacher: We can start whenever you feel comfortable. 
Let's just harmonize again when you're ready.
MC: Ok.
~nextBrain()
-> END
// Starts again after harmonizing just once

=== teacherAtEdge5 ===
~nextBrain()
~loadLevel("Main Act 1 - 2")
-> END

VAR retractHandTrigger=false
//We could definitely cut parts of this if it's too long/too much to program, I tried to give as much choice opportunities as possible
// I feel like we can definitely see after playtest. But I think we can keep it now its all good stuff to me
=== teacherCuddling ===
~ loadInt("sexIntensity")
~pause(4)
~ changeDialogueView(1)
Teacher: So... #speed: slow
~ finishTutorialPart(1)
Teacher: How was that?
+   [Good.]
    ~ finishTutorialPart(2)
    MC: Good.\\pause It felt really good.\\pause All over my body.
    Teacher: Really?
    MC: Yeah.
+   [Weird.]
    ~ finishTutorialPart(2)
    MC: Kind of weird.\\pause I'm not sure yet how I feel about it.
    Teacher: But you don't... Do you regret it?
    MC: No, no.
    MC: I'm glad we did it. 
    MC: I think.
    Teacher: Me too.
+   [I'm not sure.]
    ~ finishTutorialPart(2)
    MC: I'm not sure.
    MC: I mean, the physical sensations were amazing.\\pause Maybe the best I've ever felt.
    MC: But also... I don't know.
- MC: What about you?
{
    - sexIntensity<3:
        Teacher: It was kind of mellow. More than most I've done.
        Teacher: But I liked it. It felt also more intimate than most.
    - else:
        Teacher: It was very intense, wasn't it?
        Teacher: I enjoyed it a lot.
}
Teacher: I really liked the way you felt.
Teacher: And that climax... Did you expect it to just fall off like that?
+   [Yeah.]
    MC: It felt kind of natural, didn't it? Like it was leading up to it?
    Teacher: Maybe.
+   [No.]
    MC: No! Does that mean we can't do it again?
    Teacher: Haha, no, I think it grows back.
    MC: I see.
-MC: Yeah.
~ changeDialogueView(2)
Teacher: So...
Teacher: You said you've never done this before, right?
+   [I haven't.]
    MC: No, I haven't.
    Teacher: Just curious.
+   [Why do you ask?]
    MC: No, I haven't.
- Teacher: Not even with your... lover? Before they left?
MC: How do you know bout that?
Teacher: Oh! I mean...
Teacher: Like I said, I've seen you around. And people talk.
~ changeDialogueView(3)
Teacher: And...
MC: ?
Teacher: You sung about them, right? I think I read it on the coralnet, one time.
+   [No.]
    MC: I don't know what you're talking about. Must've been someone else.
    Teacher: Ah. Well...
    MC: But yes, I mean...
+   [Yes.]
    MC: Yes. It's kind of embarassing that you mention it.
    Teacher: Ah-- I didn't mean to.
    Teacher: I guess those things are anonymous for a reason.
    MC: Yes. But yeah...
-MC: We never did. It wasn't...
~ changeDialogueView(4)
MC: I didn't know about it yet, back then. I think it was just starting to happen.
Teacher: Right.
Teacher: Would you have liked to... with them?
+   [Yes.]
    MC: Well, yes.
    MC: I mean, obviously.
    MC: But there's really no point in thinking about it. At this point.
    Teacher: Right...
+   [No.]
    MC: No. I'd really rather not think about it.
    Teacher: That makes sense. Sorry for bringing that up.
    MC: It's fine.
+   [I don't know.]
    MC: I don't know. Maybe. Probably. Maybe not. It's hard to visualize.
    MC: I'd really rather not think about it.
    Teacher: Right... Sorry for bringing that up.
- Teacher: But, you know... \\pause\\pause\\pause things could be different once we're up there.
~ retractHandTrigger=true
MC: What?
MC: What do you mean?
Teacher: Well you know... 
Teacher: They're saying that we're all starting to change to be able to live in the outside.
Teacher: And since we've entangled... We must be changing too.
MC: I don't-- What?
//Maybe transition to overworld view at this point
MC: Why would I want that? That's not why we did this. I--
MC: Is that really how it works?
Teacher: Oh? Um, I mean...?
Teacher: I'm not sure? It's just what I've heard. 
MC: Is that why you were trying to do it with me?
Teacher: What? No! I told you, I'm really attracted to you.
Teacher: And you asked first! I wasn't--
MC: I know, I know.
MC: I didn't know it was like that. I don't want to change. I can't--
MC: I'm sorry. I really need to get going.
Teacher: No, I'm sorry. I...
MC: Let's never do this again.
~ loadLevel("Migration 1")
-> END
// Transition to migration




/* RANDOM NPCS */


=== npcStart ===
~ stopSinging()
~ pauseTutorial(true)
~ muffleNPCsVolume()
~ setDialogueBubble("standard")
->->

=== npcEnd ===
~pauseTutorial(false)
~ continueSinging()
~ restoreNPCsVolume()
->->

=== libraryReceptionist ===
# color: 95B79B
{ npcsTalkedTo==0:
    ~npcsTalkedTo=npcsTalkedTo+1
}
-> npcStart ->
NPC: Welcome to the library.
Let me know if you need any help.
+   [What is this place?]
    This is the entrance to the library. 
    If you head further in, you can find a lot of the community's coralnet.
    Feel free to read or speak into any of them.
+   [Who are you?]
    My name is Elevide! Nice to meet you.
    It's nice that you came to talk to me today.
    ++  [Do you work here?]
        NPC: Oh, no. 
        I don't think anyone "works" here?
        I just really like helping people.
        If you want you can join me too.
        MC: Maybe some other time...
    ++  [No worries.]
        MC: Sure, uh, no worries.
+   [I'm good thanks.]
    NPC: No troubles.
- ~pauseTutorial(false)
{ npcsTalkedTo > 2:
    ~finishTutorialPart(6)
}
~continueSinging()
- -> npcEnd ->
-> END

=== npcAtLibrary1 ===
# color: 7E0D13
-> npcStart ->
NPC: I LOVE SINGING INTO THE CORALNET 
{->one->|->two->}
 -> npcEnd ->
-> END
= one
I FEEL FREE TO EXPRESS MY DEEPEST MOST EMBARASSING SECRETS THAT I WOULD NEVER TELL ANYONE
LIKE HOW I WANTED TO END MY LAST ROMANTIC RELATIONSHIP BUT I COULDN'T BRING MYSELF TO GO THROUGH WITH IT
SO I COVERED MYSELF IN SAND AND I ATTACHED SCALES ON MY BODY AND PRETENDED TO BE A BURROWING FISH 
UNTIL MY EX LEFT ME ALONE
BUT THEN I REALLY ENJOYED THE BOTTOM-FEEDER LIFESTYLE AND I BECAME ADDICTED TO BURROWING 
AND EATING ANYTHING THAT FELL INTO MY MOUTH 
MOSTLY OTHER PEOPLE'S DROPPINGS PROBABLY
I WAS SO WRAPPED UP IN IT THAT I DIDN'T REALIZE EVERYONE HAD MOVED ON TO THE NEXT SEASON
SO I JUST STAYED THERE BUT THEN I MISSED ANOTHER SEASON 
THEN ANOTHER
AND NOW I THINK IT'S BEEN A FEW AND I THINK I JUST MET MY EX'S GRANDCHILD
MC: ...And you're feeling okay with telling me this?
NPC: YOU SEEM REALLY TRUSTWORTHY
->->
=two
MC: That's good.
->->

=== npcAtLibrary2 ===
# color: 7E0D13
-> npcStart ->
NPC: 
 -> npcEnd ->
-> END

=== npcInCenter1 ===    //Eelor
# color: 6D6787
-> npcStart ->
NPC: I love kicking off walls! It's my favorite part about swimming!
Do you know how to do it?
+   [Yes.]
    Awesome! So do I!
+   [No.]
    Oh!
    Would you like me to teach you how?!
    ++  [Sure.]
        Ok!
        All you have to do is kick your feet against it!
        +++ [...What?]
            Oh!
            You mean like, teach you ~how~ to do it, huh?
            You basically just have to rapidly swim away from a wall while being near one.
            Does that make more sense?
            ++++ [Yes.]
                Good!
                Don't worry, I know what you are.
            ++++ [No.]
                Oh!
                Basically, you need to do a "rapid button press".
                And it only works if you're swimming forward or backward.
                Surely that explains it?
                +++++   [Yes.]
                    Yeah, I figured it would.
                +++++ [No.]
                    Oh!
                    Sorry, I thought you were...
                    Nevermind! Goodbye!
        +++ [Oh, duh.]
            Yeah! Ask me again if you forget!
    ++  [No thanks.]
        Aw! Too bad!
-  -> npcEnd ->
-> END

=== npcInCenter2 ===    //(Fonsh)
#color: 99AFAA
-> npcStart ->
NPC: Do you know the secret to swimming really fast?
+   [Yes.]
    What is it then?
    ++  [A protein-rich diet.]
        What's a protein?
    ++  [Resoluteness.]
        Oh wait... really?
    ++  [An aerodynamic body.]
        I heard that before... Can you help me with mine? I think my back's sort of sticking out.
+   [No.]
    Well do you wanna hear it?
    ++  [Yes.]
        The secret is to keep striding forward frequently, but not so rapidly that you get tired out!
        And in-between, keep pressing onwards so as not to lose your momentum!
        Pretty helpful right?
        +++ [Totally.]
            Yeah, you're welcome!
        +++ [Not really.]
            Oh? Well...
            How about this: bouncing off walls not only gives you a boost, it also preserves your momentum!
            How about that?
            ++++ [Great.]
                You're welcome.
            ++++ [Whatever.]
                Ok.
    ++  [No.]
        Uh... okay.
-  -> npcEnd ->
-> END

=== npcInCenter3 ===
# color: 1d1c29
-> npcStart ->
NPC: A strange phenomenon occurs, whenever I attempt to leave stray too far from this place.
NPC: It's as if there are invisible walls block my passage.
Some say it's the current, but I know the truth.
It is gigantic and powerful psychics who are holding us in place, because they are still preparing the next place for our arrival.
What do you believe?
+   [It's the current.]
    Foolish sea slug. Remain blind.
+   [It's psychics.]
    Yes!
+   [Mindset.]
    NPC: Hmmm...
    So you're saying our mindset is keeping us here and unconsciously stopping us from moving forth? I see...
-  -> npcEnd ->
->END

=== npcInCenter4 ===
# color: 2b6136
-> npcStart ->
NPC: Ahem. Do you mind?
- ~continueSinging()
->END

=== npcInCenter5 ===    //Eelor
# color: 6D6787
-> npcStart ->
NPC: I'm just standing next to the hole, no big deal.
 -> npcEnd ->
-> END

=== npcInCenter6 ===  //(Fonsh)
#color: 99AFAA
-> npcStart ->
NPC: When I am swimming around this column I am filled with... a special feeling.
 -> npcEnd ->
-> END

=== npcInDiner1 ===
# color: 1F7A6E
-> npcStart ->
NPC: I like the food here... but it's nothing compared to the buffets at Enkidu's parties!
Have you been to one of those?
+ [Yes.]
    Oh yeah? Me too!
    What was your favorite dish?
    ++ [The jellied morsels.]
        Mmm.. Aah!! I wish the current could take me there here and now!
    ++ [The en-croute noisettes.]
        Mmm.. Aah!! I wish the current could take me there here and now!
    ++ [It's not about the food.]
        MC: You're going to Enkidu's parties for the food? Is that really all you care about?
        NPC: Oh! I mean, uhm, no, of course not! I'm, uuuh, all about the scene!
        The... dancing scene, of course! And the music! I'm with the tides!
        And I really like all that stuff that everyone has been doing lately! 
        I have totally done it too, many times! Do you want to do it with me right now?!
        +++ [Sure.]
            Ah, erm..! Maybe after I finish my meal!
            Or actually, maybe even later than that. 
            I'm supposed to eat you at the end, right? I need an empty stomach! Come back a different time!
            MC: Okay..
        +++ [No thanks.]
            Ah... Well, not for lack of trying!
+ [No.]
    You absolutely have to try it! 
    They will be where most of us are going next, so prepare yourself for the bacchanalia of a lifetime!
-  -> npcEnd ->
-> END

=== npcInDiner2 ===
# color: 2b6136
-> npcStart ->
NPC: Have you ever noticed how certain people harmonize differently than others?
NPC: Sometimes I'm unable to connect with someone until I've sung the same note as them, or one that's closer to it.
NPC: What do you think that says about them?
+   [It speaks on their nature.]
    You're right. I think some people are pure-hearted and others naturally evil and this is a really good tell!
+   [It speaks on your relationship.]
    That must be true. Lots of people have evil intentions towards me and this is why they make it so difficult!
+   [It speaks on their strength of character.]
    This has to be right. Some have weaker character and they just cannot bother indulging in beautiful and harmonious compositions!
- -> npcEnd ->
-> END

=== npcInDiner3 ===
# color: 1F7A6E
-> npcStart ->
NPC: The food here was much better in the last cycle. Do you recall?
In truth, the further I go in cycles, the better my memory of the food is. Isn't that peculiar?
If I were employed in the kitchen, I would make sure the quality was always maintained to the most delectable standards of yester-cycle, for I have a perfect palate.
Alas, my constitution is too delicate to gaze at fish guts. A shame!\\pause For everyone.
- -> npcEnd ->
-> END

=== npcInDiner4 === //Beloo
# color: 1d1c29
-> npcStart ->
NPC: Have you heard of the microwave technology? It sounds simply portentous!
Apparently, powerful psychics can focus radiation waves into food to make it warm and thus more delectable!
I envision a wonderful future where we are no longer slaves to the tyranny of current for temperature control!
(Unfortunately, a future that is not for me because I am probably allergic to the microwave.)
(I haven't tried it but it seems likely so I would rather not risk it.)
- -> npcEnd ->
-> END

=== npcInDiner5 ===
# color: 966382
-> npcStart ->
NPC: Like clownfish to bioluminescent sea anemone.
-> npcEnd ->
-> END


// TO-DO: Add more random npcs




