# episerver-prerender-io
Episerver Prerender.io SDK

# Setup

Prerequisites: 
- Go through the Foundation-Spa-React setup process.
- Install **ngrok**: `npm install -g ngrok`

1. Build and start Foundation-Spa-React CMS and frontend.
2. Start **ngrok** from `./src/Spa.Frontend`.
3. Copy the HTTP `ngrok.io`. E.g., `http://123xyz.ngrok.io`
4. Open the frontend site in IIS and set the `ngrok` URL as an HTTP binding.
5. Open the frontend `.env` file and append the `ngrok` URL to the `EPI_CORS_URL` parameter. E.g., `EPI_CORS_URL=http://www.episerver-foundation-spa.local,http://123xyz.ngrok.io`

All done!
