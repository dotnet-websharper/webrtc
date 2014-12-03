#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.WebRTC", "3.0-alpha")
    |> fun bt -> bt.WithFramework(bt.Framework.Net40)

let main =
    (bt.WebSharper.Extension("IntelliFactory.WebSharper.WebRTC")
    |> FSharpConfig.BaseDir.Custom "WebRTC")
        .SourcesFromProject("WebRTC.fsproj")

(*let test =
    (bt.WebSharper.BundleWebsite("IntelliFactory.WebSharper.WebRTC.Tests")
    |> FSharpConfig.BaseDir.Custom "Tests")
        .SourcesFromProject("Tests.fsproj")
        .References(fun r -> [r.Project main])*)

bt.Solution [
    main
    //test

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "WebSharper.WebRTC"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://bitbucket.org/intellifactory/websharper.webrtc"
                Description = "WebSharper Extensions for WebRTC"
                Authors = ["IntelliFactory"]
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
