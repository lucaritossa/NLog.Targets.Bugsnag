![logo](/misc/logo/logo-home.png)

[![pipeline](https://github.com/lucaritossa/NLog.Targets.Bugsnag.Test.Pipeline/actions/workflows/pipeline.yml/badge.svg?branch=main)](https://github.com/lucaritossa/NLog.Targets.Bugsnag.Test.Pipeline/actions/workflows/pipeline.yml)
[![NuGet Badge](https://buildstats.info/nuget/NLog.Targets.Bugsnag?includePreReleases=true)](https://www.nuget.org/packages/NLog.Targets.Bugsnag)

NLog.Targets.Busgnag
=======================

NLog Target to send your messages to Bugsnag service with the possibility to include all details like App, Request and (still in WIP) User infos.

**This is a new way to report to Bugsnag**, different from what it's offered by the [Bugsnag.Client](https://docs.bugsnag.com/platforms/dotnet/other/#basic-configuration) through the Notify method (based only on passing an Exception).

You will be able to send custom messages (error, warning or with the severity you want) in a simple way, as you always do with NLog.

It can be used with version 5.1.1 and later of NLog

### How to use

1) Install the package

    `Install-Package NLog.Target.Bugsnag`

2) Use the target type `"Bugsnag, NLog.Targets.Bugsnag"` in your nlog.config including apiKey and releaseStage (see [Configuration](/docs/configuration.md) page for all parameters). Best with [AsyncWrappers](https://github.com/NLog/NLog/wiki/AsyncWrapper-target)

    ```xml
    <target name="bugsnagAsync" xsi:type="AsyncWrapper">
      <target xsi:type="Bugsnag, NLog.Targets.Bugsnag"
              name="bugsnag" 
              apikey="your-busgnag-api-key-goes-here"
              releaseStage="development|production"
      </target>
    </target>
    <rules>
      <logger minLevel="Error" writeTo="bugsnagAsync" />
    </rules>
    ```
     
### Support

NLog Bugsnag is open source software maintained by voluntary contributors in their spare time and on a best effort basis.

Support is provided for the latest version only: please update before submitting any issue.


### License

NLog.Targets.Bugsnag is licensed under the terms of BSD license.

Please see the [LICENSE file](./LICENSE.md) for further information.
