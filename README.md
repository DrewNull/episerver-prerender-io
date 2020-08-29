# episerver-prerender-io
Episerver Prerender.io SDK

## Example with Episerver Foundation SPA + ngrok

Prerequisites: 

- Go through the Foundation-Spa-React setup process.
- Install **ngrok**: `npm install -g ngrok`

Steps:

1. Build and start Foundation-Spa-React CMS and frontend.
2. Start **ngrok** from `./src/Foundation`.
3. Copy the HTTP `ngrok.io`. E.g., `http://123xyz.ngrok.io`
4. Open the CMS site in IIS and set the `ngrok` URL as an HTTP binding.
5. Open the ngrok URL in your browser. 

All set 👍
