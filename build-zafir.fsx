#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.WebRTC")
        .VersionFrom("WebSharper")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)

let main =
    bt.WebSharper4.Extension("WebSharper.WebRTC", directory = "WebRTC")
        .SourcesFromProject("WebRTC.fsproj")

(*let test =
    bt.WebSharper.BundleWebsite("IntelliFactory.WebSharper.WebRTC.Tests", directory = "Tests")
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
