EXTERNAL pause(time)
EXTERNAL stopSinging()
EXTERNAL continueSinging()
EXTERNAL restartSinging()
EXTERNAL loadLevel(destination)
EXTERNAL goToNextLevel()
EXTERNAL nextBrain()  //Switch the brain/behavior used by NPC

VAR sexIntensity=0

/* CORALNET */

=== coralnetProgress === //the coralnet to read to progress story
Coralnet: topic: my entanglement
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
-> END

/* MAIN STORY */

// Initiated by the teacher as you finish reading coralnetProgress
=== teacherAtLibrary ===
Teacher: Sounds amazing, doesn't it?
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
-> END

// MC finds teacher at diner sitting. They start singing when MC gets nearby to invite conversation
=== teacherAtDiner ===
~ stopSinging()
Teacher: I'm surprised you've come to talk to me. 
Teacher: I must've been really bothersome at the library.
MC: I was really curt with you.
Teacher: It's alright.
~pause(4)
Teacher: So what brings you?
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
Teacher: Go somewhere with less other people?
MC: Sure.
Teacher: Ok. Follow me. I'll show you one of my favorite places.
MC: What about your food...?
Teacher: Oh, someone else will eat it.
-> END
// MC follows teacher in gameplay portion to edge 2

// MC and Teacher harmonize then sit and talk
=== teacherAtEdge1 ===
Teacher: It's nice here, isn't it?
MC: Yeah. I come here all the time.
Teacher: Oh, haha, wait, really?
MC: Yeah.
Teacher: That makes sense.
~pause(4)
MC: So do you want to try it with me?
Teacher: Oh!
Teacher: Yes, sure. I mean, that's what I thought this was leading to.
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
Teacher: Ah... Well that's the thing, it's been different everytime I've done it. 
Teacher: It would just kind of happen spontaneously. But maybe...
MC: Yes?
Teacher: Maybe we should try just harmonizing for a while? Like they were saying on the coralnet?
MC: Ok. Let's do it.
Teacher: You take the lead.
-> END
// Teacher and MC harmonize a few times

//After harmonizing a few times
=== teacherAtEdge2 ===
MC: I can feel the... 
Teacher: Yes, me too.
MC: Should we...?
Teacher: Let's keep going.
-> END
// Teacher and MC harmonize a few more times, then main act starts

//Right after the organs come out
=== teacherMainAct1 ===
MC: Wait, stop!
-> END
// Main act interrupted and go back to edge sitting

=== teacherAtEdge3 ===
MC: Sorry, that was...
Teacher: Are you okay?
MC: Yeah, I guess I just didn't expect it to feel that way.
Teacher: Did it hurt?
MC: No.
MC: It was just something I've never felt before.
Teacher: Right.
~pause(4)
Teacher: Do you wanna stop for now...?
MC: No I want to keep going.
Teacher: Ok.
Teacher: We can start whenever you feel comfortable. Let's just harmonize again when you're ready.
-> END
// Starts again after harmonizing just once

//We could definitely cut parts of this if it's too long/too much to program, I tried to give as much choice opportunities as possible
=== teacherCuddling ===
Teacher: How was that?
+   [Good.]
    MC: Good. It felt really good. All over my body.
    Teacher: Really?
    MC: Yeah.
+   [Weird.]
    MC: Kind of weird. I'm not sure yet how I feel about it.
    Teacher: But you don't... Do you regret it?
    MC: No, no.
    MC: I'm glad we did it. 
    MC: I think.
    Teacher: Me too.
+   [I'm not sure.]
    MC: I'm not sure.
    MC: I mean, the physical sensations were amazing. Maybe the best I've ever felt.
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
MC: Yeah.
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
MC: We never did. It wasn't...
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
// Retract hand at this point
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
-> END




/* RANDOM NPCS */












