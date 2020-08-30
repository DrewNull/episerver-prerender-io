# Prerender.io middleware for Episerver CMS

## Background

**Search engine spiders** aren't great at crawling asynchronous JavaScript.
Ideally, websites would serve static, HTML-and-CSS-only pages for spiders to
crawl. But consumer and business expectations demand rich web experiences that
can only be delivered with sophisticated JavaScript implementations.

Additionally, the time it takes spiders to crawl a new site is nontrivial.
Say you have a new site with 100,000 product pages. On launch day, once DNS
cutover takes place, spiders won't instantly start crawling all 100,000 pages
at once. The pages will queue for a set of workers to process. The heavier the
JavaScript, the longer each will take to process. AJAX requests, even when the
spider is smart enough to follow them, will also take time. You might
encounter a scenario where your site takes hours, days, or even weeks to
ingest. Which translates directly to page rank, conversions, and&mdash;for
ecommerce sites&mdash;revenue.

**Prerender.io** is a paid web service that addresses these challenges with
very little friction. It accepts a request for a web page, renders it using
Google Chrome in headless mode, and returns the HTML and CSS outputs as a
static, "prerendered" page ideal for crawling. The prerendered pages are then
cached for future requests.

Pages can be seeded using the Prerender web portal. Either by hand or by
pointing Prerender to a Sitemap.xml. This can reduce the SEO rank dip that
sites experience immediately following a new launch.

[https://prerender.io](https://prerender.io)

#### Without Prerender

```
    SPIDER              WEBSITE

01  ······ GET -------> ··········
02  ·········· <------- 200 ······
03  ··········          ··········
04  (thinking)          ··········
05  ··········          ··········
06  GET (AJAX) -------> ··········
07  ·········· <------- 200·······
08  ··········          ··········
09  (thinking)          ··········

    Done

```

We're dependent on how well the spider handles JavaScript and whether it follows AJAX
requests. Also, it does too much waiting and thinking...

#### With Prerender (before caching)

```
    SPIDER              WEBSITE             PRERENDER

01  ······ GET -------> ··········          ··········
02  ··········          ······ GET -------> ··········
03  ··········          ··········          ··········
04  ··········          ·········· <------- GET ······
05  ··········          ······ 200 -------> ··········
06  ··········          ··········          ··········
07  ··········          ··········          (thinking)
08  ··········          ··········          ··········
09  ··········          ·········· <------- GET (AJAX)
10  ··········          ······ 200 -------> ··········
11  ··········          ··········          ··········
12  ··········          ··········          (thinking)
13  ··········          ··········          ··········
14  ··········          ·········· <------- 200 ······
15  ·········· <------- 200 ······          ··········

    Done
```

-   The middleware routes the spider's request to Prerender on line `2`.
-   Prerender requests the page from our website itself (lines `4-5`).
-   Prerender renders the page, conducting any AJAX requests along the way, caches the
    output, and sends it back to the original request from the website (lines `7-14`).
-   The website sends the request back to the spider (line `15`).

Very little thinking on the part of the spider.

And once the page has been cached...

#### With Prerender (after caching)

```
    SPIDER              WEBSITE             PRERENDER

01  ······ GET -------> ··········          ··········
02  ··········          ······ GET -------> ··········
03  ··········          ·········· <------- 200 ······
04  ·········· <------- 200 ······          ··········

    Done
```

Prerender immediately sends the cached page back to the website, which forwards it to the
spider.

### Middleware

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

## Episerver Solution Setup

An Episerver CMS Visual Studio solution is needed to get started.

1.  Copy the `*.cs` files into your feature folder of choice within your Episerver CMS
    solution.

2.  [Sign up](https://prerender.io/signup) for a Prerender.io account and get an API token.

3.  Add the `Prerender:Token` appSetting to your Episerver CMS project Web.config.

4.  Consider explicitly telling Prerender when your page is ready to be saved:
    [more info](https://prerender.io/documentation/best-practices).

    Prerender looks for a `window.prerenderReady` boolean to determine whether the page
    has finished loading. Set it to `false` at the top of the HTML, and then to `true`
    once your JavaScript has finished executing and your page is ready.

    As a simplistic example, use `setTimeout` to tell Prerender to wait for a moment
    before saving.

    ```html
    <!-- Immediately below the opening <head> tag -->
    <script>
        window.prerenderReady = false;
    </script>
    ```

    ```html
    <!-- Immediately above the closing </body> tag -->
    <script>
        setTimeout(
            function () {
                window.prerenderReady = true;
            },
            // Wait 5 seconds
            5000
        );
    </script>
    ```

## Testing (with ngrok)

Prerender.io must be able to access the page that it renders. In other words, your page(s)
must be publicly navigable. This can be done by exposing your local site publicly with the
free tier of [ngrok](https://ngrok.com) and some minor configuration changes.

If your site is already public, skip down to step 5.

1.  **Install ngrok**. This can be done in a number of ways. You can
    [download](https://ngrok.com/download) the app directly from their website. My preferred
    way is to use [npm](https://www.npmjs.com/package/ngrok):
    `npm install -g ngrok`

2.  **Start ngrok** from your Visual Studio Episerver CMS directory.

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

3.  Depending on your frontend implementation, you might need to **configure CORS**.
    Foundation SPA, for example, will not accept AJAX requests from a hostname that it doesn't
    expect.

    To get Foundation SPA working, find the `.env` file in `Spa.Frontend` project:
    `./Spa.Frontend/.env`. Note that this file is hidden by default.

    Change the `EPI_URL` parameter to your ngrok URL. E.g.,

    ```
    EPI_URL=http://123xyz.ngrok.io
    ```

4.  If you run your site locally through IIS, you'll need to **setup an HTTP binding** in
    IIS for the ngrok URL. If this is the case, go into IIS and add your `123xyz.ngrok.io`
    hostname as a binding for your site now.

5.  **Build and start** your Episerver local web site (`Ctrl+F5`).

6.  **Navigate** to your ngrok URL (e.g., `http://123xyz.ngrok.io`). If you did everything
    right, the site should come right up.

7.  Finally, test Prerender, use devtools to **set your browser's user-agent** to that of a
    known spider. (e.g., `Googlebot`). Then reload the page. Wait a moment, and then your
    prerendered page should come up.

    Optional: As an alternative to setting your user agent string, append the
    `?_escaped_fragment=` query to your URL (e.g.,
    `http://123xyz.ngrok.io?_escaped_fragment_=`). The ASP.NET middleware is
    programmed to call Prerender if this parameter is present. Just don't forget the `=`
    at the end. Also note that this might interfere with your JS frontend framework's
    routing.

8.  **Verify** that the prerender was successful. Sign into your Prerender.io account and
    navigate to the Cached Pages page. Your page should appear in the list. Click on its URL
    to see the output of the prerender.

    All done!
