INCLUDE Functions.ink

VAR talkedToVirgin=false


// * IMPORTANT DIALOGUE *

//You meet them at the smoking area.
// establish: MC feelings after ch1, feelings about world 2
// difference as opposed to prev world 2 parties,
// virgin being disgusted by sex
//    HMM this is probably too long, maybe could be broken up in more convos.. but fear played wont know that u can talk several times
=== virgin ===
-> npcStart ->
NPC: Good tidings friend.
{ 
    - talkedToVirgin:
        NPC: Please be careful in there.
        MC: I will be.
    - else:
        -> chat ->
}
- -> npcEnd ->
-> END
=chat 
Care for a chat?
+   [Let's talk.]
    MC: Sure.
    ~talkedToVirgin=true
+   [Maybe later.]
    MC: Maybe later. I've got things on my mind.
    -> npcEnd ->
    -> END
- How's your current?
+   [Crowded.]
    MC: Feels suffocating honestly.
    I can't swim a pace without bumping into some weirdos doing... whatever.
    NPC: I know. At least here it's a bit calmer.
    But you still can't escape that incessant thumping...
    MC: Yeah. I really had to get out of there.
+   [Cozy.]
    MC: It feels like I'm enveloped in all the warmth and festivities.
    It's kind of nice. I don't have to be just me and my thoughts.
    NPC: Really? 
    Well. At least you're not spending all your time in the coralnet anymore.
    MC: Sure.
    MC: I mean, it's also kind of overwhelming. That's why I'm here now.
+   [Liberating.]
    MC: It has sort of a fresh, liberating taste. 
    It feels like I could act however I want to here, and no one would care.
    NPC: What do you mean by that? Act like how?
    MC: Uh... I don't know. I was just saying.
    NPC: No, really. What would you do here that you wouldn't do in other instances?
    MC: I don't know! It's just, I guess maybe I don't have to think about what others will think about what I do differently for once.
    NPC: ...
    NPC: I know you think I'm judgemental.
    MC: I wasn't saying that.
    Besides, it's also a lot. I couldn't really bear it anymore so I had to come out here.
- NPC: I see.
I'm sure you've... seen a lot in there.
MC: Yeah. There's a lot going on.
NPC: I used to enjoy these parties. Really. But they've... perverted it. It's against current.
MC: ...It's not that bad.
~pause(2)
NPC: So,\pause how have you been feeling? I know that last one you had relations with also went away...
+   [Fine.]
    MC: I'm fine. Really.
    NPC: Really?
    MC: Verily.
    NPC: You know...
    NPC: Just because both of your exes went to the surface...
    MC: I wouldn't call them that.
    NPC ...in a row doesn't necessarily mean that it's a pattern. Lots of people nowadays are heading up there and...
    MC: I said it's fine!!! I don't think it's a pattern!!!
    I'm great and lovable and besides I don't even need that sort of stuff anyway!!
    NPC: ....
    If you put it that way.
+   [Bad.]
    MC: Bad.
    NPC: Ah.
    ~pause(2)
    NPC: Care to elaborate?
    MC: Everything is weird. Everything is doing this stuff and it feels like they're in on something that I don't understand and --
    NPC: You know, just because both of your exes went to the surface...
    MC: What? I wasn't even talking about that. And I wouldn't even call that last one an EX.
    NPC: ...in a row doesn't necessarily mean that it's a pattern. Lots of people nowadays are heading up there and...
    MC: I-- I don't care! I wasn't talking about them!! #speed: fast
    MC: I mean, of course, I'm lonely but--
    ~pause(2)
    NPC: Your feeling of loneliness is really worrying to me.
    MC: I think you're making too big a deal out of it.
    NPC: Getting that attached to someone is unnatural. Perhaps you too are changing.
    MC: Aghhhh!!
    NPC: It's okay. Maybe if you just eat leaner fish and --
    MC: Can we talk about something else?
    NPC: ....
    Okay.
+   [Not gonna talk about it.]
    MC: I don't want to talk about it.
    NPC: It doesn't necessarily mean to be a pattern, you know. Lots of people nowadays are going to the surface, and just because your two exes...
    MC: I said I don't want to talk about it!! And I wouldn't even call that last one "my ex"!!! #speed:fast
    NPC: Burying things like this won't do you any good.
    MC: I'm not burying!!! 
    I'm just taking the space and time to process everything and just because no one ever wants to stay with me doesn't mean I'm sad or whatever!!!!!
    NPC: ....
    If you put it that way.
- NPC: Anyway...
Did you see that Enkidu person? What's their deal?
What's with the name? I've never needed one before, or anyone else that I know. What makes them think they're so important?
MC: Well it does make it easier to refer to them.
NPC: ...
MC: But yeah it's weird.
Still. There's something kind of intriguing about them.
NPC: Intriguing? Really? Say more.
MC: I don't know. I guess I just wonder what it must be like to have that much attention put upon you.
NPC: Must be asphyxiating. //im trying to think of a better word that also would make sense for a fish to say
MC: I'm sure, yeah.
...
Can I ask you a question?
NPC: Sure.
MC: Why do you hate the entanglement so much?
NPC: Well, hate is a strong word. And I would say that my emotions are always in balance.
MC: ...Right.
NPC: Regardless, I just don't trust it. We never used to do it, so why now?
MC: Yeah...
And the whole thing with the surface...
NPC: Well, that part is fine with me. If people want to try their luck up there, it's none of my business.
It's just the act itself... The way those two organs just come out... I wish I didn't know that I could grow one to be frank.
And the way they writhe and... Ugh! It nauseates me to even thing about it.
MC: Right...
NPC: Anyway...
What now? Do you want to take a stroll around the edge?
MC: Actually...
I was thinking, maybe I'd go back in there for a bit.
NPC: Really?
MC: Yeah, I don't know. I guess I'm just curious to see if anything big happened. Maybe Enkidu dropped their vizor or something.
NPC: ...
Well, be careful in there please. 
I know you've done "it" before but with all the other changes you never know what might happen this time. They might eat you this time.
MC: Ok. And that's not why I'm going back in there.
NPC: Right.
MC: Right.
->->


=== EnkiduAtParty1 ===
-> npcStart ->
Enkidu: Hello there!
MC: ...Hello?
Enkidu: That's how they start conversations on the surface!
MC: Oh...
-> npcEnd ->
-> END

//Maybe for this one they're hanging from the ceiling or sth crazy/not dancing
=== EnkiduAtParty2 ===
-> npcStart ->
Enkidu: You are not dancing!
MC: Neither are you.
Enkidu: I'm using psychic power to recenter the party vibes!
-> npcEnd ->
-> END

=== EnkiduAtParty3 ===
-> npcStart ->
Enkidu: You are still not dancing!
MC: I'm fully aware.
Enkidu: Make that be different!!
-> npcEnd ->
-> END

=== EnkiduAtParty4 ===
-> npcStart ->
MC: So... About your whole "name" thing --
Enkidu: Are you seriously trying to have a conversation with me right now??
MC: Yes?
Enkidu: Dance! Now!
-> npcEnd ->
-> END

=== EnkiduAtParty5 ===
-> npcStart ->
Enkidu: Little minnow!
MC: What?
Enkidu: That's you! That's what you look like!
MC: ...
-> npcEnd ->
-> END

=== EnkiduAtParty6 ===
-> npcStart ->
Enkidu: I like your style!
MC: I feel like I'm acting really awkwardly...
Enkidu: You are! There's a quiet melancholy about you! It's so different! I love it!
-> npcEnd ->
-> END


=== EnkiduAtParty7 ===
-> npcStart ->
Enkidu: Meet me later!
MC: Why? I mean, um. Where? #speed: fast
Enkidu: You'll find me!
-> npcEnd ->
-> END


VAR talkedToEnkiduBeforeMainAct1=false

=== EnkiduBeforeMainAct1 ===
-> npcStart ->
{talkedToEnkiduBeforeMainAct1:
    ->chat->
}
Enkidu: {Let's do it.|Come on.|There is only one time.|I want to entangle with you.}
Now.
+   [Yes!]
    MC: Don't we need to harmonize more first?
    Enkidu: You don't need to when you're with me little minnow.
    Enkidu: Just let it out.
    MC: Oh... Oh!
    // Transition to Main Act
+   [No!]
    MC: No! Uhmm! I'm not ready!
    Enkidu: I will not be here forever.
    {But take your time. I love that you're assertive.|But I will be for a bit more.|}
--> npcEnd ->
-> END
= chat
MC: Wow. It's so much quieter in here.
~talkedToEnkiduBeforeMainAct1=true
Enkidu: Do you like it?
+   [Yes.]
+   [No.]
+   [It's fine.]
- MC: It's --
->->

// goals: MC talks about ambivalence of sex, enkidu proposes naming
=== EnkiduAftercare1 ===
MC: To be honest...
Enkidu: Yes?
MC: I didn't think I was going to entangle ever again.
Enkidu: And why is that?
//Maybe at this point loop and explore all of these
+   [I was disguted.]
    MC: I was... disgusted.
    Enkidu: Why?
    MC: Well it's... kind of gross, isn't it?
    Enkidu: In what way?
    MC: I don't know. The shape of the organs. And the residue. The writhing. It's...
    Enkidu: Yet, it feels like nothing else, doesn't it?
    MC: Uhm... Yeah. I was about to say something like that.
+   [I was afraid.]
    MC: I was afraid.
    Enkidu: Of?
    MC: I don't know. Of changes happening to me. That I don't understand.
    // Enkidu: And why is that bad?
    // MC: Well, the changes could be irreversible.
    // Enkidu: Why be afraid of something that feels so wonderful? Change is the most natural thing to happen to anything.
    // MC: ...is it? 
    MC: It felt so good, but also like... like I was standing at the precipice of something that is completely unknown.
    Enkidu: And why is that bad?
    MC: Well... the changes could be irreversible.
    Enkidu: And?
    MC: What if I don't like the changes?
    Enkidu: Yet it feels marvellous, doesn't it?
    MC: It does, yeah.
    // Enkidu: Why be scared of what feels good?
    // MC: ...
    // I don't know.
+   [I was angry.]
    MC: I was angry.
    Enkidu: At?
    MC: My last partner. And...
    And the one before that, too.
    Enkidu: Because they left you?
    MC: Uh... I guess, yeah.
    Enkidu: And you're still here.
    MC: Yeah.
    Enkidu: So why blame the entanglement? It sounds like it's them that you're angry at. The act is just an act.
    MC: Well... When you put it that way... I guess so, Yeah.
Enkidu: But there's more to it, isn't it?
// LOOP BACK LOGIC
Enkidu: The way I see it... If something feels this good, why resist it?
You say you fear change. Yet, isn't it in our nature to go with the current?
MC: Huh.
I guess so.
    
// the way i see it... if something feels this good, why resist it?
// hmm yeah maybe 
// can i ask u about name?
// why/how/ ...
//do u want one?
-> END

// * OVERWORLD NPCS *




