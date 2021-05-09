# GeoFinder
GeoFinder app can find locations by input IP or city name.
During development I tried to reach criteria:
- lightweight CSS (https://jenil.github.io/chota/)
- fast local DB loading (less than 30ms)
- optimised search logic (using BinarySearch, ArraySegment, specific collections)
- front end part of the application constructed without SPA framework but following that paradigm

You can find some benchmarks in GeoFinder.Benchmarks project.

Moreover, I have load tested with [West Wind WebSurge](https://websurge.west-wind.com/)
![ScreenShot](https://i.postimg.cc/jqPZz9F5/Screenshot-2021-05-09-173714.png)
