namespace WebSharper.WebRTC.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebRTC

[<JavaScript>]
module Client =

    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        let error = Var.Create ""

        let video () =
            JS.Document.GetElementById("camera") |> As<HTMLVideoElement>

        let stopCamera () =
            ClearSrcObject (video ())
            error.Value <- ""

        let startCamera () =
            error.Value <- ""
            // GetUserMedia (Promise-based) and SetSrcObject come from the
            // WebSharper.WebRTC.Extensions assembly. They replace the legacy
            // navigator.getUserMedia + URL.createObjectURL approach, both of
            // which were removed from modern browsers.
            let constraints = MediaStreamConstraints(Video = true)
            let promise = GetUserMedia (JS.Window.Navigator.MediaDevices) constraints
            promise.Then(
                    (fun (stream: MediaStream) ->
                        let v = video ()
                        SetSrcObject v stream
                        v.Play() |> ignore
                        ()),
                    (fun (err: obj) ->
                        error.Value <-
                            "Could not access the camera: " + (As<MediaStreamError> err).Message
                        ())
                )
            |> ignore

        IndexTemplate()
            .StartBtn(fun _ -> startCamera ())
            .StopBtn(fun _ -> stopCamera ())
            .Error(error.View)
            .Bind()
