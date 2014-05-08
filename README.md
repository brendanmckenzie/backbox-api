# backbox
---------

BackBox is an example application to get used to programming again.

I had an idea when stranded in Dong Hui, Vietnam trying to find a way back to civilisation.  I realised that having the ability to communicate with other people around me - preferably other travelers in a similar situation - could have been beneficial in organising a chartered bus instead of working with unhelpful locals.

The idea was shelved whilst I continued traveling but now that I am in possession of a computer again and have the time to work on it, the project has become active.

In addition to being an application for users the main driving factor behind actually coding this app was to get some self exposure to the open source community by releasing this project to the world without any conditions to prove that I'm actually a capable coder ;)

## API

This is the API portion of the service.

There are four parts to it.

 1. The service - the backend that connects everything
 2. The mobile applications (iOS, Android, Windows Phone)
 3. The website
 4. The administration portal
 
There are only a few endpoints for the API.

 * `/connect` is called when a client first connects. If it's the first time the client has connected they are assigned an id and it's returned to them - it's of no use to them but it at least gives a response from the server. a simple `"ok"` response would be fine, but I thought what the hell - why not.
 * `/set-name` is called when the user wants to have a more meaningful name assigned to themselves.  There is no uniquity to it so there are no server-side checks.
 * `/set-bounds` is called when a user changes their location (either manually or automatically).  They have the option of specifying the area they wish to be a part of, otherwise their location is taken from the GPS unit of their mobile device
 * `/send` is used to send a message form the user's current location
 * `/get-latest` is used to get the latest messages in the area designated by the user (specified with the `/set-bounds` call)