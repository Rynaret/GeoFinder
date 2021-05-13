<img align="left" width="80" height="80" src="https://cdn.iconscout.com/icon/free/png-256/location-pin-1437358-1213053.png" />

# GeoFinder

GeoFinder app can find locations by input IP or city name.
You can [find demo here](https://geo-finder.herokuapp.com/find-by-city).

## Goals

During development I tried to reach criteria:

- lightweight CSS (<https://jenil.github.io/chota/>)
- fast local DB loading (less than 30ms)
- optimised search logic (using BinarySearch, ArraySegment, Span, specific collections)

HTML highlighting in the js files is supported by [es6-string-html
](https://marketplace.visualstudio.com/items?itemName=Tobermory.es6-string-html)

## Build
You can build and run project in visual studio or use docker.
For docker:

- go to solution folder and open here cmd
- type and run command `docker build . -t geo-finder -f Dockerfile`

## Benchmarks and Tests

You can find:

- some benchmarks in the GeoFinder.Benchmarks project
- some simple tests in the GeoFinder.Tests

## Load Testing

Moreover, I have load tested with [West Wind WebSurge](https://websurge.west-wind.com/)
![ScreenShot](https://i.postimg.cc/jqPZz9F5/Screenshot-2021-05-09-173714.png)
