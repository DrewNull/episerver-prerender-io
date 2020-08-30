# Prerender.io middleware for Episerver CMS

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

## Setup

An Episerver CMS Visual Studio solution is needed to get started. 

1. Copy the `*.cs` files into your feature folder of choice within your Episerver CMS 
solution. 

2. [Sign up](https://prerender.io/signup) for a Prerender.io account and get an API token.

3. Add the `Prerender:Token` appSetting to your Episerver CMS project Web.config.

````````````````````````````````````````````````````````````````````````````````
WORK IN PROGRESS
````````````````````````````````````````````````````````````````````````````````

## Testing

Prerender.io must be able to access the page that it renders. In other words, your page(s) 
must be publicly navigable. This can be done by exposing your local site publicly  with the 
free tier of [ngrok](https://ngrok.com) and some minor configuration changes. 

If your site is already public, skip down to step X.

1. **Install ngrok**. This can be done in a number of ways. You can 
[download](https://ngrok.com/download) the app directly from their website. My preferred 
way is to use [npm](https://www.npmjs.com/package/ngrok):
    ```
    npm install -g ngrok
    ```

2. **Start ngrok** from your Visual Studio Episerver CMS directory. 

    Example: in the Episerver Foundation React SPA solution, this is the 
    `./src/Foundation/` folder. 
   
    If you use a custom local hostname (e.g., `foundation-spa.local`), run ngrok like this:
    ```
    ngrok http foundation-spa.local
    ```

    If you use `localhost` (e.g., with port `12345`):
    ```
    ngrok http 12345 -host-header=localhost
    ```

    Ngrok should start and show details about your session. 

    Take note of the `Forwarding` line. E.g., 
    ```
    Forwarding http://123xyz.ngrok.io -> http://foundation-spa.local:80
    ```

    Copy the ngrok URL (e.g., `http://123xyz.ngrok.io`) so that you can paste it later. 

3. Depending on your frontend implementation, you might need to **configure CORS**.
Foundation SPA, for example, will not accept AJAX requests from a hostname that it doesn't 
expect. 

    To get Foundation SPA working, find the `.env` file in `Spa.Frontend` project:
    `./Spa.Frontend/.env`. Note that this file is hidden by default.

    Change the `EPI_URL` parameter to your ngrok URL. E.g., 
    ```
    EPI_URL=http://123xyz.ngrok.io
    ```

4. If you run your site locally through IIS, you'll need to **setup an HTTP binding** in 
IIS for the ngrok URL. If this is the case, go into IIS and add your `123xyz.ngrok.io` 
hostname as a binding for your site now.

5. **Build and start** your Episerver local web site (`Ctrl+F5`).

6. **Navigate** to your ngrok URL (e.g., `http://123xyz.ngrok.io`). If you did everything
right, the site should come right up. 

7. Finally, test Prerender, use devtools to **set your browser's user-agent** to that of a 
known spider. (e.g., `Googlebot`). Then reload the page. Wait a moment, and then your 
prerendered page should come up. 

    Optional: As an alternative to setting your user agent string, append the 
    `?_escaped_fragment=` query to your URL (e.g., 
    `http://123xyz.ngrok.io?_escaped_fragment_=`). The ASP.NET middleware is 
    programmed to call Prerender if this parameter is present. Just don't forget the `=` 
    at the end. Also note that this might interfere with your JS frontend framework's 
    routing.

8. **Verify** that the prerender was successful. Sign into your Prerender.io account and 
navigate to the Cached Pages page. Your page should appear in the list. Click on its URL 
to see the output of the prerender. 

    All done!
