Readme

Not much of a readme tbh!

So, I'm hoping very fervently that this is going to work if
you just straight-up import the scripts into your project.
-crosses fingers, kisses lucky rabbit's foot-

1. Make sure you've downloaded Pete's splenderific audio tracks.
	(for debugging I guess you can just use any old music on
	your harddrive, but it's not going to be as nice to listen to!)

2. Attach a Metronome to an object in the scene (empty is ideal)
	Play with the sliders as you desire.
	If you like, attach an AudioSource to the same object.
	Your metronome will then play whatever audio file you gave
	it every beat. 

3. Attach a SimpleBeatScript and an AudioSource to another object
	in the scene. Again, play with sliders as desired.
	If you give it a material (or give it a child with a material)
	it should change colour on the beat.


What's next?
- Offset. Things listening to the metronome have to be able to
	come in at different times.
- Looping. Currently, I don't think this arrangement is going
	to loop all that well! Things playing audio have to know
	how long they are so they don't constantly stop and restart
	if they are too long.