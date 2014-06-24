namespace WebRTC

open IntelliFactory.WebSharper.InterfaceGenerator

module Definition =
    open IntelliFactory.WebSharper.Html5
    open IntelliFactory.WebSharper.EcmaScript

    let O = T<unit>
    let Event = T<IntelliFactory.WebSharper.Dom.Event>

    let MediaStreamTrack = Type.New ()

    let MediaStreamTrackState = 
        Pattern.EnumStrings "MediaStreamTrackState" [
            "new"
            "live"
            "ended"
        ]

    let SourceTypeEnum =
        Pattern.EnumStrings "SourceTypeEnum" [
            "nonce"
            "camera"
            "microphone"
        ]    

    module Constraints =
        let DoubleRange =
            Pattern.Config "ConstrainDoubleRange" {
                Required = [ "max", T<double>; "min", T<double> ]
                Optional = []
            }
        let LongRange =
            Pattern.Config "ConstrainLongRange" {
                Required = [ "max", T<int>; "min", T<int> ]
                Optional = []
            }

        let ConstrainString = T<string> + Type.ArrayOf T<string>
        let ConstrainDouble = T<double> + DoubleRange
        let ConstrainLong = T<int> + LongRange
        let FacingMode = 
            Pattern.EnumStrings "VideoFacingMode" [ "left"; "right"; "user"; "environment" ]

        let ConstrainFacingMode = FacingMode + Type.ArrayOf FacingMode

    let MediaTrackConstraintSet = 
        Pattern.Config "MediaTrackContraintSet" {
            Required = []
            Optional = [
                            "width", Constraints.ConstrainLong
                            "height", Constraints.ConstrainLong
                            "aspectRatio", Constraints.ConstrainDouble
                            "frameRate", Constraints.ConstrainDouble
                            "facingMode", Constraints.ConstrainFacingMode
                            "volume", Constraints.ConstrainDouble
                            "sampleRate", Constraints.ConstrainLong
                            "sampleSize", Constraints.ConstrainLong
                            "echoCancelation", T<bool>
                            "sourceId", Constraints.ConstrainString
                       ]
        }

    let MediaTrackContraints =
        Pattern.Config "MediaTrackConstraints" {
            Required = []
            Optional = ["advanced", Type.ArrayOf MediaTrackConstraintSet; "require", T<string> ]
        }
        |=> Inherits MediaTrackConstraintSet

    let MediaStreamTrackEventInit =
        Pattern.Config "MediaStreamTrackEventInit" {
            Required = [ "track", MediaStreamTrack ]
            Optional = []
        }

    let MediaStreamTrackEvent =
        Class "MediaStreamTrackEvent"
        |+> [
            Constructor (T<string>?``type`` * MediaStreamTrackEventInit?eventInitDict)
        ]
        |+> Protocol [
            "track" =? MediaStreamTrack
        ]

    let MediaStreamError = 
        Class "MediaStreamError"
        |+> Protocol [
            "name" =? T<string>
            "message" =? T<string>
            "constraintName" =? T<string>
        ]

    let MediaStreamErrorEventInit =
        Pattern.Config "MediaStreamErrorEventInit" {
            Required = [ "error", MediaStreamError.Type ]
            Optional = []
        }       

    let MediaStreamErrorEvent =
        Class "MediaStreamErrorEvent"
        |+> [
            Constructor (T<string>?``type`` * MediaStreamErrorEventInit?eventInitDict)
        ]
        |+> Protocol [
            "error" =? MediaStreamError
        ]

    let MediaDeviceKind =
        Pattern.EnumStrings "MediaDeviceKind" [
            "audioinput"
            "audiooutput"
            "videoinput"
        ]

    let MediaDeviceInfo =
        Pattern.Config "MediaDeviceInfo" {
            Required = []
            Optional =  [
                            "deviceId", T<string>
                            "label", T<string>
                            "groupId", T<string>
                            "kind", MediaDeviceKind.Type
                        ]
        }

    let MediaDevices =
        Class "MediaDevices"
        |=> Inherits T<IntelliFactory.WebSharper.Dom.EventTarget>
        |+> [
            "getSupportedConstraints" => T<string>?kind ^-> T<obj>
        ]
        |+> Protocol [
            "ondevicechange" =@ Event ^-> O
            "enumerateDevices" => (Type.ArrayOf MediaDeviceInfo ^-> O)?resultCallback ^-> O
        ]

    let MediaStreamConstraints =
        Pattern.Config "MediaStreamConstraints" {
            Required =  [
                            "video", T<bool> + MediaTrackContraints
                            "audio", T<bool> + MediaTrackContraints
                        ]

            Optional =  [
                            "peerIdentity", T<string>
                        ]
        }

    let MediaStream = Type.New ()

    let UserMedia =
        Class "UserMedia"
        |+> [
            Constructor O
            |> WithInline "navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia"
            ///Not supported
            //"getMediaDevices" => ((Type.ArrayOf MediaDeviceInfo) ^-> O)?resultCallback ^-> O
            //|> WithInline "navigator.getMediaDevices($resultCallback)$"
        ]
        |+> Protocol [
            "getUserMedia" => (MediaStreamConstraints?constraints * (MediaStream ^-> O)?successCallback * (MediaStreamError ^-> O)?errorCallback) ^-> O
            |> WithInline "navigator.getUserMedia($constraints, $successCallback, $errorCallback)"
        ]
        

    let MediaStreamTrackClass = 
        Class "MediaStreamTrack"
        |=> MediaStreamTrack
        |=> Inherits T<IntelliFactory.WebSharper.Dom.EventTarget>
        |+> Protocol [
            "kind" =? T<string>
            "id" =? T<string>
            "label" =? T<string>
            "enabled" =@ T<bool>
            "muted" =? T<bool>

            "_readonly" =? T<bool>
            "remote" =? T<bool>
            "readyState" =? MediaStreamTrackState

            "onmute" =@ Event ^-> O
            "onunmute" =@ Event ^-> O
            "onstarted" =@ Event ^-> O
            "onended" =@ Event ^-> O

            "getNativeSettings" => O ^-> T<obj> 
            "clone" => O ^-> MediaStreamTrack
            "stop" => O ^-> O
            "getCapabilities" => O ^-> T<obj>
            "getConstraints" => O ^-> MediaTrackContraints
            "getSettings" => O ^-> T<obj>
            "applyConstraints" => (MediaTrackContraints?constraints * (O ^-> O)?successCallback * (MediaStreamError ^-> O)?errorCallback) ^-> O
        ]

    let MediaStreamClass = 
        Class "MediaStream"
        |=> MediaStream
        |=> Inherits T<IntelliFactory.WebSharper.Dom.EventTarget>
        |+> [
            Constructor O
            Constructor MediaStream
            Constructor (Type.ArrayOf MediaStream)
        ]
        |+> Protocol [
            "id" =? T<string>
            "getAudioTracks" => O ^-> Type.ArrayOf MediaStreamTrack
            "getVideoTracks" => O ^-> Type.ArrayOf MediaStreamTrack
            "getTrackById" => T<string>?trackId ^-> MediaStreamTrack
            "addTrack" => MediaStreamTrack?track ^-> O
            "removeTrack" => MediaStreamTrack ^-> O
            "clone" => O ^-> MediaStream

            "active" =? T<bool>
            "onactive" =@ Event ^-> O
            "oninactive" =@ Event ^-> O
            "onaddtrack" =@ MediaStreamTrackEvent ^-> O
            "onremovetrack" =@ MediaStreamTrackEvent ^-> O
        ]

    let Assembly = 
        Assembly [
            Namespace "IntelliFactory.WebSharper.Html5" [
                MediaStreamTrackState
                SourceTypeEnum
                Constraints.DoubleRange
                Constraints.LongRange
                Constraints.FacingMode
                MediaTrackConstraintSet
                MediaTrackContraints
                MediaStreamTrackEventInit
                MediaStreamTrackEvent
                MediaStreamError
                MediaStreamErrorEventInit
                MediaStreamErrorEvent
                MediaDeviceKind
                MediaDeviceInfo
                MediaDevices
                MediaStreamConstraints
                UserMedia
                MediaStreamTrackClass
                MediaStreamClass
            ]
        ] 

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()