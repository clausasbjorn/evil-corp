# Evil Corp - share the fun

There are lots of puzzles in this solution, where you only play a
small part. Unless you join us to take over the world, you know where to
send the pull requests. Muhahaha...

This is a project is all part of the [ARIoT 2016](http://localhost:4400/api/stickers) challenge.

### The evil api

Observations is important to our goal, without them we do not know anything
about you. We want to know more about you, and this API is the core
component for that.

Details can be found at [evil-api](https://github.com/clausasbjorn/evil-corp/tree/master/evil-api/).

### The evil signal hub

This is connection between the evil app, evil accesspoint, events from our evil api, and the friendly dashboard we use to follow you.

Details can be found at [evil-signalhub](https://github.com/clausasbjorn/evil-corp/tree/master/evil-signalhub).

### The evil sensor

Delivers observations done on the raspberry pi to the api.

Details can be found at [evil-sensor](https://github.com/clausasbjorn/evil-corp/tree/master/evil-sensor/).

### The evil access point
This is a niftly little thing that connects to the singal hub and gets a QR code linking to the evil app. When someone opens the evil app, the evil access point takes your picture. And uploads it to a safe, safe, place.
Details can be found at [evil-accesspoint](https://github.com/clausasbjorn/evil-corp/tree/master/evil-accesspoint).

### The evil app

This is the trickster that will get your MAC address (from the evil helper, see below), your name, and trigger a picture when opening it. It will send all this to our signal hub, conecting it to the data from the evil sensor.

Details can be found at [evil-app](https://github.com/clausasbjorn/evil-corp/tree/master/evil-app/).

### The evil helper (optional)

Sometimes it is not enough to observe alone, in those cases it is nice to
help out by providing some extra information.

Details can be found at [evil-helper](https://github.com/clausasbjorn/evil-corp/tree/master/evil-helper/).

### The evil console (optional)

Tools of the trade. If you are going to be one of us, you need a black
terminal with white text scrolling over it.

Details can be found at [evil-console](https://github.com/clausasbjorn/evil-corp/tree/master/evil-console/).

