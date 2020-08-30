## Background

**Search engine spiders** aren't great at crawling asynchronous JavaScript. 
Ideally, websites would serve static, HTML-and-CSS-only pages for spiders to 
crawl. But consumer and business expectations demand rich web experiences that 
can only be delivered with sophisticated JavaScript implementations. 

**Prerender.io** is a paid web service that makes this 
possible with very little friction. It accepts a request for a web page, 
renders it using Google Chrome in headless mode, and returns the HTML and CSS 
outputs as a static, "prerendered" page. The prerendered pages are cached for 
future requests.

[https://prerender.io](https://prerender.io)


**Official and community 
[middleware SDKs](https://prerender.io/documentation/install-middleware)**
exist for quickly integrating Prerender into your web application. The 
ASP.NET version can be found at the following GitHub repo:

[https://github.com/greengerong/Prerender_asp_mvc](https://github.com/greengerong/Prerender_asp_mvc)

**Episerver CMS**, however, requires additional bootstrapping to work with the 
Prerender ASP.NET middleware by [greengerong](https://github.com/greengerong). 
The purpose of this repository is to illustrate how to quickly stand up a 
Prerender proxy in an Episerver CMS solution. The code borrows heavily from 
greengerong's repo, but has been trimmed down to highlight the necessary 
technique for integrating with Episerver.

--------------------------------------------------------------------------------

## Setup

An Episerver CMS Visual Studio solution is needed to get started. 

1. Copy the `*.cs` files into your feature folder of choice within your Episerver CMS solution. 
2. [Sign up](https://prerender.io/signup) for a Prerender.io account and get an API token.
3. Add the `Prerender:Token` appSetting to your Episerver CMS project Web.config.

## Testing

Prerender.io must be able to access the page that it renders. In other words, your page(s) must be publicly navigable. 

```
WORK IN PROGRESS
```

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
